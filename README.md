<p align="center">
	<img src="https://img.shields.io/badge/C%23-%23239120.svg" alt="CSharp" />
	<a href="https://www.nexusmods.com/mountandblade2bannerlord/mods/4607" alt="Fast Dialogue (UPDATED)">
	<img src="https://img.shields.io/badge/download-Nexus%20Mods-yellow" /></a>
</p>

# Fast Dialogue (UPDATED)

This is a **fork** of the **Mount & Blade II: Bannerlord** mod **[MBFastDialogue](https://github.com/DonoA/MBFastDialogue)** by **[DonaA](https://github.com/DonoA)**. This repository contains an update for latest versions, patches and some improvements.

## Features

Removes loading screens from interactions with essentially all map interactions. Including lords, bandits, villagers, caravans, and small faction meetings.

## Installation

- Install latest version of [Harmony](https://github.com/BUTR/Bannerlord.Harmony).
- Install the mod with [Vortex manager](https://www.nexusmods.com/mountandblade2bannerlord/mods/4607), or extract manually in your Modules folder
- Load MBFastDialogue after Harmony and default modules on the launcher
- Check that the DLLs are not blocked

### How unlock DDL ?
_MBFastDialogue > bin > Win64_Shipping_Client > Right click on MBFastDialogue.dll > Propriety > If it is blocked, you will see "Unblock DLL", click on it._

## Settings
You can choose which parties are affected by the mod by editing the settings.xml in the root of the mod folder.

By default, settings.xml looks like this and affects all parties :﻿<MBFastDialogue.Settings>
```xml
<pattern_whitelist>
         <!--pattern>looter</pattern-->
         <!--pattern>bandit</pattern-->
    </pattern_whitelist>
  <toggle_key>X</toggle_key>
</MBFastDialogue.Settings>
```

For example, if you want the mod affect only looters and bandits, you can uncomment these lines :
```xml
﻿<MBFastDialogue.Settings>
    <pattern_whitelist>
         <pattern>looter</pattern>
         <pattern>bandit</pattern>
    </pattern_whitelist>
  <toggle_key>X</toggle_key>
</MBFastDialogue.Settings>
```

## For modders

From version 2.5.9, menu options added to the native menu ID "encounter" by mods are automatically added to Fast Dialogue.

If you wish to add specific menu options to Fast Dialogue, add them to menu ID "fast_combat_menu".

## Language support

The mod supports all languages available in the game.

## Any issues or questions ?

Don't hesitate to open issue or use "posts" and "bugs" tabs on Nexus Mods.

## Credits

All credits go to their original creators (Dallen1393 & Aragasas). Thanks to them!

The updated version includes improvements, minor fixes and compatibility patch for latest versions.

## Contributions

All contributions are welcome!