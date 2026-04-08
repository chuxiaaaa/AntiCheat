using GameNetcodeStuff;

using HarmonyLib;

using System.Collections.Generic;
using System.Linq;

using Unity.Netcode;
using UnityEngine;

using static AntiCheat.Patches.PatchHelper;

namespace AntiCheat.Patches
{
    [HarmonyPatch(typeof(PlayerControllerB))]
    [HarmonyWrapSafe]
    public static class PlayerControllerMovementPatch
    {
        [HarmonyPatch(typeof(PlayerControllerB), "__rpc_handler_2013428264")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static bool UpdatePlayerPositionServerRpc(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            reader.ReadValueSafe(out Vector3 newPos);
            reader.Seek(0);
            if (Check(rpcParams, out var player))
            {
                if (PluginConfig.Invisibility.Value)
                {
                    if (StartOfRound.Instance.currentLevel != null && StartOfRound.Instance.currentLevel.spawnEnemiesAndScrap)
                    {
                        if (!recentPlayerPositions.ContainsKey(player.playerSteamId))
                        {
                            recentPlayerPositions[player.playerSteamId] = new List<(Vector3, float)>();
                        }
                        float now = Time.time;
                        recentPlayerPositions[player.playerSteamId] = recentPlayerPositions[player.playerSteamId]
                            .Where(entry => now - entry.time <= 5f)
                            .ToList();
                        recentPlayerPositions[player.playerSteamId].Add((newPos, now));
                    }
                    var oldPos = player.serverPlayerPosition;
                    if (player.teleportedLastFrame)
                    {
                        return true;
                    }
                    if (Vector3.Distance(oldPos, newPos) > 100 && Vector3.Distance(newPos, Vector3.zero) > 10)
                    {
                        ShowMessage(locale.Msg_GetString("Invisibility", new Dictionary<string, string>()
                        {
                            { "{player}", player.playerUsername },
                            { "{player_position}", newPos.ToString() }
                        }));
                        if (PluginConfig.Invisibility2.Value)
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
