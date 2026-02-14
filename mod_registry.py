#!/usr/bin/env python3
"""
Mod registry management tool.

Usage:
    python mod_registry.py list                           # List all registered mods
    python mod_registry.py add <ModName>                  # Add a mod to registry
    python mod_registry.py update <ModName> --version 1.2.0  # Update mod metadata
    python mod_registry.py sync                           # Sync registry with workspace
"""

import json
import sys
import argparse
from pathlib import Path

WORKSPACE_ROOT = Path(__file__).parent.resolve()
REGISTRY_FILE = WORKSPACE_ROOT / "mods.json"
EXCLUDE_DIRS = {"Template", ".git", "bin", "obj", "__pycache__", ".vs", "node_modules"}


def load_registry():
    """Load the mod registry."""
    if not REGISTRY_FILE.exists():
        return {}
    try:
        with open(REGISTRY_FILE, 'r') as f:
            return json.load(f)
    except json.JSONDecodeError:
        print(f"Warning: Could not parse {REGISTRY_FILE}")
        return {}


def save_registry(registry):
    """Save the mod registry."""
    with open(REGISTRY_FILE, 'w') as f:
        json.dump(registry, f, indent=2)


def find_mods_in_workspace():
    """Find all mod directories in the workspace."""
    mods = []
    for item in WORKSPACE_ROOT.iterdir():
        if item.is_dir() and item.name not in EXCLUDE_DIRS:
            csproj_files = list(item.glob("*.csproj"))
            if csproj_files:
                mods.append(item.name)
    return sorted(mods)


def cmd_list():
    """List all registered mods."""
    registry = load_registry()
    
    if not registry:
        print("No mods in registry.")
        return 0
    
    print(f"Registered mods ({len(registry)}):\n")
    for mod_name, metadata in sorted(registry.items()):
        status = "✓" if metadata.get("active", True) else "✗"
        version = metadata.get("version", "unknown")
        author = metadata.get("author", "unknown")
        print(f"{status} {mod_name:<30} v{version:<10} by {author}")
        if metadata.get("description"):
            print(f"  └─ {metadata['description']}")
        if metadata.get("dependencies"):
            print(f"  └─ Dependencies: {', '.join(metadata['dependencies'])}")
    
    return 0


def cmd_add(mod_name, **kwargs):
    """Add a mod to the registry."""
    registry = load_registry()
    
    if mod_name in registry:
        print(f"Mod '{mod_name}' already in registry. Use 'update' to modify.")
        return 1
    
    registry[mod_name] = {
        "version": kwargs.get("version", "1.0.0"),
        "author": kwargs.get("author", "YourName"),
        "description": kwargs.get("description", ""),
        "active": kwargs.get("active", True),
        "dependencies": kwargs.get("dependencies", []),
        "tags": kwargs.get("tags", [])
    }
    
    save_registry(registry)
    print(f"✓ Added '{mod_name}' to registry")
    return 0


def cmd_update(mod_name, **kwargs):
    """Update mod metadata in registry."""
    registry = load_registry()
    
    if mod_name not in registry:
        print(f"Mod '{mod_name}' not in registry. Use 'add' first.")
        return 1
    
    # Update fields that were provided
    if "version" in kwargs and kwargs["version"]:
        registry[mod_name]["version"] = kwargs["version"]
    if "author" in kwargs and kwargs["author"]:
        registry[mod_name]["author"] = kwargs["author"]
    if "description" in kwargs and kwargs["description"]:
        registry[mod_name]["description"] = kwargs["description"]
    if "active" in kwargs:
        registry[mod_name]["active"] = kwargs["active"]
    
    save_registry(registry)
    print(f"✓ Updated '{mod_name}' in registry")
    return 0


def cmd_sync():
    """Sync registry with workspace (add missing mods)."""
    registry = load_registry()
    workspace_mods = find_mods_in_workspace()
    
    added = []
    for mod_name in workspace_mods:
        if mod_name not in registry:
            registry[mod_name] = {
                "version": "1.0.0",
                "author": "YourName",
                "description": "",
                "active": True,
                "dependencies": [],
                "tags": []
            }
            added.append(mod_name)
    
    if added:
        save_registry(registry)
        print(f"✓ Added {len(added)} mod(s) to registry:")
        for mod in added:
            print(f"  - {mod}")
    else:
        print("Registry is already in sync with workspace.")
    
    return 0


def main():
    parser = argparse.ArgumentParser(description="Manage mod registry")
    subparsers = parser.add_subparsers(dest="command", help="Command to execute")
    
    # List command
    subparsers.add_parser("list", help="List all registered mods")
    
    # Add command
    add_parser = subparsers.add_parser("add", help="Add a mod to registry")
    add_parser.add_argument("mod_name", help="Name of the mod")
    add_parser.add_argument("--version", help="Version number")
    add_parser.add_argument("--author", help="Author name")
    add_parser.add_argument("--description", help="Mod description")
    
    # Update command
    update_parser = subparsers.add_parser("update", help="Update mod metadata")
    update_parser.add_argument("mod_name", help="Name of the mod")
    update_parser.add_argument("--version", help="New version number")
    update_parser.add_argument("--author", help="New author name")
    update_parser.add_argument("--description", help="New description")
    update_parser.add_argument("--active", type=bool, help="Set active status")
    
    # Sync command
    subparsers.add_parser("sync", help="Sync registry with workspace")
    
    args = parser.parse_args()
    
    if not args.command:
        parser.print_help()
        return 1
    
    if args.command == "list":
        return cmd_list()
    elif args.command == "add":
        kwargs = {k: v for k, v in vars(args).items() if k not in ('command', 'mod_name')}
        return cmd_add(args.mod_name, **kwargs)
    elif args.command == "update":
        kwargs = {k: v for k, v in vars(args).items() if k not in ('command', 'mod_name')}
        return cmd_update(args.mod_name, **kwargs)
    elif args.command == "sync":
        return cmd_sync()
    
    return 0


if __name__ == "__main__":
    sys.exit(main())

