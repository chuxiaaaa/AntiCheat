using HarmonyLib;

using System.Collections.Generic;

using Unity.Netcode;

using static AntiCheat.Patches.PatchHelper;

namespace AntiCheat.Patches
{
    [HarmonyPatch(typeof(DepositItemsDesk))]
    [HarmonyWrapSafe]
    public static class DepositItemsDeskHazardPatch
    {
        [HarmonyPatch("__rpc_handler_3230280218")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static bool AttackPlayersServerRpc(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (Check(rpcParams, out var player))
            {
                if (PluginConfig.Boss.Value)
                {
                    ShowMessage(locale.Msg_GetString("Boss", new Dictionary<string, string>()
                    {
                        { "{player}", player.playerUsername }
                    }));
                    if (PluginConfig.Boss2.Value)
                    {
                        KickPlayer(player);
                    }
                }
                return false;
            }
            else if (player == null)
            {
                return false;
            }
            return true;
        }
    }
}
