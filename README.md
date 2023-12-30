<p align="center">
  <img src="https://github.com/arikidev/MoveStyler/blob/main/CrewBoom%20Plugin/Resources/logo_background.png" />
</p>
    
This is a repository for a BepInEx plugin to load custom movestyles for the game *Bomb Rush Cyberfunk* 

This is retooling of Giygas's CrewBoom and there are still numerous references to Crewboom code. Shoutout to Giygas for their work.

Example:  
![image](https://github.com/SGiygas/BrcCustomCharacters/assets/50772474/43ff8ca8-0805-4409-9547-234e26fcedda)
 
## Usage

Use the keys 'G,H' to navigate through default and custom movestyles

## Downloads, instructions and help

### [Plugin](https://github.com/arikidev/MoveStyler/releases)
### [Unity project](https://github.com/arikidev/MoveStyler/releases/download/WIP-0.1-Proj/moveStylerProj.zip)
### [Example Movestyle](https://github.com/arikidev/MoveStyler/releases/download/WIP_0.1_Example/MoveStyler_Ring.zip)

To create a movestyle mod you will need both of these. If you just want to use a character you downloaded, you only need the plugin.  

## Installation

This plugin requires [BepInEx](https://thunderstore.io/package/bbepis/BepInExPack/), a plugin framework for Unity games.
The easiest way to get BepInEx going for Bomb Rush Cyberfunk is to use the [r2modman mod manager](https://thunderstore.io/package/ebkr/r2modman/).  

*r2modman was used to test this plugin, so other solutions may not work*

### Using r2modman

To install BepInEx, follow these steps:  
1. Open r2modman and select *Bomb Rush Cyberfunk* as the game and choose any profile
2. Navigate to the *Online* tab
3. Find *BepInEx Pack* by BepInEx and install it

## Adding characters

1. Navigate to your BepInEx installation for the game and open the `BepInEx/config/MoveStyler/` folder
2. Copy your character files (.cbb and .json) into the `MoveStyler` folder


## Building the plugin

This pertains to if you want to build the plugin yourself, not the Unity project.  

You will need to copy Bomb Rush Cyberfunk's `Assembly-CSharp.dll` into the `Libraries` folder of the project, as it is not provided.  
It can be found at `<path to where your games are stored>\BombRushCyberfunk\Bomb Rush Cyberfunk_Data\Managed`

## Authors and acknowledgment

This project was based on [CrewBoom] by SGiygas https://github.com/SGiygas/CrewBoom

## License
This project is licensed under the GNU General Public License v3.0
