# AntiCheat

[**English(Support)**](./docs/README-en.md) | [**Русский**](./docs/README-ru.md)

一个致命公司反作弊模组(仅限主机使用)

## 简介

模组交流QQ群：263868521

该模组用于解决外挂炸房问题，确保路人房玩家的游戏体验

在使用模组时你可能会遇到"误检测"问题，但这并不一定是 反作弊 模组的问题，请尝试删除 除了(BepInExPack，汉化，反作弊，多人联机）以外的所有模组复现问题（如不再误测，则是模组的兼容性问题）

## 使用说明

1. 安装 [BepInExPack](https://thunderstore.io/c/lethal-company/p/BepInEx/BepInExPack/) 模组
2. 将 AntiCheat.dll 放置于 Lethal Company\BepInEx\plugins\ 目录下
3. 启动游戏

## 创建房间

使用反作弊时，建议您在房间名标注 [AC] 字样，向其他玩家标识这是一个启用反作弊的房间！
例如：[v49/AC] 战斗爽！

## 功能

### 检测
* **防踢检测**

  描述：检测玩家是否开启防踢功能加入游戏房间
  
* **拉杆检测**

  * 强制拉杆检测
    
    描述：检测新进房间的玩家进行拉杆
  * 起飞拉杆检测
    
    描述：当飞船着陆后，检测玩家拉杆是否符合条件(在配置文件中修改)
  * 降落拉杆检测
    
    描述：飞船在轨道时，检测玩家拉杆是否符合条件(在配置文件中修改)
  
* 飞船物品位置异常检测
* 物品使用冷却异常检测
* 灯关秀检测
* 终端噪音检测
* 销毁物品检测
* 发言伪造检测
* 刷假人检测
* 刷礼物盒检测
* 刷蜘蛛网检测
* 激怒机枪检测
* 隐身检测
* 喷气背包爆炸检测
* 引爆地雷检测
* 怪物异常检测
* 小地图检测
* 击杀敌人检测
* 隔空取物检测
* 铲子伤害异常检测
* 玩家名字检测(Nameless或Unknown)
* 零元购检测（购买物品/解锁飞船装饰）
* 无限子弹检测
* 禁止客户端调用怪物击杀其他玩家RPC (防秒杀)

### 配置

* **Lanauge**

  描述：模组语言，对配置文件描述以及游戏内文本生效，可选值(中文/English)

## 贡献
<a href="https://github.com/chuxiaaaa/AntiCheat/graphs/contributors">
  <img src="https://contrib.rocks/image?repo=chuxiaaaa/AntiCheat" />
</a>

### 游戏语言

* 中文(chuxiaaaa&CoolLKKPS)
* 英文(DeathWrench&CoolLKKPS)

### 自述语言

* 中文(chuxiaaaa&CoolLKKPS)
* 英文(NilaierMusic&CoolLKKPS)
* 俄文(NilaierMusic)

### 模组测试人员
* 超级骇人鲸 
* Alan Backer
* 沈阳最速傳說疾走の猛虎！貴物刀一郎
* 柒小鸭yz
* 喜欢睡觉の极茶龙 
* 东南枝
* Melissa
* 我不吃牛肉

欢迎各位有能力的开发者为本模组提交贡献~

## 反馈
如果你遇到误检测并确定不是模组导致，你可以提交issue来说明如何复现问题帮助我们修复问题！
