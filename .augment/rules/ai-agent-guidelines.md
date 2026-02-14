---
type: always
description: Guidelines for AI agents working in this workspace
---

# AI Agent Guidelines

## Core Principles

1. **Use automation tools** - Never manually perform tasks that have tools
2. **Check before acting** - Run `doctor.py` to validate state
3. **Use JSON output** - Always add `--json` flag for programmatic operations
4. **Verify exit codes** - 0 = success, non-zero = failure
5. **Update registry** - Keep `mods.json` in sync with changes

## When Creating Mods

### DO
```bash
# Use automation
python automate.py create --name MyMod --template config --author "Name" --description "Desc" --json

# Verify creation
python doctor.py MyMod --json
```

### DON'T
- Create mod files from scratch
- Copy/paste template files manually
- Skip registry updates

### Template Selection
| Choose | When |
|--------|------|
| `basic` | Simple patches, no UI, no config |
| `config` | User settings needed (most common) |
| `monobehaviour` | Unity components, UI, Update loop |

## When Modifying Mods

### DO
```bash
# 1. Make code changes to .cs files
# 2. Build to verify
python build.py ModName --json

# 3. Health check
python doctor.py ModName --json

# 4. Bump version after changes
python version.py ModName patch --json
```

### DON'T
- Manually edit `.csproj` files - use `dotnet add package`
- Manually edit `mods.json` for complex changes - use `mod_registry.py`
- Forget to bump version after changes

## When Releasing Mods

### DO
```bash
# Complete release workflow
python automate.py release ModName --bump patch --json
```

### DON'T
- Skip health checks before packaging
- Forget to bump version
- Build in Debug mode for release

## Parsing Tool Output

### Check Success
```python
import subprocess, json

result = subprocess.run(
    ["python", "build.py", "ModName", "--json"],
    capture_output=True, text=True
)

if result.returncode == 0:
    data = json.loads(result.stdout)
    # Process success
else:
    # Handle failure
```

### Common JSON Fields
| Tool | Success Field |
|------|---------------|
| build.py | `successful > 0` |
| doctor.py | `healthy == total` |
| automate.py | `success == true` |

## Error Recovery

| Error | Recovery Action |
|-------|-----------------|
| "Mod not found in registry" | `python mod_registry.py add ModName` |
| "DLL not found" | `python build.py ModName` |
| "Health check failed" | `python doctor.py ModName --json` to see issues |
| "Template not found" | Use `basic`, `config`, or `monobehaviour` |

## Best Practices

1. **Start with health check** - `python doctor.py --json`
2. **Use automate.py** for high-level workflows
3. **Always use --json** for machine-readable output
4. **Check mods.json** before operations
5. **Run doctor.py** before commits
6. **Bump version** after any code changes
7. **Use watch mode** during development: `python watch.py ModName`

## Quick Reference

| Task | Command |
|------|---------|
| Create mod | `python automate.py create --name X --template config --json` |
| Build | `python build.py X --json` |
| Health check | `python doctor.py X --json` |
| Release | `python automate.py release X --bump patch --json` |
| Package | `python package.py X --json` |
| Version bump | `python version.py X patch --json` |
| List mods | `python mod_registry.py list --json` |

