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
        public static FeatureConfig Jetpack = null!;
        public static FeatureConfig ShipBuild = null!;
        public static FeatureConfig Landmine = null!;
        public static FeatureConfig InfiniteAmmo = null!;
        public static FeatureConfig Mask = null!;
        public static FeatureConfig Gift = null!;
        public static FeatureConfig Turret = null!;
        public static FeatureConfig Invisibility = null!;
        public static FeatureConfig KillEnemy = null!;
        public static FeatureConfig SpawnWebTrap = null!;
        public static FeatureConfig Map = null!;
        public static FeatureConfig FreeBuy = null!;
        public static FeatureConfig RemoteTerminal = null!;
        public static FeatureConfig Nameless = null!;
        public static FeatureConfig DespawnItem = null!;
        public static FeatureConfig ChatReal = null!;
        public static FeatureConfig ShipLight = null!;
        public static FeatureConfig TerminalNoise = null!;
        public static FeatureConfig ItemCooldown = null!;
        public static FeatureConfig Health = null!;
        public static FeatureConfig RPCReport = null!;
        public static FeatureConfig Boss = null!;
        public static FeatureConfig Shovel = null!;
        public static FeatureConfig GrabObject = null!;

        public static ConfigEntry<string> Prefix = null!;
        public static ConfigEntry<string> PlayerJoin = null!;
        public static ConfigEntry<bool> Log = null!;
        public static ConfigEntry<bool> OperationLog = null!;
        public static ConfigEntry<bool> ShipSetting_OnlyOneVote = null!;
        public static ConfigEntry<bool> ShipSetting_ChangToFreeMoon = null!;
        public static ConfigEntry<int> ShipLight_Cooldown = null!;
        public static ConfigEntry<int> TerminalNoise_Cooldown = null!;
        public static ConfigEntry<int> ChatReal_Cooldown = null!;
        public static ConfigEntry<MessageType> DetectedMessageType = null!;
        public enum MessageType
        {
            GUI,
            HostChat,
            PublicChat
        }



        //public static ConfigEntry<bool> Shovel = null!;
        //public static ConfigEntry<bool> Shovel_Kick = null!;
        //public static ConfigEntry<bool> Shovel3 = null!;



        //public static ConfigEntry<bool> ShipConfig = null!;
        //public static ConfigEntry<int> ShipConfig2 = null!;
        //public static ConfigEntry<string> ShipConfig3 = null!;
        //public static ConfigEntry<int> ShipConfig4 = null!;
        //public static ConfigEntry<bool> Ship_Kick = null!;

        //public static ConfigEntry<bool> ShipBuild = null!;
        //public static ConfigEntry<bool> ShipBuild_Kick = null!;

        //public static ConfigEntry<bool> ShipLight = null!;

        //public static ConfigEntry<bool> TerminalNoise = null!;

        //public static ConfigEntry<bool> DespawnItem = null!;
        //public static ConfigEntry<bool> DespawnItem_Kick = null!;

        //public static ConfigEntry<bool> ChatReal = null!;
        //public static ConfigEntry<bool> ChatReal_Kick = null!;

        //public static ConfigEntry<bool> Mask = null!;
        //public static ConfigEntry<bool> Mask_Kick = null!;

        //public static ConfigEntry<bool> Gift = null!;
        //public static ConfigEntry<bool> Gift_Kick = null!;

        //public static ConfigEntry<bool> Invisibility = null!;
        //public static ConfigEntry<bool> Invisibility_Kick = null!;

        //public static ConfigEntry<bool> GrabObject = null!;
        //public static ConfigEntry<bool> GrabObject_MoreSlot = null!;
        //public static ConfigEntry<bool> GrabObject_TwoHand = null!;
        //public static ConfigEntry<bool> GrabObject_BeltBag = null!;
        //public static ConfigEntry<bool> GrabObject_Kick = null!;

        //public static ConfigEntry<bool> Boss = null!;
        //public static ConfigEntry<bool> Boss2 = null!;

        //public static ConfigEntry<bool> Landmine = null!;
        //public static ConfigEntry<bool> Landmine_Kick = null!;

        //public static ConfigEntry<bool> PlayerCarryWeight = null!;
        //public static ConfigEntry<bool> PlayerCarryWeight2 = null!;
        //public static ConfigEntry<bool> PlayerCarryWeight3 = null!;


        //public static ConfigEntry<bool> Turret = null!;
        //public static ConfigEntry<bool> Turret_Kick = null!;

        //public static ConfigEntry<bool> Enemy = null!;

        //public static ConfigEntry<bool> KillEnemy = null!;
        //public static ConfigEntry<bool> KillEnemy_Kick = null!;

        //public static ConfigEntry<bool> SpawnWebTrap = null!;
        //public static ConfigEntry<bool> SpawnWebTrap_Kick = null!;

        //public static ConfigEntry<bool> Map = null!;
        //public static ConfigEntry<bool> Map_Kick = null!;

        //public static ConfigEntry<bool> Jetpack = null!;
        //public static ConfigEntry<bool> Jetpack_Kick = null!;

        //public static ConfigEntry<bool> ItemCooldown = null!;
        //public static ConfigEntry<bool> ItemCooldown_Kick = null!;

        //public static ConfigEntry<bool> InfiniteAmmo = null!;
        //public static ConfigEntry<bool> InfiniteAmmo_Kick = null!;

        //public static ConfigEntry<bool> FreeBuy = null!;
        //public static ConfigEntry<bool> FreeBuy_Kick = null!;

        //public static ConfigEntry<bool> RemoteTerminal = null!;
        //public static ConfigEntry<bool> RemoteTerminal_Kick = null!;

        //public static ConfigEntry<bool> Nameless = null!;
        //public static ConfigEntry<bool> Nameless_Kick = null!;

        //public static ConfigEntry<int> RPCReport_Delay = null!;
        //public static ConfigEntry<bool> RPCReport_Hit = null!;
        //public static ConfigEntry<bool> RPCReport_KillPlayer = null!;
        //public static ConfigEntry<bool> RPCReport_Kick = null!;

        //public static ConfigEntry<bool> Health_Recover = null!;
        //public static ConfigEntry<bool> Health_Kick = null!;



        internal static void Init(BaseUnityPlugin plugin)
        {
            var config = plugin.Config;


            Prefix = config.Bind("ServerNameSetting",
                "Prefix",
                "AC",
                localizationManager.Cfg_GetString("Prefix"));

            Log = config.Bind("LogSetting",
                "Log",
                true,
                localizationManager.Cfg_GetString("Log"));

            OperationLog = config.Bind("LogSetting",
                "OperationLog",
                true,
                localizationManager.Cfg_GetString("OperationLog"));


            ShipConfig = config.Bind("ShipSetting",
                "StartGameOnlyHost",
                true,
                localizationManager.Cfg_GetString("ShipSetting"));

            Ship_Kick = config.Bind("ShipSetting",
                "Kick",
                false,
                localizationManager.Cfg_GetString("ShipConfig5"));

            ShipConfig2 = config.Bind("ShipSetting",
                "StartGamePlayerCount",
                8,
                localizationManager.Cfg_GetString("ShipConfig2"));

            ShipConfig3 = config.Bind("ShipSetting",
                "EndGamePlayerTime",
                "14:00",
                localizationManager.Cfg_GetString("ShipConfig3"));

            ShipConfig4 = config.Bind("ShipSetting",
                "EndGamePlayerCount",
                50,
                localizationManager.Cfg_GetString("ShipConfig4"));

            ShipSetting_OnlyOneVote = config.Bind("ShipSetting",
                "OnlyOneVote",
                true,
                localizationManager.Cfg_GetString("ShipConfig6"));

            ShipSetting_ChangToFreeMoon = config.Bind("ShipSetting",
                "ChangToFreeMoon",
                false,
                localizationManager.Cfg_GetString("ChangToFreeMoon"));

            // ===== FeatureConfig 功能类 =====

            ShipBuild = new FeatureConfig(config, "ShipBuildSetting", localizationManager.Cfg_GetString("ShipBuild"));
            ItemCooldown = new FeatureConfig(config, "ItemCooldownSetting", localizationManager.Cfg_GetString("ItemCooldown"));
            ShipLight = new FeatureConfig(config, "ShipLightSettings", localizationManager.Cfg_GetString("ShipLight"));
            TerminalNoise = new FeatureConfig(config, "TerminalNoiseSettings", localizationManager.Cfg_GetString("ShipTerminal"));
            DespawnItem = new FeatureConfig(config, "DespawnItemSettings", localizationManager.Cfg_GetString("DespawnItem"));
            ChatReal = new FeatureConfig(config, "ChatRealSettings", localizationManager.Cfg_GetString("ChatReal"));
            Mask = new FeatureConfig(config, "MaskSettings", localizationManager.Cfg_GetString("Mask"));
            Gift = new FeatureConfig(config, "GiftSettings", localizationManager.Cfg_GetString("Gift"));
            Turret = new FeatureConfig(config, "TurretSettings", localizationManager.Cfg_GetString("Turret"));
            InfiniteAmmo = new FeatureConfig(config, "InfiniteAmmoSettings", localizationManager.Cfg_GetString("InfiniteAmmo"));
            Invisibility = new FeatureConfig(config, "InvisibilitySettings", localizationManager.Cfg_GetString("Invisibility"));
            Boss = new FeatureConfig(config, "BossSetting", localizationManager.Cfg_GetString("Boss"));
            Jetpack = new FeatureConfig(config, "JetpackSetting", localizationManager.Cfg_GetString("Jetpack"));
            Landmine = new FeatureConfig(config, "LandmineSetting", localizationManager.Cfg_GetString("Landmine"));
            SpawnWebTrap = new FeatureConfig(config, "SpawnWebTrapSetting", localizationManager.Cfg_GetString("SpawnWebTrap"));
            KillEnemy = new FeatureConfig(config, "KillEnemySetting", localizationManager.Cfg_GetString("KillEnemy"));
            Map = new FeatureConfig(config, "MapSetting", localizationManager.Cfg_GetString("Map"));
            FreeBuy = new FeatureConfig(config, "FreeBuySettings", localizationManager.Cfg_GetString("FreeBuy"));
            RemoteTerminal = new FeatureConfig(config, "RemoteTerminalSettings", localizationManager.Cfg_GetString("RemoteTerminal"));
            Nameless = new FeatureConfig(config, "NamelessSettings", localizationManager.Cfg_GetString("Nameless"));
            Health = new FeatureConfig(config, "HealthSetting", localizationManager.Cfg_GetString("Health_Recover"));
            RPCReport = new FeatureConfig(config, "RPCReportSetting", localizationManager.Cfg_GetString("RPCReport_Hit"));

            // ===== 独立参数 =====

            ShipLight_Cooldown = config.Bind("ShipLightSettings",
                "Cooldown",
                2000,
                localizationManager.Cfg_GetString("Cooldown"));

            TerminalNoise_Cooldown = config.Bind("TerminalNoiseSettings",
                "Cooldown",
                1000,
                localizationManager.Cfg_GetString("Cooldown"));

            ChatReal_Cooldown = config.Bind("ChatRealSettings",
                "Cooldown",
                100,
                localizationManager.Cfg_GetString("Cooldown"));

            RPCReport_Delay = config.Bind("RPCReportSetting",
                "Delay",
                1000,
                localizationManager.Cfg_GetString("RPCReport_Delay"));

            DetectedMessageType = config.Bind("DetectedMessageType",
                "Type",
                MessageType.PublicChat,
                localizationManager.Cfg_GetString("DetectedMessageType"));

            PlayerJoin = config.Bind("MsgSettings",
                "PlayerJoinShip",
                localizationManager.Msg_GetString("wlc_player"),
                localizationManager.Msg_GetString("wlc_player"));

            CooldownManager.Reset();

            RegisterCooldown("TerminalNoise", TerminalNoise._Enable, TerminalNoise_Cooldown);
            RegisterCooldown("ShipLight", ShipLight._Enable, ShipLight_Cooldown);
            RegisterCooldown("Chat", ChatReal._Enable, ChatReal_Cooldown);

            AntiCheatPlugin.LogInfo($"{localizationManager.Log_GetString("load")}");
        }

        private static void RegisterCooldown(string name,ConfigEntry<bool> enable,ConfigEntry<int> cooldownMs)
        {
            CooldownManager.RegisterCooldownGroup(
                name,
                () => enable.Value,
                () => cooldownMs.Value / 1000f
            );
        }
    }
}
