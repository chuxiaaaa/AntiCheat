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
                if (target != null)
                {
                    var grab = (GrabbableObject)target;
                    AntiCheat.Core.AntiCheat.LogInfo(p, $"({grab.itemProperties.itemName})GrabbableObject.ActivateItemServerRpc");
                    if (grab != null)
                    {
                        if (grab is RemoteProp)
                        {
                            bool canUse = CooldownManager.CheckCooldown("ShipLight", p);
                            if (!canUse)
                            {
                                ((ShipLights)target).SetShipLightsClientRpc(true);
                            }
                            return canUse;
                        }
                    }
                }
                else
                {
                    return false;
                }
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


        //[HarmonyPrefix]
        //[HarmonyPatch("DiscardItemOnClient")]
        //public static bool DiscardItemOnClient(GrabbableObject __instance)
        //{
        //    AntiCheat.Core.AntiCheat.LogInfo($"itemName:{__instance.itemProperties.itemName}|charge:{__instance.insertedBattery.charge}|syncDiscardFunction:{__instance.itemProperties.syncDiscardFunction}");
        //    return true;
        //}

        //[HarmonyPrefix]
        //[HarmonyPatch("__rpc_handler_4280509730")]
        //public static bool SyncBatteryServerRpc(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        //{
        //    if (!Patches.Check(rpcParams, out var p) && false)
        //        return p != null;
        //    ByteUnpacker.ReadValueBitPacked(reader, out int num);
        //    reader.Seek(0);
        //    GrabbableObject target1 = ((GrabbableObject)target);
        //    AntiCheat.Core.AntiCheat.LogInfo(p, $"({target1.itemProperties.itemName})GrabbableObject.SyncBatteryServerRpc", $"num:{num}",$"charge:{target1.insertedBattery.charge}");
        //    return true;
        //}

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
