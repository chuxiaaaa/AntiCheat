# AntiCheat

[![Thunderstore Version](https://img.shields.io/thunderstore/v/chuxiaaaa/AntiCheat?style=for-the-badge&logo=thunderstore&logoColor=white)](https://thunderstore.io/c/lethal-company/p/chuxiaaaa/AntiCheat/versions/)
[![Thunderstore Downloads](https://img.shields.io/thunderstore/dt/chuxiaaaa/AntiCheat?style=for-the-badge&logo=thunderstore&logoColor=white)](https://thunderstore.io/c/lethal-company/p/chuxiaaaa/AntiCheat/)

[**简体中文(Support)**](https://github.com/chuxiaaaa/AntiCheat/blob/main/README.md) | **English(Support)** | [**Русский**](https://github.com/chuxiaaaa/AntiCheat/blob/main/docs/README-ru.md) | [**한국어(Support)**](https://github.com/chuxiaaaa/AntiCheat/blob/main/docs/README-ko.md)

A Lethal Company anti-cheat server-side mod.

## i18 globally

After version 0.8.2，AntiCheat support fully customized prompt content.

### Specify the language file to be used
  1. In ``AntiCheat\locales`` folder，open ``localization_cfg.json`` file;
  2. Change ``current_language`` value by following rule:
      - If the value is empty, the mod will automatically set the value depends on the system language;
      - If you want to specify，set the value to the corresponding language file name.

### Create a new language file
  1. Make a copy of ``en_US.json`` language file and rename it (to the language name you want to localize);
  2. Localize the language file you made，and then change ``current_language`` value from ``localization_cfg.json``;
  3. (Optional) Submit your language file by using PR in Github to let other people enjoy your creation.

## Introduction

Discord: [https://discord.gg/ZdWr2rKR](https://discord.gg/zem2eFFBHj)

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

- Anti-kick detection

  - Detects if player join the lobby with Anti-kick

- Lever pull detection

  - Limited ship lever to landing and departure by checking how many player are in the ship and if current time are approved to do so or not

  - Prevent malicious lever pull

- Abnormal furniture position detection

- Abnormal item interaction cooldown detection

  - Detects item interaction cooldown, including switch slot to reduce shovel swing cooldown

- Spamming light switch detection

- Spamming terminal noise detection

- Abnormal item despawn detection

- Fake chat message detection

- Abnormal masked enemy detection

- Abnormal gift box interaction detection

- Abnormal spiderweb spawning detection

- Abnormal turret berserk mode detection

- Invisible player detection

- Abnormal jetpack explosion detection

- Abnormal landmine triggering detection

- Abnormal monster detection

- Mini-map detection

  - Detects player who snooping, which detect snooping on loot, mines, turret, enemy locations and more

- Enemy kill detection

  - Detects if player kill an enemy from too far away

- Item pickup distance detection

- Abnormal shovel damage detection

  - No 99 shovel dmg!

- Player name detection

  - Notify if player have Nameless or Unknown name

- Abnormal purchases detection

  - Detects if player purchase items, unlockables or routing moon for free

- Infinite ammo detection

  - Detects if player load and firing the ammo that doesn't even exist

  - Normal player can trigger this, as long as the shotgun has the ammo that doesn't exist

- Boss attack spoofing detection

- Abnormal credits detection

- Remote terminal detection

- Prevention of client-side monster killing RPC to kill other players

  - Prevents instant-kills

- Prevent the player join the lobby twice if they already in the lobby

* [ ]  Speed detection

* [ ]  Stamina detection

### Other features

- Lobby name prefix

- Editable lobby welcome message

- Logging action made by the client

- Ignore network configuration differences

  - try to avoid 'an error occurred' dialog

## Contributions

<a href="https://github.com/chuxiaaaa/AntiCheat/graphs/contributors">
  <img src="https://contrib.rocks/image?repo=chuxiaaaa/AntiCheat" />
</a>

### Localized translation contributor

- Chinese (chuxiaaaa & CoolLKKPS)
- English (DeathWrench & CoolLKKPS)
- Korean (P-Asta)

### Readme file translation contributor

- Simplified Chinese (chuxiaaaa & CoolLKKPS)
- English (NilaierMusic & CoolLKKPS)
- Russian (NilaierMusic)
- Korean (P-Asta)

### Mod Testers

- 超级骇人鲸
- Alan Backer
- 沈阳最速傳說疾走の猛虎！貴物刀一郎
- 柒小鸭 yz
- 喜欢睡觉の极茶龙
- 东南枝
- Melissa
- 我不吃牛肉

We welcome contributions from capable developers to improve this mod.

## Feedback

If you encounter a false positive and can confirm it's not caused by another mod, please report the issue on my GitHub, explaining how to reproduce the problem. This will help us fix it!
