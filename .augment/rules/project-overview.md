---
type: always
description: Core project identity and philosophy for ErenshorMods workspace
---

# ErenshorMods Project Overview

## Project Identity

This is a **BepInEx mod development workspace** for Erenshor (Unity game). The workspace is designed for creating, building, and distributing **multiple small, focused mods** rather than one large mod.

## Development Philosophy

- **Multiple small mods** over one "giga-mod" - each mod should do one thing well
- **Streamlined workflow** - automate tedious tasks (building, versioning, packaging)
- **Quality assurance** - health checks and validation before commits
- **Distribution-ready** - easy packaging for Thunderstore/Gale mod managers
- **AI-agent friendly** - all tools support `--json` output for programmatic usage

## Technology Stack

| Component | Technology |
|-----------|------------|
| Target Framework | .NET Framework 4.7.2 |
| Mod Framework | BepInEx 5.x |
| Patching Library | Harmony 2.x |
| Build System | Python 3 automation scripts |
| Package Format | Thunderstore/Gale compatible |

## Source of Truth

- **`mods.json`** - Central registry tracking all mod metadata (version, author, description, dependencies)
- **`.csproj` files** - Build configuration and dependencies for each mod
- **`Plugin.cs`** - BepInEx entry point with version attribute

## Key Directories

| Directory | Purpose |
|-----------|---------|
| `Template/` | Basic mod template (Harmony patches only) |
| `TemplateMonoBehaviour/` | Template with Unity component |
| `TemplateConfig/` | Template with BepInEx configuration |
| `releases/` | Output directory for packaged mods |
| `[ModName]/` | Individual mod directories |

## File Structure Per Mod

```
[ModName]/
├── Plugin.cs           # BepInEx entry point (thin, delegates to logic)
├── [ModName].cs        # Mod logic and Harmony patches
├── [ModName].csproj    # Project file with PostBuild deployment
├── README.md           # Mod documentation
├── CHANGELOG.md        # Version history
└── icon.png            # 256x256 icon for Thunderstore
```

