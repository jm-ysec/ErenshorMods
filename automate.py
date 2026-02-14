#!/usr/bin/env python3
"""
High-level automation for common mod development workflows.
Designed for easy use by AI agents and developers.

Usage:
    python automate.py create --name MyMod --template config --author "Name" --description "Desc"
    python automate.py develop MyMod
    python automate.py release MyMod --bump patch
    python automate.py status [ModName]
"""

import os
import sys
import json
import subprocess
import argparse
from pathlib import Path

WORKSPACE_ROOT = Path(__file__).parent.resolve()


def run_command(cmd, cwd=None, capture=False):
    """Run a command and return success status."""
    if cwd is None:
        cwd = WORKSPACE_ROOT
    
    if capture:
        result = subprocess.run(cmd, cwd=cwd, capture_output=True, text=True)
        return result.returncode == 0, result.stdout, result.stderr
    else:
        result = subprocess.run(cmd, cwd=cwd)
        return result.returncode == 0, "", ""


def create_mod(name, template="basic", author="YourName", description="A mod for Erenshor", json_output=False):
    """Create a new mod from template."""
    import re
    import shutil

    if json_output:
        result = {"action": "create", "mod": name, "success": False}

    # Validate mod name
    if not re.match(r"^[A-Za-z_][A-Za-z0-9_]*$", name):
        error = "Mod name must be a valid C# identifier (letters, digits, underscores)"
        if json_output:
            result["error"] = error
            print(json.dumps(result))
            return False
        else:
            print(f"✗ Error: {error}")
            return False

    # Map template names
    template_map = {
        "basic": "Templates/basic",
        "monobehaviour": "Templates/monobehaviour",
        "config": "Templates/config"
    }

    template_dir = template_map.get(template.lower(), "Template")
    template_path = WORKSPACE_ROOT / template_dir

    if not template_path.exists():
        if json_output:
            result["error"] = f"Template '{template}' not found"
            print(json.dumps(result))
            return False
        else:
            print(f"✗ Error: Template '{template}' not found")
            return False

    # Check if mod already exists
    output_dir = WORKSPACE_ROOT / name
    if output_dir.exists():
        if json_output:
            result["error"] = f"Mod directory already exists: {output_dir}"
            print(json.dumps(result))
            return False
        else:
            print(f"✗ Error: Mod directory already exists: {output_dir}")
            return False

    try:
        # Create output directory
        output_dir.mkdir(parents=True)

        # Derived values
        mod_id = name.lower()
        mod_namespace = name
        mod_class_name = f"{name}Patches" if template != "monobehaviour" else name
        mod_assembly_name = name
        mod_display_name = re.sub(r"(?<=[a-z])(?=[A-Z])", " ", name)
        author_id = re.sub(r"[^a-z0-9]", "", author.lower())

        # Detect game/gale paths
        game_path = "$(HOME)/.steam/steam/steamapps/common/Erenshor"
        gale_path = "$(HOME)/.local/share/com.kesomannen.gale/erenshor/profiles/Default/BepInEx"

        steam_paths = [
            Path.home() / ".steam/steam/steamapps/common/Erenshor",
            Path.home() / ".local/share/Steam/steamapps/common/Erenshor",
        ]
        for p in steam_paths:
            if p.exists():
                game_path = str(p)
                break

        gale_paths = [
            Path.home() / ".local/share/com.kesomannen.gale/erenshor/profiles/Default/BepInEx",
        ]
        for p in gale_paths:
            if p.exists():
                gale_path = str(p)
                break

        # Template files mapping
        template_files = {
            "ModName.cs": f"{name}.cs",
            "ModName.csproj": f"{name}.csproj",
            "Plugin.cs": "Plugin.cs",
            "README.md": "README.md",
        }

        # Placeholder replacements
        replacements = {
            "__MOD_NAMESPACE__": mod_namespace,
            "__MOD_CLASS_NAME__": mod_class_name,
            "__MOD_ASSEMBLY_NAME__": mod_assembly_name,
            "__MOD_DESCRIPTION__": description,
            "__Mod Display Name__": mod_display_name,
            "__mod_id__": mod_id,
            "com.noone.": f"com.{author_id}.",
        }
        csproj_replacements = {
            "$(HOME)/.steam/steam/steamapps/common/Erenshor": game_path,
            "$(HOME)/.local/share/com.kesomannen.gale/erenshor/profiles/Default/BepInEx": gale_path,
        }

        # Copy and transform template files
        for template_name, output_name in template_files.items():
            template_file = template_path / template_name
            if not template_file.exists():
                continue

            content = template_file.read_text()

            for placeholder, value in replacements.items():
                content = content.replace(placeholder, value)

            if template_name.endswith(".csproj"):
                for placeholder, value in csproj_replacements.items():
                    content = content.replace(placeholder, value)

            (output_dir / output_name).write_text(content)

        # Generate solution file
        if shutil.which("dotnet"):
            subprocess.run(
                ["dotnet", "new", "sln", "--name", name],
                cwd=output_dir, check=True, capture_output=True, text=True
            )
            # Handle both .sln and .slnx
            sln_file = f"{name}.slnx" if (output_dir / f"{name}.slnx").exists() else f"{name}.sln"
            subprocess.run(
                ["dotnet", "sln", sln_file, "add", f"{name}.csproj"],
                cwd=output_dir, check=True, capture_output=True, text=True
            )

        # Add to registry
        run_command([
            sys.executable, str(WORKSPACE_ROOT / "mod_registry.py"),
            "add", name, "--author", author, "--description", description
        ])

        if json_output:
            result["success"] = True
            result["path"] = str(output_dir)
            result["template"] = template
            print(json.dumps(result))
        else:
            print(f"✓ Created mod: {name}")
            print(f"  Template: {template}")
            print(f"  Path: {output_dir}")
        return True

    except Exception as e:
        # Clean up on failure
        if output_dir.exists():
            shutil.rmtree(output_dir)

        if json_output:
            result["error"] = str(e)
            print(json.dumps(result))
        else:
            print(f"✗ Failed to create mod: {name}")
            print(f"  Error: {e}")
        return False


def develop_mod(name, json_output=False):
    """Start development session (watch mode)."""
    if json_output:
        result = {"action": "develop", "mod": name, "success": False}
    
    # Check if mod exists
    mod_path = WORKSPACE_ROOT / name
    if not mod_path.exists():
        if json_output:
            result["error"] = f"Mod '{name}' not found"
            print(json.dumps(result))
            return False
        else:
            print(f"✗ Error: Mod '{name}' not found")
            return False
    
    if not json_output:
        print(f"Starting development session for {name}...")
        print("Watch mode will auto-rebuild on file changes.")
        print("Press Ctrl+C to stop.")
    
    # Start watch mode
    success, _, _ = run_command([
        sys.executable, str(WORKSPACE_ROOT / "watch.py"), name
    ])
    
    if json_output:
        result["success"] = success
        print(json.dumps(result))
    
    return success


def release_mod(name, bump="patch", json_output=False):
    """Complete release workflow: version bump, build, health check, package."""
    if json_output:
        result = {"action": "release", "mod": name, "success": False, "steps": {}}
    else:
        print(f"Starting release workflow for {name}...")
        print(f"Version bump: {bump}")
    
    # Step 1: Version bump
    if not json_output:
        print("\n[1/4] Bumping version...")
    
    success, _, _ = run_command([
        sys.executable, str(WORKSPACE_ROOT / "version.py"), name, bump
    ])
    
    if json_output:
        result["steps"]["version_bump"] = success
    elif not success:
        print("✗ Version bump failed")
        return False
    else:
        print("✓ Version bumped")
    
    # Step 2: Build in release mode
    if not json_output:
        print("\n[2/4] Building in release mode...")
    
    success, _, _ = run_command([
        sys.executable, str(WORKSPACE_ROOT / "build.py"), name, "--release"
    ])
    
    if json_output:
        result["steps"]["build"] = success
    elif not success:
        print("✗ Build failed")
        return False
    else:
        print("✓ Build successful")
    
    # Step 3: Health check
    if not json_output:
        print("\n[3/4] Running health check...")
    
    success, _, _ = run_command([
        sys.executable, str(WORKSPACE_ROOT / "doctor.py"), name
    ])
    
    if json_output:
        result["steps"]["health_check"] = success
    elif not success:
        print("✗ Health check failed")
        return False
    else:
        print("✓ Health check passed")
    
    # Step 4: Package
    if not json_output:
        print("\n[4/4] Packaging for distribution...")
    
    success, _, _ = run_command([
        sys.executable, str(WORKSPACE_ROOT / "package.py"), name
    ])
    
    if json_output:
        result["steps"]["package"] = success
        result["success"] = all(result["steps"].values())
        print(json.dumps(result))
    elif not success:
        print("✗ Packaging failed")
        return False
    else:
        print("✓ Package created")
        print(f"\n✓ Release workflow complete for {name}")
    
    return success


def status_mod(name=None, json_output=False):
    """Show status of mod(s)."""
    if name:
        # Single mod status
        success, stdout, _ = run_command([
            sys.executable, str(WORKSPACE_ROOT / "doctor.py"), name, "--json" if json_output else ""
        ], capture=True)
        
        if json_output:
            print(stdout)
        return success
    else:
        # All mods status
        success, stdout, _ = run_command([
            sys.executable, str(WORKSPACE_ROOT / "mod_registry.py"), "list", "--json" if json_output else ""
        ], capture=True)
        
        if json_output:
            print(stdout)
        return success


def main():
    parser = argparse.ArgumentParser(description="High-level mod automation")
    parser.add_argument("--json", action="store_true", help="Output in JSON format")
    
    subparsers = parser.add_subparsers(dest="command", help="Command to run")
    
    # Create command
    create_parser = subparsers.add_parser("create", help="Create a new mod")
    create_parser.add_argument("--name", required=True, help="Mod name")
    create_parser.add_argument("--template", default="basic", 
                              choices=["basic", "monobehaviour", "config"],
                              help="Template type")
    create_parser.add_argument("--author", default="YourName", help="Author name")
    create_parser.add_argument("--description", default="A mod for Erenshor", help="Mod description")
    
    # Develop command
    develop_parser = subparsers.add_parser("develop", help="Start development session")
    develop_parser.add_argument("mod_name", help="Mod name")
    
    # Release command
    release_parser = subparsers.add_parser("release", help="Release workflow")
    release_parser.add_argument("mod_name", help="Mod name")
    release_parser.add_argument("--bump", default="patch", 
                               choices=["major", "minor", "patch"],
                               help="Version bump type")
    
    # Status command
    status_parser = subparsers.add_parser("status", help="Show mod status")
    status_parser.add_argument("mod_name", nargs="?", help="Mod name (optional)")
    
    args = parser.parse_args()
    
    if args.command == "create":
        success = create_mod(args.name, args.template, args.author, args.description, args.json)
    elif args.command == "develop":
        success = develop_mod(args.mod_name, args.json)
    elif args.command == "release":
        success = release_mod(args.mod_name, args.bump, args.json)
    elif args.command == "status":
        success = status_mod(args.mod_name, args.json)
    else:
        parser.print_help()
        return 1
    
    return 0 if success else 1


if __name__ == "__main__":
    sys.exit(main())

