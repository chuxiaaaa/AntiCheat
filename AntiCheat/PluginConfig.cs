using AntiCheat.Utils;

using BepInEx;
using BepInEx.Configuration;

namespace AntiCheat
{
    internal static class PluginConfig
    {
        public enum MessageType
        {
            GUI,
            HostChat,
            PublicChat,
        }

        public static FeatureConfig Shovel = null!;
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
        public static FeatureConfig GrabObject = null!;
        public static FeatureConfig Jetpack = null!;
        public static FeatureConfig Enemy = null!;

        public static ConfigEntry<bool> IgnoreClientConfig = null!;
        public static ConfigEntry<string> Prefix = null!;
        public static ConfigEntry<string> PlayerJoin = null!;
        public static ConfigEntry<bool> Log = null!;
        public static ConfigEntry<bool> OperationLog = null!;
        public static ConfigEntry<bool> ShipConfig = null!;
        public static ConfigEntry<int> ShipConfig2 = null!;
        public static ConfigEntry<string> ShipConfig3 = null!;
        public static ConfigEntry<int> ShipConfig4 = null!;
        public static ConfigEntry<bool> Ship_Kick = null!;
        public static ConfigEntry<bool> ShipSetting_OnlyOneVote = null!;
        public static ConfigEntry<bool> ShipSetting_ChangToFreeMoon = null!;
        public static ConfigEntry<int> ShipLight_Cooldown = null!;
        public static ConfigEntry<int> TerminalNoise_Cooldown = null!;
        public static ConfigEntry<int> ChatReal_Cooldown = null!;
        public static ConfigEntry<int> RPCReport_Delay = null!;
        public static ConfigEntry<bool> RPCReport_Hit = null!;
        public static ConfigEntry<bool> RPCReport_KillPlayer = null!;
        public static ConfigEntry<bool> Shovel3 = null!;
        public static ConfigEntry<bool> GrabObject_SendLog = null!;
        public static ConfigEntry<bool> GrabObject_MoreSlot = null!;
        public static ConfigEntry<bool> GrabObject_TwoHand = null!;
        public static ConfigEntry<bool> GrabObject_BeltBag = null!;
        public static ConfigEntry<MessageType> DetectedMessageType = null!;

        public static ConfigEntry<bool> Shovel2 => Shovel.KickEntry!;
        public static ConfigEntry<bool> ShipBuild2 => ShipBuild.KickEntry!;
        public static ConfigEntry<bool> ItemCooldown2 => ItemCooldown.KickEntry!;
        public static ConfigEntry<bool> DespawnItem2 => DespawnItem.KickEntry!;
        public static ConfigEntry<bool> ChatReal2 => ChatReal.KickEntry!;
        public static ConfigEntry<bool> Mask2 => Mask.KickEntry!;
        public static ConfigEntry<bool> Gift2 => Gift.KickEntry!;
        public static ConfigEntry<bool> Turret2 => Turret.KickEntry!;
        public static ConfigEntry<bool> InfiniteAmmo2 => InfiniteAmmo.KickEntry!;
        public static ConfigEntry<bool> Invisibility2 => Invisibility.KickEntry!;
        public static ConfigEntry<bool> Boss2 => Boss.KickEntry!;
        public static ConfigEntry<bool> Jetpack2 => Jetpack.KickEntry!;
        public static ConfigEntry<bool> Landmine2 => Landmine.KickEntry!;
        public static ConfigEntry<bool> SpawnWebTrap2 => SpawnWebTrap.KickEntry!;
        public static ConfigEntry<bool> KillEnemy2 => KillEnemy.KickEntry!;
        public static ConfigEntry<bool> Map2 => Map.KickEntry!;
        public static ConfigEntry<bool> FreeBuy2 => FreeBuy.KickEntry!;
        public static ConfigEntry<bool> FreeBuy_Kick => FreeBuy.KickEntry!;
        public static ConfigEntry<bool> RemoteTerminal2 => RemoteTerminal.KickEntry!;
        public static ConfigEntry<bool> Nameless2 => Nameless.KickEntry!;
        public static ConfigEntry<bool> GrabObject_Kick => GrabObject.KickEntry!;
        public static ConfigEntry<bool> Health_Recover => Health.EnableEntry;
        public static ConfigEntry<bool> Health_Kick => Health.KickEntry!;
        public static ConfigEntry<bool> RPCReport_Kick => RPCReport.KickEntry!;

        internal static void Init(BaseUnityPlugin plugin)
        {
            var config = plugin.Config;
            var localizationManager = AntiCheatPlugin.localizationManager;

            IgnoreClientConfig = config.Bind(
                "VersionSetting",
                "NetworkSetting",
                false,
                localizationManager.Cfg_GetString("NetworkSetting"));

            Prefix = config.Bind(
                "ServerNameSetting",
                "Prefix",
                "AC",
                localizationManager.Cfg_GetString("Prefix"));

            Log = config.Bind(
                "LogSetting",
                "Log",
                true,
                localizationManager.Cfg_GetString("Log"));

            OperationLog = config.Bind(
                "LogSetting",
                "OperationLog",
                true,
                localizationManager.Cfg_GetString("OperationLog"));

            ShipConfig = config.Bind(
                "ShipSetting",
                "StartGameOnlyHost",
                true,
                localizationManager.Cfg_GetString("ShipSetting"));

            Ship_Kick = config.Bind(
                "ShipSetting",
                "Kick",
                false,
                localizationManager.Cfg_GetString("ShipConfig5"));

            ShipConfig2 = config.Bind(
                "ShipSetting",
                "StartGamePlayerCount",
                8,
                localizationManager.Cfg_GetString("ShipConfig2"));

            ShipConfig3 = config.Bind(
                "ShipSetting",
                "EndGamePlayerTime",
                "14:00",
                localizationManager.Cfg_GetString("ShipConfig3"));

            ShipConfig4 = config.Bind(
                "ShipSetting",
                "EndGamePlayerCount",
                50,
                localizationManager.Cfg_GetString("ShipConfig4"));

            ShipSetting_OnlyOneVote = config.Bind(
                "ShipSetting",
                "OnlyOneVote",
                true,
                localizationManager.Cfg_GetString("ShipConfig6"));

            ShipSetting_ChangToFreeMoon = config.Bind(
                "ShipSetting",
                "ChangToFreeMoon",
                false,
                localizationManager.Cfg_GetString("ChangToFreeMoon"));

            ShipBuild = new FeatureConfig(config, "ShipBuildSetting", localizationManager.Cfg_GetString("ShipBuild"));
            ItemCooldown = new FeatureConfig(config, "ItemCooldownSetting", localizationManager.Cfg_GetString("ItemCooldown"));
            ShipLight = new FeatureConfig(config, "ShipLightSettings", localizationManager.Cfg_GetString("ShipLight"), hasKick: false);
            TerminalNoise = new FeatureConfig(config, "TerminalNoiseSettings", localizationManager.Cfg_GetString("ShipTerminal"), hasKick: false);
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
            Enemy = new FeatureConfig(config, "EnemySetting", localizationManager.Cfg_GetString("Enemy"), hasKick: false);
            KillEnemy = new FeatureConfig(config, "KillEnemySetting", localizationManager.Cfg_GetString("KillEnemy"));
            Map = new FeatureConfig(config, "MapSetting", localizationManager.Cfg_GetString("Map"));
            FreeBuy = new FeatureConfig(config, "FreeBuySettings", localizationManager.Cfg_GetString("FreeBuy"));
            RemoteTerminal = new FeatureConfig(config, "RemoteTerminalSettings", localizationManager.Cfg_GetString("RemoteTerminal"));
            Nameless = new FeatureConfig(config, "NamelessSettings", localizationManager.Cfg_GetString("Nameless"));
            Health = new FeatureConfig(config, "HealthSetting", localizationManager.Cfg_GetString("Health_Recover"));
            RPCReport = new FeatureConfig(config, "RPCReportSetting", localizationManager.Cfg_GetString("RPCReport_Hit"));
            Shovel = new FeatureConfig(config, "ShovelSettings", localizationManager.Cfg_GetString("Shovel"));
            GrabObject = new FeatureConfig(config, "GrabObjectSetting", localizationManager.Cfg_GetString("GrabObject"));

            ShipLight_Cooldown = config.Bind(
                "ShipLightSettings",
                "Cooldown",
                2000,
                localizationManager.Cfg_GetString("Cooldown"));

            TerminalNoise_Cooldown = config.Bind(
                "TerminalNoiseSettings",
                "Cooldown",
                1000,
                localizationManager.Cfg_GetString("Cooldown"));

            ChatReal_Cooldown = config.Bind(
                "ChatRealSettings",
                "Cooldown",
                100,
                localizationManager.Cfg_GetString("Cooldown"));

            RPCReport_Delay = config.Bind(
                "RPCReportSetting",
                "Delay",
                1000,
                localizationManager.Cfg_GetString("RPCReport_Delay"));

            RPCReport_Hit = config.Bind(
                "RPCReportSetting",
                "Hit",
                true,
                localizationManager.Cfg_GetString("RPCReport_Hit"));

            RPCReport_KillPlayer = config.Bind(
                "RPCReportSetting",
                "KillPlayer",
                true,
                localizationManager.Cfg_GetString("RPCReport_KillPlayer"));

            DetectedMessageType = config.Bind(
                "DetectedMessageType",
                "Type",
                MessageType.PublicChat,
                localizationManager.Cfg_GetString("DetectedMessageType"));

            PlayerJoin = config.Bind(
                "MsgSettings",
                "PlayerJoinShip",
                localizationManager.Msg_GetString("wlc_player"),
                localizationManager.Msg_GetString("wlc_player"));

            Shovel3 = config.Bind(
                "ShovelSettings",
                "EmptyHand",
                false,
                localizationManager.Cfg_GetString("Shovel2"));

            GrabObject_SendLog = config.Bind(
                "GrabObjectSetting",
                "SendLog",
                true,
                localizationManager.Cfg_GetString("GrabObject_SendLog"));

            GrabObject_MoreSlot = config.Bind(
                "GrabObjectSetting",
                "MoreSlot",
                true,
                localizationManager.Cfg_GetString("GrabObject_MoreSlot"));

            GrabObject_TwoHand = config.Bind(
                "GrabObjectSetting",
                "TwoHand",
                true,
                localizationManager.Cfg_GetString("GrabObject_TwoHand"));

            GrabObject_BeltBag = config.Bind(
                "GrabObjectSetting",
                "BeltBag",
                true,
                localizationManager.Cfg_GetString("GrabObject_BeltBag"));

            CooldownManager.Reset();
            RegisterCooldown("TerminalNoise", TerminalNoise.EnableEntry, TerminalNoise_Cooldown);
            RegisterCooldown("ShipLight", ShipLight.EnableEntry, ShipLight_Cooldown);
            RegisterCooldown("Chat", ChatReal.EnableEntry, ChatReal_Cooldown);

            AntiCheatPlugin.LogInfo(localizationManager.Log_GetString("load"));
        }

        private static void RegisterCooldown(string name, ConfigEntry<bool> enable, ConfigEntry<int> cooldownMs)
        {
            CooldownManager.RegisterCooldownGroup(
                name,
                () => enable.Value,
                () => cooldownMs.Value / 1000f);
        }
    }
}
