---
type: auto
description: Step-by-step workflow for AI agents to create new mods from templates
---

# New Mod Creation Workflow

This document describes the process for setting up a new mod from templates based on a description of what the mod should accomplish.

## Overview

When asked to create a new mod, follow these steps:
1. Analyze the requirements to choose the right template
2. Create the mod using automation tools
3. Implement the mod logic
4. Build and verify
5. Create documentation

## Step 1: Analyze Requirements

Before creating a mod, determine which template is appropriate based on the mod's requirements:

| Template | When to Use | Example Mods |
|----------|-------------|--------------|
| `basic` | Simple Harmony patches only, no UI, no persistent state | FastTravelMod |
| `config` | Needs user-configurable settings via BepInEx config | MovementSpeedManager |
| `monobehaviour` | Needs Unity Update loop, OnGUI rendering, or persistent GameObject | RealWorldClock, BetterStatsPage, AreaMapMod |

### Decision Tree

```
Does the mod need to render UI (IMGUI/OnGUI)?
  YES → monobehaviour
  NO  → Does it need persistent state across frames?
          YES → monobehaviour
          NO  → Does it need user-configurable settings?
                  YES → config
                  NO  → basic
```

### Common Patterns

| Mod Type | Template | Key Features |
|----------|----------|--------------|
| HUD overlay (clock, stats) | monobehaviour | OnGUI() for rendering |
| Spell/skill modifications | basic | Harmony patches on CastSpell, UseSkill |
| Speed/stat adjustments | config | Harmony patches + ConfigEntry values |
| Map/UI replacements | monobehaviour | Scene load handling, texture loading |
| Hotkey-triggered features | monobehaviour | Update() for Input.GetKeyDown checks |

## Step 2: Create the Mod

Use the automation tool to create the mod structure:

```bash
python automate.py create --name ModName --template <template> --author "noone" --description "Description" --json
```

### Parameters

| Parameter | Value | Notes |
|-----------|-------|-------|
| `--name` | PascalCase name | Must be valid C# identifier |
| `--template` | `basic`, `config`, or `monobehaviour` | See decision tree above |
| `--author` | `"noone"` | Always use "noone" for this workspace |
| `--description` | Short description | Used in plugin metadata |
| `--json` | (flag) | Machine-readable output |

### What Gets Created

```
ModName/
├── ModName.cs         # Main mod logic (patches or controller)
├── ModName.csproj     # Build configuration
├── ModName.slnx       # Solution file
├── Plugin.cs          # BepInEx entry point
└── README.md          # Template README (needs replacement)
```

## Step 3: Implement the Mod Logic

### For `basic` Template

Edit `ModName.cs` to add Harmony patches:

```csharp
[HarmonyPatch(typeof(TargetClass), "MethodName")]
public class TargetClass_MethodName_Patch
{
    private static void Postfix(TargetClass __instance)
    {
        // Your logic here
    }
}
```

### For `config` Template

1. Edit `ModName.cs` to define config entries in the Config class
2. Add Harmony patches that reference config values

### For `monobehaviour` Template

1. Edit `ModName.cs` controller class
2. Implement lifecycle methods as needed:
   - `Awake()` - Initialization
   - `Update()` - Per-frame logic, input handling
   - `OnGUI()` - IMGUI rendering
   - `OnDestroy()` - Cleanup

### DontDestroyOnLoad Pattern

For MonoBehaviour mods that persist across scenes, use this proven pattern in Plugin.cs:

```csharp
private void Awake()
{
    GameObject controllerGO = new GameObject("ModNameController");
    controllerGO.transform.SetParent(null);
    controllerGO.hideFlags = HideFlags.HideAndDontSave;
    controllerGO.AddComponent<ModNameController>();
    Object.DontDestroyOnLoad(controllerGO);
}
```

### Referencing Game Objects

Consult `.augment/rules/game-objects.md` for:
- GameData static references (PlayerControl, PlayerStats, etc.)
- Database access patterns (SpellDB, ItemDB, etc.)
- UI system integration (EditUIMode, DraggingUIElement)

## Step 4: Build and Verify

```bash
# Build the mod
python build.py ModName

# Verify health
python doctor.py ModName --json
```

### Common Build Issues

| Issue | Solution |
|-------|----------|
| Missing Unity module | Add reference to .csproj |
| GameData not found | Add Assembly-CSharp reference |
| DLL not deployed | Check PostBuild target in .csproj |

## Step 5: Create Documentation

Replace the template README.md with proper documentation:

```markdown
# Mod Display Name

Brief description of what the mod does.

## Features

- Feature 1
- Feature 2

## Configuration

(If applicable - list config options)

## Usage

How to use the mod in-game.

## Based On

(If applicable)
This mod is based on [Original Mod Name](URL).
```

## Quick Reference

### Full Creation Command
```bash
python automate.py create --name MyMod --template monobehaviour --author "noone" --description "My mod description" --json
```

### Post-Creation Checklist
- [ ] Mod created successfully
- [ ] Implement logic in ModName.cs
- [ ] Build succeeds: `python build.py ModName`
- [ ] Health check passes: `python doctor.py ModName`
- [ ] Replace template README.md
- [ ] Test in game

