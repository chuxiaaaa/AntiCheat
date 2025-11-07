using AntiCheat.Utils;

using BepInEx;
using BepInEx.Configuration;

using System;
using System.Collections.Generic;
using System.Text;

using static UnityEngine.InputSystem.InputRemoting;

namespace AntiCheat
{
    internal class PluginConfig
    {
        public static ConfigEntry<bool> Log = null!;
        public static ConfigEntry<bool> OperationLog = null!;
        public static ConfigEntry<bool> Shovel = null!;
        public static ConfigEntry<bool> Shovel2 = null!;
        public static ConfigEntry<bool> Shovel3 = null!;

        public static ConfigEntry<string> Prefix = null!;
        public static ConfigEntry<string> PlayerJoin = null!;

        public static ConfigEntry<bool> ShipConfig = null!;
        public static ConfigEntry<int> ShipConfig2 = null!;
        public static ConfigEntry<string> ShipConfig3 = null!;
        public static ConfigEntry<int> ShipConfig4 = null!;
        public static ConfigEntry<bool> Ship_Kick = null!;
        public static ConfigEntry<bool> ShipSetting_OnlyOneVote = null!;
        public static ConfigEntry<bool> ShipSetting_ChangToFreeMoon = null!;

        public static ConfigEntry<bool> ShipBuild = null!;
        public static ConfigEntry<bool> ShipBuild2 = null!;

        public static ConfigEntry<bool> ShipLight = null!;
        public static ConfigEntry<int> ShipLight_Cooldown = null!;

        public static ConfigEntry<bool> TerminalNoise = null!;
        public static ConfigEntry<int> TerminalNoise_Cooldown = null!;

        public static ConfigEntry<bool> DespawnItem = null!;
        public static ConfigEntry<bool> DespawnItem2 = null!;

        public static ConfigEntry<bool> ChatReal = null!;
        public static ConfigEntry<int> ChatReal_Cooldown = null!;
        public static ConfigEntry<bool> ChatReal2 = null!;

        public static ConfigEntry<bool> Mask = null!;
        public static ConfigEntry<bool> Mask2 = null!;

        public static ConfigEntry<bool> Gift = null!;
        public static ConfigEntry<bool> Gift2 = null!;

        public static ConfigEntry<bool> Invisibility = null!;
        public static ConfigEntry<bool> Invisibility2 = null!;

        public static ConfigEntry<bool> GrabObject = null!;
        public static ConfigEntry<bool> GrabObject_MoreSlot = null!;
        public static ConfigEntry<bool> GrabObject_TwoHand = null!;
        public static ConfigEntry<bool> GrabObject_BeltBag = null!;
        public static ConfigEntry<bool> GrabObject_Kick = null!;

        public static ConfigEntry<bool> Boss = null!;
        public static ConfigEntry<bool> Boss2 = null!;

        public static ConfigEntry<bool> Landmine = null!;
        public static ConfigEntry<bool> Landmine2 = null!;

        public static ConfigEntry<bool> PlayerCarryWeight = null!;
        public static ConfigEntry<bool> PlayerCarryWeight2 = null!;
        public static ConfigEntry<bool> PlayerCarryWeight3 = null!;


        public static ConfigEntry<bool> Turret = null!;
        public static ConfigEntry<bool> Turret2 = null!;

        public static ConfigEntry<bool> Enemy = null!;

        public static ConfigEntry<bool> KillEnemy = null!;
        public static ConfigEntry<bool> KillEnemy2 = null!;

        public static ConfigEntry<bool> SpawnWebTrap = null!;
        public static ConfigEntry<bool> SpawnWebTrap2 = null!;

        public static ConfigEntry<bool> Map = null!;
        public static ConfigEntry<bool> Map2 = null!;

        public static ConfigEntry<bool> Jetpack = null!;
        public static ConfigEntry<bool> Jetpack2 = null!;

        public static ConfigEntry<bool> ItemCooldown = null!;
        public static ConfigEntry<bool> ItemCooldown2 = null!;

        public static ConfigEntry<bool> InfiniteAmmo = null!;
        public static ConfigEntry<bool> InfiniteAmmo2 = null!;

        public static ConfigEntry<bool> FreeBuy = null!;
        public static ConfigEntry<bool> FreeBuy2 = null!;

        public static ConfigEntry<bool> RemoteTerminal = null!;
        public static ConfigEntry<bool> RemoteTerminal2 = null!;

        public static ConfigEntry<bool> Nameless = null!;
        public static ConfigEntry<bool> Nameless2 = null!;

        public static ConfigEntry<int> RPCReport_Delay = null!;
        public static ConfigEntry<bool> RPCReport_Hit = null!;
        public static ConfigEntry<bool> RPCReport_KillPlayer = null!;
        public static ConfigEntry<bool> RPCReport_Kick = null!;

        public static ConfigEntry<bool> Health_Recover = null!;
        public static ConfigEntry<bool> Health_Kick = null!;


        public static ConfigEntry<MessageType> DetectedMessageType = null!;

        internal static void Init(BaseUnityPlugin plugin)
        {
            var Config = plugin.Config;
            //IgnoreClientConfig = Config.Bind("VersionSetting", "NetworkSetting", false, localizationManager.Cfg_GetString("NetworkSetting"));
            Prefix = Config.Bind("ServerNameSetting", "Prefix", "AC", localizationManager.Cfg_GetString("Prefix"));
            ShipConfig = Config.Bind("ShipSetting", "StartGameOnlyHost", true, localizationManager.Cfg_GetString("ShipSetting"));
            Log = Config.Bind("LogSetting", "Log", true, localizationManager.Cfg_GetString("Log"));
            OperationLog = Config.Bind("LogSetting", "OperationLog", true, localizationManager.Cfg_GetString("OperationLog"));

            Ship_Kick = Config.Bind("ShipSetting", "Kick", false, localizationManager.Cfg_GetString("ShipConfig5"));
            ShipConfig2 = Config.Bind("ShipSetting", "StartGamePlayerCount", 8, localizationManager.Cfg_GetString("ShipConfig2"));
            ShipConfig3 = Config.Bind("ShipSetting", "EndGamePlayerTime", "14:00", localizationManager.Cfg_GetString("ShipConfig3"));
            ShipConfig4 = Config.Bind("ShipSetting", "EndGamePlayerCount", 50, localizationManager.Cfg_GetString("ShipConfig4"));
            ShipSetting_OnlyOneVote = Config.Bind("ShipSetting", "OnlyOneVote", true, localizationManager.Cfg_GetString("ShipConfig6"));
            ShipSetting_ChangToFreeMoon = Config.Bind<bool>("ShipSetting", "ChangToFreeMoon", false, localizationManager.Cfg_GetString("ChangToFreeMoon"));


            RPCReport_Delay = Config.Bind("RPCReportSetting", "Delay", 1000, localizationManager.Cfg_GetString("RPCReport_Delay"));
            RPCReport_Hit = Config.Bind("RPCReportSetting", "Hit", true, localizationManager.Cfg_GetString("RPCReport_Hit"));
            RPCReport_KillPlayer = Config.Bind("RPCReportSetting", "KillPlayer", true, localizationManager.Cfg_GetString("RPCReport_KillPlayer"));
            RPCReport_Kick = Config.Bind("RPCReportSetting", "Kick", false, localizationManager.Cfg_GetString("Kick"));

            Health_Recover = Config.Bind("HealthSetting", "Recover", true, localizationManager.Cfg_GetString("Health_Recover"));
            Health_Kick = Config.Bind("HealthSetting", "Kick", false, localizationManager.Cfg_GetString("Kick"));

            ShipBuild = Config.Bind("ShipBuildSetting", "Enable", true, localizationManager.Cfg_GetString("ShipBuild"));
            ShipBuild2 = Config.Bind("ShipBuildSetting", "Kick", false, localizationManager.Cfg_GetString("Kick"));

            ItemCooldown = Config.Bind("ItemCooldownSetting", "Enable", true, localizationManager.Cfg_GetString("ItemCooldown"));
            ItemCooldown2 = Config.Bind("ItemCooldownSetting", "Kick", false, localizationManager.Cfg_GetString("Kick"));

            ShipLight = Config.Bind("ShipLightSettings", "Enable", true, localizationManager.Cfg_GetString("ShipLight"));
            ShipLight_Cooldown = Config.Bind("ShipLightSettings", "Cooldown", 2000, localizationManager.Cfg_GetString("Cooldown"));

            TerminalNoise = Config.Bind("TerminalNoiseSettings", "Enable", true, localizationManager.Cfg_GetString("ShipTerminal"));
            TerminalNoise_Cooldown = Config.Bind("TerminalNoiseSettings", "Cooldown", 1000, localizationManager.Cfg_GetString("Cooldown"));

            DetectedMessageType = Config.Bind("DetectedMessageType", "Type", MessageType.PublicChat, localizationManager.Cfg_GetString("DetectedMessageType"));

            DespawnItem = Config.Bind("DespawnItemSettings", "Enable", true, localizationManager.Cfg_GetString("DespawnItem"));
            DespawnItem2 = Config.Bind("DespawnItemSettings", "Kick", false, localizationManager.Cfg_GetString("Kick"));

            ChatReal = Config.Bind("ChatRealSettings", "Enable", true, localizationManager.Cfg_GetString("ChatReal"));
            ChatReal_Cooldown = Config.Bind("ChatRealSettings", "Cooldown", 100, localizationManager.Cfg_GetString("Cooldown"));
            ChatReal2 = Config.Bind("ChatRealSettings", "Kick", false, localizationManager.Cfg_GetString("Kick"));

            Mask = Config.Bind("MaskSettings", "Enable", true, localizationManager.Cfg_GetString("Mask"));
            Mask2 = Config.Bind("MaskSettings", "Kick", false, localizationManager.Cfg_GetString("Kick"));

            Gift = Config.Bind("GiftSettings", "Enable", true, localizationManager.Cfg_GetString("Gift"));
            Gift2 = Config.Bind("GiftSettings", "Kick", false, localizationManager.Cfg_GetString("Kick"));

            Turret = Config.Bind("TurretSettings", "Enable", true, localizationManager.Cfg_GetString("Turret"));
            Turret2 = Config.Bind("TurretSettings", "Kick", false, localizationManager.Cfg_GetString("Kick"));

            InfiniteAmmo = Config.Bind("InfiniteAmmoSettings", "Enable", true, localizationManager.Cfg_GetString("InfiniteAmmo"));
            InfiniteAmmo2 = Config.Bind("InfiniteAmmoSettings", "Kick", false, localizationManager.Cfg_GetString("Kick"));

            Invisibility = Config.Bind("InvisibilitySettings", "Enable", true, localizationManager.Cfg_GetString("Invisibility"));
            Invisibility2 = Config.Bind("InvisibilitySettings", "Kick", false, localizationManager.Cfg_GetString("Kick"));


            Boss = Config.Bind("BossSetting", "Enable", true, localizationManager.Cfg_GetString("Boss"));
            Boss2 = Config.Bind("BossSetting", "Kick", false, localizationManager.Cfg_GetString("Kick"));

            Jetpack = Config.Bind("JetpackSetting", "Enable", true, localizationManager.Cfg_GetString("Jetpack"));
            Jetpack2 = Config.Bind("JetpackSetting", "Kick", false, localizationManager.Cfg_GetString("Kick"));

            //PlayerCarryWeight = Config.Bind("PlayerCarryWeightSetting", "Enable", true, localizationManager.Cfg_GetString("PlayerCarryWeight"));
            //PlayerCarryWeight2 = Config.Bind("PlayerCarryWeightSetting", "Recovery", false, localizationManager.Cfg_GetString("PlayerCarryWeight2"));
            //PlayerCarryWeight3 = Config.Bind("PlayerCarryWeightSetting", "Kick", false, localizationManager.Cfg_GetString("Kick"));

            Landmine = Config.Bind("LandmineSetting", "Enable", true, localizationManager.Cfg_GetString("Landmine"));
            Landmine2 = Config.Bind("LandmineSetting", "Kick", false, localizationManager.Cfg_GetString("Kick"));

            SpawnWebTrap = Config.Bind("SpawnWebTrapSetting", "Enable", true, localizationManager.Cfg_GetString("SpawnWebTrap"));
            SpawnWebTrap2 = Config.Bind("SpawnWebTrapSetting", "Kick", false, localizationManager.Cfg_GetString("Kick"));

            Enemy = Config.Bind("EnemySetting", "Enable", true, localizationManager.Cfg_GetString("Enemy"));

            KillEnemy = Config.Bind("KillEnemySetting", "Enable", true, localizationManager.Cfg_GetString("KillEnemy"));
            KillEnemy2 = Config.Bind("KillEnemySetting", "Kick", false, localizationManager.Cfg_GetString("Kick"));

            Map = Config.Bind("MapSetting", "Enable", true, localizationManager.Cfg_GetString("Map"));
            Map2 = Config.Bind("MapSetting", "Kick", false, localizationManager.Cfg_GetString("Kick"));

            GrabObject = Config.Bind("GrabObjectSetting", "Enable", true, localizationManager.Cfg_GetString("GrabObject"));
            GrabObject_MoreSlot = Config.Bind("GrabObjectSetting", "MoreSlot", true, localizationManager.Cfg_GetString("GrabObject_MoreSlot"));
            GrabObject_TwoHand = Config.Bind("GrabObjectSetting", "TwoHand", true, localizationManager.Cfg_GetString("GrabObject_TwoHand"));
            GrabObject_BeltBag = Config.Bind("GrabObjectSetting", "BeltBag", true, localizationManager.Cfg_GetString("GrabObject_BeltBag"));
            GrabObject_Kick = Config.Bind("GrabObjectSetting", "Kick", false, localizationManager.Cfg_GetString("Kick"));

            Shovel = Config.Bind("ShovelSettings", "Enable", true, localizationManager.Cfg_GetString("Shovel"));
            Shovel3 = Config.Bind("ShovelSettings", "EmptyHand", false, localizationManager.Cfg_GetString("Shovel2"));
            Shovel2 = Config.Bind("ShovelSettings", "Kick", false, localizationManager.Cfg_GetString("Kick"));

            Nameless = Config.Bind("NamelessSettings", "Enable", true, localizationManager.Cfg_GetString("Nameless"));
            Nameless2 = Config.Bind("NamelessSettings", "Kick", false, localizationManager.Cfg_GetString("Kick"));

            FreeBuy = Config.Bind("FreeBuySettings", "Enable", true, localizationManager.Cfg_GetString("FreeBuy"));
            FreeBuy2 = Config.Bind("FreeBuySettings", "Kick", false, localizationManager.Cfg_GetString("Kick"));

            RemoteTerminal = Config.Bind("RemoteTerminalSettings", "Enable", true, localizationManager.Cfg_GetString("RemoteTerminal"));
            RemoteTerminal2 = Config.Bind("RemoteTerminalSettings", "Kick", false, localizationManager.Cfg_GetString("Kick"));

            PlayerJoin = Config.Bind("MsgSettings", "PlayerJoinShip", localizationManager.Msg_GetString("wlc_player"), localizationManager.Msg_GetString("wlc_player"));
            CooldownManager.Reset();
            CooldownManager.RegisterCooldownGroup(
                "TerminalNoise",
                () => PluginConfig.TerminalNoise.Value,
                () => PluginConfig.TerminalNoise_Cooldown.Value / 1000f
            );
            CooldownManager.RegisterCooldownGroup(
                "ShipLight",
                () => PluginConfig.ShipLight.Value,
                () => PluginConfig.ShipLight_Cooldown.Value / 1000f
            );
            CooldownManager.RegisterCooldownGroup(
               "Chat",
               () => PluginConfig.ChatReal.Value,
               () => PluginConfig.ChatReal_Cooldown.Value / 1000f
            );
            AntiCheat.Log.LogInfo($"{localizationManager.Log_GetString("load")}");
        }
    }
}
