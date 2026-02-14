#!/usr/bin/env python3
"""
Watch mode for auto-rebuilding mods on file changes.

Usage:
    python watch.py <ModName>     # Watch specific mod
    python watch.py --all          # Watch all mods
"""

import os
import sys
import time
import subprocess
import argparse
from pathlib import Path
from datetime import datetime

WORKSPACE_ROOT = Path(__file__).parent.resolve()
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


def get_file_mtimes(mod_path):
    """Get modification times of all .cs files in a mod."""
    mtimes = {}
    for cs_file in mod_path.rglob("*.cs"):
        # Skip bin and obj directories
        if any(part in ["bin", "obj"] for part in cs_file.parts):
            continue
        try:
            mtimes[cs_file] = cs_file.stat().st_mtime
        except OSError:
            pass
    return mtimes


def build_mod(mod_name, mod_path):
    """Build a mod."""
    print(f"\n{'=' * 60}")
    print(f"[{datetime.now().strftime('%H:%M:%S')}] Building {mod_name}...")
    print(f"{'=' * 60}")
    
    result = subprocess.run(
        ["dotnet", "build", "--configuration", "Debug", "-v", "minimal"],
        cwd=mod_path
    )
    
    if result.returncode == 0:
        print(f"✓ Build successful at {datetime.now().strftime('%H:%M:%S')}")
    else:
        print(f"✗ Build failed at {datetime.now().strftime('%H:%M:%S')}")
    
    return result.returncode == 0


def watch_mod(mod):
    """Watch a single mod for changes."""
    print(f"Watching {mod['name']} for changes...")
    print(f"Path: {mod['path']}")
    print(f"Press Ctrl+C to stop\n")
    
    # Initial build
    build_mod(mod['name'], mod['path'])
    
    # Track file modification times
    file_mtimes = get_file_mtimes(mod['path'])
    
    try:
        while True:
            time.sleep(1)  # Check every second
            
            # Get current modification times
            current_mtimes = get_file_mtimes(mod['path'])
            
            # Check for changes
            changed_files = []
            
            # Check for modified files
            for file_path, mtime in current_mtimes.items():
                if file_path not in file_mtimes or file_mtimes[file_path] != mtime:
                    changed_files.append(file_path)
            
            # Check for deleted files
            for file_path in file_mtimes:
                if file_path not in current_mtimes:
                    changed_files.append(file_path)
            
            if changed_files:
                print(f"\nDetected changes in:")
                for f in changed_files:
                    print(f"  - {f.relative_to(mod['path'])}")
                
                # Rebuild
                build_mod(mod['name'], mod['path'])
                
                # Update tracked times
                file_mtimes = current_mtimes
    
    except KeyboardInterrupt:
        print(f"\n\nStopped watching {mod['name']}")


def watch_all_mods(mods):
    """Watch all mods for changes (simplified version)."""
    print(f"Watching {len(mods)} mod(s) for changes...")
    print("Press Ctrl+C to stop\n")
    
    # Track file modification times for all mods
    all_file_mtimes = {}
    for mod in mods:
        all_file_mtimes[mod['name']] = get_file_mtimes(mod['path'])
    
    try:
        while True:
            time.sleep(1)
            
            for mod in mods:
                current_mtimes = get_file_mtimes(mod['path'])
                old_mtimes = all_file_mtimes[mod['name']]
                
                # Check for changes
                changed = False
                for file_path, mtime in current_mtimes.items():
                    if file_path not in old_mtimes or old_mtimes[file_path] != mtime:
                        changed = True
                        break
                
                if not changed:
                    for file_path in old_mtimes:
                        if file_path not in current_mtimes:
                            changed = True
                            break
                
                if changed:
                    build_mod(mod['name'], mod['path'])
                    all_file_mtimes[mod['name']] = current_mtimes
    
    except KeyboardInterrupt:
        print("\n\nStopped watching all mods")


def main():
    parser = argparse.ArgumentParser(description="Watch mods for changes and auto-rebuild")
    parser.add_argument("mod_name", nargs="?", help="Name of mod to watch")
    parser.add_argument("--all", "-a", action="store_true", help="Watch all mods")
    
    args = parser.parse_args()
    
    mods = find_mods()
    
    if not mods:
        print("No mods found in workspace.")
        return 1
    
    if args.all:
        watch_all_mods(mods)
    elif args.mod_name:
        mod = next((m for m in mods if m['name'] == args.mod_name), None)
        if not mod:
            print(f"Error: Mod '{args.mod_name}' not found.")
            print(f"\nAvailable mods:")
            for m in mods:
                print(f"  - {m['name']}")
            return 1
        watch_mod(mod)
    else:
        print("Error: Please specify a mod name or use --all")
        print(f"\nAvailable mods:")
        for m in mods:
            print(f"  - {m['name']}")
        return 1
    
    return 0


if __name__ == "__main__":
    sys.exit(main())

