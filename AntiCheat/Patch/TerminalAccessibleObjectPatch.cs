using HarmonyLib;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Unity.Netcode;

namespace AntiCheat.Patch
{
    [HarmonyPatch(typeof(TerminalAccessibleObject))]
    [HarmonyWrapSafe]
    public static class TerminalAccessibleObjectPatch
    {
        [HarmonyPatch("__rpc_handler_1181174413")]
        [HarmonyPrefix]
        public static bool Prefix(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (Patches.Check(rpcParams, out var p))
            {
                AntiCheat.Core.AntiCheat.LogInfo(p, "TerminalAccessibleObject.SetDoorOpenServerRpc");
            }
            else if (p == null)
            {
                return false;
            }
            return true;
        }
    }
}
