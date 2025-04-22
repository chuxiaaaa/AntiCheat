using HarmonyLib;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Unity.Netcode;

namespace AntiCheat
{
    [HarmonyPatch(typeof(GrabbableObject))]
    [HarmonyWrapSafe]
    public static class GrabbableObjectPatch
    {
        /// <summary>
        /// ActivateItemServerRpc
        /// </summary>
        [HarmonyPrefix]
        [HarmonyPatch("__rpc_handler_4280509730")]
        public static bool __rpc_handler_4280509730(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (Patch.Check(rpcParams, out var p))
            {
                var grab = (GrabbableObject)target;
                if(grab is RemoteProp)
                {
                    return CooldownManager.CheckCooldown("ShipLight", p);
                }
                return false;
            }
            else if (p == null)
            {
                return false;
            }
            return true;
        }
    }
}
