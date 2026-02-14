# Erenshor Mods Workspace

A comprehensive development workspace for creating BepInEx mods for Erenshor. This workspace provides a streamlined workflow with automation tools for building, testing, and distributing multiple mods.

**Designed for both human developers and AI agents** - all tools support JSON output and programmatic usage.

## Features

- 🚀 **Guided mod creation** with multiple template variants
- 🔨 **Automated build system** with parallel building support
- 📦 **Package mods** for Thunderstore/Gale distribution
- 🔍 **Health checks** to diagnose common issues
- 📝 **Version management** with semantic versioning
- 👀 **Watch mode** for auto-rebuild during development
- 🎯 **Mod registry** for centralized metadata management
- 🪝 **Git pre-commit hooks** for quality assurance
- 🤖 **AI agent friendly** - JSON output, clear exit codes, high-level automation

## Quick Start

### High-Level Automation (Recommended)

```bash
# Create a new mod
python automate.py create --name MyMod --template config --author "YourName" --description "Mod description"

# Start development session (watch mode)
python automate.py develop MyMod

# Complete release workflow (version + build + health check + package)
python automate.py release MyMod --bump patch

# Check status
python automate.py status MyMod
```

All commands support `--json` flag for machine-readable output (great for AI agents).

### Creating a New Mod (Interactive)

```bash
python new_mod.py
```

This interactive tool lets you choose from three template types:
1. **Basic** - Simple Harmony patches only
2. **MonoBehaviour** - Includes Unity component for persistent logic and UI
3. **Config** - Includes BepInEx configuration system

### Building Mods

```bash
# Build all mods
python build.py

# Build specific mods
python build.py FastTravelMod AreaMapMod

# Build in Release mode
python build.py --release

# Build with parallel processing
python build.py --parallel --jobs 4

# Clean before building
python build.py --clean
```

### Watch Mode (Auto-rebuild)

```bash
# Watch a specific mod
python watch.py FastTravelMod

# Watch all mods
python watch.py --all
```

### Packaging for Distribution

```bash
# Package a specific mod
python package.py FastTravelMod

# Package all active mods
python package.py --all

# Custom output directory
python package.py FastTravelMod --output ./releases
```

Packages are created in Thunderstore/Gale format with:
- `manifest.json` - Mod metadata
- `README.md` - Mod documentation
- `icon.png` - 256x256 icon (required for Thunderstore)
- `CHANGELOG.md` - Version history
- Mod DLL(s)

### Version Management

```bash
# Bump patch version (1.0.0 -> 1.0.1)
python version.py FastTravelMod patch

# Bump minor version (1.0.0 -> 1.1.0)
python version.py FastTravelMod minor

# Bump major version (1.0.0 -> 2.0.0)
python version.py FastTravelMod major

# Set specific version
python version.py FastTravelMod set 2.5.3
```

This automatically updates:
- `mods.json` registry
- `.csproj` Version element
- `Plugin.cs` BepInPlugin attribute
- `CHANGELOG.md` with new entry

### Health Checks

```bash
# Check all mods
python doctor.py

# Check specific mod
python doctor.py FastTravelMod
```

Validates:
- Registry entries (version, author, description)
- .csproj configuration (TargetFramework, BepInEx references, PostBuild)
- Required files (Plugin.cs, README.md, icon.png, CHANGELOG.md)
- Build output (DLL existence)

### Mod Registry

The `mods.json` file tracks metadata for all mods:

```json
{
  "FastTravelMod": {
    "version": "1.0.0",
    "author": "YourName",
    "description": "Fixes fast travel portal spells",
    "active": true,
    "dependencies": [],
    "tags": ["utility", "quality-of-life"]
  }
}
```

Use `mod_registry.py` to manage the registry:

```bash
# Add a mod to registry
python mod_registry.py add FastTravelMod

# Update mod metadata
python mod_registry.py update FastTravelMod --version 1.1.0 --author "YourName"

# List all mods
python mod_registry.py list

# Deactivate a mod
python mod_registry.py deactivate FastTravelMod
```

### Git Pre-commit Hooks

```bash
# Install hooks
python setup_hooks.py install

# Uninstall hooks
python setup_hooks.py uninstall
```

The pre-commit hook runs health checks before each commit. To skip: `git commit --no-verify`

## Mod Manager CLI

Unified command-line interface for common operations:

```bash
# Create new mod
python mod_manager.py new

# Build mods
python mod_manager.py build [ModName]

# List all mods
python mod_manager.py list

# Verify deployment configuration
python mod_manager.py verify
```

## Template Variants

### Basic Template
Simple mod with Harmony patches. Best for straightforward game modifications.

### MonoBehaviour Template
Includes a persistent Unity component with:
- Singleton pattern
- `Update()` loop for per-frame logic
- `OnGUI()` for custom UI
- DontDestroyOnLoad for scene persistence

### Config Template
Includes BepInEx configuration system with examples:
- Boolean settings
- Integer/Float with value ranges
- String settings
- Configuration binding in patches

## Tech Stack

- **.NET Framework 4.7.2** - Target framework for Erenshor
- **BepInEx** - Mod framework for Unity games
- **Harmony** - Runtime patching library
- **Python 3** - Build automation and tooling
- **Gale/Thunderstore** - Mod distribution platforms

## Project Structure

```
ErenshorMods/
├── FastTravelMod/          # Example mod
├── AreaMapMod/             # Example mod
├── MovementSpeedManager/   # Example mod
├── Template/               # Basic template
├── TemplateMonoBehaviour/  # MonoBehaviour template
├── TemplateConfig/         # Config template
├── mods.json               # Mod registry
├── build.py                # Build system
├── watch.py                # File watcher
├── package.py              # Distribution packager
├── version.py              # Version management
├── doctor.py               # Health checks
├── new_mod.py              # Template selector
├── mod_manager.py          # Unified CLI
├── mod_registry.py         # Registry management
└── setup_hooks.py          # Git hooks installer
```

## Workflow Example

```bash
# 1. Create a new mod with config support
python new_mod.py
# Select option 3 (Config template)
# Enter mod name: MyAwesomeMod

# 2. Start watch mode for auto-rebuild
python watch.py MyAwesomeMod

# 3. Edit your mod files
# (watch mode will auto-rebuild on save)

# 4. Run health check
python doctor.py MyAwesomeMod

# 5. Bump version when ready
python version.py MyAwesomeMod patch

# 6. Package for distribution
python package.py MyAwesomeMod

# 7. Install git hooks for quality assurance
python setup_hooks.py install
```

## For AI Agents

This workspace is designed to work seamlessly with AI coding assistants. See **[AI_AGENT_GUIDE.md](AI_AGENT_GUIDE.md)** for:
- High-level automation commands
- JSON output parsing
- Common task patterns
- Error handling
- Integration examples

Key features for AI agents:
- All tools support `--json` flag for machine-readable output
- Clear exit codes (0 = success, non-zero = failure)
- `automate.py` provides high-level workflows
- Comprehensive error messages
- Health checks for validation

## Contributing

When adding new mods:
1. Use `python new_mod.py` to create from template
2. Add mod to `mods.json` registry
3. Run `python doctor.py` to validate
4. Build and test with `python build.py`
5. Package with `python package.py` before distribution

## Documentation

- **[README.md](README.md)** - This file, general overview
- **[AI_AGENT_GUIDE.md](AI_AGENT_GUIDE.md)** - Quick guide for AI coding assistants
- **`.augment/rules/`** - Augment Code rules (auto-detected):
  - `project-overview.md` - Project identity and philosophy
  - `coding-standards.md` - C# code style guidelines
  - `workflows.md` - Development, build, release workflows
  - `tools-reference.md` - Complete tool documentation
  - `ai-agent-guidelines.md` - AI agent best practices

## License

Individual mods may have their own licenses. Check each mod's README for details.

