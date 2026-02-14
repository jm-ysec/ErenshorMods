# Real World Clock

A BepInEx mod for Erenshor that displays a real-world clock on your screen.

## Features

- **Real-Time Display**: Shows your system's current time on screen
- **Configurable Format**: Use any .NET time format string (default: `HH:mm:ss`)
- **Adjustable Font Size**: Configure text size to your preference
- **Draggable Position**: Integrates with the game's "Toggle UI Movement" system
- **Position Persistence**: Clock position saved to config and restored on restart
- **Auto-Hide**: Clock only shows when logged into a character

## Configuration

Edit the config file at `BepInEx/config/com.noone.realworldclock.cfg`:

| Setting | Default | Description |
|---------|---------|-------------|
| ShowClock | true | Enable/disable the clock display |
| TimeFormat | HH:mm:ss | .NET DateTime format string |
| FontSize | 18 | Font size for the clock text |
| PosX | 10 | Horizontal position (pixels from left) |
| PosY | 10 | Vertical position (pixels from top) |

### Time Format Examples

| Format | Example Output |
|--------|----------------|
| `HH:mm:ss` | 14:30:45 |
| `hh:mm tt` | 02:30 PM |
| `HH:mm` | 14:30 |
| `h:mm:ss tt` | 2:30:45 PM |

## Usage

1. The clock appears automatically when you log into a character
2. To reposition: Open the game menu → "Toggle UI Movement"
3. Drag the blue handle below the clock to move it
4. Click "Toggle UI Movement" again to lock the position

## Based On

This mod is a fixed version of [RealWorldClock by Recks](https://thunderstore.io/c/erenshor/p/Recks/RealWorldClock/).

Fixes and enhancements include:
- DontDestroyOnLoad pattern to prevent errors on scene changes
- Integration with game's native UI movement system
- Click-through prevention while dragging
- Auto-hide when not logged into a character
