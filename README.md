# Bomb Rush Cyberfunk Custom Characters

This is a repository for a BepInEx plugin to load custom characters for the game *Bomb Rush Cyberfunk*

## Features

The plugin allows for the following:
- Custom character name/title
- Custom character model
- Custom outfits (up to four)
- Custom character graffiti
- Custom character voice clips

## Getting started

To create a character mod you will need the [plugin](https://github.com/SGiygas/BrcCustomCharacters/releases/download/Stable/BrcCustomCharacterPlugin.zip) as well as the [unity project](https://github.com/SGiygas/BrcCustomCharacters/releases/download/Stable/BrcCustomCharacterUnityKit.zip).  
The documentation on how to use the unity project  can be found [here](https://github.com/SGiygas/BrcCustomCharacters/wiki).

## Installation

This plugin requires [BepInEx](https://thunderstore.io/package/bbepis/BepInExPack/), a plugin framework for Unity games.
The easiest way to get BepInEx going for Bomb Rush Cyberfunk is to use either the [Thunderstore Mod Manager](https://www.overwolf.com/app/Thunderstore-Thunderstore_Mod_Manager) or the [r2modman mod manager](https://thunderstore.io/package/ebkr/r2modman/).  

*r2modman was used to test this plugin, so it is not guaranteed to work with the Thunderstore Mod Manager as well*

### Using r2modman

To install BepInEx, follow these steps:  
1. Open r2modman and select *Bomb Rush Cyberfunk* as the game and choose any profile
2. Navigate to the *Online* tab
3. Find *BepInEx Pack* by BepInEx and install it

To install the plugin for BepInEx, follow these steps:

1. Download the [plugin]()
2. Open r2modman and select *Bomb Rush Cyberfunk* as the game and choose any profile
3. Navigate to the *Settings* tab 
4. Find the option labeled "Browse profile folder" and click it
5. Navigate to the folder `BepInEx/plugins/`
6. Copy the folder `brcCustomCharacters` from the plugin archive to `BepInEx/plugins/`

## Adding characters

1. Navigate to your BepInEx installation for the game and open the `BepInEx/plugins/brcCustomCharacters/` folder
2. Copy any character file (e.g. "metalhead") into the `CharAssets` folder

## Authors and acknowledgment
- Programming and Unity SDK: SGiygas
- Test model and assets: minty_cups

This project was loosely based on [BRCModelReplacement](https://github.com/TheSmallBlue/BRC-ModelReplacement) by TheSmallBlue

## Changes
### 1.0.0
Release

## License
This project is licensed under the GNU General Public License v3.0