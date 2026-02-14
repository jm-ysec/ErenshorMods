---
type: auto
description: Complete reference for all automation tools in the workspace
---

# Tools Reference

All tools support `--json` flag for machine-readable output. Exit code 0 = success, non-zero = failure.

## High-Level Automation

### automate.py
Primary tool for AI agents. Combines multiple operations into single commands.

```bash
# Create new mod
python automate.py create --name MyMod --template config --author "Name" --description "Desc" --json

# Start development (watch mode)
python automate.py develop MyMod --json

# Complete release workflow
python automate.py release MyMod --bump patch --json

# Check status
python automate.py status MyMod --json
python automate.py status --json  # All mods
```

## Individual Tools

### build.py - Build System
```bash
python build.py                        # Build all mods
python build.py ModName                # Build specific mod
python build.py Mod1 Mod2              # Build multiple mods
python build.py --parallel --jobs 4    # Parallel build
python build.py --release              # Release mode
python build.py --clean                # Clean before build
python build.py --verify-deploy        # Verify DLL deployment
python build.py --json                 # JSON output
```

### doctor.py - Health Checks
```bash
python doctor.py                       # Check all mods
python doctor.py ModName               # Check specific mod
python doctor.py --json                # JSON output
```

**Checks performed:**
- Registry entry in mods.json
- .csproj configuration (TargetFramework, BepInEx refs, PostBuild)
- Required files (Plugin.cs, README.md, icon.png, CHANGELOG.md)
- Built DLL existence

### version.py - Version Management
```bash
python version.py ModName patch        # 1.0.0 → 1.0.1
python version.py ModName minor        # 1.0.0 → 1.1.0
python version.py ModName major        # 1.0.0 → 2.0.0
python version.py ModName set 2.5.3    # Set specific version
python version.py ModName patch --json # JSON output
```

**Files updated:**
- `mods.json` registry
- `[ModName].csproj` Version element
- `Plugin.cs` BepInPlugin attribute
- `CHANGELOG.md` new entry with date

### package.py - Distribution Packaging
```bash
python package.py ModName              # Package single mod
python package.py --all                # Package all active mods
python package.py ModName -o ./dist    # Custom output directory
python package.py --json               # JSON output
```

**Creates Thunderstore/Gale package:**
- `manifest.json` - Mod metadata
- `README.md` - Documentation
- `icon.png` - 256x256 icon
- `CHANGELOG.md` - Version history
- `[ModName].dll` - Built mod

### watch.py - File Watcher
```bash
python watch.py ModName                # Watch specific mod
python watch.py --all                  # Watch all mods
```

Auto-rebuilds when `.cs` files change. Press Ctrl+C to stop.

### mod_registry.py - Registry Management
```bash
python mod_registry.py add ModName                    # Add mod to registry
python mod_registry.py update ModName --version 1.1.0 # Update metadata
python mod_registry.py update ModName --author "Name"
python mod_registry.py update ModName --description "Desc"
python mod_registry.py list                           # List all mods
python mod_registry.py deactivate ModName             # Mark inactive
python mod_registry.py --json                         # JSON output
```

### new_mod.py - Template Selector
```bash
python new_mod.py  # Interactive template selection
```

Presents menu:
1. Basic (Harmony patches only)
2. MonoBehaviour (Unity component)
3. Config (BepInEx configuration)

### mod_manager.py - Unified CLI
```bash
python mod_manager.py new              # Create mod (interactive)
python mod_manager.py build [ModName]  # Build mods
python mod_manager.py list             # List all mods
python mod_manager.py verify           # Check deployment config
```

### setup_hooks.py - Git Hooks
```bash
python setup_hooks.py install          # Install pre-commit hooks
python setup_hooks.py uninstall        # Remove hooks
```

## JSON Output Examples

### Build Output
```json
{
  "total": 2,
  "successful": 2,
  "failed": 0,
  "configuration": "Debug",
  "parallel": true,
  "mods": [
    {"name": "ModName", "success": true, "build_time": 2.34}
  ]
}
```

### Doctor Output
```json
{
  "total": 1,
  "healthy": 1,
  "mods": [
    {"mod": "ModName", "healthy": true, "issues": []}
  ]
}
```

## Error Handling

| Error | Solution |
|-------|----------|
| "Mod not found in registry" | `python mod_registry.py add ModName` |
| "DLL not found" | `python build.py ModName` |
| "No icon.png" | Create 256x256 PNG icon |
| "Health check failed" | `python doctor.py ModName --json` |

