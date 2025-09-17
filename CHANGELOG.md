# Beta(公开测试版)

## Version 0.8.7

### 中文

* [+] 新增了一些 RPC 调用日志(在AntiCheat.log中查看他们！)
* [/] 修复了客户端使用反作弊也会生成日志内容的问题
* [/] 客户端修改电量超过100时将会被拦截(目前没有做提醒，如果想检查是否有玩家使用了无限电量请查看日志文件)
* [/] 修复传送器补丁的报错
* [/] 允许玩家在物品栏中有铲子，但手上是空手的情况下造成伤害了
* [/] 现在引爆地雷应该不会存在误报了!(如果是FairAI引爆的，不算误报)
* [/] 修复击杀管家与其他模组的兼容性问题
* [+] 新增韩文本地化 @P-Asta (如果你想固定语言至韩文，请手动设置 `locales\localization_cfg.json` 文件,将 `current_language` 改为 `ko_KR`)

### English

* [+] Added some RPC call logs (check them in AntiCheat.log!)
* [/] Fixed an issue where the client would generate log content even when using anti-cheat
* [/] Client modifications to battery level exceeding 100 will now be blocked (no notification is currently implemented; check log files to identify players using infinite battery)
* [/] Fixed errors related to the teleporter patch
* [/] Players can now deal damage while having a shovel in their inventory but empty hands
* [/] LandMine detonations should no longer produce false positives! (If detonated by **FairAI**, it is not considered a false positive)
* [/] Fixed the compatibility issue between Kill Butler and other mods
* [+] New Korean localization @P-Asta (If you want to fix the language to Korean, please manually set the `locales\localization_cfg.json` file and change `current_language` to `ko_KR`)


## Version 0.8.6

### 中文

* [/] 修复客机使用礼物盒会卡手的问题
* [/] 修复遥控器冷却未正确生效

### English

* [/] Fixed an issue where using a gift box in the passenger aircraft would cause hands to get stuck
* [/] Fixed the remote cooldown not taking effect properly

## Version 0.8.5

### 中文(重大更新)

- [+] 新增 AntiCheat.log 文件，你在游戏根目录可以找到它，反作弊所有的日志都会输出到该文件
- [+] 零元购新增对卡车的检测
- [+] 现在投票完成后，可以立即拉杆起飞（无论有没有满足起飞条件）
- [+] 现在允许被抱脸虫报之后，短时间内空手造成伤害
- [/] 新增部分 RPC 日志，优化大部分日志内容
- [/] 修复重开房间未重置变量
- [/] 修复因操作过快导致被检测远程终端
- [/] 修复爆炸导致伤害异常检测
- [/] 修复主机看不见客机使用部分物品
- [-] 移除 Hit RPC 检测

### English(Major Update)

- [+] Added AntiCheat.log file – you can find it in the game's root directory, where all anti-cheat logs will be recorded.
- [+] Zero-dollar purchases now include truck detection.
- [+] After voting is completed, the aircraft can now take off immediately by pulling the lever (regardless of whether the usual conditions are met).
- [+] Players can now deal melee damage briefly after being attacked by a facehugger.
- [/] Added partial RPC logs and optimized most log content.
- [/] Fixed an issue where room variables were not reset when reopening a room.
- [/] Fixed false remote terminal detection caused by excessively fast inputs.
- [/] Fixed abnormal damage detection caused by explosions.
- [/] Fixed an issue where the host couldn't see clients using certain items.
- [-] Removed Hit RPC detection.

### 한국어(주요 업데이트)

- [+] AntiCheat.log 파일이 추가되었습니다 – 게임 루트 디렉토리에서 찾을 수 있으며, 모든 안티치트 로그가 해당 파일에 기록됩니다.
- [+] 무료 구매에 트럭 감지 기능이 추가되었습니다.
- [+] 투표 완료 후 이제 레버를 당겨 즉시 이륙할 수 있습니다 (일반적인 조건 충족 여부와 무관).
- [+] 플레이어가 페이스허거에게 공격받은 후 잠시 동안 맨손으로 피해를 입힐 수 있습니다.
- [/] 부분적인 RPC 로그가 추가되고 대부분의 로그 내용이 최적화되었습니다.
- [/] 방을 다시 열 때 변수가 재설정되지 않는 문제가 수정되었습니다.
- [/] 너무 빠른 입력으로 인한 잘못된 원격 터미널 감지가 수정되었습니다.
- [/] 폭발로 인한 비정상적인 피해 감지가 수정되었습니다.
- [/] 호스트가 클라이언트의 특정 아이템 사용을 볼 수 없는 문제가 수정되었습니다.
- [-] Hit RPC 감지가 제거되었습니다.

## Version 0.8.4

### 中文

- [+] 加入传送器冷却限制
- [+] 远程终端现在会检测关闭机枪
- [/] 现在可以在 v70 正常运行了
- [/] 由于双手作弊过于泛滥，现在使用喷气背包的时候禁止抓取双手物品（如果你允许同时拥有多个双手物品，那么依旧可以抓取）

### English

- [+] Added warp drive cooling restrictions
- [+] Remote terminals now detect turret shutdowns
- [/] Fixed v70 compatibility issues
- [/] Due to widespread two-handed cheating, grabbing two-handed items is now disabled while using a jetpack (if you allow multiple two-handed items, grabbing is still permitted)

### 한국어

- [+] 워프 드라이브 냉각 제한이 추가되었습니다
- [+] 원격 터미널에서 터렛 종료를 감지합니다
- [/] v70 호환성 문제가 수정되었습니다
- [/] 양손 치트가 너무 만연하여, 이제 제트팩 사용 중에는 양손 아이템 잡기가 비활성화됩니다 (여러 양손 아이템을 허용하는 경우에는 여전히 잡기가 가능합니다)

## Version 0.8.3

### 中文

- [/] 修复取消换弹会导致触发无限子弹检测
- [/] 修复部分本地化内容没有正常获取
- [/] 修复起飞/降落时两次检测提示

### English

- [/] Fixed canceling reload triggering infinite ammo detection
- [/] Fixed some localized content not being fetched correctly
- [/] Fixed duplicate detection prompts during takeoff/landing

### 한국어

- [/] 재장전 취소 시 무한 탄약 감지가 트리거되는 문제가 수정되었습니다
- [/] 일부 현지화 콘텐츠가 제대로 불러와지지 않는 문제가 수정되었습니다
- [/] 이륙/착륙 시 중복 감지 알림이 수정되었습니다

## Version 0.8.2

### 中文

- [/] i18방式从资源文件迁移到 json 文件，现在你可以完全自定义反作弊的提示消息
- [/] 修复客户端在重新加入游戏后，无法看到家具的正确位置
- [/] 修复在某些地点击杀管家检测到异常伤害的问题

### English

- [/] The i18n method has been migrated from resource files to JSON files, now you can fully customize the anti-cheat warning messages
- [/] Fixed an issue where the client could not see the correct position of furniture after rejoining the game
- [/] Fixed abnormal damage detection when killing the butler in certain locations

### 한국어

- [/] i18n 방식이 리소스 파일에서 JSON 파일로 마이그레이션되었으며, 이제 안티치트 경고 메시지를 완전히 사용자 정의할 수 있습니다
- [/] 게임에 다시 참가한 후 클라이언트가 가구의 정확한 위치를 볼 수 없는 문제가 수정되었습니다
- [/] 특정 위치에서 관리인을 죽일 때 비정상적인 피해가 감지되는 문제가 수정되었습니다

## Version 0.8.1

### 中文

- [/] 修复了房主无法从付费星球切换至免费星球的问题
- [+] 新增配置 `ShipSetting.ChangToFreeMoon`，用于开关 玩家从付费星球切换到免费星球 检测
- [+] 对于部分 RPC 进行限制，避免用于卡房

### English

- [/] Fixed an issue where host were unable to switch from paid planets to free planets.
- [+] Added a new configuration `ShipSetting.ChangToFreeMoon` to toggle the detection of players switching from paid planets to free planets.
- [+] Implemented restrictions on certain RPCs to prevent their use for room exploits.

### 한국어

- [/] 호스트가 유료 행성에서 무료 행성으로 전환할 수 없는 문제가 수정되었습니다.
- [+] `ShipSetting.ChangToFreeMoon` 설정이 추가되어 플레이어가 유료 행성에서 무료 행성으로 전환하는 것을 감지하는 기능을 토글할 수 있습니다.
- [+] 특정 RPC에 제한을 구현하여 방 익스플로잇에 사용되는 것을 방지합니다.

## Version 0.8.0

### 中文

- [+] 新增配置 `VersionSetting.IgnoreClientConfig` ，用于忽略客户端配置有差异(这将缓解客户端出现发生错误的情况)
- [+] 新增配置 `LogSetting.OperationLog` ，显示玩家操作信息(加入房间/购买物品/切换星球)
- [/] 修复了星球零元购的误报

### English

- [+] Added configuration `VersionSetting.IgnoreClientConfig` to ignore discrepancies in client configurations (this will alleviate the occurrence of errors on the client side).
- [+] Added configuration `LogSetting.OperationLog` to display player operation information (such as joining a room, purchasing items, switching planets).
- [/] Fixed false positives for Planet Zero purchase

### 한국어

- [+] `VersionSetting.IgnoreClientConfig` 설정이 추가되어 클라이언트 설정 불일치를 무시합니다 (클라이언트 측에서 오류 발생을 완화합니다).
- [+] `LogSetting.OperationLog` 설정이 추가되어 플레이어 작업 정보를 표시합니다 (방 참가/아이템 구매/행성 전환).
- [/] 행성 무료 구매의 오탐이 수정되었습니다.

## Version 0.7.9

### 中文

- [+] 增加了对无限子弹的两种检测

### English

- [+] Added two kinds of detection for unlimited ammo

### 한국어

- [+] 무한 탄약에 대한 두 가지 감지 방법이 추가되었습니다

## Version 0.7.8

### 中文

- [+] 修改配置文件后将会即时生效（不需要重启游戏了）
- [+] 新增 韩文 支持(@P-Asta)，配置文件 `LanguageSetting.Language = Korean` 即可启用

### English

- [+] Changes made to the configuration file will take effect immediately (no need to restart the game).
- [+] Added support for Korean (@P-Asta); set `LanguageSetting.Language = Korean` in the configuration file to enable.

### 한국어

- [+] 설정 파일 변경 사항이 즉시 적용됩니다 (게임을 재시작할 필요가 없습니다).
- [+] 한국어 지원이 추가되었습니다 (@P-Asta); 설정 파일에서 `LanguageSetting.Language = Korean`으로 설정하여 활성화할 수 있습니다.

## Version 0.7.7

### 中文

- [+] 작비玩家被踢出时将会显示被反作弊踢出的提示
- [/] 修复了卡车击杀/击伤敌人时的误报

### English

- [+] When a cheating player is kicked out, a message will be displayed saying that he was kicked out by anti-cheat
- [/] Fixed the false positive when the truck kills / injures the enemy

### 한국어

- [+] 치트를 사용하는 플레이어가 킥당할 때 안티치트에 의해 킥되었다는 메시지가 표시됩니다
- [/] 트럭이 적을 죽이거나 다치게 할 때의 오탐이 수정되었습니다

## Version 0.7.6

### 中文

- [/] GrabObject 配置新增了 MoreSlot TwoHand BeltBag 配置项
- [+] GrabObject.BeltBag 启用时，可以禁用玩家使用腰包拾取废料

### English

- [/] The GrabObject configuration has been updated with a new option: MoreSlot TwoHand BeltBag configuration.
- [+] When GrabObject.BeltBag is enabled, players can be disabled from picking up scrap using the belt bag.

### 한국어

- [/] GrabObject 설정에 MoreSlot TwoHand BeltBag 설정 옵션이 추가되었습니다.
- [+] GrabObject.BeltBag가 활성화되면 플레이어가 벨트 백을 사용하여 스크랩을 줍는 것을 비활성화할 수 있습니다.

## Version 0.7.5

### 中文

- [+] 现在禁止使用双手拿取物品了
- [/] 修改了销毁物品的检测机制
- [/] 修复死亡后投票提示问题
- [/] 房主一票直接起飞 修改为 一个小时后起飞
- [/] 修复客户端拉杆失败导致无法继续拉杆

### English

- [+] Added chat speed limit `ChatRealSettings.Cooldown`, default 200 milliseconds
- [/] Optimized some log output
- [/] Optimized chat detection
- [+] Monster kills player, added detection for ogres

### 한국어

- [+] 채팅 속도 제한 `ChatRealSettings.Cooldown`이 추가되었습니다 (기본값: 200밀리초)
- [/] 일부 로그 출력이 최적화되었습니다
- [/] 채팅 감지가 최적화되었습니다
- [+] 몬스터가 플레이어를 죽일 때 오우거에 대한 감지가 추가되었습니다

## Version 0.7.4

### 中文

- [+] 远程终端检测现在会检测使用信号发射器了
- [/] 修复了 RPC 上报检测部分问题
- [/] 修复了家具能被异常旋转

### English

- [+] Remote terminal detection now detects the use of signal transmitters
- [/] Fixed some issues with RPC reporting detection
- [/] Fixed the problem that furniture can be rotated abnormally

### 한국어

- [+] 원격 터미널 감지에서 이제 신호 발신기 사용을 감지합니다
- [/] RPC 보고 감지의 일부 문제가 수정되었습니다
- [/] 가구가 비정상적으로 회전될 수 있는 문제가 수정되었습니다

## Version 0.7.3

### 中文

- [+] 现在 隔空取物检测 开启时，会禁止使用喷气背包的玩家拿取双手重物
- [/] 优化了盒子异常行为检测，应该能减少误报

### English

- [+] Now when Picking Up Items Detection is on, players using jetpacks will be prohibited from picking up two-handed heavy objects
- [/] Optimized the abnormal behavior detection of jester, which should reduce false positives

### 한국어

- [+] 이제 아이템 픽업 감지가 활성화되면 제트팩을 사용하는 플레이어가 양손 무거운 물체를 집는 것이 금지됩니다
- [/] 제스터의 비정상적인 행동 감지가 최적화되어 오탐이 줄어들 것입니다

## Version 0.7.2

### 中文

- [/] 修复 0.4.6/0.4.7 版本导致의帧率过低问题

### English

- [/] Fix low fps issue caused by version 0.4.6/0.4.7

### 한국어

- [/] 0.4.6/0.4.7 버전으로 인한 낮은 프레임률 문제가 수정되었습니다

## Version 0.7.1

### 中文

- [+] 新增玩家使用负伤害回血检测(0.7.0 已实装但未加配置)
- [/] 修复了部分检测在客机生效的问题

### English

- [+] Added player health recovery detection when using negative damage (already implemented in 0.7.0 but not configured)
- [/] Fixed the problem that some detections were effective on client

### 한국어

- [+] 음수 피해를 사용한 플레이어 체력 회복 감지가 추가되었습니다 (0.7.0에서 이미 구현되었지만 설정되지 않았음)
- [/] 일부 감지가 클라이언트에서 작동하는 문제가 수정되었습니다

## Version 0.7.0

### 中文

- [+] 新增 RPC 上报检测(用于检测玩家是否开启无敌)
- [/] 飞船物品位置异常检测新增旋转角度检测

### English

- [+] Added RPC reporting detection (used to detect whether the player has turned on invincibility)
- [/] Added rotation angle detection to the abnormal position detection of furniture

### 한국어

- [+] RPC 보고 감지가 추가되었습니다 (플레이어가 무적을 켰는지 감지하는 데 사용)
- [/] 가구의 비정상적인 위치 감지에 회전 각도 감지가 추가되었습니다

## Version 0.6.9

### 中文

- [/] 修复了铲子伤害检测误报的问题

### English

- [/] Fixed a bug that caused shovel damage detection to be false positive

### 한국어

- [/] 삽 피해 감지가 오탐을 일으키는 버그가 수정되었습니다

## Version 0.6.8

### 中文

- [-] 移除 serverTag 的 AC 标识（该标识导致新版本必须要输入标识才能找到房间）

### English

- [-] Remove the AC tag from serverTag (this tag requires you to enter the tag to find the lobby in the new version)

### 한국어

- [-] serverTag에서 AC 태그가 제거되었습니다 (이 태그로 인해 새 버전에서 로비를 찾으려면 태그를 입력해야 했습니다)

## Version 0.6.7

### 中文

- [+] 适配 v56 版본
- [-] 移除玩家负重异常检测(zeekeers 在 v56 beta2 修复了该 bug)

### English

- [+] Adapt to v56 version
- [-] Removed abnormal player carry weight detection (zeekeers fixed this bug in v56 beta2)

### 한국어

- [+] v56 버전에 적응
- [-] 비정상적인 플레이어 운반 무게 감지가 제거되었습니다 (zeekeers가 v56 beta2에서 이 버그를 수정했습니다)

## Version 0.6.6

### 中文

- [/] 更改强制引爆地雷逻辑

### English

- [/] Change the logic for detecting landmine detonation

### 한국어

- [/] 지뢰 폭발 감지 로직이 변경되었습니다

## Version 0.6.5

### 中文

- [+] 语言配置将会根据电脑区域设置来决定，如果你的区域设置为以下几种 香港特别行政区，澳门特别行政区，中国，中文（简体），新加坡，台湾，中文（繁体） 那么将会使用中文作为默认模组语言，否则将会使用英语作为默认语言
- [+] 新增配置项 DetectedMessageType ，现在你可以更改配置决定检测的消息如何显示了

### English

- [+] The language configuration will be determined according to the locale of the computer, if your locale is set to the following Hong Kong SAR, Macau SAR, China, Chinese (Simplified), Singapore, Taiwan, Chinese (Traditional) Then Chinese will be used as the default mod language, otherwise English will be used as the default language
- [+] Added DetectedMessageType to the config item, now you can change the configuration to determine how the detected messages are displayed

### 한국어

- [+] 언어 설정은 컴퓨터의 지역 설정에 따라 결정됩니다. 지역 설정이 홍콩 특별행정구, 마카오 특별행정구, 중국, 중국어(간체), 싱가포르, 대만, 중국어(번체) 중 하나로 설정되어 있으면 중국어가 기본 모드 언어로 사용되고, 그렇지 않으면 영어가 기본 언어로 사용됩니다
- [+] DetectedMessageType 설정 항목이 추가되어 이제 감지된 메시지가 어떻게 표시될지 설정으로 변경할 수 있습니다

## Version 0.6.4

### 中文

- [/] 修复胡桃夹子枪击其他怪物导致检测伤害异常
- [/] 现在会正确兼容 MoreCompany 模组的化妆品了
- [/] 修复了对玩家异常伤害的检测

### English

- [/] Fixed Nutcracker shooting other monsters resulting in abnormal damage detection.
- [/] Cosmetics for MoreCompany modules are now correctly compatible.
- [/] Fixed detection of abnormal damage to players

### 한국어

- [/] 호두까기 인형이 다른 몬스터를 사격할 때 비정상적인 피해 감지가 발생하는 문제가 수정되었습니다.
- [/] MoreCompany 모드의 코스메틱이 이제 올바르게 호환됩니다.
- [/] 플레이어에 대한 비정상적인 피해 감지가 수정되었습니다

## Version 0.6.3(0.6.2)

### 中文

- [/] 修复 README.MD 跳转链接错误的问题

### English

- [/] Fix README.MD Jump Link Error

### 한국어

- [/] README.MD 이동 링크 오류 문제가 수정되었습니다

## Version 0.6.2(V50)

### 中文

- [/] 当前版本仅支持 V50，不向下兼容
- [+] 新增对小刀的伤害/攻速检测
- [/] 优化部分日志信息
- [/] 修复获得加班费之后导致的刷取金钱检测

### English

- [/] Current version only supports V50, not backward compatible
- [+] Added damage/attack speed detection for knife
- [/] Optimized some log messages
- [/] Fixed swipe money detection after getting overtime payment

### 한국어

- [/] 현재 버전은 V50만 지원하며 하위 호환되지 않습니다
- [+] 칼의 피해/공격 속도 감지가 추가되었습니다
- [/] 일부 로그 메시지가 최적화되었습니다
- [/] 야근수당을 받은 후 돈 복사 감지가 수정되었습니다

## Version 0.6.1

### 中文

- [/] 新增配置 EmptyHand 你可以通过启用该项允许玩家空手的时候造成正常伤害
- [/] 优化和补充缺失的英文翻译

### English

- [/] Added configuration EmptyHand You can enable this item to allow players to deal normal damage when empty-handed.
- [/] Optimized and added missing English translations.

### 한국어

- [/] EmptyHand 설정이 추가되었습니다. 이 항목을 활성화하여 플레이어가 맨손일 때 정상적인 피해를 입힐 수 있도록 허용할 수 있습니다.
- [/] 누락된 영어 번역이 최적화되고 추가되었습니다.

## Version 0.6.0

### 中文

- [/] 禁止玩家重复加入游戏
- [/] 零元购新增付费地图检测
- [/] 零元购现在会检测将金钱设置为 0 以下了
- [+] 检测玩家远程使用终端，新增配置项 RemoteTerminalSettings
- [/] 现在反作弊的补丁将会以安全的形式运行
- [/] 修复拼写错误，之前的版本配置 LanguageSetting 被拼写成了 LangugeSetting ，现已更正，修改过配置的玩家需要重新设置

### English

- [/] Prevent players from repeatedly joining the game.
- [/] Zero-dollar purchase now includes paid map detection.
- [/] Zero-dollar purchase will now detect if money is set below 0.
- [+] Added new configuration item "RemoteTerminalSettings" to detect players remotely using terminals.
- [/] The anti-cheat patch will now run in a safer form.
- [/] Fixed a typo. In previous versions, the configuration item "LanguageSetting" was misspelled as "LangugeSetting". This has now been corrected, and players who have modified this setting will need to reconfigure it.

### 한국어

- [/] 플레이어가 게임에 반복적으로 참가하는 것을 방지합니다.
- [/] 무료 구매에 유료 맵 감지가 포함되었습니다.
- [/] 무료 구매에서 이제 돈이 0 이하로 설정되는 것을 감지합니다.
- [+] 플레이어가 원격으로 터미널을 사용하는 것을 감지하는 새로운 설정 항목 "RemoteTerminalSettings"가 추가되었습니다.
- [/] 안티치트 패치가 이제 안전한 형태로 실행됩니다.
- [/] 맞춤법 오류가 수정되었습니다. 이전 버전에서 "LanguageSetting" 설정 항목이 "LangugeSetting"으로 잘못 철자되었습니다. 이제 수정되었으며, 이 설정을 수정한 플레이어는 다시 설정해야 합니다.

## Version 0.5.9

### 中文

- [/] 优化了英文翻译(@CoolLKKPS)

### English

- [/] Optimized the English translation(@CoolLKKPS)

### 한국어

- [/] 영어 번역이 최적화되었습니다(@CoolLKKPS)

## Version 0.5.8

### 中文

- [/] 部分空手打怪/销毁物品问题已兼容(?)
- [+] 新增配置 PlayerJoin 现在你可以자定义欢迎语句了！
- [/] 一票起飞功能修复

### English

- [/] The problem of empty-handed fighting monsters/destruction of items has been resolved (?)
- [+] Added PlayerJoin configuration, now you can customize the welcome message!
- [/] One-ticket departure function is fixed

### 한국어

- [/] 맨손으로 몬스터와 싸우기/아이템 파괴 문제가 해결되었습니다 (?)
- [+] PlayerJoin 설정이 추가되어 이제 환영 메시지를 사용자 정의할 수 있습니다!
- [/] 한 표 출발 기능이 수정되었습니다

## Version 0.5.7

### 中文

- [/] 回滚销毁检测机制
- [+] 新增配置 OnlyOneVote 用于控制主机是否可以一票起飞

### English

- [/] rollback destruction detection mechanism
- [+] Add configuration OnlyOneVote to control whether the host can take off with one ticket

### 한국어

- [/] 파괴 감지 메커니즘이 롤백되었습니다
- [+] 호스트가 한 표로 이륙할 수 있는지 제어하는 OnlyOneVote 설정이 추가되었습니다

## Version 0.5.6

### 中文

- [/] 修复出售货物导致销毁物品检测

### English

- [/] Fix the issue of selling goods leading to the detection of destroyed items.

### 한국어

- [/] 상품 판매가 아이템 파괴 감지로 이어지는 문제가 수정되었습니다.

## Version 0.5.5

### 中文

- [/] 修复不是房主也能一票起飞的问题
- [/] 现在被踢出的玩家在连接时就会被断开连接
- [+] 现在使用 AntiKick 进入游戏将会在控制台显示玩家名称和 SteamId
- [+] 现在伪造 SteamId 加入游戏将会被自动踢出

### English

- [/] Fixed the issue where non-hosts could initiate the game with a single vote.
- [/] Players who are kicked out will now be disconnected upon trying to reconnect.
- [+] Players who use AntiKick to enter the game will now have their usernames and SteamIds displayed in the console.
- [+] Players attempting to join the game with a falsified SteamId will now be automatically kicked out.

### 한국어

- [/] 호스트가 아닌 플레이어도 한 표로 게임을 시작할 수 있는 문제가 수정되었습니다.
- [/] 킥당한 플레이어가 다시 연결을 시도할 때 즉시 연결이 끊어집니다.
- [+] AntiKick을 사용하여 게임에 입장하는 플레이어의 사용자명과 SteamId가 콘솔에 표시됩니다.
- [+] 위조된 SteamId로 게임에 참가하려는 플레이어는 자동으로 킥됩니다.

## Version 0.5.4

### 中文

- [/] 修复房主投票少一票的问题
- [/] 修复部分情况无法拉杆
- [/] 现在不检测枪是否被销毁

### English

- [/] Fixed the issue of homeowners voting one less vote
- [/] Fixed some situations where it was not possible to pull the rod
- [/] Currently, we are not checking if the gun has been destroyed

### 한국어

- [/] 호스트의 투표가 한 표 부족한 문제가 수정되었습니다
- [/] 일부 상황에서 레버를 당길 수 없는 문제가 수정되었습니다
- [/] 현재 총이 파괴되었는지 확인하지 않습니다

## Version 0.5.3

### 中文

- [/] 修复投票导致游戏崩溃

### English

- [/] Fix the voting system that causes the game to crash

### 한국어

- [/] 게임을 크래시시키는 투표 시스템이 수정되었습니다

## Version 0.5.2

### 中文

- [/] 修复无法正常出售货物

### English

- [/] Repair the goods that cannot be sold normally

### 한국어

- [/] 정상적으로 판매할 수 없는 상품 문제가 수정되었습니다

## Version 0.5.1

### 中文

- [/] 修复出售货物导致销毁物品检测
- [+] 进游戏卡黑屏将会自动退出房间(未测试)

### English

- [/] Fix the issue of selling goods leading to the detection of destroyed items.
- [+] The game will automatically exit the room if it crashes with a black screen (not tested)

### 한국어

- [/] 상품 판매가 아이템 파괴 감지로 이어지는 문제가 수정되었습니다.
- [+] 게임이 검은 화면으로 크래시될 경우 자동으로 방에서 나갑니다 (테스트되지 않음)

## Version 0.5.0

### 中文

- [+] 现在禁止使用双手拿取物品了
- [/] 修改了销毁物品的检测机制
- [/] 修复死亡后投票提示问题
- [/] 房主一票直接起飞 修改为 一个小时后起飞
- [/] 修复客户端拉杆失败导致无法继续拉杆

### English

- [+] It is now prohibited to pick up items with both hands.
- [/] The detection mechanism for destroying items has been modified.
- [/] Fixed the issue with the voting prompt after death.
- [/] The rule where the host can immediately take off with one vote has been changed to taking off after one hour.
- [/] Fixed the issue where failing to pull the lever on the client side prevents further lever pulling.

### 한국어

- [+] 이제 양손으로 아이템을 집는 것이 금지되었습니다.
- [/] 아이템 파괴 감지 메커니즘이 수정되었습니다.
- [/] 사망 후 투표 알림 문제가 수정되었습니다.
- [/] 호스트가 한 표로 즉시 이륙하는 규칙이 한 시간 후 이륙으로 변경되었습니다.
- [/] 클라이언트 측에서 레버 당기기 실패로 인해 더 이상 레버를 당길 수 없는 문제가 수정되었습니다.

## Version 0.4.9

### 中文

- [+] 新增 ServerNameSetting.Prefix 配置，现在你可以通过改变配置来决定房间名的前缀(如果为空则为不启用)
- [/] 修复死亡后投票提示问题
- [/] 修改了铲子范围异常检测部分机制

### English

- [+] Added ServerNameSetting.Prefix configuration, now you can determine the prefix of the room name by changing the configuration (if empty, it is not enabled).
- [/] Fixed the issue of voting prompts after death.
- [/] Modified the mechanism for detecting shovel range abnormalities.

### 한국어

- [+] ServerNameSetting.Prefix 설정이 추가되었습니다. 이제 설정을 변경하여 방 이름의 접두사를 결정할 수 있습니다 (비어있으면 비활성화됩니다).
- [/] 사망 후 투표 알림 문제가 수정되었습니다.
- [/] 삽 범위 이상 감지 메커니즘이 수정되었습니다.

## Version 0.4.8

### 中文

- [/] 修复 0.4.6/0.4.7 版本导致的帧率过低问题

### English

- [/] Fix low fps issue caused by version 0.4.6/0.4.7

### 한국어

- [/] 0.4.6/0.4.7 버전으로 인한 낮은 fps 문제가 수정되었습니다

## Version 0.4.7

### 中文

- [/] 是的，我忘记上传修改日志了，那么我们直接跳过 0.4.7 版本(当前版本改动和 0.4.6 一样，并且版本号使用 0.4.6)

### English

- [/] Yes, I forgot to upload the change log. Let's skip version 0.4.7 directly (the changes in the current version are the same as those in 0.4.6, and we'll use version number 0.4.6).

### 한국어

- [/] 네, 변경 로그 업로드를 깜빡했습니다. 0.4.7 버전을 바로 건너뛰겠습니다 (현재 버전의 변경사항은 0.4.6과 동일하며, 버전 번호는 0.4.6을 사용합니다).

## Version 0.4.6

### 中文

- [+] 当你创建房间时，房间名会自动新增[AC] 作为前缀，例如："[v49] 战斗爽" 会变成 "[v49/AC] 战斗爽！" 或者 "战斗爽！" 会变成 "[AC] 战斗爽！"
- [+] 新增死亡投票提示(你是房主，投票直接起飞！)

### English

- [+] When you create a room, the room name will automatically have "[AC]" added as a prefix. For example, "[v49] The battle is thrilling!" will become "[v49/AC] The battle is thrilling!" or "The battle is thrilling!" will become "[AC] The battle is thrilling!".
- [+] You are the homeowner, and voting can immediately make the spaceship take off.

### 한국어

- [+] 방을 생성할 때 방 이름에 자동으로 "[AC]"가 접두사로 추가됩니다. 예: "[v49] 전투 재미!" → "[v49/AC] 전투 재미!" 또는 "전투 재미!" → "[AC] 전투 재미!"
- [+] 사망 투표 알림이 추가되었습니다 (당신은 호스트입니다, 투표하면 즉시 이륙합니다!)

## Version 0.4.5

### 中文

- [/] 修复了配置文件描述丢失的问题

### English

- [/] Fix the issue of missing configuration file descriptions

### 한국어

- [/] 설정 파일 설명이 누락되는 문제가 수정되었습니다

## Version 0.4.4

### 中文

- [/] 修复缺少描述导致语言无法正常加载

### English

- [/] Fix the issue where missing descriptions cause languages to fail to load properly

### 한국어

- [/] 설명이 누락되어 언어가 제대로 로드되지 않는 문제가 수정되었습니다

## Version 0.4.3

### 中文

- [+] 自动踢出开启防踢的玩家
- [+] 新增 Log 配置，用于控制反作弊的日志输出

### English

- [+] Automatically kick out players with anti-kick enabled
- [+] New Log configuration added to control the output of anti cheating logs

### 한국어

- [+] 안티킥이 활성화된 플레이어를 자동으로 킥합니다
- [+] 안티치트 로그 출력을 제어하는 새로운 Log 설정이 추가되었습니다

## Version 0.4.2

### 中文

- [/] 更改强制引爆地雷逻辑

### English

- [/] Change the logic for detecting landmine detonation

### 한국어

- [/] 지뢰 강제 폭발 로직이 변경되었습니다

## Version 0.4.1

### 中文

- [+] 从 0.4.1 版本以后将会发布更新日志
- [/] 修复怪物不能正常击杀客机玩家

### English

- [+] There will be modification logs in future versions
- [/] Monsters cannot kill client players normally

### 한국어

- [+] 0.4.1 버전부터 업데이트 로그가 발표됩니다
- [/] 몬스터가 클라이언트 플레이어를 정상적으로 죽일 수 없는 문제가 수정되었습니다
