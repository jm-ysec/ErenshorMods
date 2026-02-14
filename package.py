#!/usr/bin/env python3
"""
Package mods for distribution (Thunderstore/Gale format).

Usage:
    python package.py <ModName>              # Package a specific mod
    python package.py <ModName> --output ./releases  # Custom output directory
    python package.py --all                  # Package all active mods
"""

import os
import sys
import json
import shutil
import zipfile
import argparse
from pathlib import Path
from datetime import datetime

WORKSPACE_ROOT = Path(__file__).parent.resolve()
REGISTRY_FILE = WORKSPACE_ROOT / "mods.json"
EXCLUDE_DIRS = {"Template", ".git", "bin", "obj", "__pycache__", ".vs", "node_modules"}


def load_registry():
    """Load the mod registry."""
    if not REGISTRY_FILE.exists():
        return {}
    with open(REGISTRY_FILE, 'r') as f:
        return json.load(f)


def find_mod_path(mod_name):
    """Find the path to a mod directory."""
    mod_path = WORKSPACE_ROOT / mod_name
    if mod_path.exists() and mod_path.is_dir():
        return mod_path
    return None


def find_dll(mod_path, mod_name):
    """Find the built DLL for a mod."""
    # Check common build output locations
    possible_paths = [
        mod_path / "bin" / "Debug" / "net472" / f"{mod_name}.dll",
        mod_path / "bin" / "Debug" / f"{mod_name}.dll",
        mod_path / "bin" / "Release" / "net472" / f"{mod_name}.dll",
        mod_path / "bin" / "Release" / f"{mod_name}.dll",
    ]
    
    for dll_path in possible_paths:
        if dll_path.exists():
            return dll_path
    
    return None


def create_manifest(mod_name, metadata):
    """Create a Thunderstore manifest.json."""
    return {
        "name": mod_name,
        "version_number": metadata.get("version", "1.0.0"),
        "website_url": metadata.get("website_url", ""),
        "description": metadata.get("description", "A mod for Erenshor"),
        "dependencies": metadata.get("dependencies", [])
    }


def package_mod(mod_name, output_dir=None):
    """Package a mod for distribution."""
    print(f"\nPackaging {mod_name}...")
    
    # Load metadata
    registry = load_registry()
    metadata = registry.get(mod_name, {})
    
    # Find mod directory
    mod_path = find_mod_path(mod_name)
    if not mod_path:
        print(f"✗ Error: Mod directory not found for '{mod_name}'")
        return False
    
    # Find DLL
    dll_path = find_dll(mod_path, mod_name)
    if not dll_path:
        print(f"✗ Error: DLL not found for '{mod_name}'. Build the mod first.")
        return False
    
    # Set output directory
    if output_dir is None:
        output_dir = WORKSPACE_ROOT / "releases"
    else:
        output_dir = Path(output_dir)
    
    output_dir.mkdir(parents=True, exist_ok=True)
    
    # Create package directory
    version = metadata.get("version", "1.0.0")
    package_name = f"{mod_name}-{version}"
    package_dir = output_dir / package_name
    
    # Clean up old package directory if it exists
    if package_dir.exists():
        shutil.rmtree(package_dir)
    
    package_dir.mkdir(parents=True, exist_ok=True)
    
    # Copy DLL
    shutil.copy2(dll_path, package_dir / dll_path.name)
    print(f"  ✓ Copied DLL: {dll_path.name}")
    
    # Create manifest.json
    manifest = create_manifest(mod_name, metadata)
    with open(package_dir / "manifest.json", 'w') as f:
        json.dump(manifest, f, indent=2)
    print(f"  ✓ Created manifest.json")
    
    # Copy or create README.md
    readme_src = mod_path / "README.md"
    if readme_src.exists():
        shutil.copy2(readme_src, package_dir / "README.md")
        print(f"  ✓ Copied README.md")
    else:
        # Create basic README
        readme_content = f"""# {mod_name}

{metadata.get('description', 'A mod for Erenshor')}

## Installation

Install using Gale mod manager or manually extract to your BepInEx plugins folder.

## Version

{version}

## Author

{metadata.get('author', 'Unknown')}
"""
        with open(package_dir / "README.md", 'w') as f:
            f.write(readme_content)
        print(f"  ✓ Created README.md")
    
    # Copy or create icon.png
    icon_src = mod_path / "icon.png"
    if icon_src.exists():
        shutil.copy2(icon_src, package_dir / "icon.png")
        print(f"  ✓ Copied icon.png")
    else:
        print(f"  ⚠ Warning: icon.png not found (required for Thunderstore)")
    
    # Copy CHANGELOG.md if it exists
    changelog_src = mod_path / "CHANGELOG.md"
    if changelog_src.exists():
        shutil.copy2(changelog_src, package_dir / "CHANGELOG.md")
        print(f"  ✓ Copied CHANGELOG.md")
    
    # Create ZIP file
    zip_path = output_dir / f"{package_name}.zip"
    with zipfile.ZipFile(zip_path, 'w', zipfile.ZIP_DEFLATED) as zipf:
        for file_path in package_dir.rglob("*"):
            if file_path.is_file():
                arcname = file_path.relative_to(package_dir)
                zipf.write(file_path, arcname)
    
    print(f"  ✓ Created package: {zip_path}")
    
    # Clean up package directory
    shutil.rmtree(package_dir)
    
    print(f"✓ Successfully packaged {mod_name}")
    return True


def main():
    parser = argparse.ArgumentParser(description="Package mods for distribution")
    parser.add_argument("mod_name", nargs="?", help="Name of mod to package")
    parser.add_argument("--all", "-a", action="store_true", help="Package all active mods")
    parser.add_argument("--output", "-o", help="Output directory for packages")
    parser.add_argument("--json", action="store_true", help="Output in JSON format")

    args = parser.parse_args()
    
    if args.all:
        registry = load_registry()
        active_mods = [name for name, meta in registry.items() if meta.get("active", True)]
        
        if not active_mods:
            print("No active mods found in registry.")
            return 1
        
        print(f"Packaging {len(active_mods)} active mod(s)...")
        
        success_count = 0
        for mod_name in active_mods:
            if package_mod(mod_name, args.output):
                success_count += 1
        
        print(f"\n{'=' * 60}")
        print(f"Packaged {success_count}/{len(active_mods)} mods successfully")
        return 0 if success_count == len(active_mods) else 1
    
    elif args.mod_name:
        return 0 if package_mod(args.mod_name, args.output) else 1
    
    else:
        parser.print_help()
        return 1


if __name__ == "__main__":
    sys.exit(main())

