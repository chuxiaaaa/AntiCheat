using AntiCheat.Core;

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
        public static bool ActivateItemServerRpc(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (Patches.Check(rpcParams, out var p))
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

        /// <summary>
        /// EquipItemServerRpc
        /// </summary>
        [HarmonyPrefix]
        [HarmonyPatch("__rpc_handler_947748389")]
        public static bool EquipItemServerRpc(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (Patches.Check(rpcParams, out var p))
            {
                var grab = (GrabbableObject)target;
                if (AntiCheat.Core.AntiCheat.OperationLog.Value)
                {
                    if (grab is LungProp lung && lung.isLungDocked)
                    {
                        Patches.ShowMessageHostOnly(Patches.locale.OperationLog_GetString("GrabLungProp"));
                    }
                }
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
