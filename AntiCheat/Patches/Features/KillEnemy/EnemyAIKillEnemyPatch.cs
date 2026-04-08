using HarmonyLib;

using System.Collections.Generic;

using Unity.Netcode;
using UnityEngine;

using static AntiCheat.Patches.PatchHelper;

namespace AntiCheat.Patches
{
    [HarmonyPatch(typeof(EnemyAI))]
    [HarmonyWrapSafe]
    public static class EnemyAIKillEnemyPatch
    {
        [HarmonyPatch("__rpc_handler_1810146992")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static bool KillEnemyServerRpc(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (Check(rpcParams, out var player))
            {
                var enemy = (EnemyAI)target;
                LogInfo(player, $"({enemy.enemyType.enemyName})EnemyAI.KillEnemyServerRpc", $"EnemyId:{enemy.GetInstanceID()}", $"HP:{enemy.enemyHP}");
                if (PluginConfig.KillEnemy.Value)
                {
                    foreach (var item in bypassKill)
                    {
                        if (item.EnemyInstanceId == enemy.GetInstanceID() && !item.CalledClient.Contains(player.playerClientId))
                        {
                            LogInfo($"{enemy.GetInstanceID()} bypass|playerClientId:{player.playerClientId}");
                            item.CalledClient.Add(player.playerClientId);
                            return true;
                        }
                    }
                    if (enemy.enemyHP <= 0)
                    {
                        return true;
                    }

                    if (Vector3.Distance(player.transform.position, enemy.transform.position) > 50f)
                    {
                        ShowMessage(locale.Msg_GetString("KillEnemy", new Dictionary<string, string>()
                        {
                            { "{player}", player.playerUsername },
                            { "{enemyName}", enemy.enemyType.enemyName },
                            { "{HP}", enemy.enemyHP.ToString() }
                        }));
                        if (PluginConfig.KillEnemy2.Value)
                        {
                            KickPlayer(player);
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        [HarmonyPatch("KillEnemyOnOwnerClient")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static void KillEnemyOnOwnerClient(EnemyAI __instance)
        {
            if (StartOfRound.Instance.localPlayerController.IsHost)
            {
                if (bypassKill.Exists(x => x.EnemyInstanceId == __instance.GetInstanceID()))
                {
                    return;
                }
                LogInfo($"bypassKill -> {__instance.enemyType.enemyName}({__instance.GetInstanceID()})");
                bypassKill.Add(new HitData()
                {
                    EnemyInstanceId = __instance.GetInstanceID(),
                    force = 0,
                    CalledClient = new List<ulong>()
                });
            }
        }

        [HarmonyPatch("KillEnemy")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static void KillEnemy(EnemyAI __instance)
        {
            if (StartOfRound.Instance.localPlayerController.IsHost && __instance is ButlerEnemyAI)
            {
                LandminePatch.SpawnExplosion(__instance.transform.position + Vector3.up * 0.15f);
            }
        }
    }
}
