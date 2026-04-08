using GameNetcodeStuff;

using HarmonyLib;

using Unity.Netcode;

using static AntiCheat.Patches.PatchHelper;

namespace AntiCheat.Patches
{
    [HarmonyPatch(typeof(PlayerControllerB))]
    [HarmonyWrapSafe]
    public static class PlayerControllerItemUsePatch
    {
        [HarmonyPatch("__rpc_handler_1786952262")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static bool DespawnHeldObjectServerRpc(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (Check(rpcParams, out var player))
            {
                if (PluginConfig.DespawnItem.Value)
                {
                    LogInfo(player, "PlayerControllerB.DespawnHeldObjectServerRpc", $"itemName:{player.currentlyHeldObjectServer.itemProperties.itemName}");
                    if (player.currentlyHeldObjectServer != null &&
                        !(player.currentlyHeldObjectServer is GiftBoxItem) &&
                        !(player.currentlyHeldObjectServer is KeyItem))
                    {
                        ShowMessage(locale.Msg_GetString("DespawnItem", new System.Collections.Generic.Dictionary<string, string>()
                        {
                            { "{player}", player.playerUsername },
                            { "{item}", player.currentlyHeldObjectServer.itemProperties.itemName }
                        }));
                        if (PluginConfig.DespawnItem2.Value)
                        {
                            KickPlayer(player);
                        }
                        return false;
                    }
                }
            }
            else if (player == null)
            {
                return false;
            }
            return true;
        }
    }
}
