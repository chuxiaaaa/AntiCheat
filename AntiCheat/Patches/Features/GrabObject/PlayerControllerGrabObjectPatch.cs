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
                    if (grabbedObject.TryGet(out var networkObject, null))
                    {
                      
                        var grabbable = networkObject.GetComponentInChildren<GrabbableObject>();
                        if (grabbable != null)
                        {
                        
                            var allSlotsFull = true;
                            var hasJetpack = false;
                            var hasTwoHanded = false;
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
                            if (player.ItemOnlySlot == null && grabbable != null && !grabbable.itemProperties.isScrap && !grabbable.itemProperties.twoHanded && !grabbable.itemProperties.disallowUtilitySlot)
                            {
                                allSlotsFull = false;
                            }
                            LogInfo(player, "PlayerControllerB.GrabObjectServerRpc", $"itemName:{grabbable.itemProperties.itemName}", $"heldByPlayerOnServer:{(grabbable.heldByPlayerOnServer ? grabbable.playerHeldBy?.playerUsername : "false")}", $"Distance:{Vector3.Distance(player.transform.position, grabbable.transform.position)}");
                            bool ban = false;

                            GrabbableObject currentSlotItem = null;
                            if (player.currentItemSlot >= 0 && player.currentItemSlot < player.ItemSlots.Length)
                            {
                                currentSlotItem = player.ItemSlots[player.currentItemSlot];
                            }

                            var holdingTwoHanded =
                                (player.currentlyHeldObjectServer != null && player.currentlyHeldObjectServer.itemProperties.twoHanded) ||
                                (currentSlotItem != null && currentSlotItem.itemProperties.twoHanded);
                            var holdingJetpack =
                                player.currentlyHeldObjectServer is JetpackItem ||
                                currentSlotItem is JetpackItem;
                            var holdingOneHandedWhileTwoHandedEquipped = holdingTwoHanded && !grabbable.itemProperties.twoHanded;
                            var heldItemName = player.currentlyHeldObjectServer != null
                                ? player.currentlyHeldObjectServer.itemProperties.itemName
                                : (currentSlotItem != null ? currentSlotItem.itemProperties.itemName : "unknown");
                            var jetpackTwoHandGrabSuppressed = holdingJetpack && grabbable.itemProperties.twoHanded && !hasTwoHanded;
                            var twoHandDetected =
                                holdingOneHandedWhileTwoHandedEquipped ||
                                (grabbable.itemProperties.twoHanded && hasTwoHanded) ||
                                (grabbable.itemProperties.twoHanded && hasJetpack) ||
                                (hasTwoHanded && hasJetpack);

                            if (twoHandDetected)
                            {
                                if (PluginConfig.GrabObject_SendLog.Value && !jetpackTwoHandGrabSuppressed)
                                {
                                    ShowMessage(locale.Msg_GetString("GrabObject_TwoHand", new System.Collections.Generic.Dictionary<string, string>()
                                    {
                                        { "{player}", player.playerUsername },
                                        { "{heldItemName}", heldItemName },
                                        { "{itemName}", grabbable.itemProperties.itemName },
                                        { "{hasTwoHand}", hasTwoHanded.ToString() },
                                        { "{jetpack}", hasJetpack.ToString() }
                                    }));
                                }

                                if (PluginConfig.GrabObject_TwoHand.Value)
                                {
                                    ban = true;
                                }
                            }

                            var moreSlotDetected = allSlotsFull && !ban;
                            if (moreSlotDetected && PluginConfig.GrabObject_SendLog.Value)
                            {
                                ShowMessage(locale.Msg_GetString("GrabObject_MoreSlot", new System.Collections.Generic.Dictionary<string, string>()
                                {
                                    { "{player}", player.playerUsername },
                                    { "{itemName}", grabbable.itemProperties.itemName }
                                }));
                            }

                            if (PluginConfig.GrabObject_MoreSlot.Value && !ban)
                            {
                                ban = allSlotsFull;
                            }

                            if (ban)
                            {
                                if (PluginConfig.GrabObject_Kick.Value)
                                {
                                    KickPlayer(player);
                                }

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

                                if (PluginConfig.GrabObject_SendLog.Value)
                                {
                                    ShowMessage(locale.Msg_GetString("GrabObject", new System.Collections.Generic.Dictionary<string, string>()
                                    {
                                        { "{player}", player.playerUsername },
                                        { "{object_position}", grabbable.transform.position.ToString() },
                                        { "{player_position}", player.serverPlayerPosition.ToString() }
                                    }));
                                }

                                if (PluginConfig.GrabObject_Kick.Value)
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
