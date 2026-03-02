using AntiCheat;
using AntiCheat.Locale;

using BepInEx;
using BepInEx.Logging;

using GameNetcodeStuff;

using HarmonyLib;

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

[BepInPlugin(LCMPluginInfo.PLUGIN_GUID, LCMPluginInfo.PLUGIN_NAME, LCMPluginInfo.PLUGIN_VERSION)]
public class AntiCheatPlugin : BaseUnityPlugin
{
    internal static ManualLogSource Log = null!;

    private void Awake()
    {
        Log = Logger;
        PluginConfig.Init(this);
    }

    public static void LogInfo(string info)
    {
        if (StartOfRound.Instance == null || StartOfRound.Instance.IsHost)
        {
            if (PluginConfig.Log == null || PluginConfig.Log.Value)
            {
                Log.LogInfo($"{info}");
                File.AppendAllLines("AntiCheat.log", new string[] { $"[{DateTime.Now.ToString("MM-dd HH:mm:ss:ff")}] {info}" });
            }
        }
    }

    public static void LogInfo(PlayerControllerB p, string rpc, params object[] param)
    {
        LogInfo($"{p.playerUsername}({p.playerClientId}) -> {rpc};{string.Join("|", param)}");
    }

    private static string lastMessage = string.Empty;

    private static readonly MethodInfo AddChatMessageMethod =
    AccessTools.DeclaredMethod(typeof(HUDManager), "AddChatMessage");

    private static readonly MethodInfo AddTextMessageClientRpcMethod =
        AccessTools.DeclaredMethod(typeof(HUDManager), "AddTextMessageClientRpc");

    /// <summary>
    /// 在游戏中输出信息
    /// </summary>
    public static void ShowMessage(string msg, string dedupeKey = null)
    {
        var template = locale.MessageFormat();
        var showmsg = template
            .Replace("{Prefix}", locale.Prefix())
            .Replace("{msg}", msg);

        var compareKey = dedupeKey ?? showmsg;

        if (lastMessage == compareKey)
            return;

        lastMessage = compareKey;

        if (HUDManager.Instance == null)
            return;

        switch (PluginConfig.DetectedMessageType.Value)
        {
            case PluginConfig.MessageType.PublicChat:
                AddTextMessageClientRpc(showmsg);
                break;

            case PluginConfig.MessageType.HostChat:
                ShowMessageHostOnly(showmsg);
                break;
            default:
                LogInfo($"ShowGUI|{showmsg}");
                break;
        }


    }

    public static void ShowMessageHostOnly(string msg)
    {
        LogInfo($"ShowMessageHostOnly -> {msg}");
        AddChatMessageMethod.Invoke(HUDManager.Instance,new object[] { msg, "", -1, false });
    }

    public static void AddTextMessageClientRpc(string showmsg)
    {
        AddTextMessageClientRpcMethod.Invoke(HUDManager.Instance,new object[] { showmsg });
    }
}
