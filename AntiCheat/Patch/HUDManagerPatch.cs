using HarmonyLib;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Unity.Netcode;

namespace AntiCheat
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
            if (!Patches.Check(rpcParams, out var p))
                return p != null;
            ByteUnpacker.ReadValueBitPacked(reader, out int logID);
            reader.Seek(0);
            var terminal = UnityEngine.Object.FindObjectOfType<Terminal>();
            if (logID < terminal.logEntryFiles.Count && logID > 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// SendErrorMessageServerRpc
        /// </summary>
        [HarmonyPrefix]
        [HarmonyPatch("__rpc_handler_1043384750")]
        public static bool SendErrorMessageServerRpc(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            return !Patches.Check(rpcParams, out _);
        }


        /// <summary>
        /// SyncAllPlayerLevelsServerRpc
        /// </summary>
        [HarmonyPrefix]
        [HarmonyPatch("__rpc_handler_4217433937")]
        public static bool SyncAllPlayerLevelsServerRpc(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (!Patches.Check(rpcParams, out var p))
                return p != null;
            if (SyncAllPlayerLevelsServerRpcCalls.Contains(p.playerSteamId))
            {
                return false;
            }
            ByteUnpacker.ReadValueBitPacked(reader, out int _);
            ByteUnpacker.ReadValueBitPacked(reader, out int playerClientId);
            reader.Seek(0);
            if (playerClientId != (int)p.playerClientId)
            {
                return false;
            }
            SyncAllPlayerLevelsServerRpcCalls.Add(p.playerSteamId);
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
            if (!Patches.Check(rpcParams, out var p))
                return p != null;
            ByteUnpacker.ReadValueBitPacked(reader, out int enemyID);
            reader.Seek(0);
            var terminal = UnityEngine.Object.FindObjectOfType<Terminal>();
            if (enemyID < terminal.enemyFiles.Count && enemyID > 0)
            {
                if (terminal.scannedEnemyIDs.Contains(enemyID) && terminal.newlyScannedEnemyIDs.Contains(enemyID))
                {
                    return false;
                }
                string msg = Core.AntiCheat.localizationManager.Msg_GetString("snc_player", new Dictionary<string, string>() {
                        { "{player}",p.playerUsername },
                        { "{enemy}",terminal.enemyFiles[enemyID].creatureName }
                    });
                Patches.LogInfo(msg);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 玩家使用信号发射器发送消息
        /// Prefix UseSignalTranslatorServerRpc
        /// </summary>
        /// <returns></returns>
        [HarmonyPatch("__rpc_handler_2436660286")]
        [HarmonyPrefix]
        public static bool UseSignalTranslatorServerRpc(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (!Patches.Check(rpcParams, out var p))
                return p != null;
            if (Core.AntiCheat.RemoteTerminal.Value)
            {
                if (!Patches.CheckRemoteTerminal(p))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
