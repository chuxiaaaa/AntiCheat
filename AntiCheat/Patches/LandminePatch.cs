using HarmonyLib;

using System;
using System.Collections.Generic;
using System.Linq;

using Unity.Netcode;

using UnityEngine;

namespace AntiCheat.Patches
{
    [HarmonyPatch(typeof(Landmine))]
    [HarmonyWrapSafe]
    public static class LandminePatch
    {
        private static readonly HashSet<int> LandMines = new HashSet<int>();
        private static readonly AccessTools.FieldRef<Landmine, bool> MineActivatedField =
            AccessTools.FieldRefAccess<Landmine, bool>("mineActivated");

        private const float MineTriggerDistance = 5f;

        [HarmonyPrefix]
        [HarmonyPatch("OnTriggerExit")]
        public static bool TriggerMineOnLocalClientByExiting(Landmine __instance, Collider other)
        {
            if (!StartOfRound.Instance.IsHost ||
                __instance.hasExploded ||
                !__instance.gameObject.activeSelf ||
                !MineActivatedField(__instance))
            {
                return true;
            }

            lock (LandMines)
            {
                LandMines.Add(__instance.GetInstanceID());
            }

            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch("SpawnExplosion")]
        public static void SpawnExplosion(Vector3 explosionPosition)
        {
            AntiCheatPlugin.LogInfo($"Landmine.SpawnExplosion -> {explosionPosition}");

            PatchHelper.explosions = PatchHelper.explosions
                .Where(x => x.CreateDateTime.AddSeconds(10) > DateTime.Now)
                .ToList();

            PatchHelper.explosions.Add(new PatchHelper.ExplosionData
            {
                ExplosionPostion = explosionPosition,
                CalledClient = new List<ulong>(),
                CreateDateTime = DateTime.Now,
            });
        }

        [HarmonyPatch("__rpc_handler_3032666565")]
        [HarmonyPrefix]
        public static bool ExplodeMineServerRpc(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (!PluginConfig.Landmine.Enable)
            {
                return true;
            }

            if (!PatchHelper.Check(rpcParams, out var player))
            {
                return player != null;
            }

            var landmine = (Landmine)target;
            var instanceId = landmine.GetInstanceID();

            lock (LandMines)
            {
                if (LandMines.Contains(instanceId) || landmine.hasExploded)
                {
                    return true;
                }
            }

            const float triggerDistanceSq = MineTriggerDistance * MineTriggerDistance;
            if (PatchHelper.recentPlayerPositions.TryGetValue(player.playerSteamId, out var positions) &&
                !positions.Any(x => (x.pos - landmine.transform.position).sqrMagnitude < triggerDistanceSq))
            {
                AntiCheatPlugin.ShowMessage(
                    PatchHelper.locale.Msg_GetString(
                        "Landmine",
                        new Dictionary<string, string>
                        {
                            ["{player}"] = player.playerUsername,
                        }));

                if (PluginConfig.Landmine.Kick)
                {
                    PatchHelper.KickPlayer(player);
                }
            }

            return true;
        }
    }
}
