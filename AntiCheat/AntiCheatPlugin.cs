using AntiCheat;
using AntiCheat.Locale;

using BepInEx;
using BepInEx.Logging;

using GameNetcodeStuff;

using HarmonyLib;

using System;
using System.IO;
using System.Reflection;

[BepInPlugin(LCMPluginInfo.PLUGIN_GUID, LCMPluginInfo.PLUGIN_NAME, LCMPluginInfo.PLUGIN_VERSION)]
public class AntiCheatPlugin : BaseUnityPlugin
{
    internal static ManualLogSource Log = null!;
    internal static LocalizationManager localizationManager = null!;

    private static string _lastMessage = string.Empty;

    private static readonly MethodInfo AddChatMessageMethod =
        AccessTools.DeclaredMethod(typeof(HUDManager), "AddChatMessage");

    private static readonly MethodInfo AddTextMessageClientRpcMethod =
        AccessTools.DeclaredMethod(typeof(HUDManager), "AddTextMessageClientRpc");

    private void Awake()
    {
        Log = Logger;
        localizationManager = new LocalizationManager();

        PluginConfig.Init(this);

        var harmony = new Harmony(LCMPluginInfo.PLUGIN_GUID);
        harmony.PatchAll(Assembly.GetExecutingAssembly());
    }

    public static void LogInfo(string info)
    {
        if (StartOfRound.Instance != null && !StartOfRound.Instance.IsHost)
        {
            return;
        }

        if (PluginConfig.Log == null || PluginConfig.Log.Value)
        {
            Log.LogInfo(info);
            File.AppendAllLines(
                "AntiCheat.log",
                new[] { $"[{DateTime.Now:MM-dd HH:mm:ss:ff}] {info}" });
        }
    }

    public static void LogInfo(PlayerControllerB player, string rpc, params object[] param)
    {
        LogInfo($"{player.playerUsername}({player.playerClientId}) -> {rpc};{string.Join("|", param)}");
    }

    public static void ShowMessage(string msg, string dedupeKey = null)
    {
        var showMessage = localizationManager
            .MessageFormat()
            .Replace("{Prefix}", localizationManager.Prefix())
            .Replace("{msg}", msg);

        var compareKey = dedupeKey ?? showMessage;
        if (_lastMessage == compareKey)
        {
            return;
        }

        _lastMessage = compareKey;

        if (HUDManager.Instance == null)
        {
            return;
        }

        switch (PluginConfig.DetectedMessageType.Value)
        {
            case PluginConfig.MessageType.PublicChat:
                AddTextMessageClientRpc(showMessage);
                break;
            case PluginConfig.MessageType.HostChat:
                ShowMessageHostOnly(showMessage);
                break;
            default:
                LogInfo($"ShowGUI|{showMessage}");
                break;
        }
    }

    public static void ShowMessageHostOnly(string msg)
    {
        if (HUDManager.Instance == null)
        {
            return;
        }

        LogInfo($"ShowMessageHostOnly -> {msg}");
        AddChatMessageMethod.Invoke(HUDManager.Instance, new object[] { msg, "", -1, false });
    }

    public static void AddTextMessageClientRpc(string showMessage)
    {
        if (HUDManager.Instance == null)
        {
            return;
        }

        AddTextMessageClientRpcMethod.Invoke(HUDManager.Instance, new object[] { showMessage });
    }
}
