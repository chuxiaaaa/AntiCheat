using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;

using HarmonyLib;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;
using UnityEngine.Diagnostics;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

namespace AntiCheat
{
    [BepInPlugin("AntiCheat", "AntiCheat", Version)]
    public class AntiCheatPlugin : BaseUnityPlugin
    {
        public const string Version = "0.8.2";
        public static ManualLogSource ManualLog = null;
        public enum Language
        {
            简体中文,
            English,
            Korean
        }

        public enum MessageType
        {
            GUI,
            HostChat,
            PublicChat
        }

        public static ConfigEntry<Language> LanguageConfig;

        public static ConfigEntry<bool> Log;
        public static ConfigEntry<bool> OperationLog;

        public static ConfigEntry<bool> IgnoreClientConfig;

        public static GameObject ui;

        public static ConfigEntry<bool> Shovel;
        public static ConfigEntry<bool> Shovel2;
        public static ConfigEntry<bool> Shovel3;

        public static ConfigEntry<string> Prefix;
        public static ConfigEntry<string> PlayerJoin;

        public static ConfigEntry<bool> ShipConfig;
        public static ConfigEntry<int> ShipConfig2;
        public static ConfigEntry<string> ShipConfig3;
        public static ConfigEntry<int> ShipConfig4;
        public static ConfigEntry<bool> Ship_Kick;
        public static ConfigEntry<bool> ShipSetting_OnlyOneVote;
        public static ConfigEntry<bool> ShipSetting_ChangToFreeMoon;

        public static ConfigEntry<bool> ShipBuild;
        public static ConfigEntry<bool> ShipBuild2;

        public static ConfigEntry<bool> ShipLight;
        public static ConfigEntry<int> ShipLight_Cooldown;

        public static ConfigEntry<bool> TerminalNoise;
        public static ConfigEntry<int> TerminalNoise_Cooldown;

        public static ConfigEntry<bool> DespawnItem;
        public static ConfigEntry<bool> DespawnItem2;

        public static ConfigEntry<bool> ChatReal;
        public static ConfigEntry<int> ChatReal_Cooldown;
        public static ConfigEntry<bool> ChatReal2;

        public static ConfigEntry<bool> Mask;
        public static ConfigEntry<bool> Mask2;

        public static ConfigEntry<bool> Gift;
        public static ConfigEntry<bool> Gift2;

        public static ConfigEntry<bool> Invisibility;
        public static ConfigEntry<bool> Invisibility2;

        public static ConfigEntry<bool> GrabObject;
        public static ConfigEntry<bool> GrabObject_MoreSlot;
        public static ConfigEntry<bool> GrabObject_TwoHand;
        public static ConfigEntry<bool> GrabObject_BeltBag;
        public static ConfigEntry<bool> GrabObject_Kick;

        public static ConfigEntry<bool> Boss;
        public static ConfigEntry<bool> Boss2;

        public static ConfigEntry<bool> Landmine;
        public static ConfigEntry<bool> Landmine2;

        public static ConfigEntry<bool> PlayerCarryWeight;
        public static ConfigEntry<bool> PlayerCarryWeight2;
        public static ConfigEntry<bool> PlayerCarryWeight3;


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

        public static ConfigEntry<bool> RemoteTerminal;
        public static ConfigEntry<bool> RemoteTerminal2;

        public static ConfigEntry<bool> Nameless;
        public static ConfigEntry<bool> Nameless2;

        public static ConfigEntry<int> RPCReport_Delay;
        public static ConfigEntry<bool> RPCReport_Hit;
        public static ConfigEntry<bool> RPCReport_KillPlayer;
        public static ConfigEntry<bool> RPCReport_Kick;

        public static ConfigEntry<bool> Health_Recover;
        public static ConfigEntry<bool> Health_Kick;


        public static ConfigEntry<MessageType> DetectedMessageType;

        public static bool Gui;


        public static void String(string text, GUIStyle style, float yPos)
        {
            float screenWidth = Screen.width;
            Vector2 textSize = style.CalcSize(new GUIContent(text));
            float xPos = (screenWidth - textSize.x) / 2;
            GUI.Label(new Rect(xPos, yPos, textSize.x, textSize.y), text, style);
        }

        [DllImport("User32.dll")]
        public static extern short GetAsyncKeyState(int key);

        public void Update()
        {
            if (StartOfRound.Instance != null && !StartOfRound.Instance.IsHost)
            {
                return;
            }
            if (UnityInput.Current.GetKeyDown(KeyCode.F10))
            {
                Gui = !Gui;
            }
        }

        //public void OnGUI()
        //{
        //    if (StartOfRound.Instance != null && !StartOfRound.Instance.IsHost)
        //    {
        //        return;
        //    }
        //    Scene activeScene = SceneManager.GetActiveScene();
        //    Color darkBackground = new Color(23f / 255f, 23f / 255f, 23f / 255f, 1f);

        //    GUI.backgroundColor = darkBackground;
        //    GUI.contentColor = Color.white;

        //    var Style = new GUIStyle(GUI.skin.label);
        //    Style.normal.textColor = Color.white;
        //    Style.fontStyle = FontStyle.Bold;
        //    if (activeScene.name == "SampleSceneRelay")
        //    {
        //        if (!Gui)
        //        {
        //            String($"AntiCheat(Beta{AntiCheatPlugin.Version}) Press F10", Style, 10);
        //            Cursor.visible = false;
        //            Cursor.lockState = CursorLockMode.Locked;
        //        }
        //        else
        //        {
        //            Cursor.visible = true;
        //            Cursor.lockState = CursorLockMode.None;
        //            ShowMenu();
        //        }
        //    }
        //}

        public Dictionary<string,bool> Tabs { get; set; }

        //public void ShowMenu()
        //{
        //    var e = Event.current.type;
        //    if (e == EventType.Layout || e == EventType.Repaint)
        //    {
        //        float screenWidth = (Screen.width - 600) / 2;
        //        GUILayout.Window(0, new Rect(screenWidth, 10, 600, 300), (x) =>
        //        {
        //            foreach (var item in Tabs)
        //            {
        //                if (GUILayout.Button(item.Key) || item.Value)
        //                {
        //                    if (!item.Value)
        //                    {
        //                        Tabs[item.Key] = true;
        //                    }
        //                    GUILayout.BeginScrollView(Vector2.zero);
        //                    foreach (var item2 in Config.Keys.GroupBy(p => p.Section))
        //                    {
        //                        GUILayout.Label(item2.Key);
        //                        foreach (var item3 in item2)
        //                        {
        //                            if (GUILayout.Button(item3.Key))
        //                            {

        //                            }
        //                        }
        //                    }
        //                    GUILayout.EndScrollView();
        //                }
        //            }

        //            GUILayout.BeginHorizontal();
        //            foreach (var item in StartOfRound.Instance.allPlayerScripts)
        //            {
        //                //if (item.isPlayerControlled)
        //                //{
        //                //    GUILayout.Label($"{item.playerUsername}:{item.gameObject.}");
        //                //}
        //            }
        //            if (GUILayout.Button("检测记录"))
        //            {

        //            }
        //            else if (GUILayout.Button("玩家管理"))
        //            {

        //            }
        //            else if (GUILayout.Button("黑名单管理"))
        //            {

        //            }
        //            else if (GUILayout.Button("语言设置"))
        //            {

        //            }
        //            GUILayout.EndHorizontal();
        //        }, $"AntiCheat(Beta{AntiCheatPlugin.Version})");
        //    }
        //}

        void Awake()
        {
            ManualLog = Logger;
            Tabs = new Dictionary<string, bool>();
            Tabs.Add("模组配置", false);
            FileSystemWatcher watcher = new FileSystemWatcher();
            var fi = new FileInfo(Config.ConfigFilePath);
            watcher.Path = fi.DirectoryName;
            watcher.Filter = fi.Name;
            watcher.Changed += Watcher_Changed;
            LoadConfig();
            watcher.EnableRaisingEvents = true;

            Harmony.CreateAndPatchAll(typeof(Patch));
            Harmony.CreateAndPatchAll(typeof(HUDManagerPatch));
            Harmony.CreateAndPatchAll(typeof(StartOfRoundPatch));
            Harmony.CreateAndPatchAll(typeof(GrabbableObjectPatch));

        }

        private void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            ManualLog.LogInfo($"Reload Config");
            LoadConfig();
        }

        private void LoadConfig()
        {
            var defaultLang = Language.简体中文;
            if (!File.Exists(Config.ConfigFilePath))
            {
                var lang = System.Globalization.CultureInfo.CurrentCulture.Name.ToLower();
                if (lang.StartsWith("ko"))
                {
                    defaultLang = Language.Korean;
                }
                else if (!lang.StartsWith("zh-"))
                {
                    defaultLang = Language.English;
                }
            }

            //LanguageConfig = Config.Bind("LanguageSetting", "Language", defaultLang, string.Join(",", LocalizationManager.Languages.Select(x => x.Key)));
            //LocalizationManager.SetLanguage(LanguageConfig.Value.ToString());

            IgnoreClientConfig = Config.Bind("VersionSetting", "IgnoreClientConfig", false, LocalizationManager.GetString("config_IgnoreClientConfig"));
            Prefix = Config.Bind("ServerNameSetting", "Prefix", "AC", LocalizationManager.GetString("config_Prefix"));
            ShipConfig = Config.Bind("ShipSetting", "StartGameOnlyHost", true, LocalizationManager.GetString("config_ShipSetting"));
            Log = Config.Bind("LogSetting", "Log", true, LocalizationManager.GetString("config_Log"));
            OperationLog = Config.Bind("LogSetting", "OperationLog", true, LocalizationManager.GetString("config_OperationLog"));

            Ship_Kick = Config.Bind("ShipSetting", "Kick", false, LocalizationManager.GetString("config_ShipConfig5"));
            ShipConfig2 = Config.Bind("ShipSetting", "StartGamePlayerCount", 8, LocalizationManager.GetString("config_ShipConfig2"));
            ShipConfig3 = Config.Bind("ShipSetting", "EndGamePlayerTime", "14:00", LocalizationManager.GetString("config_ShipConfig3"));
            ShipConfig4 = Config.Bind("ShipSetting", "EndGamePlayerCount", 50, LocalizationManager.GetString("config_ShipConfig4"));
            ShipSetting_OnlyOneVote = Config.Bind("ShipSetting", "OnlyOneVote", true, LocalizationManager.GetString("config_ShipConfig6"));
            ShipSetting_ChangToFreeMoon = Config.Bind<bool>("ShipSetting", "ChangToFreeMoon", false, LocalizationManager.GetString("config_ChangToFreeMoon"));


            RPCReport_Delay = Config.Bind("RPCReportSetting", "Delay", 1000, LocalizationManager.GetString("config_RPCReport_Delay"));
            RPCReport_Hit = Config.Bind("RPCReportSetting", "Hit", true, LocalizationManager.GetString("config_RPCReport_Hit"));
            RPCReport_KillPlayer = Config.Bind("RPCReportSetting", "KillPlayer", true, LocalizationManager.GetString("config_RPCReport_KillPlayer"));
            RPCReport_Kick = Config.Bind("RPCReportSetting", "Kick", false, LocalizationManager.GetString("config_Kick"));

            Health_Recover = Config.Bind("HealthSetting", "Recover", true, LocalizationManager.GetString("config_Health_Recover"));
            Health_Kick = Config.Bind("HealthSetting", "Kick", false, LocalizationManager.GetString("config_Kick"));

            ShipBuild = Config.Bind("ShipBuildSetting", "Enable", true, LocalizationManager.GetString("config_ShipBuild"));
            ShipBuild2 = Config.Bind("ShipBuildSetting", "Kick", false, LocalizationManager.GetString("config_Kick"));

            ItemCooldown = Config.Bind("ItemCooldownSetting", "Enable", true, LocalizationManager.GetString("config_ItemCooldown"));
            ItemCooldown2 = Config.Bind("ItemCooldownSetting", "Kick", false, LocalizationManager.GetString("config_Kick"));

            ShipLight = Config.Bind("ShipLightSettings", "Enable", true, LocalizationManager.GetString("config_ShipLight"));
            ShipLight_Cooldown = Config.Bind("ShipLightSettings", "Cooldown", 1000, LocalizationManager.GetString("config_Cooldown"));

            TerminalNoise = Config.Bind("TerminalNoiseSettings", "Enable", true, LocalizationManager.GetString("config_ShipTerminal"));
            TerminalNoise_Cooldown = Config.Bind("TerminalNoiseSettings", "Cooldown", 1000, LocalizationManager.GetString("config_Cooldown"));

            DetectedMessageType = Config.Bind("DetectedMessageType", "Type", MessageType.PublicChat, LocalizationManager.GetString("config_DetectedMessageType"));

            DespawnItem = Config.Bind("DespawnItemSettings", "Enable", true, LocalizationManager.GetString("config_DespawnItem"));
            DespawnItem2 = Config.Bind("DespawnItemSettings", "Kick", false, LocalizationManager.GetString("config_Kick"));

            ChatReal = Config.Bind("ChatRealSettings", "Enable", true, LocalizationManager.GetString("config_ChatReal"));
            ChatReal_Cooldown = Config.Bind("ChatRealSettings", "Cooldown", 100, LocalizationManager.GetString("config_Cooldown"));
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

            //PlayerCarryWeight = Config.Bind("PlayerCarryWeightSetting", "Enable", true, LocalizationManager.GetString("config_PlayerCarryWeight"));
            //PlayerCarryWeight2 = Config.Bind("PlayerCarryWeightSetting", "Recovery", false, LocalizationManager.GetString("config_PlayerCarryWeight2"));
            //PlayerCarryWeight3 = Config.Bind("PlayerCarryWeightSetting", "Kick", false, LocalizationManager.GetString("config_Kick"));

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
            GrabObject_MoreSlot = Config.Bind("GrabObjectSetting", "MoreSlot", true, LocalizationManager.GetString("config_GrabObject_MoreSlot"));
            GrabObject_TwoHand = Config.Bind("GrabObjectSetting", "TwoHand", true, LocalizationManager.GetString("config_GrabObject_TwoHand"));
            GrabObject_BeltBag = Config.Bind("GrabObjectSetting", "BeltBag", true, LocalizationManager.GetString("config_GrabObject_BeltBag"));
            GrabObject_Kick = Config.Bind("GrabObjectSetting", "Kick", false, LocalizationManager.GetString("config_Kick"));

            Shovel = Config.Bind("ShovelSettings", "Enable", true, LocalizationManager.GetString("config_Shovel"));
            Shovel3 = Config.Bind("ShovelSettings", "EmptyHand", false, LocalizationManager.GetString("config_Shovel2"));
            Shovel2 = Config.Bind("ShovelSettings", "Kick", false, LocalizationManager.GetString("config_Kick"));

            Nameless = Config.Bind("NamelessSettings", "Enable", true, LocalizationManager.GetString("config_Nameless"));
            Nameless2 = Config.Bind("NamelessSettings", "Kick", false, LocalizationManager.GetString("config_Kick"));

            FreeBuy = Config.Bind("FreeBuySettings", "Enable", true, LocalizationManager.GetString("config_FreeBuy"));
            FreeBuy2 = Config.Bind("FreeBuySettings", "Kick", false, LocalizationManager.GetString("config_Kick"));

            RemoteTerminal = Config.Bind("RemoteTerminalSettings", "Enable", true, LocalizationManager.GetString("config_RemoteTerminal"));
            RemoteTerminal2 = Config.Bind("RemoteTerminalSettings", "Kick", false, LocalizationManager.GetString("config_Kick"));

            PlayerJoin = Config.Bind("MsgSettings", "PlayerJoinShip", LocalizationManager.GetString("msg_wlc_player"), LocalizationManager.GetString("msg_wlc_player"));
            CooldownManager.Reset();
            CooldownManager.RegisterCooldownGroup(
                "TerminalNoise",
                () => AntiCheatPlugin.TerminalNoise.Value,
                () => AntiCheatPlugin.TerminalNoise_Cooldown.Value / 1000f
            );
            CooldownManager.RegisterCooldownGroup(
                "ShipLight",
                () => AntiCheatPlugin.ShipLight.Value,
                () => AntiCheatPlugin.ShipLight_Cooldown.Value / 1000f
            );
            CooldownManager.RegisterCooldownGroup(
               "Chat",
               () => AntiCheatPlugin.ChatReal.Value,
               () => AntiCheatPlugin.ChatReal_Cooldown.Value / 1000f
            );
            ManualLog.LogInfo($"{LocalizationManager.GetString("log_load")}");
        }

    }
}
