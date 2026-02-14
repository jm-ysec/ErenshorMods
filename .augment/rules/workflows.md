---
type: auto
description: Development, build, and release workflows for mod development
---

# Development Workflows

## Creating a New Mod

### For AI Agents (Programmatic)
```bash
python automate.py create --name MyMod --template config --author "AuthorName" --description "Mod description" --json
```

### For Humans (Interactive)
```bash
python new_mod.py
```

### Template Selection
| Template | Use When |
|----------|----------|
| `basic` | Simple Harmony patches, no UI, no config |
| `config` | User-configurable settings (most common) |
| `monobehaviour` | Needs Unity components, UI, or Update loop |

### Post-Creation Checklist
1. ✅ Mod directory created with template files
2. ✅ Add to registry: `python mod_registry.py add MyMod`
3. ✅ Verify setup: `python doctor.py MyMod --json`
4. ✅ Initial build: `python build.py MyMod --json`

## Development Workflow

### Start Development Session
```bash
# Watch mode auto-rebuilds on file changes
python automate.py develop MyMod

# Or manually:
python watch.py MyMod
```

### Development Loop
1. Edit `.cs` files in mod directory
2. Watch mode auto-rebuilds on save
3. Test in game
4. Repeat

### Validate Changes
```bash
python doctor.py MyMod --json
```

## Release Workflow

### Complete Release (Recommended)
```bash
# Single command: version bump + build + health check + package
python automate.py release MyMod --bump patch --json
```

### Manual Release Steps
```bash
# 1. Bump version (updates mods.json, .csproj, Plugin.cs, CHANGELOG.md)
python version.py MyMod patch

# 2. Build in release mode
python build.py MyMod --release

# 3. Health check
python doctor.py MyMod

# 4. Package for distribution
python package.py MyMod

# Output: releases/MyMod-X.Y.Z.zip
```

### Version Bump Types
| Type | Example | When to Use |
|------|---------|-------------|
| `patch` | 1.0.0 → 1.0.1 | Bug fixes, minor changes |
| `minor` | 1.0.0 → 1.1.0 | New features, backward compatible |
| `major` | 1.0.0 → 2.0.0 | Breaking changes |

## Build Workflow

### Build All Mods
```bash
# Sequential
python build.py --json

# Parallel (faster)
python build.py --parallel --jobs 4 --json
```

### Build Specific Mods
```bash
python build.py ModName1 ModName2 --json
```

### Build Options
| Flag | Effect |
|------|--------|
| `--release` | Build in Release mode |
| `--clean` | Clean before building |
| `--parallel` | Enable parallel builds |
| `--jobs N` | Number of parallel jobs |
| `--json` | Machine-readable output |

## Health Check Workflow

### Before Committing
```bash
python doctor.py --json
```

### Diagnose Specific Mod
```bash
python doctor.py MyMod --json
```

### Common Issues Detected
- ⚠️ Missing icon.png (required for Thunderstore)
- ⚠️ Author not set in registry
- ⚠️ No PostBuild target for deployment
- ⚠️ Missing README.md or CHANGELOG.md
- ℹ️ No built DLL found

## Git Workflow

### Setup Pre-commit Hooks
```bash
python setup_hooks.py install
```

### Pre-commit Hook Actions
1. Runs `doctor.py` health checks
2. Blocks commit if issues found
3. Skip with: `git commit --no-verify`

## Quick Commands Reference

| Task | Command |
|------|---------|
| Create mod | `python automate.py create --name X --template config` |
| Develop | `python automate.py develop X` |
| Release | `python automate.py release X --bump patch` |
| Build all | `python build.py --parallel` |
| Check health | `python doctor.py --json` |
| Package | `python package.py X` |

