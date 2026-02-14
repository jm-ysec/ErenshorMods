# Resizable Chat & Combat Windows

A BepInEx mod for Erenshor that makes the chat and combat log windows resizable when UI edit mode is enabled.

## Features

- **Resizable Windows**: Drag the corner handles to resize chat and combat windows
- **Integrates with Game UI**: Resize handles appear when you enable "Toggle UI Movement" in the game's options
- **Persistent Settings**: Window sizes are saved to the config file and restored on game load
- **Click-Through Prevention**: Resize handles block game input while dragging to prevent accidental actions

## Usage

1. Enable UI edit mode in the game by pressing the "Toggle UI Movement" key (default: check your keybindings)
2. Blue resize handles will appear at the bottom-right corner of the chat and combat windows
3. Drag the handles to resize the windows
4. Your settings are automatically saved when you release the mouse

## Configuration

The mod creates a config file at `BepInEx/config/com.noone.resizablechatcombat.cfg` with the following options:

### Chat Window
- **Width** (200-800, default: 350) - Width of the chat window
- **Height** (100-600, default: 200) - Height of the chat window

### Combat Window
- **Width** (200-800, default: 350) - Width of the combat window
- **Height** (80-400, default: 150) - Height of the combat window

## Installation

1. Ensure BepInEx is installed for Erenshor
2. Copy `ResizableChatCombat.dll` to your `BepInEx/plugins/ResizableChatCombat/` folder
3. Start the game

## Original Work

This is an original mod created for the ErenshorMods collection.

