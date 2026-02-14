# Movement Speed Manager

A BepInEx mod for Erenshor that allows you to configure movement speeds for the player, NPCs, and SimPlayers.

## Features

- **Global Speed**: Set a base movement speed for all entities
- **Player Speed**: Override speed specifically for your character
- **NPC Speed**: Override speed for NPCs and monsters
- **SimPlayer Speed**: Override speed for SimPlayers (AI companions)
- **Grouped SimPlayer Speed**: Special speed for SimPlayers in your party

## Configuration

Edit the config file at `BepInEx/config/com.noone.movementspeedmanager.cfg`:

| Setting | Default | Description |
|---------|---------|-------------|
| GlobalSpeed | 6 | Base movement speed for all entities |
| PlayerSpeed | -1 | Player speed (-1 = use GlobalSpeed) |
| NPCSpeed | -1 | NPC/monster speed (-1 = use GlobalSpeed) |
| SimPlayerSpeed | -1 | SimPlayer speed (-1 = use GlobalSpeed) |
| GroupedSimPlayerSpeed | -1 | Grouped SimPlayer speed (-1 = use SimPlayerSpeed) |

### Speed Priority

- **Player**: PlayerSpeed > GlobalSpeed
- **NPCs/Monsters**: NPCSpeed > GlobalSpeed
- **SimPlayers (not grouped)**: SimPlayerSpeed > GlobalSpeed
- **SimPlayers (grouped)**: GroupedSimPlayerSpeed > SimPlayerSpeed > GlobalSpeed

## Usage

1. Install the mod
2. Launch the game once to generate the config file
3. Edit the config values as desired
4. Restart the game for changes to take effect

## Based On

This mod is based on [Erenshor Walking Speed Adjust Mod by Lenzork](https://thunderstore.io/c/erenshor/p/Lenzork/Erenshor_Walking_Speed_Adjust_Mod/).

Enhancements include:
- Separate speed controls for different entity types
- Special handling for grouped SimPlayers
- Cleaner Harmony patch implementation

