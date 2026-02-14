# AI Agent Guide for ErenshorMods Workspace

This guide is specifically designed for AI agents (like Augment, Cursor, GitHub Copilot, etc.) to efficiently work with this mod development workspace.

> **Note:** For Augment Code users, detailed rules are in `.augment/rules/` directory and are automatically indexed by the Context Engine.

## Quick Reference

### High-Level Automation (Recommended for AI Agents)

The `automate.py` script provides simple, high-level commands for common workflows:

```bash
# Create a new mod
python automate.py create --name MyMod --template config --author "AuthorName" --description "Description"

# Start development session (watch mode)
python automate.py develop MyMod

# Complete release workflow (version bump + build + health check + package)
python automate.py release MyMod --bump patch

# Check status
python automate.py status MyMod
python automate.py status  # All mods
```

All commands support `--json` flag for machine-readable output.

## Common AI Agent Tasks

### Task 1: Create a New Mod

**User Request:** "Create a new mod called SpeedBoost that increases player movement speed"

**AI Agent Actions:**
```bash
# 1. Create the mod with config template (for user-configurable settings)
python automate.py create --name SpeedBoost --template config --author "UserName" --description "Increases player movement speed with configurable multiplier" --json

# 2. Verify creation
python doctor.py SpeedBoost --json

# 3. Report to user
# "Created SpeedBoost mod at /path/to/SpeedBoost. Ready for development."
```

**Template Selection Logic:**
- `basic` - Simple patches, no UI, no config
- `config` - User-configurable settings (most common)
- `monobehaviour` - Needs Unity components, UI, or Update loop

### Task 2: Modify Existing Mod

**User Request:** "Update FastTravelMod to also unlock all waypoints"

**AI Agent Actions:**
```bash
# 1. Start watch mode in background (optional but recommended)
python watch.py FastTravelMod &

# 2. Make code changes to FastTravelMod/FastTravelMod.cs

# 3. Watch mode auto-rebuilds, or manually build:
python build.py FastTravelMod --json

# 4. Verify health
python doctor.py FastTravelMod --json

# 5. If all good, bump version
python version.py FastTravelMod patch --json
```

### Task 3: Release a Mod

**User Request:** "Package FastTravelMod for release"

**AI Agent Actions:**
```bash
# Single command does everything:
python automate.py release FastTravelMod --bump patch --json

# Or manually:
# 1. Bump version
python version.py FastTravelMod patch --json

# 2. Build in release mode
python build.py FastTravelMod --release --json

# 3. Health check
python doctor.py FastTravelMod --json

# 4. Package
python package.py FastTravelMod --json

# Output will be in releases/FastTravelMod-X.Y.Z.zip
```

### Task 4: Build All Mods

**User Request:** "Build all mods"

**AI Agent Actions:**
```bash
# Parallel build (faster)
python build.py --parallel --jobs 4 --json

# Sequential build
python build.py --json
```

### Task 5: Check Mod Health

**User Request:** "Check if my mods are configured correctly"

**AI Agent Actions:**
```bash
# Check all mods
python doctor.py --json

# Check specific mod
python doctor.py ModName --json
```

**Parse JSON output to identify issues:**
```json
{
  "total": 3,
  "healthy": 2,
  "mods": [
    {
      "mod": "FastTravelMod",
      "healthy": true,
      "issues": []
    },
    {
      "mod": "AreaMapMod",
      "healthy": false,
      "issues": [
        "⚠ No icon.png (required for Thunderstore)",
        "⚠ Author not set in registry"
      ]
    }
  ]
}
```

## Error Handling

### Exit Codes
- `0` = Success
- `Non-zero` = Failure

Always check exit codes when running commands programmatically.

### Common Errors and Solutions

**Error:** "Mod not found in registry"
```bash
# Solution: Add to registry
python mod_registry.py add ModName --author "Name" --description "Desc"
```

**Error:** "DLL not found. Build the mod first."
```bash
# Solution: Build the mod
python build.py ModName
```

**Error:** "No icon.png (required for Thunderstore)"
```bash
# Solution: Create or copy a 256x256 PNG icon
# AI agents should inform user to provide an icon
```

**Error:** "Health check failed"
```bash
# Solution: Run doctor to see specific issues
python doctor.py ModName --json
# Then fix issues based on output
```

## JSON Output Parsing

All tools support `--json` flag for machine-readable output.

### Build Output
```json
{
  "total": 2,
  "successful": 2,
  "failed": 0,
  "configuration": "Debug",
  "parallel": true,
  "mods": [
    {
      "name": "FastTravelMod",
      "success": true,
      "build_time": 2.34,
      "dll_path": "/path/to/FastTravelMod.dll"
    }
  ]
}
```

### Doctor Output
```json
{
  "total": 1,
  "healthy": 1,
  "mods": [
    {
      "mod": "FastTravelMod",
      "healthy": true,
      "issues": []
    }
  ]
}
```

### Registry List Output
```bash
python mod_registry.py list --json
```
```json
{
  "FastTravelMod": {
    "version": "1.0.0",
    "author": "AuthorName",
    "description": "Fixes fast travel",
    "active": true,
    "dependencies": [],
    "tags": ["utility"]
  }
}
```

## Best Practices for AI Agents

1. **Always use `--json` flag** when calling tools programmatically
2. **Check exit codes** to determine success/failure
3. **Run health checks** before and after modifications
4. **Use `automate.py`** for high-level workflows
5. **Update mods.json** when creating or modifying mods
6. **Never manually edit .csproj files** - use dotnet CLI
7. **Use watch mode** during development for auto-rebuild
8. **Bump versions** after making changes
9. **Run doctor.py** before packaging for release
10. **Parse JSON output** to provide detailed feedback to users

## File Locations

- **Mod registry:** `mods.json` (source of truth for metadata)
- **Build output:** `ModName/bin/Debug/net472/ModName.dll`
- **Packages:** `releases/ModName-X.Y.Z.zip`
- **Templates:** `Templates/basic/`, `Templates/config/`, `Templates/monobehaviour/`

## Integration Examples

### Python Integration
```python
import subprocess
import json

def build_mod(mod_name):
    result = subprocess.run(
        ["python", "build.py", mod_name, "--json"],
        capture_output=True,
        text=True
    )
    
    if result.returncode == 0:
        data = json.loads(result.stdout)
        return data["successful"] > 0
    return False
```

### Shell Integration
```bash
#!/bin/bash
# Build and package if successful
if python build.py MyMod --release --json; then
    python package.py MyMod --json
fi
```

## Troubleshooting

If a command fails:
1. Run with `--json` to get structured error info
2. Run `python doctor.py ModName --json` to diagnose
3. Check `mods.json` for correct metadata
4. Verify mod directory structure matches template

## Summary

For AI agents, the recommended workflow is:
1. Use `automate.py` for high-level operations
2. Always use `--json` flag for programmatic usage
3. Check exit codes and parse JSON output
4. Run health checks before and after changes
5. Provide clear feedback to users based on structured output

