#!/usr/bin/env python3
"""
Erenshor Mod Manager - Unified tool for creating, building, and deploying mods.

Usage:
    python mod_manager.py new          # Create a new mod
    python mod_manager.py build        # Build all mods
    python mod_manager.py build <mod>  # Build specific mod
    python mod_manager.py list         # List all mods
    python mod_manager.py verify       # Verify deployment paths
"""

import os
import sys
import subprocess
import argparse
from pathlib import Path

WORKSPACE_ROOT = Path(__file__).parent.resolve()
TEMPLATE_DIR = WORKSPACE_ROOT / "Template"
TEMPLATE_SCRIPT = TEMPLATE_DIR / "setup_mod.py"

# Directories to exclude from mod detection
EXCLUDE_DIRS = {"Template", ".git", "bin", "obj", "__pycache__", ".vs", "node_modules"}


def find_mods():
    """Find all mod directories in the workspace."""
    mods = []
    for item in WORKSPACE_ROOT.iterdir():
        if item.is_dir() and item.name not in EXCLUDE_DIRS:
            # Check if it has a .csproj file
            csproj_files = list(item.glob("*.csproj"))
            if csproj_files:
                mods.append({
                    "name": item.name,
                    "path": item,
                    "csproj": csproj_files[0]
                })
    return sorted(mods, key=lambda m: m["name"])


def cmd_new():
    """Create a new mod using the template."""
    if not TEMPLATE_SCRIPT.exists():
        print(f"Error: Template script not found at {TEMPLATE_SCRIPT}")
        return 1
    
    print("Creating new mod from template...\n")
    result = subprocess.run([sys.executable, str(TEMPLATE_SCRIPT)], cwd=TEMPLATE_DIR)
    return result.returncode


def cmd_list():
    """List all mods in the workspace."""
    mods = find_mods()
    
    if not mods:
        print("No mods found in workspace.")
        return 0
    
    print(f"Found {len(mods)} mod(s) in workspace:\n")
    for mod in mods:
        print(f"  • {mod['name']}")
        print(f"    └─ {mod['csproj'].name}")
    print()
    return 0


def cmd_build(mod_names=None):
    """Build one or more mods."""
    mods = find_mods()
    
    if not mods:
        print("No mods found in workspace.")
        return 1
    
    # Filter mods if specific names provided
    if mod_names:
        mods = [m for m in mods if m["name"] in mod_names]
        if not mods:
            print(f"Error: No matching mods found for: {', '.join(mod_names)}")
            return 1
    
    print(f"Building {len(mods)} mod(s)...\n")
    
    failed = []
    succeeded = []
    
    for mod in mods:
        print(f"{'=' * 60}")
        print(f"Building: {mod['name']}")
        print(f"{'=' * 60}")
        
        result = subprocess.run(
            ["dotnet", "build", "--configuration", "Debug"],
            cwd=mod["path"]
        )
        
        if result.returncode == 0:
            succeeded.append(mod["name"])
            print(f"✓ {mod['name']} built successfully\n")
        else:
            failed.append(mod["name"])
            print(f"✗ {mod['name']} build failed\n")
    
    # Summary
    print(f"{'=' * 60}")
    print("Build Summary")
    print(f"{'=' * 60}")
    print(f"Succeeded: {len(succeeded)}")
    print(f"Failed:    {len(failed)}")
    
    if succeeded:
        print(f"\n✓ Success: {', '.join(succeeded)}")
    if failed:
        print(f"\n✗ Failed:  {', '.join(failed)}")
    
    return 1 if failed else 0


def cmd_verify():
    """Verify that mods are configured to deploy to plugin directories."""
    mods = find_mods()
    
    if not mods:
        print("No mods found in workspace.")
        return 0
    
    print(f"Verifying deployment configuration for {len(mods)} mod(s)...\n")
    
    for mod in mods:
        print(f"Mod: {mod['name']}")
        
        # Read .csproj to check for PostBuild target
        csproj_content = mod["csproj"].read_text()
        
        if "PostBuild" in csproj_content and "Copy" in csproj_content:
            print("  ✓ PostBuild copy target found")
        else:
            print("  ✗ No PostBuild copy target found")
            print("    Add a PostBuild target to auto-copy DLL to plugin directory")
        
        print()
    
    return 0


def main():
    parser = argparse.ArgumentParser(
        description="Erenshor Mod Manager",
        formatter_class=argparse.RawDescriptionHelpFormatter,
        epilog="""
Examples:
  python mod_manager.py new                    # Create a new mod
  python mod_manager.py build                  # Build all mods
  python mod_manager.py build FastTravelMod    # Build specific mod
  python mod_manager.py list                   # List all mods
  python mod_manager.py verify                 # Verify deployment config
        """
    )
    
    parser.add_argument(
        "command",
        choices=["new", "build", "list", "verify"],
        help="Command to execute"
    )
    parser.add_argument(
        "mods",
        nargs="*",
        help="Mod name(s) for build command"
    )
    
    args = parser.parse_args()
    
    if args.command == "new":
        return cmd_new()
    elif args.command == "list":
        return cmd_list()
    elif args.command == "build":
        return cmd_build(args.mods if args.mods else None)
    elif args.command == "verify":
        return cmd_verify()
    
    return 0


if __name__ == "__main__":
    sys.exit(main())

