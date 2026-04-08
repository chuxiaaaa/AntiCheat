using GameNetcodeStuff;

using HarmonyLib;

using System.Reflection;

using Unity.Netcode;
using UnityEngine;

using static AntiCheat.Patches.PatchHelper;

namespace AntiCheat.Patches
{
    [HarmonyPatch(typeof(PlayerControllerB))]
    [HarmonyWrapSafe]
    public static class PlayerControllerGrabObjectPatch
    {
        [HarmonyPatch("__rpc_handler_412259855")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static bool SwitchItemSlotsServerRpc(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (Check(rpcParams, out var player))
            {
                if (player.currentlyHeldObjectServer != null && player.currentlyHeldObjectServer.itemProperties.twoHanded)
                {
                    return false;
                }

                return true;
            }
            else if (player == null)
            {
                return false;
            }
            return true;
        }

        [HarmonyPatch("__rpc_handler_1554282707")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static bool GrabObjectServerRpc(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (Check(rpcParams, out var player))
            {
                if (PluginConfig.GrabObject.Value)
                {
                    reader.ReadValueSafe(out NetworkObjectReference grabbedObject, default);
                    reader.Seek(0);
                    var hasJetpack = false;
                    if (grabbedObject.TryGet(out var networkObject, null))
                    {
                        var allSlotsFull = true;
                        bool hasTwoHanded = false;
                        foreach (var item in player.ItemSlots)
                        {
                            if (item == null)
                            {
                                allSlotsFull = false;
                            }
                            else if (item.itemProperties.twoHanded)
                            {
                                hasTwoHanded = true;
                            }
                            else if (item is JetpackItem)
                            {
                                hasJetpack = true;
                            }
                        }
                        var grabbable = networkObject.GetComponentInChildren<GrabbableObject>();
                        if (grabbable != null)
                        {
                            LogInfo(player, "PlayerControllerB.GrabObjectServerRpc", $"itemName:{grabbable.itemProperties.itemName}", $"heldByPlayerOnServer:{(grabbable.heldByPlayerOnServer ? grabbable.playerHeldBy?.playerUsername : "false")}", $"Distance:{Vector3.Distance(player.transform.position, grabbable.transform.position)}");
                            bool ban = false;
                            if (PluginConfig.GrabObject_TwoHand.Value)
                            {
                                if (grabbable.itemProperties.twoHanded && hasTwoHanded)
                                {
                                    ban = true;
                                }
                                else if (grabbable.itemProperties.twoHanded && hasJetpack)
                                {
                                    ban = true;
                                }
                                else if (hasTwoHanded && hasJetpack)
                                {
                                    ban = true;
                                }
                            }
                            if (PluginConfig.GrabObject_MoreSlot.Value && !ban)
                            {
                                ban = allSlotsFull;
                            }
                            if (ban)
                            {
                                ForceCancelGrab(target);
                                return false;
                            }
                            if (Vector3.Distance(grabbable.transform.position, player.serverPlayerPosition) > 100 &&
                                !StartOfRound.Instance.shipIsLeaving &&
                                StartOfRound.Instance.shipHasLanded)
                            {
                                if (player.teleportedLastFrame)
                                {
                                    return true;
                                }
                                ShowMessage(locale.Msg_GetString("GrabObject", new System.Collections.Generic.Dictionary<string, string>()
                                {
                                    { "{player}", player.playerUsername },
                                    { "{object_position}", grabbable.transform.position.ToString() },
                                    { "{player_position}", player.serverPlayerPosition.ToString() }
                                }));
                                if (PluginConfig.GrabObject_MoreSlot.Value)
                                {
                                    KickPlayer(player);
                                }
                                ForceCancelGrab(target);
                                return false;
                            }
                        }
                    }
                }
            }
            else if (player == null)
            {
                return false;
            }
            return true;
        }

        private static void ForceCancelGrab(NetworkBehaviour target)
        {
            FieldInfo rpcExecStage = typeof(NetworkBehaviour).GetField("__rpc_exec_stage", BindingFlags.NonPublic | BindingFlags.Instance);
            rpcExecStage.SetValue(target, 1);
            typeof(PlayerControllerB).GetMethod("GrabObjectServerRpc", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(
                (PlayerControllerB)target,
                new object[]
                {
                    default(NetworkObjectReference)
                });
            rpcExecStage.SetValue(target, 0);
        }
    }
}
