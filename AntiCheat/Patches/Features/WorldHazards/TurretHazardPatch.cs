using HarmonyLib;

using System.Collections.Generic;

using Unity.Netcode;
using UnityEngine;

namespace AntiCheat.Patches
{
    [HarmonyPatch(typeof(Turret))]
    [HarmonyWrapSafe]
    public static class TurretHazardPatch
    {
        [HarmonyPatch("__rpc_handler_4195711963")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static bool EnterBerserkModeServerRpc(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (PatchHelper.Check(rpcParams, out var player))
            {
                if (PluginConfig.Turret.Enable)
                {
                    var item = player.ItemSlots[player.currentItemSlot];
                    if (item != null && (PatchHelper.isShovel(item) || PatchHelper.isKnife(item)))
                    {
                        var turret = (Turret)target;
                        float distance = Vector3.Distance(turret.transform.position, player.transform.position);
                        if (distance > 12)
                        {
                            PatchHelper.ShowMessage(PatchHelper.locale.Msg_GetString("Turret", new Dictionary<string, string>()
                            {
                                { "{player}", player.playerUsername },
                                { "{Distance}", distance.ToString() }
                            }));
                            if (PluginConfig.Turret.Kick)
                            {
                                PatchHelper.KickPlayer(player);
                            }
                            return false;
                        }
                    }
                    else
                    {
                        PatchHelper.ShowMessage(PatchHelper.locale.Msg_GetString("Turret2", new Dictionary<string, string>()
                        {
                             { "{player}", player.playerUsername }
                        }));
                        if (PluginConfig.Turret.Kick)
                        {
                            PatchHelper.KickPlayer(player);
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
