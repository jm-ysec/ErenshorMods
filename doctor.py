#!/usr/bin/env python3
"""
Mod health check tool - diagnose common issues.

Usage:
    python doctor.py              # Check all mods
    python doctor.py <ModName>    # Check specific mod
"""

import os
import sys
import json
import xml.etree.ElementTree as ET
from pathlib import Path

WORKSPACE_ROOT = Path(__file__).parent.resolve()
REGISTRY_FILE = WORKSPACE_ROOT / "mods.json"
EXCLUDE_DIRS = {"Template", ".git", "bin", "obj", "__pycache__", ".vs", "node_modules"}


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


def load_registry():
    """Load the mod registry."""
    if not REGISTRY_FILE.exists():
        return {}
    with open(REGISTRY_FILE, 'r') as f:
        return json.load(f)


def check_registry(mod_name):
    """Check if mod is in registry."""
    registry = load_registry()
    issues = []
    
    if mod_name not in registry:
        issues.append("⚠ Mod not found in mods.json registry")
    else:
        meta = registry[mod_name]
        if not meta.get("version"):
            issues.append("⚠ No version specified in registry")
        if not meta.get("author") or meta.get("author") == "YourName":
            issues.append("⚠ Author not set in registry")
        if not meta.get("description"):
            issues.append("⚠ No description in registry")
    
    return issues


def check_csproj(csproj_path):
    """Check .csproj file for common issues."""
    issues = []
    
    try:
        tree = ET.parse(csproj_path)
        root = tree.getroot()
        
        # Check for TargetFramework
        target_framework = root.find(".//TargetFramework")
        if target_framework is None:
            issues.append("⚠ No TargetFramework specified in .csproj")
        elif target_framework.text != "net472":
            issues.append(f"⚠ TargetFramework is {target_framework.text}, expected net472")
        
        # Check for BepInEx references
        has_bepinex = False
        for reference in root.findall(".//PackageReference"):
            if "BepInEx" in reference.get("Include", ""):
                has_bepinex = True
                break
        
        if not has_bepinex:
            issues.append("⚠ No BepInEx package reference found")
        
        # Check for PostBuild target
        has_postbuild = root.find(".//Target[@Name='PostBuild']") is not None
        if not has_postbuild:
            issues.append("⚠ No PostBuild target for deployment")
        
    except Exception as e:
        issues.append(f"✗ Error parsing .csproj: {e}")
    
    return issues


def check_files(mod_path):
    """Check for required and recommended files."""
    issues = []
    
    # Check for Plugin.cs
    if not (mod_path / "Plugin.cs").exists():
        issues.append("⚠ No Plugin.cs file found")
    
    # Check for README.md
    if not (mod_path / "README.md").exists():
        issues.append("⚠ No README.md (recommended for distribution)")
    
    # Check for icon.png
    if not (mod_path / "icon.png").exists():
        issues.append("⚠ No icon.png (required for Thunderstore)")
    else:
        # Check icon size
        try:
            from PIL import Image
            img = Image.open(mod_path / "icon.png")
            if img.size != (256, 256):
                issues.append(f"⚠ icon.png should be 256x256, found {img.size[0]}x{img.size[1]}")
        except ImportError:
            pass  # PIL not available, skip size check
        except Exception as e:
            issues.append(f"⚠ Could not validate icon.png: {e}")
    
    # Check for CHANGELOG.md
    if not (mod_path / "CHANGELOG.md").exists():
        issues.append("ℹ No CHANGELOG.md (recommended)")
    
    return issues


def check_build_output(mod_path, mod_name):
    """Check if mod has been built."""
    issues = []
    
    dll_locations = [
        mod_path / "bin" / "Debug" / "net472" / f"{mod_name}.dll",
        mod_path / "bin" / "Debug" / f"{mod_name}.dll",
        mod_path / "bin" / "Release" / "net472" / f"{mod_name}.dll",
        mod_path / "bin" / "Release" / f"{mod_name}.dll",
    ]
    
    found_dll = any(dll.exists() for dll in dll_locations)
    
    if not found_dll:
        issues.append("ℹ No built DLL found (run 'python build.py' to build)")
    
    return issues


def check_mod(mod, json_mode=False):
    """Run all health checks on a mod."""
    if not json_mode:
        print(f"\n{'=' * 60}")
        print(f"Checking: {mod['name']}")
        print(f"{'=' * 60}")

    all_issues = []

    # Registry check
    issues = check_registry(mod['name'])
    if issues:
        all_issues.extend(issues)

    # .csproj check
    issues = check_csproj(mod['csproj'])
    if issues:
        all_issues.extend(issues)

    # Files check
    issues = check_files(mod['path'])
    if issues:
        all_issues.extend(issues)

    # Build output check
    issues = check_build_output(mod['path'], mod['name'])
    if issues:
        all_issues.extend(issues)

    if json_mode:
        return {
            "mod": mod['name'],
            "healthy": len(all_issues) == 0,
            "issues": all_issues
        }
    else:
        if all_issues:
            for issue in all_issues:
                print(f"  {issue}")
            return False
        else:
            print("  ✓ No issues found")
            return True


def main():
    import argparse

    parser = argparse.ArgumentParser(description="Check mod health and diagnose issues")
    parser.add_argument("mod_name", nargs="?", help="Name of mod to check (default: all)")
    parser.add_argument("--json", action="store_true", help="Output in JSON format")

    args = parser.parse_args()
    
    mods = find_mods()

    if not mods:
        if args.json:
            print(json.dumps({"error": "No mods found in workspace"}))
        else:
            print("No mods found in workspace.")
        return 1

    if args.mod_name:
        mod = next((m for m in mods if m['name'] == args.mod_name), None)
        if not mod:
            if args.json:
                print(json.dumps({"error": f"Mod '{args.mod_name}' not found"}))
            else:
                print(f"Error: Mod '{args.mod_name}' not found.")
            return 1
        mods = [mod]

    if args.json:
        results = []
        for mod in mods:
            results.append(check_mod(mod, json_mode=True))

        output = {
            "total": len(mods),
            "healthy": sum(1 for r in results if r["healthy"]),
            "mods": results
        }
        print(json.dumps(output, indent=2))
        return 0 if output["healthy"] == output["total"] else 1
    else:
        print(f"Running health checks on {len(mods)} mod(s)...")

        healthy_count = 0
        for mod in mods:
            if check_mod(mod):
                healthy_count += 1

        print(f"\n{'=' * 60}")
        print(f"Health Check Summary: {healthy_count}/{len(mods)} mods healthy")
        print(f"{'=' * 60}")

        return 0 if healthy_count == len(mods) else 1


if __name__ == "__main__":
    sys.exit(main())

