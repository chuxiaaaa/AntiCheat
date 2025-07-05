using AntiCheat.Core;

using HarmonyLib;

using Steamworks.Ugc;

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
                if (grab is RemoteProp)
                {
                    bool canUse = CooldownManager.CheckCooldown("ShipLight", p);
                    if (!canUse)
                    {
                        ((ShipLights)target).SetShipLightsClientRpc(true);
                    }
                    return canUse;
                }
                return false;
            }
            else if (p == null)
            {
                return false;
            }
            return true;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(GiftBoxItem), "ItemActivate")]
        public static void ItemActivate(GiftBoxItem __instance)
        {
            UnityEngine.Object.Destroy(__instance.gameObject);
        }

        /// <summary>
        /// EquipItemServerRpc
        /// </summary>
        [HarmonyPrefix]
        [HarmonyPatch(typeof(LungProp), "EquipItem")]
        public static bool EquipItem(LungProp __instance)
        {
            if (AntiCheat.Core.AntiCheat.OperationLog.Value)
            {
                if (__instance.isLungDocked && StartOfRound.Instance.shipHasLanded)
                {
                    Patches.ShowMessageHostOnly(Patches.locale.OperationLog_GetString("GrabLungProp", new Dictionary<string, string>() {
                        { "{player}", $"{StartOfRound.Instance.allPlayerScripts.First(x => x.OwnerClientId == __instance.OwnerClientId).playerUsername}" }
                    }));
                }
            }
            return true;

        }
    }
}
