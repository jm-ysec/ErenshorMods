#!/usr/bin/env python3
"""
Advanced build script with deployment verification.

Usage:
    python build.py                    # Build all mods
    python build.py <mod1> <mod2>      # Build specific mods
    python build.py --clean            # Clean before building
    python build.py --release          # Build in Release mode
    python build.py --verify-deploy    # Verify DLLs are deployed after build
    python build.py --parallel         # Build mods in parallel
"""

import os
import sys
import json
import subprocess
import argparse
import xml.etree.ElementTree as ET
from pathlib import Path
from datetime import datetime
from concurrent.futures import ThreadPoolExecutor, as_completed

WORKSPACE_ROOT = Path(__file__).parent.resolve()
EXCLUDE_DIRS = {"Templates", ".git", "bin", "obj", "__pycache__", ".vs", "node_modules"}


def find_mods():
    """Find all mod directories in the workspace."""
    mods = []
    for item in WORKSPACE_ROOT.iterdir():
        if item.is_dir() and item.name not in EXCLUDE_DIRS:
            csproj_files = list(item.glob("*.csproj"))
            if csproj_files:
                mods.append({
                    "name": item.name,
                    "path": item,
                    "csproj": csproj_files[0]
                })
    return sorted(mods, key=lambda m: m["name"])


def parse_csproj(csproj_path):
    """Extract build configuration from .csproj file."""
    try:
        tree = ET.parse(csproj_path)
        root = tree.getroot()
        
        config = {
            "assembly_name": None,
            "target_framework": None,
            "deploy_path": None,
            "output_path": None
        }
        
        # Find PropertyGroup elements
        for prop_group in root.findall(".//PropertyGroup"):
            for child in prop_group:
                tag = child.tag.split('}')[-1]  # Remove namespace
                if tag == "AssemblyName":
                    config["assembly_name"] = child.text
                elif tag == "TargetFramework":
                    config["target_framework"] = child.text
                elif tag == "OutputPath":
                    config["output_path"] = child.text
        
        # Find PostBuild copy destination
        for target in root.findall(".//Target[@Name='PostBuild']"):
            for copy in target.findall(".//Copy"):
                dest = copy.get("DestinationFolder")
                if dest:
                    config["deploy_path"] = dest
        
        return config
    except Exception as e:
        print(f"Warning: Could not parse .csproj: {e}")
        return None


def clean_mod(mod_path):
    """Clean build artifacts for a mod."""
    print(f"  Cleaning {mod_path.name}...")
    result = subprocess.run(
        ["dotnet", "clean"],
        cwd=mod_path,
        capture_output=True,
        text=True
    )
    return result.returncode == 0


def build_mod(mod, configuration="Debug", clean=False):
    """Build a single mod and return build info."""
    print(f"\n{'=' * 70}")
    print(f"Building: {mod['name']}")
    print(f"{'=' * 70}")
    
    if clean:
        clean_mod(mod["path"])
    
    # Parse .csproj for configuration
    config = parse_csproj(mod["csproj"])
    
    start_time = datetime.now()
    
    result = subprocess.run(
        ["dotnet", "build", "--configuration", configuration, "-v", "minimal"],
        cwd=mod["path"]
    )
    
    build_time = (datetime.now() - start_time).total_seconds()
    
    return {
        "name": mod["name"],
        "success": result.returncode == 0,
        "config": config,
        "build_time": build_time,
        "path": mod["path"]
    }


def verify_deployment(build_result):
    """Verify that the built DLL was deployed to the plugin directory."""
    config = build_result["config"]
    if not config or not config["assembly_name"]:
        return None
    
    # Find the built DLL
    dll_name = f"{config['assembly_name']}.dll"
    output_path = build_result["path"] / "bin" / "Debug" / config.get("target_framework", "net472") / dll_name
    
    if not output_path.exists():
        return {"deployed": False, "reason": f"DLL not found at {output_path}"}
    
    # Check if deploy path is configured
    if not config["deploy_path"]:
        return {"deployed": False, "reason": "No PostBuild deploy target configured"}
    
    # Expand variables in deploy path (basic support)
    deploy_path = config["deploy_path"]
    deploy_path = deploy_path.replace("$(HOME)", str(Path.home()))
    deploy_path = Path(deploy_path) / dll_name
    
    if deploy_path.exists():
        # Check if file was recently modified (within last minute)
        dll_mtime = output_path.stat().st_mtime
        deploy_mtime = deploy_path.stat().st_mtime
        
        if abs(dll_mtime - deploy_mtime) < 60:
            return {"deployed": True, "path": str(deploy_path)}
        else:
            return {"deployed": False, "reason": f"Deployed DLL is outdated at {deploy_path}"}
    else:
        return {"deployed": False, "reason": f"DLL not found at deploy path: {deploy_path}"}


def main():
    parser = argparse.ArgumentParser(description="Build Erenshor mods with verification")
    parser.add_argument("mods", nargs="*", help="Specific mod(s) to build (default: all)")
    parser.add_argument("--clean", "-c", action="store_true", help="Clean before building")
    parser.add_argument("--release", "-r", action="store_true", help="Build in Release mode")
    parser.add_argument("--verify-deploy", "-v", action="store_true", help="Verify deployment after build")
    parser.add_argument("--parallel", "-p", action="store_true", help="Build mods in parallel")
    parser.add_argument("--jobs", "-j", type=int, default=4, help="Number of parallel jobs (default: 4)")
    parser.add_argument("--json", action="store_true", help="Output in JSON format")

    args = parser.parse_args()

    configuration = "Release" if args.release else "Debug"

    # Find mods
    all_mods = find_mods()

    if not all_mods:
        if args.json:
            print(json.dumps({"error": "No mods found in workspace"}))
        else:
            print("No mods found in workspace.")
        return 1

    # Filter if specific mods requested
    if args.mods:
        mods_to_build = [m for m in all_mods if m["name"] in args.mods]
        if not mods_to_build:
            if args.json:
                print(json.dumps({"error": f"No matching mods found for: {', '.join(args.mods)}"}))
            else:
                print(f"Error: No matching mods found for: {', '.join(args.mods)}")
            return 1
    else:
        mods_to_build = all_mods

    if not args.json:
        print(f"Building {len(mods_to_build)} mod(s) in {configuration} mode...")
        if args.parallel:
            print(f"Using parallel build with {args.jobs} jobs")

    # Build all mods
    results = []

    if args.parallel and len(mods_to_build) > 1:
        # Parallel build
        with ThreadPoolExecutor(max_workers=args.jobs) as executor:
            future_to_mod = {
                executor.submit(build_mod, mod, configuration, args.clean): mod
                for mod in mods_to_build
            }

            for future in as_completed(future_to_mod):
                result = future.result()
                results.append(result)
    else:
        # Sequential build
        for mod in mods_to_build:
            result = build_mod(mod, configuration, args.clean)
            results.append(result)
    
    succeeded = [r for r in results if r["success"]]
    failed = [r for r in results if not r["success"]]

    if args.json:
        # JSON output
        output = {
            "total": len(results),
            "successful": len(succeeded),
            "failed": len(failed),
            "configuration": configuration,
            "parallel": args.parallel,
            "mods": results
        }
        print(json.dumps(output, indent=2))
    else:
        # Human-readable output
        print(f"\n{'=' * 70}")
        print("BUILD SUMMARY")
        print(f"{'=' * 70}\n")

        for result in results:
            status = "✓" if result["success"] else "✗"
            print(f"{status} {result['name']:<30} ({result['build_time']:.2f}s)")

        print(f"\nTotal: {len(succeeded)} succeeded, {len(failed)} failed")

        # Verify deployment if requested
        if args.verify_deploy and succeeded:
            print(f"\n{'=' * 70}")
            print("DEPLOYMENT VERIFICATION")
            print(f"{'=' * 70}\n")

            for result in succeeded:
                deploy_status = verify_deployment(result)
                if deploy_status:
                    if deploy_status["deployed"]:
                        print(f"✓ {result['name']}: Deployed to {deploy_status['path']}")
                    else:
                        print(f"✗ {result['name']}: {deploy_status['reason']}")

    return 1 if failed else 0


if __name__ == "__main__":
    sys.exit(main())

