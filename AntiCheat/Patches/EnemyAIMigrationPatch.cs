using AntiCheat.Utils;

using BepInEx;
using BepInEx.Configuration;

using GameNetcodeStuff;

using HarmonyLib;

using Netcode.Transports.Facepunch;

using Steamworks;
using Steamworks.Data;
using Steamworks.ServerList;

using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using TMPro;

using Unity.Collections.LowLevel.Unsafe;
using Unity.Netcode;

using UnityEngine;
using UnityEngine.Events;

using static UnityEngine.GraphicsBuffer;

using static AntiCheat.Patches.PatchHelper;

namespace AntiCheat.Patches
{
    [HarmonyPatch(typeof(EnemyAI))]
    [HarmonyWrapSafe]
    public static class EnemyAIMigrationPatch
    {

        /// <summary>
        /// Prefix EnemyAI.SwitchToBehaviourServerRpc
        /// </summary>
        [HarmonyPatch("__rpc_handler_2081148948")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static bool __rpc_handler_2081148948(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (Check(rpcParams, out var p))
            {
                ByteUnpacker.ReadValueBitPacked(reader, out int stateIndex);
                reader.Seek(0);
                var e = (EnemyAI)target;
                LogInfo(p, $"({e.enemyType.enemyName})EnemyAI.SwitchToBehaviourServerRpc", $"stateIndex:{stateIndex}");
                if (PluginConfig.Enemy.Value)
                {
                    if (e is JesterAI j)
                    {
                        if (j.currentBehaviourStateIndex == 0 && stateIndex == 1)
                        {
                            if (j.targetPlayer != null)
                            {
                                return true;
                            }
                        }
                        else if (j.currentBehaviourStateIndex == 1 && stateIndex == 2 && j.popUpTimer <= 0)
                        {
                            return true;
                        }
                        else if (j.currentBehaviourStateIndex == 2 && stateIndex == 0)
                        {
                            return true;
                        }
                        else
                        {
                            LogInfo(p, $"({e.enemyType.enemyName})EnemyAI.SwitchToBehaviourServerRpc", $"stateIndex:{stateIndex}", $"popUpTimer:{j.popUpTimer}");
                            //ShowMessage(locale.Msg_GetString("Enemy_SwitchToBehaviour", new Dictionary<string, string>() {
                            //    { "{player}", p.playerUsername }
                            //}));
                            //return false;
                            return true;
                        }
                    }
                }
            }
            else if (p == null)
            {
                return false;
            }
            return true;
        }


        /// <summary>
        /// Prefix EnemyAI.UpdateEnemyPositionServerRpc
        /// </summary>
        [HarmonyPatch("__rpc_handler_255411420")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static bool __rpc_handler_255411420(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (Check(rpcParams, out var p))
            {
                //reader.ReadValueSafe(out Vector3 newPos);
                //reader.Seek(0);
                //LogInfo($"{p.playerUsername} call EnemyAI.UpdateEnemyPositionServerRpc|newPos:{newPos}");
            }
            else if (p == null)
            {
                return false;
            }
            return true;
        }



        [HarmonyPatch("__rpc_handler_1810146992")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static bool __rpc_handler_1810146992(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (Check(rpcParams, out var p))
            {
                var e = (EnemyAI)target;
                LogInfo(p, $"({e.enemyType.enemyName})EnemyAI.KillEnemyServerRpc", $"EnemyId:{e.GetInstanceID()}", $"HP:{e.enemyHP}");
                if (PluginConfig.KillEnemy.Value)
                {
                    foreach (var item in bypassKill)
                    {
                        if (item.EnemyInstanceId == e.GetInstanceID() && !item.CalledClient.Contains(p.playerClientId))
                        {
                            LogInfo($"{e.GetInstanceID()} bypass|playerClientId:{p.playerClientId}");
                            item.CalledClient.Add(p.playerClientId);
                            return true;
                        }
                    }
                    if (e.enemyHP <= 0)
                    {
                        return true;
                    }

                    if (Vector3.Distance(p.transform.position, e.transform.position) > 50f)
                    {
                        ShowMessage(locale.Msg_GetString("KillEnemy", new Dictionary<string, string>() {
                            { "{player}",p.playerUsername },
                            { "{enemyName}",e.enemyType.enemyName },
                            { "{HP}",e.enemyHP.ToString() }
                        }));
                        if (PluginConfig.KillEnemy2.Value)
                        {
                            KickPlayer(p);
                            return false;
                        }
                    }
                }
            }
            return true;
        }


        [HarmonyPatch("__rpc_handler_3079913705")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static bool __rpc_handler_3079913705(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (Check(rpcParams, out var p))
            {
                if (PluginConfig.Enemy.Value)
                {
                    //LogInfo($"{p.playerUsername} call EnemyAI.UpdateEnemyRotationServerRpc");
                    return true;
                }
            }
            return true;
        }


        //[HarmonyPatch("__rpc_handler_255411420")]
        //[HarmonyPrefix]
        //public static bool __rpc_handler_255411420(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        //{
        //    if (Check(rpcParams, out var p))
        //    {
        //        if (AntiCheatPlugin.Enemy.Value)
        //        {
        //            LogInfo($"{p.playerUsername} call EnemyAI.UpdateEnemyPositionServerRpc");
        //            return true;
        //        }
        //    }
        //    else if (p == null)
        //    {
        //        return false;
        //    }
        //    return true;
        //}

        [HarmonyPatch(typeof(EnemyAI), "__rpc_handler_3587030867")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static bool __rpc_handler_3587030867(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (Check(rpcParams, out var p))
            {
                if (PluginConfig.Enemy.Value)
                {
                    ByteUnpacker.ReadValueBitPacked(reader, out int clientId);
                    reader.Seek(0);
                    var enemy = target.GetComponent<EnemyAI>();
                    if (enemy is MouthDogAI || enemy is DressGirlAI)
                    {
                        return true;
                    }
                    float v = Vector3.Distance(p.transform.position, enemy.transform.position);
                    LogInfo(p, $"({enemy.enemyType.enemyName})EnemyAI.ChangeEnemyOwnerServerRpc", $"Distance:{v}", $"CallClientId:{p.playerClientId}", $"OwnerClientId:{enemy.OwnerClientId}", $"NewClientId:{clientId}");
                    return true;
                }
            }
            else if (p == null)
            {
                return false;
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
                if (bypassKill.Any(x => x.EnemyInstanceId == __instance.GetInstanceID()))
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



        [HarmonyPatch("HitEnemyOnLocalClient")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static void HitEnemyOnLocalClient(EnemyAI __instance, int force, Vector3 hitDirection, PlayerControllerB playerWhoHit, bool playHitSFX, int hitID)
        {
            if (StartOfRound.Instance.localPlayerController.IsHost)
            {
                //if (bypassHit.Any(x => x.EnemyInstanceId == __instance.GetInstanceID()))
                //{
                //    return;
                //}
                //if (playerWhoHit == null)
                //{
                bypassHit.Add(new HitData()
                {
                    EnemyInstanceId = __instance.GetInstanceID(),
                    force = force,
                    CalledClient = new List<ulong>()
                });
                //}
            }
        }



        [HarmonyPatch("KillEnemy")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static void ButlerEnemyAI_KillEnemy(EnemyAI __instance)
        {
            if (StartOfRound.Instance.localPlayerController.IsHost)
            {
                if (__instance is ButlerEnemyAI)
                {
                    LandminePatch.SpawnExplosion(__instance.transform.position + Vector3.up * 0.15f);
                }
            }
        }


        //[HarmonyPatch("HitEnemy")]
        //[HarmonyPrefix]
        //[HarmonyWrapSafe]
        //public static void HitEnemy(EnemyAI __instance, int force = 1, PlayerControllerB playerWhoHit = null, bool playHitSFX = false, int hitID = -1)
        //{
        //    AntiCheat.AntiCheatPlugin.LogInfo($"({__instance.enemyType.enemyName})EnemyAI.HitEnemy;EnemyId:{__instance.GetInstanceID()}|HP:{__instance.enemyHP}|force:{force}|playerWhoHit:{playerWhoHit?.playerUsername}|playHitSFX:{playHitSFX}|hitID:{hitID}");
        //}

        [HarmonyPatch(typeof(EnemyAI), "__rpc_handler_3538577804")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static bool HitEnemyServerRpcPatch(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (Check(rpcParams, out var p) || StartOfRound.Instance.localPlayerController.IsHost)
            {
                if (p == null)
                {
                    return false;
                }
                int force;
                ByteUnpacker.ReadValueBitPacked(reader, out force);
                ByteUnpacker.ReadValueBitPacked(reader, out int playerWhoHit);
                reader.ReadValueSafe(out bool playHitSFX, default);
                reader.Seek(0);
                var e = (EnemyAI)target;
                LogInfo(p, $"({e.enemyType.enemyName})EnemyAI.HitEnemyServerRpc", $"EnemyId:{e.GetInstanceID()}", $"HP:{e.enemyHP}", $"force:{force}", $"playerWhoHit:{PlayerClientIdConvertName(playerWhoHit)}({playerWhoHit})");
                if (p.isHostPlayerObject)
                {
                    return true;
                }
                if (playerWhoHit != -1 && StartOfRound.Instance.allPlayerScripts[playerWhoHit] != p)
                {
                    LogInfo("return false;");
                    return false;
                }
                VehicleController vehicleController = UnityEngine.Object.FindFirstObjectByType<VehicleController>();
                if (vehicleController != null)
                {
                    PlayerControllerB currentDriver = vehicleController.currentDriver;
                    if (force == 2 && currentDriver != null && currentDriver.playerClientId == p.playerClientId)
                    {
                        lastDriver = currentDriver;
                        return true;
                    }
                    if (force == 2 && currentDriver == null && lastDriver.playerClientId == p.playerClientId)
                    {
                        return true;
                    }
                }
                //if (e.isEnemyDead)
                //{
                //    return true;
                //}
                foreach (var item in bypassHit)
                {
                    if (item.EnemyInstanceId == e.GetInstanceID() && item.force == force && !item.CalledClient.Contains(p.playerClientId))
                    {
                        LogInfo($"{e.GetInstanceID()} bypass|playerClientId:{p.playerClientId}");
                        item.CalledClient.Add(p.playerClientId);
                        return true;
                    }
                }
                if (PluginConfig.Shovel.Value)
                {
                    //if (playerWhoHit == -1 && Vector3.Distance(p.transform.position, e.transform.position) < 30)
                    //{
                    //    return true;
                    //}
                    var obj = p.ItemSlots[p.currentItemSlot];
                    string playerUsername = p.playerUsername;
                    if (force == 6 && playerWhoHit == -1)
                    {
                        LogInfo($"force = 6||enemyPostion:{e.transform.position}");
                        explosions = explosions.Where(x => x.CreateDateTime.AddSeconds(10) > DateTime.Now).ToList();
                        for (int i = explosions.Count - 1; i > 0; i--)
                        {
                            var item = explosions[i];
                            if (item.CalledClient.Contains(p.playerSteamId))
                            {
                                continue;
                            }
                            float ExplosionDistance = Vector3.Distance(item.ExplosionPostion, e.transform.position);
                            LogInfo($"ExplosionPostion:{item.ExplosionPostion}||Distance:{ExplosionDistance}");
                            if (ExplosionDistance < 5f)
                            {
                                item.CalledClient.Add(p.playerSteamId);
                                return true;
                            }
                        }
                    }
                    else if (force != 1 && obj != null && (isShovel(obj) || isKnife(obj)))
                    {
                        if (!jcs.Contains(p.playerSteamId))
                        {
                            ShowMessage(locale.Msg_GetString("Shovel4", new Dictionary<string, string>() {
                                { "{player}",p.playerUsername },
                                { "{enemyName}",e.enemyType.enemyName },
                                { "{damageAmount}",force.ToString() },
                                { "{item}", isShovel(obj) ? locale.Item_GetString("Shovel") : locale.Item_GetString("Knife") }
                            }));
                            jcs.Add(p.playerSteamId);
                            if (PluginConfig.Shovel2.Value)
                            {
                                KickPlayer(p);
                            }
                            return false;
                        }
                    }
                    else if (!p.isPlayerDead && obj == null)
                    {
                        if (ClingTime.ContainsKey(p.playerSteamId) && ClingTime[p.playerSteamId].AddSeconds(5) > DateTime.Now)
                        {
                            return true;
                        }
                        if (p.ItemSlots.Any(x => isShovel(x)) && force == 1)
                        {
                            return true;
                        }
                        if (!jcs.Contains(p.playerSteamId))
                        {
                            for (int i = 0; i < p.ItemSlots.Length; i++)
                            {
                                LogInfo($"p:{p.playerUsername}|i:{i}|itemName:{p.ItemSlots[i]?.itemProperties?.itemName}");
                            }
                            LogInfo($"currentItemSlot:{p.currentItemSlot}");
                            LogInfo($"currentlyHeldObjectServer:{p.currentlyHeldObjectServer?.itemProperties?.itemName}");
                            LogInfo($"obj:{obj}");
                            if (!PluginConfig.Shovel3.Value && (force == 1 || force == 2 || force == 3 || force == 5))
                            {
                                ShowMessage(locale.Msg_GetString("Shovel6", new Dictionary<string, string>() {
                                    { "{player}",p.playerUsername },
                                    { "{enemyName}",e.enemyType.enemyName },
                                    { "{damageAmount}",force.ToString() }
                                }));
                                jcs.Add(p.playerSteamId);
                                if (PluginConfig.Shovel2.Value)
                                {
                                    KickPlayer(p);
                                }
                            }
                            else
                            {
                                ShowMessage(locale.Msg_GetString("Shovel6", new Dictionary<string, string>() {
                                    { "{player}",p.playerUsername },
                                    { "{enemyName}",e.enemyType.enemyName },
                                    { "{damageAmount}",force.ToString() }
                                }));
                            }
                            return false;
                        }
                    }
                    else if (jcs.Contains(p.playerSteamId))
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
