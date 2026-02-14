#!/usr/bin/env python3
"""
Interactive mod creation tool with template selection.

Usage:
    python new_mod.py
"""

import os
import sys
import subprocess
from pathlib import Path

WORKSPACE_ROOT = Path(__file__).parent.resolve()

TEMPLATES = {
    "1": {
        "name": "Basic (Harmony Patches)",
        "path": WORKSPACE_ROOT / "Templates" / "basic",
        "description": "Simple mod with Harmony patches only"
    },
    "2": {
        "name": "MonoBehaviour",
        "path": WORKSPACE_ROOT / "Templates" / "monobehaviour",
        "description": "Mod with Unity MonoBehaviour component for persistent logic and UI"
    },
    "3": {
        "name": "Config",
        "path": WORKSPACE_ROOT / "Templates" / "config",
        "description": "Mod with BepInEx configuration system"
    }
}


def select_template():
    """Interactive template selection."""
    print("\n" + "=" * 60)
    print("Select a mod template:")
    print("=" * 60)
    
    for key, template in TEMPLATES.items():
        print(f"\n{key}. {template['name']}")
        print(f"   {template['description']}")
    
    print("\n" + "=" * 60)
    
    while True:
        choice = input("Enter template number (1-3): ").strip()
        if choice in TEMPLATES:
            return TEMPLATES[choice]
        print("Invalid choice. Please enter 1, 2, or 3.")


def main():
    print("\n" + "=" * 60)
    print("Erenshor Mod Creator")
    print("=" * 60)
    
    # Select template
    template = select_template()
    
    print(f"\nUsing template: {template['name']}")
    print(f"Template path: {template['path']}")
    
    # Check if template has setup script
    setup_script = template['path'] / "setup_mod.py"

    if not setup_script.exists():
        # Copy setup script from basic template
        base_setup = WORKSPACE_ROOT / "Templates" / "basic" / "setup_mod.py"
        if base_setup.exists():
            import shutil
            shutil.copy2(base_setup, setup_script)
            print(f"Copied setup script to {template['path']}")
    
    # Run the template's setup script
    if setup_script.exists():
        print("\nRunning mod setup...\n")
        result = subprocess.run([sys.executable, str(setup_script)], cwd=template['path'])
        return result.returncode
    else:
        print(f"Error: Setup script not found at {setup_script}")
        return 1


if __name__ == "__main__":
    sys.exit(main())

