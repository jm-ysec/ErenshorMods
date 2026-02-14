# Fast Travel Spellbook

A BepInEx mod for Erenshor that grants all portal spells to the player and fixes their range limitations.

## Features

- **All Portal Spells Unlocked**: Automatically teaches all portal spells to your character on login:
  - Portal to Hidden
  - Portal to Braxonian
  - Portal to Ripper's Keep
  - Portal to Silkengrass
  - Portal to Soluna's Landing
  - Portal to Reliquary

- **Unlimited Range**: Portal spells can be cast from anywhere (range set to 9999)

## Usage

1. Log into your character
2. All portal spells are automatically added to your spellbook
3. Cast any portal spell to fast travel to that location

## Technical Details

Uses Harmony patches to:
- `PlayerControl.Start` - Teaches portal spells on character load
- `CastSpell.StartSpell` - Fixes portal spell range at cast time

## Based On

This mod is a fixed version of [Fast Travel System by Recks](https://thunderstore.io/c/erenshor/p/Recks/Fast_Travel_System/).

Fixes include:
- Proper Harmony patching for overloaded methods
- Range fix applied at cast time to prevent game from resetting it

