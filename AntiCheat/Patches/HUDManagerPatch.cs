using HarmonyLib;

using System.Collections.Generic;

using Unity.Netcode;

namespace AntiCheat.Patches
{
    [HarmonyPatch(typeof(HUDManager))]
    [HarmonyWrapSafe]
    public static class HUDManagerPatch
    {
        public static List<ulong> SyncAllPlayerLevelsServerRpcCalls { get; set; } = new List<ulong>();

        /// <summary>
        /// GetNewStoryLogServerRpc
        /// </summary>
        [HarmonyPrefix]
        [HarmonyPatch("__rpc_handler_3153465849")]
        public static bool GetNewStoryLogServerRpc(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (!PatchHelper.Check(rpcParams, out var player))
            {
                return player != null;
            }

            ByteUnpacker.ReadValueBitPacked(reader, out int logId);
            reader.Seek(0);

            var terminal = UnityEngine.Object.FindObjectOfType<Terminal>();
            return logId < terminal.logEntryFiles.Count && logId > 0;
        }

        /// <summary>
        /// SendErrorMessageServerRpc
        /// </summary>
        [HarmonyPrefix]
        [HarmonyPatch("__rpc_handler_1043384750")]
        public static bool SendErrorMessageServerRpc(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            return !PatchHelper.Check(rpcParams, out _);
        }

        /// <summary>
        /// SyncAllPlayerLevelsServerRpc
        /// </summary>
        [HarmonyPrefix]
        [HarmonyPatch("__rpc_handler_4217433937")]
        public static bool SyncAllPlayerLevelsServerRpc(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (!PatchHelper.Check(rpcParams, out var player))
            {
                return player != null;
            }

            if (SyncAllPlayerLevelsServerRpcCalls.Contains(player.playerSteamId))
            {
                return false;
            }

            ByteUnpacker.ReadValueBitPacked(reader, out int _);
            ByteUnpacker.ReadValueBitPacked(reader, out int playerClientId);
            reader.Seek(0);

            if (playerClientId != (int)player.playerClientId)
            {
                return false;
            }

            SyncAllPlayerLevelsServerRpcCalls.Add(player.playerSteamId);
            return true;
        }

        /// <summary>
        /// ScanNewCreatureServerRpc
        /// </summary>
        [HarmonyPatch("__rpc_handler_1944155956")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static bool ScanNewCreatureServerRpc(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (!PatchHelper.Check(rpcParams, out var player))
            {
                return player != null;
            }

            ByteUnpacker.ReadValueBitPacked(reader, out int enemyId);
            reader.Seek(0);

            var terminal = UnityEngine.Object.FindObjectOfType<Terminal>();
            if (enemyId >= terminal.enemyFiles.Count || enemyId <= 0)
            {
                return false;
            }

            if (terminal.scannedEnemyIDs.Contains(enemyId) && terminal.newlyScannedEnemyIDs.Contains(enemyId))
            {
                return false;
            }

            string msg = AntiCheatPlugin.localizationManager.Msg_GetString(
                "snc_player",
                new Dictionary<string, string>
                {
                    ["{player}"] = player.playerUsername,
                    ["{enemy}"] = terminal.enemyFiles[enemyId].creatureName,
                });

            PatchHelper.LogInfo(msg);
            return true;
        }

        /// <summary>
        /// UseSignalTranslatorServerRpc
        /// </summary>
        [HarmonyPatch("__rpc_handler_2436660286")]
        [HarmonyPrefix]
        public static bool UseSignalTranslatorServerRpc(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (!PatchHelper.Check(rpcParams, out var player))
            {
                return player != null;
            }

            if (PluginConfig.RemoteTerminal.Enable &&
                !PatchHelper.CheckRemoteTerminal(player, "HUDManager.UseSignalTranslatorServerRpc"))
            {
                return false;
            }

            return true;
        }
    }
}
