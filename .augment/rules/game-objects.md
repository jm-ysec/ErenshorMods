---
type: "always_apply"
---

# Erenshor Game Objects Reference

This document indexes the decompiled game classes available in the `GameLibrary/` folder for mod development reference.  If you notice an object reference you need that isn't here, ask and I will add it from ILSpy.  Make sure you update this list when you notice a new one.  

## Core Systems

| File | Class | Summary |
|------|-------|---------|
| `GameData.cs` | `GameData` | Central static hub for all game state - player references, databases, settings, damage types enum (`Physical`, `Magic`, `Elemental`, `Poison`, `Void`), scene management |
| `GameManager.cs` | `GameManager` | Main game controller - save/load, escape menu, zone announcements, UI window management, guild/party systems, server settings |
| `MainMenu.cs` | `MainMenu` | Main menu controller - login, backup system, server settings (XP/HP/Loot mods), graphics settings |

## Character System

| File | Class | Summary |
|------|-------|---------|
| `Character.cs` | `Character` | Base character class - handles alive state, damage/healing, targeting, audio, faction hits, DPS tracking |
| `Stats.cs` | `Stats` | Character stats - level, HP/Mana/Stamina, base stats (Str/End/Dex/Agi/Int/Wis/Cha), resists (MR/ER/PR/VR), status effects array, combat stance |
| `Class.cs` | `Class` | ScriptableObject defining character class - class name, stat scaling, base values |
| `ClassDB.cs` | `ClassDB` | Database manager for character classes - loads from Resources, lookup methods |
| `PlayerControl.cs` | `PlayerControl` | Player input/movement controller - targeting, camera, swimming, gamepad support, hunting aggro list |
| `PlayerCombat.cs` | `PlayerCombat` | Player combat logic - auto-attack toggle, DPS display, shield bonus tracking |
| `Stance.cs` | `Stance` | ScriptableObject for combat stances - modifiers for MaxHP, Damage, ProcRate, DamageTaken, Aggro, SpellDamage, Lifesteal, Resonance |
| `HardSetStats.cs` | `HardSetStats` | Boss/special NPC stat scaling based on ward NPCs alive |

## NPC System

| File | Class | Summary |
|------|-------|---------|
| `NPC.cs` | `NPC` | NPC controller - AI, aggro, pathing, attack types (`Hit`, `Slash`, `Stab`, `Bash`, `Claw`, `Bite`, `Crush`), SimPlayer support |

## Spell System

| File | Class | Summary |
|------|-------|---------|
| `Spell.cs` | `Spell` | ScriptableObject - spell data with `SpellType` enum (Damage, StatusEffect, Beneficial, AE, PBAE, Misc, Heal, Pet) and `SpellLine` enum for buff/debuff categories |
| `SpellDB.cs` | `SpellDB` | Spell database manager - loads from Resources/Spells, `GetSpellByID()` lookup |
| `CastSpell.cs` | `CastSpell` | Spell casting logic - `KnownSpells` list, casting state, cooldowns, spell resolution |
| `SpellBook.cs` | `SpellBook` | Spellbook UI manager - paginated display (12 per page), hotkey assignment |
| `SpellbookSlot.cs` | `SpellbookSlot` | Individual spellbook slot UI - click/drag handling, stat display |
| `SpellEffectDB.cs` | `SpellEffectDB` | Spell visual effects database - particle systems, pet prefabs, lighting effects |
| `StatusEffect.cs` | `StatusEffect` | Active status effect instance - spell reference, duration, owner, DPS credit |
| `StatusEffectIcon.cs` | `StatusEffectIcon` | Status effect UI icon - tooltip, duration display |
| `StatusEffectSaveData.cs` | `StatusEffectSaveData` | Serializable status effect for save/load |
| `WandBolt.cs` | `WandBolt` | Projectile logic for wand/bow attacks - damage delivery, proc effects |

## Skill System

| File | Class | Summary |
|------|-------|---------|
| `UseSkill.cs` | `UseSkill` | Skill execution - `KnownSkills` list, `MyAscensions` list, `AscensionPoints`, shield requirement checks |
| `SkillDB.cs` | `SkillDB` | Skill/Ascension database - loads from Resources, stance references (Normal, Aggressive, Reckless, Taunting, Defensive) |
| `SkillBook.cs` | `SkillBook` | Skillbook UI manager - paginated display (12 per page), hotkey assignment |
| `SkillbookSlot.cs` | `SkillbookSlot` | Individual skillbook slot UI - click/drag handling, stance display |

## Item System

| File | Class | Summary |
|------|-------|---------|
| `Item.cs` | `Item` | ScriptableObject - item data with `SlotType` enum (General, Head, Chest, Legs, etc.), stats, procs |
| `ItemDatabase.cs` | `ItemDatabase` | Item database manager - loads from Resources/Items, `GetItemByID()` lookup |
| `Inventory.cs` | `Inventory` | Player/NPC inventory - equipment slots, stored items, gold, modular appearance |
| `ItemIcon.cs` | `ItemIcon` | Item slot UI component - drag/drop, quality display, quantity stacking |
| `ItemIconData.cs` | `ItemIconData` | Serializable item icon state |
| `ItemInfoWindow.cs` | `ItemInfoWindow` | Item tooltip/info popup |
| `ItemSaveData.cs` | `ItemSaveData` | Serializable item data for save/load |
| `ItemSearchEntry.cs` | `ItemSearchEntry` | Item search/filter entry |
| `ItemToHK.cs` | `ItemToHK` | Item-to-hotkey assignment |

## Loot System

| File | Class | Summary |
|------|-------|---------|
| `LootTable.cs` | `LootTable` | Loot generation - rarity pools (Common/Uncommon/Rare/Legendary/UltraRare), special drops (Sivak, Planar, XP Pot), quality system |
| `LootWindow.cs` | `LootWindow` | Loot window UI - item display, loot all, destroy all |

## Vendor System

| File | Class | Summary |
|------|-------|---------|
| `VendorInventory.cs` | `VendorInventory` | NPC vendor data - items for sale, quest unlocks |
| `VendorWindow.cs` | `VendorWindow` | Vendor UI - buy/sell transactions, pagination, buyback |

## UI Components

| File | Class | Summary |
|------|-------|---------|
| `DragUI.cs` | `DragUI` | Draggable UI element - shows blue handle in edit mode, saves position to PlayerPrefs, registers with `GameData.AllUIElements` |
| `Minimap.cs` | `Minimap` | Minimap controller - zoom, north lock, big/small toggle |
| `UIAnchors.cs` | `UIAnchors` | UI anchor transform references (Inv, Status, Chat, Combat, Hotbar, Group, Char) |
| `SetBind.cs` | `SetBind` | Respawn bind point UI and logic |

## Key Properties & Access Patterns

### Getting Player References
```csharp
GameData.PlayerControl      // PlayerControl component
GameData.PlayerStats        // Stats component
GameData.PlayerInv          // Inventory component
GameData.PlayerCombat       // PlayerCombat component
GameData.GM                 // GameManager instance
```

### Database Access
```csharp
GameData.SpellDatabase.GetSpellByID(string id)
GameData.ItemDB.GetItemByID(int id)
GameData.SkillDatabase.GetSkillByID(string id)
GameData.SkillDatabase.GetSkillByName(string name)
GameData.ClassDB            // Class database
```

### Known Spells/Skills Lists
```csharp
GameData.PlayerControl.GetComponent<CastSpell>().KnownSpells  // List<Spell>
GameData.PlayerControl.GetComponent<UseSkill>().KnownSkills   // List<Skill>
```

### Status Effects
```csharp
GameData.PlayerStats.StatusEffects[0..29]  // StatusEffect[] array
```

### UI Edit Mode
```csharp
GameData.EditUIMode              // bool - true when "Toggle UI Movement" is active
GameData.AllUIElements           // List<DragUI> - all registered draggable UI elements
GameData.DraggingUIElement       // bool - true while a UI element is being dragged
```

## Damage Types
```csharp
GameData.DamageType.Physical
GameData.DamageType.Magic
GameData.DamageType.Elemental
GameData.DamageType.Poison
GameData.DamageType.Void
```

## Save/Load
Game data is saved via `GameManager.SaveGameData(bool wScene)` which serializes to `CharacterSlot` and writes to persistent data path.

