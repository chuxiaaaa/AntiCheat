# AntiCheat

[![Thunderstore Version](https://img.shields.io/thunderstore/v/chuxiaaaa/AntiCheat?style=for-the-badge&logo=thunderstore&logoColor=white)](https://thunderstore.io/c/lethal-company/p/chuxiaaaa/AntiCheat/versions/)
[![Thunderstore Downloads](https://img.shields.io/thunderstore/dt/chuxiaaaa/AntiCheat?style=for-the-badge&logo=thunderstore&logoColor=white)](https://thunderstore.io/c/lethal-company/p/chuxiaaaa/AntiCheat/)

**简体中文(Support)** | [**English(Support)**](https://github.com/chuxiaaaa/AntiCheat/blob/main/docs/README-en.md) | [**Русский**](https://github.com/chuxiaaaa/AntiCheat/blob/main/docs/README-ru.md) | [**한국어(Support)**](https://github.com/chuxiaaaa/AntiCheat/blob/main/docs/README-ko.md)

一个致命公司反作弊主机端模组

## i18国际化

在 0.8.2 版本以后，反作弊支持完全自定义提示内容

### 指定使用语言文件
  1. 在 AntiCheat 文件目录下找到 locales 文件夹，打开 localization_cfg.json 文件
  2. 修改 ``current_language`` 的值
      - 如果为空，反作弊将会根据系统语言自动设置为 zh_CN/en_US
      - 如果想指定，请将值设定为 en_US/zh_CN

### 创建新语言文件
  1. 复制 en_US.json 文件
  2. 修改文件名，将其修改为你的语言文件名称，例如 en_US.json -> zh_CN.json
  3. 更改文件内容，修改完毕后将 ``localization_cfg.json`` 中 ``current_language`` 的值
  4. (可选)在 Github 中提交pull request，说明并提交你的语言文件

## 简介

模组交流 QQ 群：263868521

该模组用于解决外挂炸房问题，确保路人房玩家的游戏体验

在使用模组时你可能会遇到"误检测"问题，但这并不一定是 反作弊 模组的问题，请尝试删除 除了（BepInExPack，汉化，反作弊，多人联机）以外的所有模组复现问题（如不再误测，则是模组的兼容性问题）

## 使用说明

1. 安装 [BepInExPack](https://thunderstore.io/c/lethal-company/p/BepInEx/BepInExPack) 模组
2. 将 AntiCheat.dll 放置于 Lethal Company\BepInEx\plugins\ 目录下
3. 启动游戏

## 创建房间

使用反作弊时，建议您在房间名标注 [AC] 字样，向其他玩家标识这是一个启用反作弊的房间！
例如：[v69/AC] 战斗爽！

## 功能

### 检测

- **防踢检测**

  描述：检测玩家是否开启防踢功能加入游戏房间

- **拉杆检测**

  - 强制拉杆检测

    描述：检测新进房间的玩家进行拉杆

  - 起飞拉杆检测

    描述：当飞船着陆后，检测玩家拉杆是否符合条件（在配置文件中修改）

  - 降落拉杆检测

    描述：飞船在轨道时，检测玩家拉杆是否符合条件（在配置文件中修改）

- **飞船物品位置异常检测**

  描述：检测飞船物品坐标是否在飞船范围内

- **物品使用冷却异常检测**

  描述：检测挥铲速度以及击发霰弹枪的速度

- **灯关秀检测**

  描述：检测玩家是否频繁切换灯光

- **终端噪音检测**

  描述：检测玩家是否导致终端发出大量噪音

- **销毁物品检测**

  描述：检测玩家销毁不应该被销毁的物品

- **发言伪造检测**

  描述：检测玩家是否以其他玩家的身份发言

- **刷假人检测**

  描述：检测玩家是否通过面具多次生成假人

- **刷礼物盒检测**

  描述：检测礼物盒开启次数

- **刷蜘蛛网检测**

  描述：检测玩家调用蜘蛛生成大量蜘蛛网

- **激怒机枪检测**

  描述：检测玩家是否远距离导致机枪狂暴

- **隐身检测**

  描述：检测玩家是否隐身

- **喷气背包爆炸检测**

  描述：检测玩家是否引爆其他玩家的喷气背包

- **引爆地雷检测**

  描述：检测玩家是否恶意引爆地雷

- **怪物异常检测**

  描述：检测玩家是否控怪以及修改怪物机制

- **小地图检测**

  描述：检测玩家是否安装小地图模组

- **击杀敌人检测**

  描述：检测玩家是否远距离击杀敌人

- **隔空取物检测**

  描述：检测玩家是否远距离拿取物品

- **铲子伤害异常检测**

  描述：检测玩家使用铲子时的伤害

- **玩家名字检测**

  描述：出现玩家名字异常时提示，例如 Nameless 或 Unknown

- **零元购检测**

  描述：检测玩家是否免费购买物品、免费解锁飞船装饰以及免费导航至付费星球

- **无限子弹检测**

  描述：检测玩家使用霰弹枪时是否有子弹

- **召唤老板检测**

  描述：检测玩家调用老板攻击函数

- **远程终端检测**

  描述：检测玩家是否远程使用终端

- **怪物击杀玩家检测**

  描述：检测玩家是否调用怪物的击杀函数击杀其他玩家

- **重复加入游戏检测**

  描述：检测玩家已经在房间的时候继续加入房间

* [ ] **速度检测**

  描述：检测玩家当前以异常的速度行走或奔跑(等待解决，目前没有思路)

* [ ] **飞行检测**

  描述：检测玩家不借助任何物品进行飞行(等待解决，目前没有思路)

### 其他功能

- **房间名称前缀**

  描述：默认为 AC，用于标识房间使用安装反作弊，你可以通过修改配置来更改标识或者设置为空取消标识。

- **加入游戏提示**

  描述：由于游戏原版加入提示过于混乱，所以反作弊取消了原版加入提示，你可以在配置中自定义玩家加入游戏的提示。

- **玩家操作信息**

  描述：显示玩家操作信息(加入房间/购买物品/切换星球)

- **允许客户端配置差异**

  描述：允许客户端存在模组差异，有助于缓解客户端无法加入房间的问题

## 贡献

<a href="https://github.com/chuxiaaaa/AntiCheat/graphs/contributors">
  <img src="https://contrib.rocks/image?repo=chuxiaaaa/AntiCheat" />
</a>

### 游戏语言

- 中文 (chuxiaaaa & CoolLKKPS)
- 英文 (DeathWrench & CoolLKKPS & glyphical)
- 韩文 (P-Asta)

### 自述语言

- 中文 (chuxiaaaa & CoolLKKPS)
- 英文 (NilaierMusic & CoolLKKPS)
- 俄文 (NilaierMusic)
- 韩文 (P-Asta)

### 模组测试人员

- 超级骇人鲸
- Alan Backer
- 沈阳最速傳說疾走の猛虎！貴物刀一郎
- 柒小鸭 yz
- 喜欢睡觉の极茶龙
- 东南枝
- Melissa
- 我不吃牛肉

欢迎各位有能力的开发者为本模组提交贡献~

## 反馈

如果你遇到误检测并确定不是模组导致，你可以提交 issue 来说明如何复现问题帮助我们修复问题！
