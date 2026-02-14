# Better Stats Page

A BepInEx mod for Erenshor that provides an enhanced stats and reputation UI with a tabbed interface.

## Features

- **Tabbed Interface**: Switch between Stats and Reputation views
- **Comprehensive Stats Display**:
  - Level and Ascension Level
  - Experience progress
  - All attributes (Str, Dex, End, Agi, Int, Wis, Cha) with item bonuses
  - Crit Chance, Dodge Chance
  - Physical Attack, Spell Damage Bonus, Healing Bonus
  - Life Steal, Damage Shield
  - Attack Speed, Move Speed, Resonance Proc Chance
- **Reputation Tab**: View all faction standings
- **Draggable Window**: Reposition the panel using the drag handle
- **Auto-Update**: Stats refresh automatically when they change
- **Fade Animations**: Smooth open/close transitions

## Configuration

Edit the config file at `BepInEx/config/com.noone.betterstatspage.cfg`:

| Setting | Default | Description |
|---------|---------|-------------|
| ToggleKey | P | Key to open/close the stats window |
| ActiveTabColor | Blue | Color for the active tab |
| InactiveTabColor | Gray | Color for inactive tabs |

## Usage

1. Press **P** (default) to open the stats panel
2. Click "Stats" or "Reputation" tabs to switch views
3. Drag the blue diamond handle to reposition the window
4. Press **P** again to close

## Based On

This mod is based on [Stat Menu Ui by Recks](https://thunderstore.io/c/erenshor/p/Recks/Stat_Menu_Ui/).

Enhancements include:
- Tabbed interface with Stats and Reputation views
- More comprehensive stat calculations
- Draggable window with proper event handling
- Auto-updating stats display
- DontDestroyOnLoad pattern for scene persistence
