using HarmonyLib;

using Unity.Netcode;

namespace AntiCheat.Patches
{
    [HarmonyPatch(typeof(GrabbableObject))]
    [HarmonyWrapSafe]
    public static class GrabbableObjectItemUsePatch
    {
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
    }
}
