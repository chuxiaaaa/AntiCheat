using GameNetcodeStuff;

using HarmonyLib;

using System;
using System.Collections.Generic;
using System.Linq;

using Unity.Netcode;
using UnityEngine;

using static AntiCheat.Patches.PatchHelper;

namespace AntiCheat.Patches
{
    [HarmonyPatch(typeof(EnemyAI))]
    [HarmonyWrapSafe]
    public static class EnemyAICombatPatch
    {
        [HarmonyPatch("HitEnemyOnLocalClient")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static void HitEnemyOnLocalClient(EnemyAI __instance, int force, Vector3 hitDirection, PlayerControllerB playerWhoHit, bool playHitSFX, int hitID)
        {
            if (StartOfRound.Instance.localPlayerController.IsHost)
            {
                bypassHit.Add(new HitData()
                {
                    EnemyInstanceId = __instance.GetInstanceID(),
                    force = force,
                    CalledClient = new List<ulong>()
                });
            }
        }

        [HarmonyPatch(typeof(EnemyAI), "__rpc_handler_3538577804")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static bool HitEnemyServerRpc(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (Check(rpcParams, out var player) || StartOfRound.Instance.localPlayerController.IsHost)
            {
                if (player == null)
                {
                    return false;
                }
                ByteUnpacker.ReadValueBitPacked(reader, out int force);
                ByteUnpacker.ReadValueBitPacked(reader, out int playerWhoHit);
                reader.ReadValueSafe(out bool playHitSFX, default);
                reader.Seek(0);
                var enemy = (EnemyAI)target;
                LogInfo(player, $"({enemy.enemyType.enemyName})EnemyAI.HitEnemyServerRpc", $"EnemyId:{enemy.GetInstanceID()}", $"HP:{enemy.enemyHP}", $"force:{force}", $"playerWhoHit:{PlayerClientIdConvertName(playerWhoHit)}({playerWhoHit})");
                if (player.isHostPlayerObject)
                {
                    return true;
                }
                if (playerWhoHit != -1 && StartOfRound.Instance.allPlayerScripts[playerWhoHit] != player)
                {
                    LogInfo("return false;");
                    return false;
                }
                VehicleController vehicleController = UnityEngine.Object.FindFirstObjectByType<VehicleController>();
                if (vehicleController != null)
                {
                    PlayerControllerB currentDriver = vehicleController.currentDriver;
                    if (force == 2 && currentDriver != null && currentDriver.playerClientId == player.playerClientId)
                    {
                        lastDriver = currentDriver;
                        return true;
                    }
                    if (force == 2 && currentDriver == null && lastDriver.playerClientId == player.playerClientId)
                    {
                        return true;
                    }
                }
                foreach (var item in bypassHit)
                {
                    if (item.EnemyInstanceId == enemy.GetInstanceID() &&
                        item.force == force &&
                        !item.CalledClient.Contains(player.playerClientId))
                    {
                        LogInfo($"{enemy.GetInstanceID()} bypass|playerClientId:{player.playerClientId}");
                        item.CalledClient.Add(player.playerClientId);
                        return true;
                    }
                }
                if (PluginConfig.Shovel.Value)
                {
                    var heldItem = player.ItemSlots[player.currentItemSlot];
                    if (force == 6 && playerWhoHit == -1)
                    {
                        LogInfo($"force = 6||enemyPostion:{enemy.transform.position}");
                        explosions = explosions.Where(x => x.CreateDateTime.AddSeconds(10) > DateTime.Now).ToList();
                        for (int i = explosions.Count - 1; i > 0; i--)
                        {
                            var explosion = explosions[i];
                            if (explosion.CalledClient.Contains(player.playerSteamId))
                            {
                                continue;
                            }
                            float explosionDistance = Vector3.Distance(explosion.ExplosionPostion, enemy.transform.position);
                            LogInfo($"ExplosionPostion:{explosion.ExplosionPostion}||Distance:{explosionDistance}");
                            if (explosionDistance < 5f)
                            {
                                explosion.CalledClient.Add(player.playerSteamId);
                                return true;
                            }
                        }
                    }
                    else if (force != 1 && heldItem != null && (isShovel(heldItem) || isKnife(heldItem)))
                    {
                        if (!jcs.Contains(player.playerSteamId))
                        {
                            ShowMessage(locale.Msg_GetString("Shovel4", new Dictionary<string, string>()
                            {
                                { "{player}", player.playerUsername },
                                { "{enemyName}", enemy.enemyType.enemyName },
                                { "{damageAmount}", force.ToString() },
                                { "{item}", isShovel(heldItem) ? locale.Item_GetString("Shovel") : locale.Item_GetString("Knife") }
                            }));
                            jcs.Add(player.playerSteamId);
                            if (PluginConfig.Shovel2.Value)
                            {
                                KickPlayer(player);
                            }
                            return false;
                        }
                    }
                    else if (!player.isPlayerDead && heldItem == null)
                    {
                        if (ClingTime.ContainsKey(player.playerSteamId) && ClingTime[player.playerSteamId].AddSeconds(5) > DateTime.Now)
                        {
                            return true;
                        }
                        if (player.ItemSlots.Any(x => isShovel(x)) && force == 1)
                        {
                            return true;
                        }
                        if (!jcs.Contains(player.playerSteamId))
                        {
                            for (int i = 0; i < player.ItemSlots.Length; i++)
                            {
                                LogInfo($"p:{player.playerUsername}|i:{i}|itemName:{player.ItemSlots[i]?.itemProperties?.itemName}");
                            }
                            LogInfo($"currentItemSlot:{player.currentItemSlot}");
                            LogInfo($"currentlyHeldObjectServer:{player.currentlyHeldObjectServer?.itemProperties?.itemName}");
                            LogInfo($"obj:{heldItem}");
                            ShowMessage(locale.Msg_GetString("Shovel6", new Dictionary<string, string>()
                            {
                                { "{player}", player.playerUsername },
                                { "{enemyName}", enemy.enemyType.enemyName },
                                { "{damageAmount}", force.ToString() }
                            }));
                            if (!PluginConfig.Shovel3.Value && (force == 1 || force == 2 || force == 3 || force == 5))
                            {
                                jcs.Add(player.playerSteamId);
                                if (PluginConfig.Shovel2.Value)
                                {
                                    KickPlayer(player);
                                }
                            }
                            return false;
                        }
                    }
                    else if (jcs.Contains(player.playerSteamId))
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
