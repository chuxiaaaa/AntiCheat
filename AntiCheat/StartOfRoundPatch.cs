using HarmonyLib;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Unity.Netcode;

namespace AntiCheat
{
    [HarmonyPatch(typeof(StartOfRound))]
    [HarmonyWrapSafe]
    public static class StartOfRoundPatch
    {
        public static List<ulong> SyncShipUnlockablesServerRpcCalls = new List<ulong>();
        public static List<ulong> SyncAlreadyHeldObjectsServerRpcCalls = new List<ulong>();

        /// <summary>
        /// SyncAlreadyHeldObjectsServerRpc
        /// </summary>
        [HarmonyPrefix]
        [HarmonyPatch("__rpc_handler_682230258")]
        public static bool __rpc_handler_682230258(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            var steamId = Patch.ConnectionIdtoSteamIdMap[Patch.ClientIdToTransportId(rpcParams.Server.Receive.SenderClientId)];
            if (SyncAlreadyHeldObjectsServerRpcCalls.Contains(steamId))
            {
                return false;
            }
            SyncAlreadyHeldObjectsServerRpcCalls.Add(steamId);
            return true;

        }

        /// <summary>
        /// SyncShipUnlockablesServerRpc
        /// </summary>
        [HarmonyPrefix]
        [HarmonyPatch("__rpc_handler_744998938")]
        public static bool __rpc_handler_744998938(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (Patch.Check(rpcParams, out var p))
            {
                if (SyncShipUnlockablesServerRpcCalls.Contains(p.playerSteamId))
                {
                    return false;
                }
                SyncShipUnlockablesServerRpcCalls.Add(p.playerSteamId);
                return true;
            }
            else if (p == null)
            {
                return false;
            }
            return true;
        }

    }
}
