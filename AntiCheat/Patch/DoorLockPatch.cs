using AntiCheat.Core;

using HarmonyLib;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Unity.Netcode;

namespace AntiCheat.Patch
{
    [HarmonyPatch(typeof(DoorLock))]
    [HarmonyWrapSafe]
    public static class DoorLockPatch
    {
        [HarmonyPatch("__rpc_handler_184554516")]
        [HarmonyPrefix]
        public static bool UnlockDoorServerRpc(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (Patches.Check(rpcParams, out var p))
            {
                AntiCheat.Core.AntiCheat.LogInfo(p, "DoorLock.UnlockDoorServerRpc");
            }
            else if (p == null)
            {
                return false;
            }
            return true;
        }
    }
}
