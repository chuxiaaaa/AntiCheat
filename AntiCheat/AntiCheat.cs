using BepInEx;
using BepInEx.Logging;

using GameNetcodeStuff;

using System;
using System.IO;

namespace AntiCheat
{
    [BepInPlugin(LCMPluginInfo.PLUGIN_GUID, LCMPluginInfo.PLUGIN_NAME, LCMPluginInfo.PLUGIN_VERSION)]
    public class AntiCheat : BaseUnityPlugin
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
    }
}
