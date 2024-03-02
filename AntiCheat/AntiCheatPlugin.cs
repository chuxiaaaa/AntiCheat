using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        public const string Version = "0.4.9";
        public static ManualLogSource ManualLog = null;
        public enum Language
        {
            简体中文,
            English,
        }
        public static ConfigEntry<Language> LanguageConfig;

        public static ConfigEntry<bool> Log;

        public static ConfigEntry<bool> Shovel;
        public static ConfigEntry<bool> Shovel2;

        public static ConfigEntry<string> Prefix;

        public static ConfigEntry<bool> ShipConfig;
        public static ConfigEntry<int> ShipConfig2;
        public static ConfigEntry<string> ShipConfig3;
        public static ConfigEntry<int> ShipConfig4;
        public static ConfigEntry<bool> ShipConfig5;

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

        public static ConfigEntry<bool> InfiniteAmmo;
        public static ConfigEntry<bool> InfiniteAmmo2;

        public static ConfigEntry<bool> FreeBuy;
        public static ConfigEntry<bool> FreeBuy2;

        public static ConfigEntry<bool> Nameless;
        public static ConfigEntry<bool> Nameless2;


        void Awake()
        {
            ManualLog = Logger;


            LanguageConfig = Config.Bind("LangugeSetting", "Language", Language.简体中文, string.Join(",", LocalizationManager.Languages.Select(x => x.Key)));
            LocalizationManager.SetLanguage(LanguageConfig.Value.ToString());
            Prefix = Config.Bind("ServerNameSetting", "Prefix", "AC", LocalizationManager.GetString("config_Prefix"));
            ShipConfig = Config.Bind("ShipSetting", "StartGameOnlyHost", true, LocalizationManager.GetString("config_ShipSetting"));
            Log = Config.Bind("LogSetting", "Log", true, LocalizationManager.GetString("config_Log"));
            ShipConfig5 = Config.Bind("ShipSetting", "Kick", false, LocalizationManager.GetString("config_ShipConfig5"));
            ShipConfig2 = Config.Bind("ShipSetting", "StartGamePlayerCount", 8, LocalizationManager.GetString("config_ShipConfig2"));
            ShipConfig3 = Config.Bind("ShipSetting", "EndGamePlayerTime", "14:00", LocalizationManager.GetString("config_ShipConfig3"));
            ShipConfig4 = Config.Bind("ShipSetting", "EndGamePlayerCount", 50, LocalizationManager.GetString("config_ShipConfig4"));

            ShipBuild = Config.Bind("ShipBuildSetting", "Enable", true, LocalizationManager.GetString("config_ShipBuild"));
            ShipBuild2 = Config.Bind("ShipBuildSetting", "Kick", false, LocalizationManager.GetString("config_Kick"));

            ItemCooldown = Config.Bind("ItemCooldownSetting", "Enable", true, LocalizationManager.GetString("config_ItemCooldown"));
            ItemCooldown2 = Config.Bind("ItemCooldownSetting", "Kick", false, LocalizationManager.GetString("config_Kick"));

            ShipLight = Config.Bind("ShipLightSettings", "Enable", true, LocalizationManager.GetString("config_ShipLight"));

            ShipTerminal = Config.Bind("ShipTerminalSettings", "Enable", true, LocalizationManager.GetString("config_ShipTerminal"));
            ShipTerminal2 = Config.Bind("ShipTerminalSettings", "Kick", false, LocalizationManager.GetString("config_Kick"));

            DespawnItem = Config.Bind("DespawnItemSettings", "Enable", true, LocalizationManager.GetString("config_DespawnItem"));
            DespawnItem2 = Config.Bind("DespawnItemSettings", "Kick", false, LocalizationManager.GetString("config_Kick"));

            ChatReal = Config.Bind("ChatRealSettings", "Enable", true, LocalizationManager.GetString("config_ChatReal"));
            ChatReal2 = Config.Bind("ChatRealSettings", "Kick", false, LocalizationManager.GetString("config_Kick"));

            Mask = Config.Bind("MaskSettings", "Enable", true, LocalizationManager.GetString("config_Mask"));
            Mask2 = Config.Bind("MaskSettings", "Kick", false, LocalizationManager.GetString("config_Kick"));

            Gift = Config.Bind("GiftSettings", "Enable", true, LocalizationManager.GetString("config_Gift"));
            Gift2 = Config.Bind("GiftSettings", "Kick", false, LocalizationManager.GetString("config_Kick"));

            Turret = Config.Bind("TurretSettings", "Enable", true, LocalizationManager.GetString("config_Turret"));
            Turret2 = Config.Bind("TurretSettings", "Kick", false, LocalizationManager.GetString("config_Kick"));

            InfiniteAmmo = Config.Bind("InfiniteAmmoSettings", "Enable", true, LocalizationManager.GetString("config_InfiniteAmmo"));
            InfiniteAmmo2 = Config.Bind("InfiniteAmmoSettings", "Kick", false, LocalizationManager.GetString("config_Kick"));

            Invisibility = Config.Bind("InvisibilitySettings", "Enable", true, LocalizationManager.GetString("config_Invisibility"));
            Invisibility2 = Config.Bind("InvisibilitySettings", "Kick", false, LocalizationManager.GetString("config_Kick"));


            Boss = Config.Bind("BossSetting", "Enable", true, LocalizationManager.GetString("config_Boss"));
            Boss2 = Config.Bind("BossSetting", "Kick", false, LocalizationManager.GetString("config_Kick"));

            Jetpack = Config.Bind("JetpackSetting", "Enable", true, LocalizationManager.GetString("config_Jetpack"));
            Jetpack2 = Config.Bind("JetpackSetting", "Kick", false, LocalizationManager.GetString("config_Kick"));


            Landmine = Config.Bind("LandmineSetting", "Enable", true, LocalizationManager.GetString("config_Landmine"));
            Landmine2 = Config.Bind("LandmineSetting", "Kick", false, LocalizationManager.GetString("config_Kick"));

            SpawnWebTrap = Config.Bind("SpawnWebTrapSetting", "Enable", true, LocalizationManager.GetString("config_SpawnWebTrap"));
            SpawnWebTrap2 = Config.Bind("SpawnWebTrapSetting", "Kick", false, LocalizationManager.GetString("config_Kick"));

            Enemy = Config.Bind("EnemySetting", "Enable", true, LocalizationManager.GetString("config_Enemy"));

            KillEnemy = Config.Bind("KillEnemySetting", "Enable", true, LocalizationManager.GetString("config_KillEnemy"));
            KillEnemy2 = Config.Bind("KillEnemySetting", "Kick", false, LocalizationManager.GetString("config_Kick"));

            Map = Config.Bind("MapSetting", "Enable", true, LocalizationManager.GetString("config_Map"));
            Map2 = Config.Bind("MapSetting", "Kick", false, LocalizationManager.GetString("config_Kick"));

            GrabObject = Config.Bind("GrabObjectSetting", "Enable", true, LocalizationManager.GetString("config_GrabObject"));
            GrabObject2 = Config.Bind("GrabObjectSetting", "Kick", false, LocalizationManager.GetString("config_Kick"));

            Shovel = Config.Bind("ShovelSettings", "Enable", true, LocalizationManager.GetString("config_Shovel"));
            Shovel2 = Config.Bind("ShovelSettings", "Kick", false, LocalizationManager.GetString("config_Kick"));

            Nameless = Config.Bind("NamelessSettings", "Enable", true, LocalizationManager.GetString("config_Nameless"));
            Nameless2 = Config.Bind("NamelessSettings", "Kick", false, LocalizationManager.GetString("config_Kick"));

            FreeBuy = Config.Bind("FreeBuySettings", "Enable", true, LocalizationManager.GetString("config_FreeBuy"));
            FreeBuy2 = Config.Bind("NamelessSettings", "Kick", false, LocalizationManager.GetString("config_Kick"));

            Harmony.CreateAndPatchAll(typeof(Patch));

            ManualLog.LogInfo($"{LocalizationManager.GetString("log_load")}");
        }


    }
}
