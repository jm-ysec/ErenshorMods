#!/usr/bin/env python3
"""
Setup script for creating a new Erenshor BepInEx mod from the Template directory.

Usage:
    python setup_mod.py
"""

import os
import re
import shutil
import subprocess
import sys

SCRIPT_DIR = os.path.dirname(os.path.abspath(__file__))

# Template files -> output file name patterns
TEMPLATE_FILES = {
    "ModName.cs": "{mod_name}.cs",
    "ModName.csproj": "{mod_name}.csproj",
    "Plugin.cs": "Plugin.cs",
}

# Common Steam install locations for Erenshor
STEAM_GAME_PATHS = [
    os.path.expanduser("~/.steam/steam/steamapps/common/Erenshor"),
    os.path.expanduser("~/.local/share/Steam/steamapps/common/Erenshor"),
    os.path.join(os.environ.get("ProgramFiles(x86)", ""), "Steam", "steamapps", "common", "Erenshor"),
    os.path.join(os.environ.get("ProgramFiles", ""), "Steam", "steamapps", "common", "Erenshor"),
    os.path.expanduser("~/Library/Application Support/Steam/steamapps/common/Erenshor"),
]

# Common Gale mod manager BepInEx locations
GALE_PATHS = [
    os.path.expanduser("~/.local/share/com.kesomannen.gale/erenshor/profiles/Default/BepInEx"),
    os.path.join(os.environ.get("APPDATA", ""), "com.kesomannen.gale", "erenshor", "profiles", "Default", "BepInEx"),
    os.path.expanduser("~/Library/Application Support/com.kesomannen.gale/erenshor/profiles/Default/BepInEx"),
]


def detect_path(candidates):
    """Return the first existing directory from candidates, or None."""
    for path in candidates:
        if path and os.path.isdir(path):
            return path
    return None


def prompt(message, default=None, required=True):
    """Prompt the user for input with an optional default value."""
    suffix = f" [{default}]" if default else ""
    while True:
        user_input = input(f"{message}{suffix}: ").strip()
        value = user_input if user_input else default
        if value or not required:
            return value or ""
        print("  This field is required. Please enter a value.")


def main():
    print("=" * 60)
    print("  Erenshor BepInEx Mod Setup")
    print("=" * 60)
    print()

    # --- Gather info ---
    mod_name = prompt("Mod name (PascalCase, e.g. MyAwesomeMod)")
    if not re.match(r"^[A-Za-z_][A-Za-z0-9_]*$", mod_name):
        print("Error: Mod name must be a valid C# identifier (letters, digits, underscores).")
        sys.exit(1)

    author_name = prompt("Author name")
    description = prompt("Short description", default="A BepInEx mod for Erenshor")

    detected_game = detect_path(STEAM_GAME_PATHS)
    game_path = prompt("Path to Erenshor game directory", default=detected_game)

    detected_gale = detect_path(GALE_PATHS)
    gale_path = prompt("Path to Gale mod manager BepInEx directory", default=detected_gale)

    # --- Derived values ---
    mod_id = mod_name.lower()
    mod_namespace = mod_name
    mod_class_name = f"{mod_name}Patches"
    mod_assembly_name = mod_name
    # Insert spaces before uppercase letters: "MyAwesomeMod" -> "My Awesome Mod"
    mod_display_name = re.sub(r"(?<=[a-z])(?=[A-Z])", " ", mod_name)
    author_id = re.sub(r"[^a-z0-9]", "", author_name.lower())

    # --- Confirmation ---
    print()
    print("-" * 60)
    print("  Summary")
    print("-" * 60)
    print(f"  Mod Name:      {mod_name}")
    print(f"  Display Name:  {mod_display_name}")
    print(f"  Author:        {author_name}")
    print(f"  Namespace:     {mod_namespace}")
    print(f"  Plugin ID:     com.{author_id}.{mod_id}")
    print(f"  Description:   {description}")
    print(f"  Game Path:     {game_path}")
    print(f"  Gale Path:     {gale_path}")
    print(f"  Output Dir:    ../{mod_name}/")
    print("-" * 60)
    print()
    if input("Proceed? [Y/n]: ").strip().lower() not in ("", "y"):
        print("Aborted.")
        sys.exit(0)

    # --- Create output directory (sibling to Template) ---
    output_dir = os.path.normpath(os.path.join(SCRIPT_DIR, "..", mod_name))
    if os.path.exists(output_dir):
        print(f"Error: Directory already exists: {output_dir}")
        sys.exit(1)
    os.makedirs(output_dir)

    # --- Placeholder replacement maps ---
    replacements = {
        "__MOD_NAMESPACE__": mod_namespace,
        "__MOD_CLASS_NAME__": mod_class_name,
        "__MOD_ASSEMBLY_NAME__": mod_assembly_name,
        "__MOD_DESCRIPTION__": description,
        "__Mod Display Name__": mod_display_name,
        "__mod_id__": mod_id,
        "com.noone.": f"com.{author_id}.",
    }
    csproj_path_replacements = {
        "$(HOME)/.steam/steam/steamapps/common/Erenshor": game_path,
        "$(HOME)/.local/share/com.kesomannen.gale/erenshor/profiles/Default/BepInEx": gale_path,
    }

    # --- Generate files ---
    for template_name, output_pattern in TEMPLATE_FILES.items():
        template_path = os.path.join(SCRIPT_DIR, template_name)
        output_name = output_pattern.format(mod_name=mod_name)
        output_path = os.path.join(output_dir, output_name)

        with open(template_path, "r") as f:
            content = f.read()

        for placeholder, value in replacements.items():
            content = content.replace(placeholder, value)

        if template_name.endswith(".csproj"):
            for placeholder, value in csproj_path_replacements.items():
                content = content.replace(placeholder, value)

        with open(output_path, "w") as f:
            f.write(content)
        print(f"  Created: {output_name}")

    # --- Generate .sln via dotnet CLI ---
    print()
    if shutil.which("dotnet"):
        print("  Generating solution file...")
        try:
            subprocess.run(
                ["dotnet", "new", "sln", "--name", mod_name],
                cwd=output_dir, check=True, capture_output=True, text=True,
            )
            # dotnet may create .sln (older SDK) or .slnx (SDK 10+)
            sln_file = f"{mod_name}.slnx" if os.path.exists(os.path.join(output_dir, f"{mod_name}.slnx")) else f"{mod_name}.sln"
            subprocess.run(
                ["dotnet", "sln", sln_file, "add", f"{mod_name}.csproj"],
                cwd=output_dir, check=True, capture_output=True, text=True,
            )
            print(f"  Created: {sln_file}")
        except subprocess.CalledProcessError as e:
            print(f"  Warning: Failed to generate solution file: {e.stderr.strip()}")
            print(f"  You can create it manually later (see instructions below).")
    else:
        print("  Warning: 'dotnet' CLI not found on PATH.")
        print("  The solution file was NOT generated. To create it manually, run:")
        print(f"    cd ../{mod_name}")
        print(f"    dotnet new sln --name {mod_name}")
        print(f"    dotnet sln add {mod_name}.csproj")
        print()
        print("  If you don't have the .NET SDK installed, download it from:")
        print("    https://dotnet.microsoft.com/download")

    print()
    print(f"Done! Mod project created at: {os.path.abspath(output_dir)}")
    print()
    print("Next steps:")
    print(f"  cd ../{mod_name}")
    print(f"  dotnet restore")
    print(f"  dotnet build")


if __name__ == "__main__":
    main()

