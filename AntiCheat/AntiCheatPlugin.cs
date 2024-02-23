using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AntiCheat
{
    [BepInPlugin("AntiCheat", "AntiCheat", Version)]
    public class AntiCheatPlugin : BaseUnityPlugin
    {
        public const string Version = "0.3.1";
        public static ManualLogSource ManualLog = null;

        public static ConfigEntry<bool> ShovelConfig;
        public static ConfigEntry<bool> ShovelConfig2;

        public static ConfigEntry<bool> ShipConfig;
        public static ConfigEntry<int> ShipConfig2;
        public static ConfigEntry<string> ShipConfig3;
        public static ConfigEntry<int> ShipConfig4;
        public static ConfigEntry<bool> ShipConfig5;
        public static ConfigEntry<int> ShipConfig6;

        public static ConfigEntry<bool> ShipBuild;
        public static ConfigEntry<bool> ShipBuild2;

        public static ConfigEntry<bool> ShipLight;
        public static ConfigEntry<bool> ShipTerminal;
        public static ConfigEntry<bool> ShipTerminal2;

        public static ConfigEntry<bool> DespawnItem;
        public static ConfigEntry<bool> DespawnItem2;

        public static ConfigEntry<bool> ChatReal;
        public static ConfigEntry<bool> ChatReal2;

        public static ConfigEntry<bool> Mask;
        public static ConfigEntry<bool> Mask2;

        public static ConfigEntry<bool> Gift;
        public static ConfigEntry<bool> Gift2;

        public static ConfigEntry<bool> Invisibility;
        public static ConfigEntry<bool> Invisibility2;

        public static ConfigEntry<bool> GrabObject;
        public static ConfigEntry<bool> GrabObject2;

        public static ConfigEntry<bool> Boss;
        public static ConfigEntry<bool> Boss2;

        public static ConfigEntry<bool> Landmine;
        public static ConfigEntry<bool> Landmine2;

        public static ConfigEntry<bool> Turret;
        public static ConfigEntry<bool> Turret2;

        public static ConfigEntry<bool> Enemy;

        public static ConfigEntry<bool> KillEnemy;
        public static ConfigEntry<bool> KillEnemy2;

        public static ConfigEntry<bool> SpawnWebTrap;
        public static ConfigEntry<bool> SpawnWebTrap2;

        public static ConfigEntry<bool> Map;
        public static ConfigEntry<bool> Map2;

        public static ConfigEntry<bool> Jetpack;
        public static ConfigEntry<bool> Jetpack2;

        public static ConfigEntry<bool> ItemCooldown;
        public static ConfigEntry<bool> ItemCooldown2;

        public static ConfigEntry<bool> FreeBuyConfig;
        public static ConfigEntry<bool> FreeBuyConfig2;

        public static ConfigEntry<bool> NamelessConfig;
        public static ConfigEntry<bool> NamelessConfig2;


        void Awake()
        {
            ManualLog = Logger;

            ShipConfig = Config.Bind("ShipSetting", "StartGameOnlyHost", true, "只有房主才有着陆权限");
            ShipConfig5 = Config.Bind("ShipSetting", "Kick", false, "检测强制拉杆行为(检测到该行为时踢出玩家)");
            ShipConfig2 = Config.Bind("ShipSetting", "StartGamePlayerCount", 8, "非房主只有在超过指定人数才能进行着陆(需要关闭房主权限)");
            ShipConfig3 = Config.Bind("ShipSetting", "EndGamePlayerTime", "18:00", "非房主只有在该时间点以后才能起飞");
            ShipConfig4 = Config.Bind("ShipSetting", "EndGamePlayerCount", 50, "非房主只有在当飞船人数(存活)超过以上比例才能起飞(0-100%)");
            ShipConfig6 = Config.Bind("ShipSetting", "EndGameVoteCount", 50, "投票超过以上比例才能起飞(0-100%)");


            ShipBuild = Config.Bind("ShipBuildSetting", "Enable", true, "飞船物品位置异常检测");
            ShipBuild2 = Config.Bind("ShipBuildSetting", "Kick", false, "检测到该行为时踢出玩家");

            ItemCooldown = Config.Bind("ItemCooldownSetting", "Enable", true, "物品使用冷却异常检测");
            ItemCooldown2 = Config.Bind("ItemCooldownSetting", "Kick", false, "检测到该行为时踢出玩家");

            ShipLight = Config.Bind("ShipLightSettings", "Enable", true, "灯关秀检测");

            ShipTerminal = Config.Bind("ShipTerminalSettings", "Enable", true, "终端噪音检测");
            ShipTerminal2 = Config.Bind("ShipTerminalSettings", "Kick", false, "检测到该行为时踢出玩家");

            DespawnItem = Config.Bind("DespawnItemSettings", "Enable", true, "销毁物品检测");
            DespawnItem2 = Config.Bind("DespawnItemSettings", "Kick", false, "检测到该行为时踢出玩家");

            ChatReal = Config.Bind("ChatRealSettings", "Enable", true, "发言伪造检测");
            ChatReal2 = Config.Bind("ChatRealSettings", "Kick", false, "检测到该行为时踢出玩家");

            Mask = Config.Bind("MaskSettings", "Enable", true, "刷假人检测");
            Mask2 = Config.Bind("MaskSettings", "Kick", false, "检测到该行为时踢出玩家");

            Gift = Config.Bind("GiftSettings", "Enable", true, "刷礼物盒检测");
            Gift2 = Config.Bind("GiftSettings", "Kick", false, "检测到该行为时踢出玩家");

            Turret = Config.Bind("TurretSettings", "Enable", true, "激怒机枪检测");
            Turret2 = Config.Bind("TurretSettings", "Kick", false, "检测到该行为时踢出玩家");

            Invisibility = Config.Bind("InvisibilitySettings", "Enable", true, "隐身检测");
            Invisibility2 = Config.Bind("InvisibilitySettings", "Kick", false, "检测到该行为时踢出玩家");


            Boss = Config.Bind("BossSetting", "Enable", true, "召唤老板检测");
            Boss2 = Config.Bind("BossSetting", "Kick", false, "检测到该行为时踢出玩家");

            Jetpack = Config.Bind("JetpackSetting", "Enable", true, "喷气背包爆炸检测");
            Jetpack2 = Config.Bind("JetpackSetting", "Kick", false, "检测到该行为时踢出玩家");


            Landmine = Config.Bind("LandmineSetting", "Enable", true, "引爆地雷检测");
            Landmine2 = Config.Bind("LandmineSetting", "Kick", false, "检测到该行为时踢出玩家");

            SpawnWebTrap = Config.Bind("SpawnWebTrapSetting", "Enable", true, "刷蜘蛛网检测");
            SpawnWebTrap2 = Config.Bind("SpawnWebTrapSetting", "Kick", false, "检测到该行为时踢出玩家");

            Enemy = Config.Bind("EnemySetting", "Enable", true, "怪物异常检测");

            KillEnemy = Config.Bind("KillEnemySetting", "Enable", true, "击杀敌人检测");
            KillEnemy2 = Config.Bind("KillEnemySetting", "Kick", false, "检测到该行为时踢出玩家");


            Map = Config.Bind("MapSetting", "Enable", true, "小地图检测");
            Map2 = Config.Bind("MapSetting", "Kick", false, "检测到该行为时踢出玩家");

            GrabObject = Config.Bind("GrabObjectSetting", "Enable", true, "隔空取物检测");
            GrabObject2 = Config.Bind("GrabObjectSetting", "Kick", false, "检测到该行为时踢出玩家");

            ShovelConfig = Config.Bind("ShovelSettings", "Enable", true, "铲子伤害异常检测");
            ShovelConfig2 = Config.Bind("ShovelSettings", "Kick", false, "检测到该行为时踢出玩家");

            NamelessConfig = Config.Bind("NamelessSettings", "Enable", true, "玩家名字检测(Nameless或Unknown)");
            NamelessConfig2 = Config.Bind("NamelessSettings", "Kick", false, "检测到该行为时踢出玩家");

            FreeBuyConfig = Config.Bind("FreeBuySettings", "Enable", true, "零元购检测");
            FreeBuyConfig2 = Config.Bind("NamelessSettings", "Kick", false, "检测到该行为时踢出玩家");

            Harmony.CreateAndPatchAll(typeof(Patch));

            ManualLog.LogInfo($"模组加载完毕!");
        }


    }
}
