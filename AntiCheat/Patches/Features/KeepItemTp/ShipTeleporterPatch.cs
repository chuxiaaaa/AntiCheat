using GameNetcodeStuff;

using HarmonyLib;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Unity.Netcode;

using UnityEngine;
using UnityEngine.UIElements;

namespace AntiCheat.Patches.Features.KeepItemTp
{
    [HarmonyPatch(typeof(ShipTeleporter))]
    [HarmonyWrapSafe]
    public static class ShipTeleporterPatch
    {

        public static Dictionary<ulong,List<ulong>> playerObjToItemIdsMap = new Dictionary<ulong, List<ulong>>();

        private static void CachePlayerItems(PlayerControllerB player)
        {
            if (player == null)
            {
                return;
            }
            if (!playerObjToItemIdsMap.ContainsKey(player.OwnerClientId))
            {
                playerObjToItemIdsMap.Add(player.OwnerClientId, new List<ulong>());
            }

            playerObjToItemIdsMap[player.OwnerClientId].Clear();

            foreach (var item in player.ItemSlots)
            {
                if (item != null)
                {
                    playerObjToItemIdsMap[player.OwnerClientId].Add(item.NetworkObjectId);
                }
            }

            if (player.ItemOnlySlot != null)
            {
                playerObjToItemIdsMap[player.OwnerClientId].Add(player.ItemOnlySlot.NetworkObjectId);
            }
        }

        private static List<GrabbableObject> GetRemainingCachedItems(PlayerControllerB player)
        {
            List<GrabbableObject> items = new List<GrabbableObject>();

            if (player == null)
            {
                return items;
            }

            if (!playerObjToItemIdsMap.TryGetValue(player.OwnerClientId, out var cachedIds))
            {
                return items;
            }

            foreach (var item in player.ItemSlots)
            {
                if (item != null && cachedIds.Contains(item.NetworkObjectId))
                {
                    items.Add(item);
                }
            }

            if (player.ItemOnlySlot != null && cachedIds.Contains(player.ItemOnlySlot.NetworkObjectId))
            {
                items.Add(player.ItemOnlySlot);
            }

            return items;
        }

        [HarmonyPatch("SetPlayerTeleporterId")]
        [HarmonyPrefix]
        public static void TeleportPlayerOutClientRpc(PlayerControllerB playerScript, int teleporterId)
        {
            if (!StartOfRound.Instance.IsHost)
            {
                return;
            }
            if (playerScript.isPlayerControlled && !playerScript.isPlayerDead)
            {
                if (teleporterId == 1 || teleporterId == 2)
                {
                    CachePlayerItems(playerScript);
                }
                else if(teleporterId == -1)
                {
                    playerScript.StartCoroutine(DelayedTeleportStateCheck(playerScript)); 
                }
            }
        }

        private static IEnumerator DelayedTeleportStateCheck(PlayerControllerB player)
        {
            float timeout = 3f;
            float timer = 0f;

            bool sawTeleportState = false;

            while (timer < timeout)
            {

                if (player == null)
                {
                    yield break;
                }

                if (player.teleportingThisFrame || player.teleportedLastFrame)
                {
                    sawTeleportState = true;
                    break;
                }

                timer += Time.deltaTime;
                yield return null;
            }

            if (!sawTeleportState || player == null)
            {
                AntiCheatPlugin.LogInfo($"[Check] teleport state not observed: {player.playerUsername}");
                yield break;
            }

            yield return null;
            yield return null;

            if (!playerObjToItemIdsMap.TryGetValue(player.OwnerClientId, out var cachedIds) || cachedIds.Count == 0)
            {
                yield break;
            }

            var items = GetRemainingCachedItems(player);
            AntiCheatPlugin.LogInfo($"[Check] player={player.playerUsername}, items={items.Count}");

            if (items.Count > 0)
            {
                AntiCheatPlugin.ShowMessage(
                    PatchHelper.locale.Msg_GetString("KeepItemTp",
                        new Dictionary<string, string>
                        {
                            ["{player}"] = player.playerUsername,
                            ["{itemNames}"] = string.Join(",", items.Select(x => x.itemProperties.itemName))
                        }
                    )
                );
            }

            cachedIds.Clear();
        }

    }
}
