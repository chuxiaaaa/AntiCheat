using AntiCheat.Utils;

using HarmonyLib;

using System.Collections.Generic;
using System.Linq;

using Unity.Netcode;

namespace AntiCheat.Patches
{
    [HarmonyPatch(typeof(GrabbableObject))]
    [HarmonyWrapSafe]
    public static class GrabbableObjectPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("__rpc_handler_4280509730")]
        public static bool ActivateItemServerRpc(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (PatchHelper.Check(rpcParams, out var player))
            {
                if (target is RemoteProp remoteProp)
                {
                    AntiCheatPlugin.LogInfo(player, $"({remoteProp.itemProperties.itemName})GrabbableObject.ActivateItemServerRpc");

                    var canUse = CooldownManager.CheckCooldown("ShipLight", player);
                    if (!canUse)
                    {
                        UnityEngine.Object.FindFirstObjectByType<ShipLights>().SetShipLightsClientRpc(true);
                    }

                    return canUse;
                }
            }
            else if (player == null)
            {
                return false;
            }

            return target != null;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(GiftBoxItem), "ItemActivate")]
        public static void ItemActivate(GiftBoxItem __instance)
        {
            if (StartOfRound.Instance.IsHost)
            {
                UnityEngine.Object.Destroy(__instance.gameObject);
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch("__rpc_handler_3484508350")]
        public static bool SyncBatteryServerRpc(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (!PatchHelper.Check(rpcParams, out var player) && false)
            {
                return player != null;
            }

            ByteUnpacker.ReadValueBitPacked(reader, out int num);
            reader.Seek(0);

            var grabbable = (GrabbableObject)target;
            if (grabbable.itemProperties.requiresBattery)
            {
                AntiCheatPlugin.LogInfo(player, $"({grabbable.itemProperties.itemName})GrabbableObject.SyncBatteryServerRpc", $"num:{num}");
                if (num > 100)
                {
                    return false;
                }
            }

            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(LungProp), "EquipItem")]
        public static bool EquipItem(LungProp __instance)
        {
            if (PluginConfig.OperationLog.Value &&
                __instance.isLungDocked &&
                StartOfRound.Instance.shipHasLanded)
            {
                PatchHelper.ShowMessageHostOnly(
                    PatchHelper.locale.OperationLog_GetString(
                        "GrabLungProp",
                        new Dictionary<string, string>
                        {
                            ["{player}"] = StartOfRound.Instance
                                .allPlayerScripts
                                .First(x => x.OwnerClientId == __instance.OwnerClientId)
                                .playerUsername,
                        }));
            }

            return true;
        }
    }
}
