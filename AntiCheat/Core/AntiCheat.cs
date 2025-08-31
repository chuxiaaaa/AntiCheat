using AntiCheat.Locale;
using AntiCheat.Patch;

using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;

using GameNetcodeStuff;

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

namespace AntiCheat.Core
{
    [BepInPlugin("AntiCheat", "AntiCheat", Version)]
    public class AntiCheat : BaseUnityPlugin
    {
        public const string Version = "0.8.7";
        public static ManualLogSource ManualLog = null;

        public enum MessageType
        {
            GUI,
            HostChat,
            PublicChat
        }

        public static Locale.LocalizationManager localizationManager = null;


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

        public Dictionary<string, bool> Tabs { get; set; }

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

            Harmony.CreateAndPatchAll(typeof(Patches));
            Harmony.CreateAndPatchAll(typeof(ShipTeleporterPatch));
            Harmony.CreateAndPatchAll(typeof(TurretPatch));
            Harmony.CreateAndPatchAll(typeof(HUDManagerPatch));
            Harmony.CreateAndPatchAll(typeof(StartOfRoundPatch));
            Harmony.CreateAndPatchAll(typeof(GrabbableObjectPatch));
            Harmony.CreateAndPatchAll(typeof(TerminalPatch));
            Harmony.CreateAndPatchAll(typeof(PlayerControllerBPatch));
            Harmony.CreateAndPatchAll(typeof(DoorLockPatch));
            Harmony.CreateAndPatchAll(typeof(LandminePatch));
            Harmony.CreateAndPatchAll(typeof(RoundManagerPatch));
            Harmony.CreateAndPatchAll(typeof(TerminalAccessibleObjectPatch));
        }

        private void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            LogInfo($"Reload Config");
            LoadConfig();
        }

        public static void LogInfo(string info)
        {
            if (StartOfRound.Instance == null || StartOfRound.Instance.IsHost)
            {
                if (Log == null || Log.Value)
                {
                    ManualLog.LogInfo($"{info}");
                    File.AppendAllLines("AntiCheat.log", new string[] { $"[{DateTime.Now.ToString("MM-dd HH:mm:ss:ff")}] {info}" });
                }
            }
        }

        public static void LogInfo(PlayerControllerB p,string rpc, params object[] param)
        {
            LogInfo($"{p.playerUsername}({p.playerClientId}) -> {rpc};{string.Join("|", param)}");
        }

        private void LoadConfig()
        {
            localizationManager = new LocalizationManager();
            IgnoreClientConfig = Config.Bind("VersionSetting", "NetworkSetting", false, localizationManager.Cfg_GetString("NetworkSetting"));
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
                () => AntiCheat.TerminalNoise.Value,
                () => AntiCheat.TerminalNoise_Cooldown.Value / 1000f
            );
            CooldownManager.RegisterCooldownGroup(
                "ShipLight",
                () => AntiCheat.ShipLight.Value,
                () => AntiCheat.ShipLight_Cooldown.Value / 1000f
            );
            CooldownManager.RegisterCooldownGroup(
               "Chat",
               () => AntiCheat.ChatReal.Value,
               () => AntiCheat.ChatReal_Cooldown.Value / 1000f
            );
            LogInfo($"{localizationManager.Log_GetString("load")}");
        }

    }
}
