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
    [HarmonyPatch(typeof(PlayerControllerB))]
    [HarmonyWrapSafe]
    public static class PlayerControllerBMigrationPatch
    {

        /// <summary>
        /// PlayerControllerB.KillPlayerServerRpc
        /// </summary>
        /// <param name="target"></param>
        /// <param name="reader"></param>
        /// <param name="rpcParams"></param>
        /// <returns></returns>
        [HarmonyPatch("__rpc_handler_4121569671")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static bool __rpc_handler_4121569671(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (Check(rpcParams, out var p))
            {
                if (rpcs.ContainsKey("KillPlayer"))
                {
                    rpcs["KillPlayer"].Remove(p.playerClientId);
                }
                ByteUnpacker.ReadValueBitPacked(reader, out int playerId);
                reader.ReadValueSafe(out bool spawnBody, default);
                reader.ReadValueSafe(out Vector3 bodyVelocity);
                ByteUnpacker.ReadValueBitPacked(reader, out int num);
                reader.Seek(0);
                LogInfo(p, "PlayerControllerB.KillPlayerServerRpc", $"playerId:{PlayerClientIdConvertName(playerId)}({playerId})", $"spawnBody:{spawnBody}", $"bodyVelocity:{bodyVelocity}", $"num:{(CauseOfDeath)num}({num})");
                if (playerId < 0) 
                {
                    LogInfo($"KillPlayerServerRpc:Invalid PlayerId({playerId})");
                    return false;
                }
                if (StartOfRound.Instance.allPlayerScripts[playerId] != p)
                {
                    LogInfo("KillPlayerServerRpc:Can't kill other player!");
                    return false;
                }
                if (p.isPlayerDead)
                {
                    LogInfo("KillPlayerServerRpc:Player death can't kill!");
                    return false;
                }
                if ((CauseOfDeath)num == CauseOfDeath.Abandoned)
                {
                    string msg = locale.Msg_GetString("behind_player", new Dictionary<string, string>() {
                        { "{player}",p.playerUsername }
                    });
                    LogInfo(msg);
                    AddTextMessageClientRpc(msg);
                }
            }
            else if (p == null)
            {
                return false;
            }
            return true;
        }



        /// <summary>
        /// Prefix PlayerControllerB.DamagePlayerFromOtherClientServerRpc
        /// </summary>
        [HarmonyPatch("__rpc_handler_638895557")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static bool __rpc_handler_638895557(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (Check(rpcParams, out var p))
            {
                ByteUnpacker.ReadValueBitPacked(reader, out int damageAmount);
                reader.ReadValueSafe(out Vector3 hitDirection);
                ByteUnpacker.ReadValueBitPacked(reader, out int playerWhoHit);
                reader.Seek(0);
                LogInfo(p, "PlayerControllerB.DamagePlayerFromOtherClientServerRpc", $"damageAmount:{damageAmount}", $"hitDirection:{hitDirection}", $"playerWhoHit:{PlayerClientIdConvertName(playerWhoHit)}({playerWhoHit})");
                var p2 = (PlayerControllerB)target;
                return CheckDamage(p2, p, ref damageAmount);
            }
            else if (p == null)
            {
                return false;
            }
            return true;
        }


        /// <summary>
        /// Prefix PlayerControllerB.HealServerRpc
        /// </summary>
        [HarmonyPatch("__rpc_handler_2585603452")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static bool __rpc_handler_2585603452(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (Check(rpcParams, out var p))
            {
                LogInfo(p, "PlayerControllerB.HealServerRpc", $"health:{p.health}", $"newHealth:20");
                p.health = 20;
            }
            else if (p == null)
            {
                return false;
            }
            return true;
        }


        /// <summary>
        /// Prefix PlayerControllerB.DamagePlayer
        /// </summary>
        /// <returns></returns>
        [HarmonyPatch("DamagePlayer")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static bool DamagePlayer(PlayerControllerB __instance, int damageNumber, bool hasDamageSFX = true, bool callRPC = true, CauseOfDeath causeOfDeath = CauseOfDeath.Unknown, int deathAnimation = 0, bool fallDamage = false, Vector3 force = default(Vector3))
        {
            //LogInfo($"PlayerControllerB.DamagePlayer|{__instance.playerUsername}|damageNumber:{damageNumber}|hasDamageSFX:{hasDamageSFX}|callRPC:{callRPC}");
            return true;
        }



        /// <summary>
        /// Prefix PlayerControllerB.DamagePlayerServerRpc
        /// </summary>
        /// <returns></returns>
        [HarmonyPatch("__rpc_handler_1084949295")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static bool __rpc_handler_1084949295(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (Check(rpcParams, out var p) || StartOfRound.Instance.IsHost)
            {
                if (rpcs.ContainsKey("Hit"))
                {
                    rpcs["Hit"].Remove(p.playerClientId);
                }
                ByteUnpacker.ReadValueBitPacked(reader, out int damageNumber);
                ByteUnpacker.ReadValueBitPacked(reader, out int newHealthAmount);
                reader.Seek(0);
                LogInfo(p, "PlayerControllerB.DamagePlayerServerRpc", $"damageNumber:{damageNumber}", $"newHealthAmount:{newHealthAmount}");
                var p2 = (PlayerControllerB)target;
                if (p2 == p)
                {
                    if (PluginConfig.Health_Recover.Value)
                    {
                        if (damageNumber < 0)
                        {
                            string msg = locale.Msg_GetString("Health_Recover", new Dictionary<string, string>() {
                                { "{player}", p.playerUsername },
                                { "{hp}", (damageNumber * -1).ToString() }
                            });
                            ShowMessage(msg);
                            if (PluginConfig.Health_Kick.Value)
                            {
                                KickPlayer(p);
                            }
                            return false;
                        }
                    }
                    return true;
                }
                return CheckDamage(p2, p, ref damageNumber);
            }
            else if (p == null)
            {
                return false;
            }
            return true;
        }


        /// <summary>
        /// 玩家切换格子事件(拿了双手物品无法切换格子，他们本地客户端依旧可以)
        /// Prefix PlayerControllerB.SwitchItemSlotsServerRpc
        /// </summary>
        [HarmonyPatch("__rpc_handler_412259855")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static bool __rpc_handler_412259855(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (Check(rpcParams, out var p))
            {

                if (p.currentlyHeldObjectServer != null && p.currentlyHeldObjectServer.itemProperties.twoHanded)
                {
                    return false;
                }

                return true;
            }
            else if (p == null)
            {
                return false;
            }
            return true;
        }


        /// <summary>
        /// 玩家捡起物品事件(用于检测多格子，单手拿双手物品，隔空取物)
        /// Prefix PlayerControllerB.GrabObjectServerRpc
        /// </summary>
        [HarmonyPatch("__rpc_handler_1554282707")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static bool __rpc_handler_1554282707(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (Check(rpcParams, out var p))
            {
                if (PluginConfig.GrabObject.Value)
                {
                    reader.ReadValueSafe(out NetworkObjectReference grabbedObject, default);
                    reader.Seek(0);
                    var jetpack = false;
                    if (grabbedObject.TryGet(out var networkObject, null))
                    {
                        var all = true;
                        bool hastwohand = false;
                        foreach (var item in p.ItemSlots)
                        {
                            if (item == null)
                            {
                                all = false;
                            }
                            else if (item.itemProperties.twoHanded)
                            {
                                hastwohand = true;
                            }
                            else if (item is JetpackItem)
                            {
                                jetpack = true;
                            }
                        }
                        var g = networkObject.GetComponentInChildren<GrabbableObject>();
                        if (g != null)
                        {
                            LogInfo(p, "PlayerControllerB.GrabObjectServerRpc", $"itemName:{g.itemProperties.itemName}", $"heldByPlayerOnServer:{(g.heldByPlayerOnServer ? g.playerHeldBy?.playerUsername : "false")}", $"Distance:{Vector3.Distance(p.transform.position, g.transform.position)}");
                            bool ban = false;
                            if (PluginConfig.GrabObject_TwoHand.Value)
                            {
                                if (g.itemProperties.twoHanded && hastwohand)
                                {
                                    ban = true;
                                }
                                else if (g.itemProperties.twoHanded && jetpack)
                                {
                                    ban = true;
                                }
                                else if (hastwohand && jetpack)
                                {
                                    ban = true;
                                }
                            }
                            if (PluginConfig.GrabObject_MoreSlot.Value && !ban)
                            {
                                ban = all;
                            }
                            if (ban)
                            {
                                var __rpc_exec_stage = typeof(NetworkBehaviour).GetField("__rpc_exec_stage", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                                __rpc_exec_stage.SetValue(target, 1);
                                typeof(PlayerControllerB).GetMethod("GrabObjectServerRpc", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).Invoke((PlayerControllerB)target, new object[] {
                                    default
                                });
                                __rpc_exec_stage.SetValue(target, 0);
                                return false;
                            }
                            if (Vector3.Distance(g.transform.position, p.serverPlayerPosition) > 100 && !StartOfRound.Instance.shipIsLeaving && StartOfRound.Instance.shipHasLanded)
                            {
                                if (p.teleportedLastFrame)
                                {
                                    return true;
                                }
                                ShowMessage(locale.Msg_GetString("GrabObject", new Dictionary<string, string>() {
                                    { "{player}",p.playerUsername },
                                    { "{object_position}",g.transform.position.ToString() },
                                    { "{player_position}",p.serverPlayerPosition.ToString() }
                                }));
                                g = default;
                                grabbedObject = default;
                                if (PluginConfig.GrabObject_MoreSlot.Value)
                                {
                                    KickPlayer(p);
                                }
                                var __rpc_exec_stage = typeof(NetworkBehaviour).GetField("__rpc_exec_stage", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                                __rpc_exec_stage.SetValue(target, 1);
                                typeof(PlayerControllerB).GetMethod("GrabObjectServerRpc", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).Invoke((PlayerControllerB)target, new object[] {
                                    default
                                });
                                __rpc_exec_stage.SetValue(target, 0);
                                return false;
                            }
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



        ///// <summary>
        ///// 玩家跳跃
        ///// Prefix PlayerControllerB.PlayerJumpedServerRpc
        ///// </summary>
        //[HarmonyPatch("__rpc_handler_2013428264")]
        //[HarmonyPrefix]
        //[HarmonyWrapSafe]
        //public static bool __rpc_handler_420292904(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        //{
        //    if (Check(rpcParams, out var p))
        //    {
        //        if (!PlayerJumping.ContainsKey(p))
        //        {
        //            PlayerJumping.Add(p, false);
        //        }
        //        if (!PlayerJumping[p])
        //        {
        //            p.StartCoroutine(PlayerJump(p));
        //        }
        //    }
        //    else if (p == null)
        //    {
        //        return false;
        //    }
        //    return true;
        //}

        //public static Dictionary<PlayerControllerB, bool> PlayerJumping { get; set; } = new Dictionary<PlayerControllerB, bool>();

        //public static IEnumerator PlayerJump(PlayerControllerB p)
        //{

        //    LogInfo($"{p.playerUsername} is Jumping");
        //    PlayerJumping[p] = true;
        //    yield return new WaitForSeconds(0.25f);
        //    LogInfo($"{p.playerUsername} wait isGrounded");
        //    yield return new WaitUntil(() => Physics.Raycast(p.transform.position, Vector3.down, out var raycastHit, 80f, 268437760, QueryTriggerInteraction.Ignore) && raycastHit.distance < 0.1);
        //    LogInfo($"{p.playerUsername} is Grounded");
        //    PlayerJumping[p] = false;
        //}



        /// <summary>
        /// 玩家坐标变动事件(玩家如果隐身会将本体传送到一个很远的位置，例如当前坐标-100)
        /// Prefix PlayerControllerB.UpdatePlayerPositionServerRpc
        /// </summary>
        [HarmonyPatch(typeof(PlayerControllerB), "__rpc_handler_2013428264")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static bool __rpc_handler_2013428264(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            reader.ReadValueSafe(out Vector3 newPos);
            reader.Seek(0);
            if (Check(rpcParams, out var p))
            {
                if (PluginConfig.Invisibility.Value)
                {
                    if (StartOfRound.Instance.currentLevel != null && StartOfRound.Instance.currentLevel.spawnEnemiesAndScrap)
                    {
                        if (!recentPlayerPositions.ContainsKey(p.playerSteamId))
                            recentPlayerPositions[p.playerSteamId] = new List<(Vector3, float)>();
                        float now = Time.time;
                        recentPlayerPositions[p.playerSteamId] = recentPlayerPositions[p.playerSteamId]
                            .Where(entry => now - entry.time <= 5f)
                            .ToList();
                        recentPlayerPositions[p.playerSteamId].Add((newPos, now));
                    }
                    var oldpos = p.serverPlayerPosition;
                    if (p.teleportedLastFrame)
                    {
                        return true;
                    }
                    if (Vector3.Distance(oldpos, newPos) > 100 && Vector3.Distance(newPos, new Vector3(0, 0, 0)) > 10)
                    {
                        ShowMessage(locale.Msg_GetString("Invisibility", new Dictionary<string, string>() {
                            { "{player}",p.playerUsername },
                            { "{player_position}",newPos.ToString() }
                        }));
                        if (PluginConfig.Invisibility2.Value)
                        {
                            KickPlayer(p);
                        }
                        return false;
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
        /// 玩家销毁物品事件(允许销毁礼物盒和钥匙)
        /// Prefix PlayerControllerB.DespawnHeldObjectServerRpc
        /// </summary>
        [HarmonyPatch("__rpc_handler_1786952262")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static bool __rpc_handler_1786952262(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (Check(rpcParams, out var p))
            {
                if (PluginConfig.DespawnItem.Value)
                {
                    LogInfo(p, "PlayerControllerB.DespawnHeldObjectServerRpc", $"itemName:{p.currentlyHeldObjectServer.itemProperties.itemName}");
                    if (p.currentlyHeldObjectServer != null && !(p.currentlyHeldObjectServer is GiftBoxItem) && !(p.currentlyHeldObjectServer is KeyItem))
                    {
                        ShowMessage(locale.Msg_GetString("DespawnItem", new Dictionary<string, string>() {
                            { "{player}",p.playerUsername },
                            { "{item}",p.currentlyHeldObjectServer.itemProperties.itemName }
                        }));
                        if (PluginConfig.DespawnItem2.Value)
                        {
                            KickPlayer(p);
                        }
                        return false;
                    }
                }
            }
            else if (p == null)
            {
                return false;
            }
            return true;
        }
    }
}
