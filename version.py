#!/usr/bin/env python3
"""
Version bumping tool for mods.

Usage:
    python version.py <ModName> patch    # 1.0.0 -> 1.0.1
    python version.py <ModName> minor    # 1.0.0 -> 1.1.0
    python version.py <ModName> major    # 1.0.0 -> 2.0.0
    python version.py <ModName> set 2.5.3  # Set specific version
"""

import os
import sys
import json
import re
import argparse
import xml.etree.ElementTree as ET
from pathlib import Path

WORKSPACE_ROOT = Path(__file__).parent.resolve()
REGISTRY_FILE = WORKSPACE_ROOT / "mods.json"


def load_registry():
    """Load the mod registry."""
    if not REGISTRY_FILE.exists():
        return {}
    with open(REGISTRY_FILE, 'r') as f:
        return json.load(f)


def save_registry(registry):
    """Save the mod registry."""
    with open(REGISTRY_FILE, 'w') as f:
        json.dump(registry, f, indent=2)


def parse_version(version_str):
    """Parse a semantic version string."""
    match = re.match(r'^(\d+)\.(\d+)\.(\d+)$', version_str)
    if not match:
        return None
    return tuple(map(int, match.groups()))


def bump_version(version_str, bump_type):
    """Bump a version according to semantic versioning."""
    parts = parse_version(version_str)
    if not parts:
        return None
    
    major, minor, patch = parts
    
    if bump_type == "major":
        return f"{major + 1}.0.0"
    elif bump_type == "minor":
        return f"{major}.{minor + 1}.0"
    elif bump_type == "patch":
        return f"{major}.{minor}.{patch + 1}"
    
    return None


def update_csproj_version(csproj_path, new_version):
    """Update version in .csproj file."""
    try:
        tree = ET.parse(csproj_path)
        root = tree.getroot()
        
        # Find or create Version element
        version_elem = root.find(".//Version")
        if version_elem is None:
            # Find PropertyGroup or create one
            prop_group = root.find(".//PropertyGroup")
            if prop_group is None:
                prop_group = ET.SubElement(root, "PropertyGroup")
            version_elem = ET.SubElement(prop_group, "Version")
        
        version_elem.text = new_version
        
        # Write back
        tree.write(csproj_path, encoding='utf-8', xml_declaration=True)
        return True
    except Exception as e:
        print(f"Warning: Could not update .csproj: {e}")
        return False


def update_plugin_version(mod_path, mod_name, new_version):
    """Update version in Plugin.cs file."""
    plugin_file = mod_path / "Plugin.cs"
    if not plugin_file.exists():
        return False
    
    try:
        content = plugin_file.read_text()
        
        # Look for BepInPlugin attribute with version
        pattern = r'(\[BepInPlugin\([^,]+,\s*[^,]+,\s*")[^"]+("\)\])'
        replacement = rf'\g<1>{new_version}\g<2>'
        
        new_content = re.sub(pattern, replacement, content)
        
        if new_content != content:
            plugin_file.write_text(new_content)
            return True
        
        return False
    except Exception as e:
        print(f"Warning: Could not update Plugin.cs: {e}")
        return False


def update_changelog(mod_path, new_version):
    """Add entry to CHANGELOG.md."""
    changelog_file = mod_path / "CHANGELOG.md"
    
    from datetime import datetime
    date_str = datetime.now().strftime("%Y-%m-%d")
    
    new_entry = f"""## [{new_version}] - {date_str}

### Added
- 

### Changed
- 

### Fixed
- 

"""
    
    if changelog_file.exists():
        content = changelog_file.read_text()
        # Insert after the header
        lines = content.split('\n')
        insert_pos = 0
        for i, line in enumerate(lines):
            if line.startswith('# '):
                insert_pos = i + 1
                break
        
        lines.insert(insert_pos, new_entry)
        changelog_file.write_text('\n'.join(lines))
    else:
        # Create new changelog
        content = f"""# Changelog

All notable changes to this project will be documented in this file.

{new_entry}
"""
        changelog_file.write_text(content)
    
    return True


def bump_mod_version(mod_name, bump_type, new_version=None):
    """Bump version for a mod."""
    print(f"Bumping version for {mod_name}...")
    
    # Load registry
    registry = load_registry()
    
    if mod_name not in registry:
        print(f"✗ Error: Mod '{mod_name}' not found in registry")
        return False
    
    # Get current version
    current_version = registry[mod_name].get("version", "1.0.0")
    print(f"  Current version: {current_version}")
    
    # Calculate new version
    if new_version:
        if not parse_version(new_version):
            print(f"✗ Error: Invalid version format '{new_version}'. Use X.Y.Z")
            return False
    else:
        new_version = bump_version(current_version, bump_type)
        if not new_version:
            print(f"✗ Error: Could not bump version")
            return False
    
    print(f"  New version: {new_version}")
    
    # Find mod path
    mod_path = WORKSPACE_ROOT / mod_name
    if not mod_path.exists():
        print(f"✗ Error: Mod directory not found")
        return False
    
    # Update registry
    registry[mod_name]["version"] = new_version
    save_registry(registry)
    print(f"  ✓ Updated mods.json")
    
    # Update .csproj
    csproj_files = list(mod_path.glob("*.csproj"))
    if csproj_files:
        if update_csproj_version(csproj_files[0], new_version):
            print(f"  ✓ Updated {csproj_files[0].name}")
    
    # Update Plugin.cs
    if update_plugin_version(mod_path, mod_name, new_version):
        print(f"  ✓ Updated Plugin.cs")
    
    # Update CHANGELOG.md
    if update_changelog(mod_path, new_version):
        print(f"  ✓ Updated CHANGELOG.md")
    
    print(f"✓ Successfully bumped {mod_name} to v{new_version}")
    return True


def main():
    parser = argparse.ArgumentParser(description="Bump mod versions")
    parser.add_argument("mod_name", help="Name of the mod")
    parser.add_argument("bump_type", choices=["major", "minor", "patch", "set"],
                       help="Type of version bump")
    parser.add_argument("version", nargs="?", help="Specific version (for 'set' command)")
    parser.add_argument("--json", action="store_true", help="Output in JSON format")

    args = parser.parse_args()
    
    if args.bump_type == "set":
        if not args.version:
            print("Error: 'set' command requires a version argument")
            return 1
        return 0 if bump_mod_version(args.mod_name, args.bump_type, args.version) else 1
    else:
        return 0 if bump_mod_version(args.mod_name, args.bump_type) else 1


if __name__ == "__main__":
    sys.exit(main())

