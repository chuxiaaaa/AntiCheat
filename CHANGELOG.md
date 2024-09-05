# Beta(公开测试版)

## Version 0.7.5

### 中文

* [+] 新增聊天限速 ChatRealSettings.Cooldown 默认 200 毫秒
* [/] 优化了部分日志输出
* [/] 优化了聊天检测
* [+] 怪物击杀玩家 加入了对食人魔的检测

### English

* [+] Added chat speed limit ChatRealSettings.Cooldown, default 200 milliseconds 
* [/] Optimized some log output 
* [/] Optimized chat detection 
* [+] Monster kills player, added detection for ogres

## Version 0.7.4

### 中文

* [+] 远程终端检测现在会检测使用信号发射器了
* [/] 修复了RPC上报检测部分问题
* [/] 修复了家具能被异常旋转

### English

* [+] Remote terminal detection now detects the use of signal transmitters 
* [/] Fixed some issues with RPC reporting detection 
* [/] Fixed the problem that furniture can be rotated abnormally

## Version 0.7.3

### 中文

* [+] 现在 隔空取物检测 开启时，会禁止使用喷气背包的玩家拿取双手重物
* [/] 优化了盒子异常行为检测，应该能减少误报

### English

* [+] Now when Picking Up Items Detection is on, players using jetpacks will be prohibited from picking up two-handed heavy objects
* [/] Optimized the abnormal behavior detection of jester, which should reduce false positives

## Version 0.7.2
 
### 中文

* [+] RPCReportSetting 新增了 Delay 配置项，用于设置检测延迟。（如果出现误报你可以尝试调高延迟）
* [-] 暂时移除了家具旋转检测，非初始家具旋转会出现误报

### English

* [+] RPCReportSetting adds a new Delay configuration item to set the detection delay. (If false alarms occur, you can try to increase the delay) 
* [-] Temporarily removed furniture rotation detection, false alarms will occur for non-initial furniture rotation

## Version 0.7.1

### 中文

* [+] 新增玩家使用负伤害回血检测(0.7.0已实装但未加配置)
* [/] 修复了部分检测在客机生效的问题

### English

* [+] Added player health recovery detection when using negative damage (already implemented in 0.7.0 but not configured) 
* [/] Fixed the problem that some detections were effective on client
## Version 0.7.0

### 中文

* [+] 新增RPC上报检测(用于检测玩家是否开启无敌)
* [/] 飞船物品位置异常检测新增旋转角度检测

### English

* [+] Added RPC reporting detection (used to detect whether the player has turned on invincibility) 
* [/] Added rotation angle detection to the abnormal position detection of furniture

## Version 0.6.9

### 中文

* [/] 修复了铲子伤害检测误报的问题

### English

* [/] Fixed a bug that caused shovel damage detection to be false positive

## Version 0.6.8

### 中文

* [-] 移除 serverTag 的 AC标识（该标识导致新版本必须要输入标识才能找到房间）

### English

* [-] Remove the AC tag from serverTag (this tag requires you to enter the tag to find the lobby in the new version)

## Version 0.6.7

### 中文

* [+] 适配v56版本
* [-] 移除玩家负重异常检测(zeekeers在v56 beta2修复了该bug)

### English

* [+] Adapt to v56 version 
* [-] Removed abnormal player carry weight detection (zeekeers fixed this bug in v56 beta2)

## Version 0.6.6

### 中文

* [+] 语言配置将会根据电脑区域设置来决定，如果你的区域设置为以下几种 香港特别行政区，澳门特别行政区，中国，中文（简体），新加坡，台湾，中文（繁体） 那么将会使用中文作为默认模组语言，否则将会使用英语作为默认语言
* [+] 新增配置项 DetectedMessageType ，现在你可以更改配置决定检测的消息如何显示了

### English

* [+] The language configuration will be determined according to the locale of the computer, if your locale is set to the following Hong Kong SAR, Macau SAR, China, Chinese (Simplified), Singapore, Taiwan, Chinese (Traditional) Then Chinese will be used as the default mod language, otherwise English will be used as the default language
* [+] Added DetectedMessageType to the config item, now you can change the configuration to determine how the detected messages are displayed

## Version 0.6.5

### 中文

* [+] 新增玩家负重异常检测
* [/] 优化日志信息
* [/] 优化飞船物品摆放检测
* [/] 更正 BepInEx 版本

### 英文

* [+] Added player carry weight anomaly detection
* [/] Optimized log messages
* [/] Optimized ship item placement detection
* [/] Corrected BepInEx version.

## Version 0.6.4

### 中文

* [/] 修复胡桃夹子枪击其他怪物导致检测伤害异常
* [/] 现在会正确兼容 MoreCompany 模组的化妆品了
* [/] 修复了对玩家异常伤害的检测

### English

* [/] Fixed Nutcracker shooting other monsters resulting in abnormal damage detection.
* [/] Cosmetics for MoreCompany modules are now correctly compatible.
* [/] Fixed detection of abnormal damage to players

## Version 0.6.3(0.6.2)

### 中文

* [/] 修复 README.MD 跳转链接错误的问题

### English

* [/] Fix README.MD Jump Link Error

## Version 0.6.2(V50)

### 中文

* [/] 当前版本仅支持V50，不向下兼容
* [+] 新增对小刀的伤害/攻速检测
* [/] 优化部分日志信息
* [/] 修复获得加班费之后导致的刷取金钱检测

### English

* [/] Current version only supports V50, not backward compatible
* [+] Added damage/attack speed detection for knife
* [/] Optimized some log messages
* [/] Fixed swipe money detection after getting overtime payment

## Version 0.6.1

### 中文

* [/] 新增配置 EmptyHand 你可以通过启用该项允许玩家空手的时候造成正常伤害
* [/] 优化和补充缺失的英文翻译

### English

* [/] Added configuration EmptyHand You can enable this item to allow players to deal normal damage when empty-handed.
* [/] Optimized and added missing English translations.

## Version 0.6.0

### 中文

* [/] 禁止玩家重复加入游戏
* [/] 零元购新增付费地图检测
* [/] 零元购现在会检测将金钱设置为0以下了
* [+] 检测玩家远程使用终端，新增配置项 RemoteTerminalSettings
* [/] 现在反作弊的补丁将会以安全的形式运行
* [/] 修复拼写错误，之前的版本配置 LanguageSetting 被拼写成了 LangugeSetting ，现已更正，修改过配置的玩家需要重新设置

### English

* [/] Prevent players from repeatedly joining the game.
* [/] Zero-dollar purchase now includes paid map detection.
* [/] Zero-dollar purchase will now detect if money is set below 0.
* [+] Added new configuration item "RemoteTerminalSettings" to detect players remotely using terminals.
* [/] The anti-cheat patch will now run in a safer form.
* [/] Fixed a typo. In previous versions, the configuration item "LanguageSetting" was misspelled as "LangugeSetting". This has now been corrected, and players who have modified this setting will need to reconfigure it.

## Version 0.5.9

### 中文

* [/] 优化了英文翻译(@CoolLKKPS)

### English

* [/] Optimized the English translation(@CoolLKKPS)

## Version 0.5.8

### 中文

* [/] 部分空手打怪/销毁物品问题已兼容(?)
* [+] 新增配置 PlayerJoin 现在你可以自定义欢迎语句了！
* [/] 一票起飞功能修复

### English

* [/] The problem of empty-handed fighting monsters/destruction of items has been resolved (?)
* [+] Added PlayerJoin configuration, now you can customize the welcome message!
* [/] One-ticket departure function is fixed

## Version 0.5.7

### 中文

* [/] 回滚销毁检测机制
* [+] 新增配置 OnlyOneVote 用于控制主机是否可以一票起飞

### English

* [/] rollback destruction detection mechanism
* [+] Add configuration OnlyOneVote to control whether the host can take off with one ticket
## Version 0.5.6

### 中文

* [/] 修复出售货物导致销毁物品检测

### English

* [/] Fix the issue of selling goods leading to the detection of destroyed items.

## Version 0.5.5

### 中文

* [/] 修复不是房主也能一票起飞的问题
* [/] 现在被踢出的玩家在连接时就会被断开连接
* [+] 现在使用AntiKick进入游戏将会在控制台显示玩家名称和SteamId
* [+] 现在伪造SteamId加入游戏将会被自动踢出

### English

* [/] Fixed the issue where non-hosts could initiate the game with a single vote.
* [/] Players who are kicked out will now be disconnected upon trying to reconnect.
* [+] Players who use AntiKick to enter the game will now have their usernames and SteamIds displayed in the console.
* [+] Players attempting to join the game with a falsified SteamId will now be automatically kicked out.

## Version 0.5.4

### 中文

* [/] 修复房主投票少一票的问题
* [/] 修复部分情况无法拉杆
* [/] 现在不检测枪是否被销毁

### English

* [/] Fixed the issue of homeowners voting one less vote
* [/] Fixed some situations where it was not possible to pull the rod
* [/] Currently, we are not checking if the gun has been destroyed

## Version 0.5.3

### 中文

* [/] 修复投票导致游戏崩溃

### English

* [/] Fix the voting system that causes the game to crash

## Version 0.5.2

### 中文

* [/] 修复无法正常出售货物

### English

* [/] Repair the goods that cannot be sold normally

## Version 0.5.1

### 中文

* [/] 修复出售货物导致销毁物品检测
* [+] 进游戏卡黑屏将会自动退出房间(未测试)

### English

* [/] Fix the issue of selling goods leading to the detection of destroyed items.
* [+] The game will automatically exit the room if it crashes with a black screen (not tested)

## Version 0.5.0

### 中文

* [+] 现在禁止使用双手拿取物品了
* [/] 修改了销毁物品的检测机制
* [/] 修复死亡后投票提示问题
* [/] 房主一票直接起飞 修改为 一个小时后起飞
* [/] 修复客户端拉杆失败导致无法继续拉杆

### English

* [+] It is now prohibited to pick up items with both hands.
* [/] The detection mechanism for destroying items has been modified.
* [/] Fixed the issue with the voting prompt after death.
* [/] The rule where the host can immediately take off with one vote has been changed to taking off after one hour.
* [/] Fixed the issue where failing to pull the lever on the client side prevents further lever pulling.

## Version 0.4.9

### 中文

* [+] 新增 ServerNameSetting.Prefix 配置，现在你可以通过改变配置来决定房间名的前缀(如果为空则为不启用)
* [/] 修复死亡后投票提示问题
* [/] 修改了铲子范围异常检测部分机制

### English

* [+] Added ServerNameSetting.Prefix configuration, now you can determine the prefix of the room name by changing the configuration (if empty, it is not enabled).
* [/] Fixed the issue of voting prompts after death.
* [/] Modified the mechanism for detecting shovel range abnormalities.

## Version 0.4.8

### 中文

* [/] 修复 0.4.6/0.4.7 版本导致的帧率过低问题

### English

* [/] Fix low fps issue caused by version 0.4.6/0.4.7

## Version 0.4.7

### 中文

* [/] 是的，我忘记上传修改日志了，那么我们直接跳过0.4.7版本(当前版本改动和0.4.6一样，并且版本号使用0.4.6)

### English

* [/] Yes, I forgot to upload the change log. Let's skip version 0.4.7 directly (the changes in the current version are the same as those in 0.4.6, and we'll use version number 0.4.6).

## Version 0.4.6

### 中文

* [+] 当你创建房间时，房间名会自动新增[AC] 作为前缀，例如："[v49] 战斗爽" 会变成 "[v49/AC] 战斗爽！" 或者 "战斗爽！" 会变成 "[AC] 战斗爽！"
* [+] 新增死亡投票提示(你是房主，投票直接起飞！)

### English

* [+] When you create a room, the room name will automatically have "[AC]" added as a prefix. For example, "[v49] The battle is thrilling!" will become "[v49/AC] The battle is thrilling!" or "The battle is thrilling!" will become "[AC] The battle is thrilling!".
* [+] You are the homeowner, and voting can immediately make the spaceship take off.

## Version 0.4.5

### 中文

* [/] 修复配置文件描述丢失的问题

### English

* [/] Fix the issue of missing configuration file descriptions

## Version 0.4.4

### 中文

* [/] 修复缺少描述导致语言无法正常加载

### Enlgish

* [/] Fix the issue where missing descriptions cause languages to fail to load properly

## Version 0.4.3

### 中文

* [+] 自动踢出开启防踢的玩家
* [+] 新增 Log 配置，用于控制反作弊的日志输出

### English

* [+] Automatically kick out players with anti-kick enabled

* [+] New Log configuration added to control the output of anti cheating logs

## Version 0.4.2

### 中文

* [/] 更改强制引爆地雷逻辑

### Enlgish

* [/] Change the logic for detecting landmine detonation

## Version 0.4.1

### 中文

* [+] 从0.4.1版本以后将会发布更新日志
* [/] 修复怪物不能正常击杀客机玩家 

### English

* [+] There will be modification logs in future versions
* [/] Monsters cannot kill client players normally 