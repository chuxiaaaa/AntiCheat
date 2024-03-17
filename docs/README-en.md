# AntiCheat

A Lethal Company anti-cheat mod (host-only)

The default language is Simplified Chinese, You can switch to English by changing the Language to "English" in the configuration file.

## Introduction

Discord: https://discord.gg/ZdWr2rKR

This mod is designed to prevent cheating when hosting public lobbies, ensuring a fair gaming experience for all players.

While using this mod, you might encounter false positives, but this may not be an issue with the AntiCheat mod itself. To troubleshoot, please try uninstalling all mods except for BepInEx and AntiCheat, and attempt to reproduce the issue. If the issue no longer occurs, it's likely a compatibility problem with another mod.

## Installation

1. Install [BepInExPack](https://thunderstore.io/c/lethal-company/p/BepInEx/BepInExPack);
2. Place AntiCheat.dll in the Lethal Company\BepInEx\plugins directory;
3. Launch the game. Once the game has started, you can find the configuration file AntiCheat.cfg in the Lethal Company\BepInEx\config directory.

## Creating Lobby

By default, the "[AC]" prefix will be added to the lobby name to indicate to other players that the anti-cheat is enabled in this lobby.
Example: [AC] The battle is thrilling!

## Features

### Detection

* Anti-kick detection

  * Detects if player join the lobby with Anti-kick

* Lever pull detection

  * Limited ship lever to landing and departure by checking how many player are in the ship and if current time are approved to do so or not
  * Prevent malicious lever pull

* Abnormal furniture position detection
* Abnormal item interaction cooldown detection

  * Detects item interaction cooldown, including switch slot to reduce shovel swing cooldown

* Spamming light switch detection
* Spamming terminal noise detection
* Abnormal item despawn detection
* Fake chat message detection
* Abnormal masked enemy detection
* Abnormal gift box interaction detection
* Abnormal spiderweb spawning detection
* Abnormal turret berserk mode detection
* Invisible player detection
* Abnormal jetpack explosion detection
* Abnormal landmine triggering detection
* Abnormal monster detection
* Mini-map detection

  * Detects player who snooping, which detect snooping on loot, mines, turret, enemy locations and more

* Enemy kill detection

  * Detects if player kill an enemy from too far away

* Item pickup distance detection
* Abnormal shovel damage detection

  * No 99 shovel dmg!

* Player name detection

  * Notify if player have Nameless or Unknown name

* Abnormal purchases detection

  * Detects if player purchase items, unlockables or routing moon for free

* Infinite ammo detection

  * Detects if player load and firing the ammo that doesn't even exist
  * Normal player can trigger this, as long as the shotgun has the ammo that doesn't exist

* Spamming bell ringing detection
* Abnormal credits detection
* Remote terminal detection
* Prevention of client-side monster killing RPC to kill other players

  * Prevents instant-kills


### Patch

* Prevent blackscreen when respawn

  * This is because of the exp value calculation

* Lobby name prefix
* Editable lobby welcome message

### Configuration

* Language

  * Again the default language is Simplified Chinese, You can switch to English by changing the Language to "English" in the configuration file.

## Contributions

<a href="https://github.com/chuxiaaaa/AntiCheat/graphs/contributors">
  <img src="https://contrib.rocks/image?repo=chuxiaaaa/AntiCheat" />
</a>

### Localized translation contributor

* Chinese (chuxiaaaa & CoolLKKPS)
* English (DeathWrench & CoolLKKPS)

### Readme file translation contributor

* Chinese (chuxiaaaa & CoolLKKPS)
* English (NilaierMusic & CoolLKKPS)
* Russian (NilaierMusic)

### Mod Testers

* 超级骇人鲸 
* Alan Backer
* 沈阳最速傳說疾走の猛虎！貴物刀一郎
* 柒小鸭yz
* 喜欢睡觉の极茶龙 
* 东南枝
* Melissa
* 我不吃牛肉

We welcome contributions from capable developers to improve this mod.

## Feedback
If you encounter a false positive and can confirm it's not caused by another mod, please report the issue on my GitHub, explaining how to reproduce the problem. This will help us fix it!
