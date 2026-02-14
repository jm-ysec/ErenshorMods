# Area Maps Mod

A BepInEx mod for Erenshor that replaces the in-game map with detailed area maps and a world map.

## Features

- **Area Maps**: Displays detailed maps for each zone when you open the map
- **World Map**: Toggle button to switch between area map and world map view
- **36 Zone Support**: Covers all major areas including Azure, Blight, Braxonian, Soluna, and more
- **Smart Toggle**: Button automatically shows/hides based on whether an area map is available

## Usage

1. Open the in-game map (default: M key)
2. If an area map is available, it will be displayed automatically
3. Click the "World Map" button to toggle to the world overview
4. Click "Area Map" to return to the detailed zone map

## Requirements

- BepInEx 5.x
- Map image assets in `BepInEx/plugins/AreaMaps/Assets/`

## Based On

This mod is a fixed version of [Area Maps by Ayloonah](https://thunderstore.io/c/erenshor/p/Ayloonah/Area_Maps/).

Fixes include:
- DontDestroyOnLoad pattern to prevent errors on scene changes
- Improved scene load handling

