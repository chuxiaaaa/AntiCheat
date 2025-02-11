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
        public static bool __rpc_handler_3153465849(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (Patch.Check(rpcParams, out var p))
            {
                ByteUnpacker.ReadValueBitPacked(reader, out int logID);
                reader.Seek(0);
                var terminal = UnityEngine.Object.FindObjectOfType<Terminal>();
                if (logID < terminal.logEntryFiles.Count && logID > 0)
                {
                    return true;
                }
                return false;
            }
            else if (p == null)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// SendErrorMessageServerRpc
        /// </summary>
        [HarmonyPrefix]
        [HarmonyPatch("__rpc_handler_1043384750")]
        public static bool __rpc_handler_1043384750(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (Patch.Check(rpcParams, out var p))
            {
                return false;
            }
            else if (p == null)
            {
                return false;
            }
            return true;
        }


        /// <summary>
        /// SyncAllPlayerLevelsServerRpc
        /// </summary>
        [HarmonyPrefix]
        [HarmonyPatch("__rpc_handler_4217433937")]
        public static bool __rpc_handler_4217433937(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (Patch.Check(rpcParams, out var p))
            {
                if (SyncAllPlayerLevelsServerRpcCalls.Contains(p.playerSteamId))
                {
                    return false;
                }
                ByteUnpacker.ReadValueBitPacked(reader, out int newPlayerLevel);
                ByteUnpacker.ReadValueBitPacked(reader, out int playerClientId);
                reader.Seek(0);
                if (playerClientId != (int)p.playerClientId)
                {
                    return false;
                }
                Patch.LogInfo($"SyncAllPlayerLevelsServerRpcCalls.Add({p.playerSteamId})");
                SyncAllPlayerLevelsServerRpcCalls.Add(p.playerSteamId);
                return true;
            }
            else if (p == null)
            {
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(HUDManager), "__rpc_handler_1944155956")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static bool __rpc_handler_1944155956(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (Patch.Check(rpcParams, out var p))
            {
                ByteUnpacker.ReadValueBitPacked(reader, out int enemyID);
                reader.Seek(0);
                var terminal = UnityEngine.Object.FindObjectOfType<Terminal>();
                if (enemyID < terminal.enemyFiles.Count && enemyID > 0)
                {
                    if (terminal.scannedEnemyIDs.Contains(enemyID) && terminal.newlyScannedEnemyIDs.Contains(enemyID))
                    {
                        return false;
                    }
                    string msg = LocalizationManager.GetString("msg_snc_player", new Dictionary<string, string>() {
                        { "{player}",p.playerUsername },
                        { "{enemy}",terminal.enemyFiles[enemyID].creatureName }
                    });
                    Patch.LogInfo(msg);
                }
                return false;
                //HUDManager.Instance.AddTextToChatOnServer(msg, -1);
            }
            else if (p == null)
            {
                return false;
            }
            return true;
        }
    }
}
