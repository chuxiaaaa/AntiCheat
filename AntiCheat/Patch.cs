﻿using BepInEx.Configuration;
using GameNetcodeStuff;
using HarmonyLib;
using Mono.Cecil;
using Steamworks;
using Steamworks.Ugc;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.EventSystems.EventTrigger;
using static UnityEngine.GraphicsBuffer;

namespace AntiCheat
{
    public static class Patch
    {
        public static List<ulong> jcs = new List<ulong>();

        public static List<long> zdcd = new List<long>();
        public static List<long> dgcd = new List<long>();
        public static Dictionary<ulong, List<string>> czcd = new Dictionary<ulong, List<string>>();
        public static Dictionary<ulong, List<string>> sdqcd = new Dictionary<ulong, List<string>>();
        public static List<long> mjs { get; set; } = new List<long>();
        public static List<long> lhs { get; set; } = new List<long>();


        [HarmonyPatch(typeof(PlayerControllerB), "__rpc_handler_638895557")]
        [HarmonyPrefix]
        public static bool __rpc_handler_638895557(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (Check(rpcParams, out var p))
            {
                try
                {
                    AntiCheatPlugin.ManualLog.LogInfo("__rpc_handler_638895557");
                    int damageAmount;
                    ByteUnpacker.ReadValueBitPacked(reader, out damageAmount);
                    reader.Seek(0);
                    var p2 = (PlayerControllerB)target;
                    return CheckDamage(p2, p, ref damageAmount);
                }
                catch (Exception)
                {

                }
            }
            else if (p == null)
            {
                return false;
            }
            return true;
        }

        private static bool CheckDamage(PlayerControllerB p2, PlayerControllerB p, ref int damageAmount)
        {
            if (damageAmount == 0)
            {
                return true;
            }
            AntiCheatPlugin.ManualLog.LogInfo("damageAmount:" + damageAmount);
            try
            {
                if (AntiCheatPlugin.ShovelConfig.Value)
                {
                    var distance = Vector3.Distance(p.transform.position, p2.transform.position);
                    var obj = p.ItemSlots[p.currentItemSlot];
                    string playerUsername = p.playerUsername;
                    if (damageAmount != 10 && obj != null && isShovel(obj))
                    {
                        if (!jcs.Contains(p.playerSteamId))
                        {
                            ShowMessage("检测到玩家 " + playerUsername + " 对玩家 " + p2.playerUsername + " 造成异常铲子伤害(" + damageAmount + ")！");
                            jcs.Add(p.playerSteamId);
                            if (AntiCheatPlugin.ShovelConfig2.Value)
                            {
                                KickPlayer(p);
                            }
                        }
                        damageAmount = 0;
                    }
                    else if (distance > 6 && obj != null && isShovel(obj))
                    {
                        if (!jcs.Contains(p.playerSteamId))
                        {
                            ShowMessage("检测到玩家 " + playerUsername + " 对玩家 " + p2.playerUsername + " 铲子范围异常，造成伤害(" + damageAmount + ")！");
                            jcs.Add(p.playerSteamId);
                            if (AntiCheatPlugin.ShovelConfig2.Value)
                            {
                                KickPlayer(p);
                            }
                        }
                        damageAmount = 0;
                    }
                    else if (obj == null || (!isGun(obj) && !isShovel(obj)))
                    {
                        if (damageAmount != 10)
                        {
                            if (!jcs.Contains(p.playerSteamId))
                            {

                                ShowMessage("检测到玩家 " + playerUsername + " 对玩家 " + p2.playerUsername + " 造成异常伤害(" + damageAmount + ")！");
                                jcs.Add(p.playerSteamId);
                                if (AntiCheatPlugin.ShovelConfig2.Value)
                                {
                                    KickPlayer(p);
                                }
                            }
                        }
                        damageAmount = 0;
                    }
                    else if (jcs.Contains(p.playerSteamId))
                    {
                        damageAmount = 0;
                    }
                    if (damageAmount == 0)
                    {
                        AntiCheatPlugin.ManualLog.LogInfo("115");
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                AntiCheatPlugin.ManualLog.LogInfo($"{ex.ToString()}");
            }
            return true;
        }


        //[HarmonyPatch(typeof(PlayerControllerB), "DamagePlayerFromOtherClientServerRpc")]
        //[HarmonyPrefix]
        //public static bool DamagePlayerFromOtherClientServerRpc(PlayerControllerB __instance, ref int damageAmount, Vector3 hitDirection, int playerWhoHit)
        //{
        //    return DamagePlayerFromOtherClientServerRpc(ref damageAmount, playerWhoHit);
        //}



        public static bool isGun(GrabbableObject item)
        {
            return item is ShotgunItem;
        }

        public static bool isShovel(GrabbableObject item)
        {
            return item is Shovel;
        }

        public static bool isJetpack(GrabbableObject item)
        {
            return item is JetpackItem;
        }

        public static ulong lastClientId { get; set; }

        [HarmonyPatch(typeof(StartOfRound), "StartTrackingAllPlayerVoices")]
        [HarmonyPostfix]
        public static void StartTrackingAllPlayerVoices()
        {
            if (!StartOfRound.Instance.localPlayerController.IsHost)
            {
                return;
            }
            foreach (var item in StartOfRound.Instance.allPlayerScripts)
            {
                if (!item.isPlayerControlled)
                {
                    continue;
                }
                var playerName = item.playerUsername;
                if (playerName == "Player #0")
                {
                    continue;
                }
                if (StartOfRound.Instance.KickedClientIds.Contains(item.playerSteamId))
                {
                    KickPlayer(item);
                    return;
                }
                AntiCheatPlugin.ManualLog.LogInfo(playerName);
                if (Regex.IsMatch(playerName, "Nameless\\d*") || Regex.IsMatch(playerName, "Unknown\\d*") || Regex.IsMatch(playerName, "Player #\\d*"))
                {
                    if (AntiCheatPlugin.NamelessConfig.Value)
                    {
                        ShowMessage("检测到玩家名称异常！");
                        if (AntiCheatPlugin.NamelessConfig2.Value)
                        {
                            KickPlayer(item, true);
                        }
                    }
                }
            }
            var p2 = StartOfRound.Instance.allPlayerScripts.OrderByDescending(x => x.actualClientId).FirstOrDefault();
            if (p2.actualClientId != lastClientId)
            {
                if (p2.isPlayerControlled && p2.playerSteamId == 0)
                {
                    KickPlayer(p2);
                    return;
                }
                else if (!p2.isPlayerControlled)
                {
                    return;
                }
                bypass = true;
                AntiCheatPlugin.ManualLog.LogInfo($"<color=green>欢迎 <color=yellow>{p2.playerUsername}</color> 加入飞船</color>");
                HUDManager.Instance.AddTextToChatOnServer($"<color=green>欢迎 <color=yellow>{p2.playerUsername}</color> 加入飞船</color>", -1);
                lastClientId = p2.actualClientId;
                bypass = false;
            }
        }

        [HarmonyPatch(typeof(HUDManager), "__rpc_handler_1944155956")]
        [HarmonyPrefix]
        public static bool __rpc_handler_1944155956(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (Check(rpcParams, out var p))
            {
                AntiCheatPlugin.ManualLog.LogInfo($"<color=yellow>[{p.playerUsername}] <color=green>新生物数据发送到终端！</color></color>");
                HUDManager.Instance.AddTextToChatOnServer($"<color=yellow>[{p.playerUsername}] <color=green>新生物数据发送到终端！</color></color>", -1);
            }
            else if (p == null)
            {
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(GameNetworkManager), "SteamMatchmaking_OnLobbyMemberJoined")]
        [HarmonyPostfix]
        public static void SteamMatchmaking_OnLobbyMemberJoined()
        {
            if (!StartOfRound.Instance.localPlayerController.IsHost)
            {
                return;
            }
            AntiCheatPlugin.ManualLog.LogInfo($"SetMoney:{Money}");
            Money = UnityEngine.Object.FindObjectOfType<Terminal>().groupCredits;
            jcs = new List<ulong>();
        }

        public static int Money = -1;

        [HarmonyPatch(typeof(StartOfRound), "__rpc_handler_3953483456")]
        [HarmonyPrefix]
        public static bool __rpc_handler_3953483456(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (Check(rpcParams, out var p))
            {
                if (AntiCheatPlugin.FreeBuyConfig.Value)
                {
                    int unlockableID;
                    ByteUnpacker.ReadValueBitPacked(reader, out unlockableID);
                    int newGroupCreditsAmount;
                    ByteUnpacker.ReadValueBitPacked(reader, out newGroupCreditsAmount);
                    reader.Seek(0);
                    AntiCheatPlugin.ManualLog.LogInfo($"__rpc_handler_3953483456|newGroupCreditsAmount:{newGroupCreditsAmount}");
                    if (Money == newGroupCreditsAmount || Money == 0)
                    {
                        var pl = GetPlayer(rpcParams);
                        AntiCheatPlugin.ManualLog.LogInfo($"Money:{Money}|newGroupCreditsAmount:{newGroupCreditsAmount}");
                        ShowMessage($"检测到玩家 {pl.playerUsername} 强制解锁飞船装饰！");
                        if (AntiCheatPlugin.FreeBuyConfig2.Value)
                        {
                            KickPlayer(pl);
                        }
                        return false;
                    }
                    else if (newGroupCreditsAmount > Money)
                    {
                        ShowMessage($"检测到玩家 {p.playerUsername} 刷钱！");
                        if (AntiCheatPlugin.FreeBuyConfig2.Value)
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





        [HarmonyPatch(typeof(StartOfRound), "BuyShipUnlockableClientRpc")]
        [HarmonyPrefix]
        public static bool BuyShipUnlockableClientRpc(int newGroupCreditsAmount, int unlockableID = -1)
        {
            if (!StartOfRound.Instance.localPlayerController.IsHost)
            {
                return true;
            }
            Money = newGroupCreditsAmount;
            return true;
        }

        [HarmonyPatch(typeof(Terminal), "__rpc_handler_4003509079")]
        [HarmonyPrefix]
        public static bool __rpc_handler_4003509079(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (Check(rpcParams, out var p))
            {
                if (AntiCheatPlugin.FreeBuyConfig.Value)
                {
                    reader.ReadValueSafe(out bool flag, default);
                    int[] boughtItems = null;
                    if (flag)
                    {
                        reader.ReadValueSafe(out boughtItems, default);
                    }
                    ByteUnpacker.ReadValueBitPacked(reader, out int newGroupCredits);
                    reader.Seek(0);
                    AntiCheatPlugin.ManualLog.LogInfo("__rpc_handler_4003509079|boughtItems:" + string.Join(",", boughtItems) + "|newGroupCredits:" + newGroupCredits + "|Money:" + Money);
                    if (Money == newGroupCredits || Money == 0)
                    {
                        ShowMessage($"检测到玩家 {p.playerUsername} 强制购买物品！");
                        if (AntiCheatPlugin.FreeBuyConfig2.Value)
                        {
                            KickPlayer(p);
                        }
                        return false;
                    }
                    else if (newGroupCredits > Money)
                    {
                        ShowMessage($"检测到玩家 {p.playerUsername} 刷钱！");
                        if (AntiCheatPlugin.FreeBuyConfig2.Value)
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

        [HarmonyPatch(typeof(Terminal), "SyncGroupCreditsClientRpc")]
        [HarmonyPrefix]
        public static bool SyncGroupCreditsClientRpc(int newGroupCredits, int numItemsInShip)
        {
            if (!StartOfRound.Instance.localPlayerController.IsHost)
            {
                return true;
            }
            Money = newGroupCredits;
            return true;
        }

        [HarmonyPatch(typeof(StartOfRound), "ChangeLevelClientRpc")]
        [HarmonyPrefix]
        public static bool ChangeLevelClientRpc(int levelID, int newGroupCreditsAmount)
        {
            if (!StartOfRound.Instance.localPlayerController.IsHost)
            {
                return true;
            }
            Money = newGroupCreditsAmount;
            return true;
        }


        [HarmonyPatch(typeof(Terminal), "BeginUsingTerminal")]
        [HarmonyPrefix]
        public static bool BeginUsingTerminal(Terminal __instance)
        {
            Money = __instance.groupCredits;
            AntiCheatPlugin.ManualLog.LogInfo($"SetMoney:{Money}");
            return true;
        }



        [HarmonyPatch(typeof(PlayerControllerB), "__rpc_handler_1084949295")]
        [HarmonyPrefix]
        public static bool __rpc_handler_1084949295(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (Check(rpcParams, out var p))
            {
                ByteUnpacker.ReadValueBitPacked(reader, out int damageNumber);
                ByteUnpacker.ReadValueBitPacked(reader, out int newHealthAmount);
                reader.Seek(0);
                var p2 = (PlayerControllerB)target;
                if (p2 == p)
                {
                    return true;
                }
                return CheckDamage(p2, p, ref damageNumber);
            }
            return true;
        }

        [HarmonyPatch(typeof(EnemyAI), "__rpc_handler_1810146992")]
        [HarmonyPrefix]
        public static bool __rpc_handler_1810146992(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (Check(rpcParams, out var p))
            {
                AntiCheatPlugin.ManualLog.LogInfo($"{p.playerUsername} call EnemyAI.KillEnemyServerRpc");
                if (AntiCheatPlugin.KillEnemy.Value)
                {
                    var e = (EnemyAI)target;
                    if (e.enemyHP <= 0)
                    {
                        return true;
                    }
                    if (Vector3.Distance(p.transform.position, e.transform.position) > 50)
                    {
                        ShowMessage($"检测到玩家 {p.playerUsername} 秒杀怪物( {e.enemyType.enemyName} ,剩余HP:{e.enemyHP})！");
                        if (AntiCheatPlugin.KillEnemy2.Value)
                        {
                            KickPlayer(p);
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        [HarmonyPatch(typeof(EnemyAI), "__rpc_handler_3079913705")]
        [HarmonyPrefix]
        public static bool __rpc_handler_3079913705(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (Check(rpcParams, out var p))
            {
                if (AntiCheatPlugin.Enemy.Value)
                {
                    AntiCheatPlugin.ManualLog.LogInfo($"{p.playerUsername} call EnemyAI.UpdateEnemyRotationServerRpc");
                    return true;
                }
            }
            return true;
        }

        [HarmonyPatch(typeof(EnemyAI), "__rpc_handler_255411420")]
        [HarmonyPrefix]
        public static bool __rpc_handler_255411420(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (Check(rpcParams, out var p))
            {
                if (AntiCheatPlugin.Enemy.Value)
                {
                    AntiCheatPlugin.ManualLog.LogInfo($"{p.playerUsername} call EnemyAI.UpdateEnemyPositionServerRpc");
                    return true;
                }
            }
            else if (p == null)
            {
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(EnemyAI), "__rpc_handler_3587030867")]
        [HarmonyPrefix]
        public static bool __rpc_handler_3587030867(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            AntiCheatPlugin.ManualLog.LogInfo("__rpc_handler_3587030867");
            if (Check(rpcParams, out var p))
            {
                if (AntiCheatPlugin.Enemy.Value)
                {
                    ByteUnpacker.ReadValueBitPacked(reader, out int clientId);
                    reader.Seek(0);
                    var enemy = target.GetComponent<EnemyAI>();
                    float v = Vector3.Distance(p.transform.position, enemy.transform.position);
                    AntiCheatPlugin.ManualLog.LogInfo($"Distance:{v}");
                    AntiCheatPlugin.ManualLog.LogInfo($"{p.playerUsername} called ChangeOwnershipOfEnemy|enemy:{enemy.enemyType.enemyName}|clientId:{clientId}");
                    if (enemy.OwnerClientId == (ulong)clientId)
                    {
                        ShowMessage($"检测到玩家 {p.playerUsername} 强制改变怪物仇恨！");
                        KickPlayer(p);
                        return false;
                    }
                    if (enemy.isEnemyDead)
                    {
                        return false;
                    }
                    else if (enemy is CrawlerAI c)
                    {
                        if (c.stunnedByPlayer != null)
                        {
                            if ((int)c.stunnedByPlayer.actualClientId != clientId)
                            {
                                return false;
                            }
                        }
                        else
                        {
                            var p2 = enemy.CheckLineOfSightForPlayer(55f, 60, -1);
                            if (p2 != null && (int)p2.actualClientId != clientId)
                            {
                                return false;
                            }

                        }
                    }
                    else if (enemy is JesterAI j)
                    {
                        if (j.currentBehaviourStateIndex != 2)
                        {
                            AntiCheatPlugin.ManualLog.LogInfo("client ChangeOwnershipOfEnemy:" + clientId);
                            return p.isHostPlayerObject;
                        }
                        else
                        {
                            if (j.TargetClosestPlayer(4f, false, 70f) && j.targetPlayer != null && (int)j.targetPlayer.actualClientId == clientId)
                            {
                                return true;
                            }
                            return false;
                        }
                    }
                    else if (enemy is FlowermanAI f)
                    {
                        if (f.currentBehaviourStateIndex != 2)
                        {
                            AntiCheatPlugin.ManualLog.LogInfo("client ChangeOwnershipOfEnemy:" + clientId);
                            return p.isHostPlayerObject;
                        }
                        else
                        {
                            if (f.TargetClosestPlayer(1.5f, false, 70f) && f.targetPlayer != null && (int)f.targetPlayer.actualClientId == clientId)
                            {
                                return true;
                            }
                            return false;
                        }
                    }
                    else if (enemy is RedLocustBees r)
                    {
                        var p2 = r.CheckLineOfSightForPlayer(360f, 16, 1);
                        if (p2 != null && Vector3.Distance(p2.transform.position, r.hive.transform.position) < (float)r.defenseDistance && (int)r.targetPlayer.actualClientId == clientId)
                        {
                            return true;
                        }
                        return false;
                    }
                    return true;
                }
            }
            else if (p == null)
            {
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(JetpackItem), "__rpc_handler_3663112878")]
        [HarmonyPrefix]
        public static bool __rpc_handler_3663112878(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (Check(rpcParams, out var p))
            {
                if (AntiCheatPlugin.Jetpack.Value)
                {
                    var jp = (JetpackItem)target;
                    if (jp.playerHeldBy != null && jp.playerHeldBy.actualClientId != p.actualClientId)
                    {
                        ShowMessage($"检测到 {p.playerUsername} 引爆喷气背包！");
                        if (AntiCheatPlugin.Jetpack2.Value)
                        {
                            KickPlayer(p);
                        }
                        return false;
                    }
                    else if (jp.playerHeldBy == null)
                    {
                        ShowMessage($"检测到 {p.playerUsername} 引爆喷气背包！");
                        if (AntiCheatPlugin.Jetpack2.Value)
                        {
                            KickPlayer(p);
                        }
                        return false;
                    }
                    return true;
                }
            }
            else if (p == null)
            {
                return false;
            }
            return true;
        }


        [HarmonyPatch(typeof(EnemyAI), "__rpc_handler_2814283679")]
        [HarmonyPrefix]
        public static bool HitEnemyServerRpcPatch(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (Check(rpcParams, out var p) || true)
            {
                if (p == null)
                {
                    return false;
                }
                AntiCheatPlugin.ManualLog.LogInfo("__rpc_handler_2814283679");
                int force;
                ByteUnpacker.ReadValueBitPacked(reader, out force);
                ByteUnpacker.ReadValueBitPacked(reader, out int playerWhoHit);
                reader.ReadValueSafe(out bool playHitSFX, default);
                reader.Seek(0);
                AntiCheatPlugin.ManualLog.LogInfo("force:" + force);
                AntiCheatPlugin.ManualLog.LogInfo("playerWhoHit:" + playerWhoHit);
                if (p.isHostPlayerObject)
                {
                    return true;
                }
                if (AntiCheatPlugin.ShovelConfig.Value)
                {
                    var obj = p.ItemSlots[p.currentItemSlot];
                    string playerUsername = p.playerUsername;
                    if (force != 1 && obj != null && isShovel(obj))
                    {
                        if (!jcs.Contains(p.playerSteamId))
                        {
                            var e = (EnemyAI)target;
                            ShowMessage("检测到玩家 " + playerUsername + " 对敌人 " + e.enemyType + " 造成异常铲子伤害(" + force + ")！");
                            jcs.Add(p.playerSteamId);
                            if (AntiCheatPlugin.ShovelConfig2.Value)
                            {
                                KickPlayer(p);
                            }
                            return false;
                        }
                    }
                    else if (!p.isPlayerDead && obj == null)
                    {
                        if (!jcs.Contains(p.playerSteamId))
                        {
                            var e = (EnemyAI)target;
                            ShowMessage("检测到玩家 " + playerUsername + " 对敌人 " + e.enemyType + " 造成异常伤害(" + force + ")！");
                            jcs.Add(p.playerSteamId);
                            if (AntiCheatPlugin.ShovelConfig2.Value)
                            {
                                KickPlayer(p);
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




        public static bool CheckSelf(PlayerControllerB p, int playerId)
        {
            if (playerId <= StartOfRound.Instance.allPlayerScripts.Length)
            {
                var p2 = StartOfRound.Instance.allPlayerScripts[playerId];
                if (p2.playerSteamId == p.playerSteamId)
                {
                    return true;
                }
            }
            return false;
        }



        private static string lastMessage = string.Empty;

        public static void ShowMessage(string msg, string lastmsg = null)
        {
            if (lastMessage == msg || lastmsg == lastMessage)
            {
                return;
            }
            if (lastmsg != null)
            {
                lastMessage = lastmsg;
            }
            else
            {
                lastMessage = msg;
            }
            bypass = true;
            AntiCheatPlugin.ManualLog.LogInfo($"<color=red>[反作弊] {msg}</color>");
            HUDManager.Instance.AddTextToChatOnServer($"<color=red>[反作弊] {msg}</color>", -1);
            bypass = false;
        }

        public static bool bypass { get; set; }

        [HarmonyPatch(typeof(GiftBoxItem), "__rpc_handler_2878544999")]
        [HarmonyPrefix]
        public static bool __rpc_handler_2878544999(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (Check(rpcParams, out var p))
            {
                if (AntiCheatPlugin.Gift.Value)
                {
                    if (lhs.Contains(target.GetInstanceID()))
                    {
                        ShowMessage($"检测到玩家 {p.playerUsername} 刷礼物盒");
                        if (AntiCheatPlugin.Gift2.Value)
                        {
                            KickPlayer(p);
                        }
                        return false;
                    }
                    else
                    {
                        lhs.Add(target.GetInstanceID());
                    }
                }
            }
            else if (p == null)
            {
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(HauntedMaskItem), "__rpc_handler_1065539967")]
        [HarmonyPrefix]
        public static bool __rpc_handler_1065539967(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (Check(rpcParams, out var p))
            {
                if (AntiCheatPlugin.Mask.Value)
                {
                    if (mjs.Contains(target.GetInstanceID()))
                    {
                        ShowMessage($"检测到玩家 {p.playerUsername} 刷假人");
                        if (AntiCheatPlugin.Mask2.Value)
                        {
                            KickPlayer(p);
                        }
                        return false;
                    }
                    else
                    {
                        mjs.Add(target.GetInstanceID());
                    }
                }
            }
            else if (p == null)
            {
                return false;
            }
            return true;
        }

        private static bool Check(__RpcParams rpcParams, out PlayerControllerB p)
        {
            if (StartOfRound.Instance.localPlayerController == null)
            {
                p = default;
                return false;
            }
            else if (!StartOfRound.Instance.localPlayerController.IsHost)//非主机
            {
                p = StartOfRound.Instance.localPlayerController;
                return false;
            }
            p = GetPlayer(rpcParams);
            if (p == null)//没玩家
            {
                NetworkManager.Singleton.DisconnectClient(rpcParams.Server.Receive.SenderClientId);
                return false;
            }
            else if (rpcParams.Server.Receive.SenderClientId == GameNetworkManager.Instance.localPlayerController.actualClientId)//非本地
            {
                p = StartOfRound.Instance.localPlayerController;
                return false;
            }
            if (StartOfRound.Instance.KickedClientIds.Contains(p.playerSteamId))//如果被踢
            {
                NetworkManager.Singleton.DisconnectClient(rpcParams.Server.Receive.SenderClientId);
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(RoundManager), "LoadNewLevel")]
        [HarmonyPrefix]
        public static bool LoadNewLevel(SelectableLevel newLevel)
        {
            if (!StartOfRound.Instance.localPlayerController.IsHost)//非主机
            {
                return true;
            }
            HUDManager.Instance.AddTextToChatOnServer("<color=green>本房间启用反作弊(v" + AntiCheatPlugin.Version + ")</color>", -1);
            return true;
        }

        [HarmonyPatch(typeof(PlayerControllerB), "__rpc_handler_1554282707")]
        [HarmonyPrefix]
        public static bool __rpc_handler_1554282707(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (Check(rpcParams, out var p))
            {
                if (AntiCheatPlugin.GrabObject.Value)
                {
                    reader.ReadValueSafe(out NetworkObjectReference grabbedObject, default);
                    reader.Seek(0);
                    if (grabbedObject.TryGet(out var networkObject, null))
                    {
                        var all = true;
                        foreach (var item in p.ItemSlots)
                        {
                            if (item == null)
                            {
                                all = false;
                            }
                        }
                        var g = networkObject.GetComponentInChildren<GrabbableObject>();
                        if (g != null)
                        {
                            if (all)
                            {
                                var __rpc_exec_stage = typeof(NetworkBehaviour).GetField("__rpc_exec_stage", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                                __rpc_exec_stage.SetValue(target, 1);
                                typeof(PlayerControllerB).GetMethod("GrabObjectServerRpc", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).Invoke((PlayerControllerB)target, new object[] {
                                    default
                                });
                                __rpc_exec_stage.SetValue(target, 0);
                                return false;
                            }
                            else if (Vector3.Distance(g.transform.position, p.serverPlayerPosition) > 100 && !StartOfRound.Instance.shipIsLeaving && StartOfRound.Instance.shipHasLanded)
                            {
                                if (p.teleportedLastFrame)
                                {
                                    return true;
                                }
                                ShowMessage($"检测到玩家 {p.playerUsername} 隔空取物{g.transform.position}|{p.serverPlayerPosition}！");
                                g = default;
                                grabbedObject = default;
                                if (AntiCheatPlugin.GrabObject2.Value)
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

        [HarmonyPatch(typeof(PlayerControllerB), "__rpc_handler_2013428264")]
        [HarmonyPrefix]
        public static bool __rpc_handler_2013428264(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (Check(rpcParams, out var p))
            {
                if (AntiCheatPlugin.Invisibility.Value)
                {
                    Vector3 newPos;
                    reader.ReadValueSafe(out newPos);
                    reader.Seek(0);
                    var oldpos = p.serverPlayerPosition;
                    if (p.teleportedLastFrame)
                    {
                        return true;
                    }
                    if (Vector3.Distance(oldpos, newPos) > 100 && Vector3.Distance(newPos, new Vector3(0, 0, 0)) > 10)
                    {
                        ShowMessage($"检测到玩家 {p.playerUsername} 坐标异常{newPos.ToString()}！");
                        if (AntiCheatPlugin.Invisibility2.Value)
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



        [HarmonyPatch(typeof(PlayerControllerB), "__rpc_handler_2504133785")]
        [HarmonyPrefix]
        public static bool __rpc_handler_2504133785(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (!StartOfRound.Instance.localPlayerController.IsHost)//非主机
            {
                return true;
            }
            ByteUnpacker.ReadValueBitPacked(reader, out ulong newPlayerSteamId);
            reader.Seek(0);
            if (StartOfRound.Instance.KickedClientIds.Contains(newPlayerSteamId))
            {
                AntiCheatPlugin.ManualLog.LogInfo($"{newPlayerSteamId} 尝试重连游戏被拒绝(因为他已被踢出)！");
                return false;
            }
            return true;
        }


        [HarmonyPatch(typeof(HUDManager), "__rpc_handler_2787681914")]
        [HarmonyPrefix]
        public static bool __rpc_handler_2787681914(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (Check(rpcParams, out var p))
            {
                bool flag;
                reader.ReadValueSafe<bool>(out flag, default(FastBufferWriter.ForPrimitives));
                string chatMessage = null;
                if (flag)
                {
                    reader.ReadValueSafe(out chatMessage, false);
                }
                reader.Seek(0);
                AntiCheatPlugin.ManualLog.LogInfo("__rpc_handler_2787681914:" + chatMessage);
                if (chatMessage.Contains("<color") || chatMessage.Contains("<size"))
                {
                    AntiCheatPlugin.ManualLog.LogInfo("playerId = -1");
                    if (chatMessage.Contains("<size=0>Tyzeron.Minimap"))
                    {
                        ShowMessage($"检测到玩家 {p.playerUsername} 使用小地图");
                        if (AntiCheatPlugin.Map2.Value)
                        {
                            KickPlayer(p);
                        }
                    }
                    else if (bypass)
                    {
                        AntiCheatPlugin.ManualLog.LogInfo("bypass");

                        return true;
                    }
                    return false;
                }
                else
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

        [HarmonyPatch(typeof(HUDManager), "__rpc_handler_168728662")]
        [HarmonyPrefix]
        public static bool __rpc_handler_168728662(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            AntiCheatPlugin.ManualLog.LogInfo("__rpc_handler_168728662");
            return __rpc_handler_2930587515(target, reader, rpcParams);
        }

        [HarmonyPatch(typeof(PlayerControllerB), "__rpc_handler_1786952262")]
        [HarmonyPrefix]
        public static bool __rpc_handler_1786952262(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (Check(rpcParams, out var p))
            {
                if (AntiCheatPlugin.DespawnItem.Value)
                {
                    if (p.currentlyHeldObjectServer != null && !(p.currentlyHeldObjectServer is GiftBoxItem) && !(p.currentlyHeldObjectServer is KeyItem))
                    {
                        ShowMessage($"检测到玩家 {p.playerUsername} 销毁物品！");
                        if (AntiCheatPlugin.DespawnItem2.Value)
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

        [HarmonyPatch(typeof(HUDManager), "__rpc_handler_2930587515")]
        [HarmonyPrefix]
        public static bool __rpc_handler_2930587515(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            AntiCheatPlugin.ManualLog.LogInfo("__rpc_handler_2930587515");
            if (Check(rpcParams, out var p) || true)
            {
                if (!StartOfRound.Instance.localPlayerController.IsHost)//非主机
                {
                    return true;
                }
                if (AntiCheatPlugin.ChatReal.Value)
                {
                    try
                    {
                        reader.ReadValueSafe(out bool flag, default);
                        string chatMessage = null;
                        if (flag)
                        {
                            reader.ReadValueSafe(out chatMessage, false);
                        }
                        ByteUnpacker.ReadValueBitPacked(reader, out int playerId);
                        reader.Seek(0);
                        AntiCheatPlugin.ManualLog.LogInfo(chatMessage);
                        if (playerId == -1)
                        {
                            AntiCheatPlugin.ManualLog.LogInfo("playerId = -1");
                            if (chatMessage.StartsWith("<color=red>[反作弊]") && bypass)
                            {
                                AntiCheatPlugin.ManualLog.LogInfo("bypass");
                                return true;
                            }
                            if (p == StartOfRound.Instance.localPlayerController)
                            {
                                return true;
                            }
                            return false;
                        }
                        if (p == StartOfRound.Instance.localPlayerController)
                        {
                            return true;
                        }
                        if (playerId <= StartOfRound.Instance.allPlayerScripts.Length)
                        {
                            if (StartOfRound.Instance.allPlayerScripts[(int)playerId].playerSteamId != p.playerSteamId)
                            {
                                ShowMessage($"{p.playerUsername} 尝试伪装成 {StartOfRound.Instance.allPlayerScripts[playerId].playerUsername} 发言！");
                                if (AntiCheatPlugin.ChatReal2.Value)
                                {
                                    KickPlayer(p);
                                }
                                return false;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        AntiCheatPlugin.ManualLog.LogInfo(ex.ToString());
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

        [HarmonyPatch(typeof(Terminal), "__rpc_handler_1713627637")]
        [HarmonyPrefix]
        public static bool __rpc_handler_1713627637(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (Check(rpcParams, out var p))
            {
                if (AntiCheatPlugin.ShipTerminal.Value)
                {
                    DateTime dt = DateTime.Now;
                    var m = dt.Ticks / 10000;
                    if (zdcd.Count > 200)
                    {
                        zdcd.RemoveRange(0, zdcd.Count - 1);
                    }
                    if (zdcd.Contains(m))
                    {
                        return false;
                    }
                    else if (zdcd.Any(x => x + 200 > m))
                    {
                        zdcd.Add(m);
                        ShowMessage($"{p.playerUsername} 频繁制造噪音！");
                        if (AntiCheatPlugin.ShipTerminal2.Value)
                        {
                            KickPlayer(p);
                        }
                        return false;
                    }
                    else
                    {
                        zdcd.Add(m);
                    }
                }
            }
            else if (p == null)
            {
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(ShipLights), "__rpc_handler_1625678258")]
        [HarmonyPrefix]
        public static bool __rpc_handler_1625678258(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (Check(rpcParams, out var p))
            {
                if (AntiCheatPlugin.ShipLight.Value)
                {

                    DateTime dt = DateTime.Now;
                    var m = dt.Ticks / 10000;
                    if (dgcd.Count > 200)
                    {
                        dgcd.RemoveRange(0, dgcd.Count - 1);
                    }
                    if (dgcd.Contains(m))
                    {
                        ShipLights shipLights = UnityEngine.Object.FindAnyObjectByType<ShipLights>();
                        if (!shipLights.areLightsOn)
                        {
                            shipLights.SetShipLightsServerRpc(true);
                        }
                        return false;
                    }
                    else if (dgcd.Any(x => x + 1000 > m))
                    {
                        dgcd.Add(m);
                        ShowMessage($"{p.playerUsername} 请勿频繁使用灯光！", "请勿频繁使用灯光");
                        ShipLights shipLights = UnityEngine.Object.FindAnyObjectByType<ShipLights>();
                        if (!shipLights.areLightsOn)
                        {
                            shipLights.SetShipLightsServerRpc(true);
                        }
                        return false;
                    }
                    else
                    {
                        dgcd.Add(m);
                    }
                }
            }
            else if (p == null)
            {
                return false;
            }
            return true;
        }

        private static PlayerControllerB GetPlayer(__RpcParams rpcParams)
        {
            foreach (var item in StartOfRound.Instance.allPlayerScripts)
            {
                if (item.actualClientId == rpcParams.Server.Receive.SenderClientId)
                {
                    return item;
                }
            }
            return null;//??
        }

        [HarmonyPatch(typeof(ShotgunItem), "__rpc_handler_1329927282")]
        [HarmonyPrefix]
        public static bool __rpc_handler_1329927282(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (Check(rpcParams, out var p) || true)
            {
                var id = p.playerSteamId;
                var m = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                if (!sdqcd.ContainsKey(id))
                {
                    sdqcd.Add(id, new List<string>());
                }
                if (sdqcd[id].Count > 200)
                {
                    sdqcd[id].RemoveRange(0, sdqcd[id].Count - 1);
                }
                if (sdqcd[id].Count(x => x == m) >= 2)
                {
                    ShowMessage($"玩家 {p.playerUsername} 使用霰弹枪频率异常！");
                    if (AntiCheatPlugin.ItemCooldown2.Value)
                    {
                        KickPlayer(p);
                    }
                    return false;
                }
                else
                {
                    sdqcd[id].Add(m);
                }
            }
            else if (p == null)
            {
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(Shovel), "__rpc_handler_2096026133")]
        [HarmonyPrefix]
        public static bool __rpc_handler_2096026133(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (Check(rpcParams, out var p) || true)
            {
                var id = p.playerSteamId;
                var m = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                if (!czcd.ContainsKey(id))
                {
                    czcd.Add(id, new List<string>());
                }
                if (czcd[id].Count > 200)
                {
                    czcd[id].RemoveRange(0, czcd[id].Count - 1);
                }
                if (czcd[id].Count(x => x == m) >= 3)
                {
                    ShowMessage($"玩家 {p.playerUsername} 使用铲子频率异常！");
                    if (AntiCheatPlugin.ItemCooldown2.Value)
                    {
                        KickPlayer(p);
                    }
                    return false;
                }
                else
                {
                    czcd[id].Add(m);
                }
            }
            else if (p == null)
            {
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(StartOfRound), "__rpc_handler_1089447320")]
        [HarmonyPrefix]
        public static bool StartGameServerRpc(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (Check(rpcParams, out var p))
            {
                if (AntiCheatPlugin.ShipConfig.Value && !GameNetworkManager.Instance.gameHasStarted)
                {
                    ShowMessage($"检测到玩家 {p.playerUsername} 强制拉杆");
                    if (AntiCheatPlugin.ShipConfig5.Value)
                    {
                        KickPlayer(p);
                        return false;
                    }
                    return false;
                }
                else if (StartOfRound.Instance.allPlayerScripts.Count(x => x.isPlayerControlled) >= AntiCheatPlugin.ShipConfig2.Value)
                {
                    return true;
                }
                else
                {
                    ShowMessage($"请等待飞船人数达到 {AntiCheatPlugin.ShipConfig2.Value} 以上再拉杆！");

                    return false;
                }
            }
            else if (p == null)
            {
                return false;
            }
            return true;
        }

        public static PlayerControllerB whoUseTerminal { get; set; }

        [HarmonyPatch(typeof(Terminal), "__rpc_handler_4047492032")]
        [HarmonyPrefix]
        public static bool __rpc_handler_4047492032(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (Check(rpcParams, out var p))
            {
                whoUseTerminal = p;
            }
            else if (p == null)
            {
                return false;
            }
            return true;
        }

        public static void KickPlayer(PlayerControllerB kick, bool canJoin = false)
        {
            if (kick.actualClientId == 0)
            {
                return;
            }
            NetworkManager.Singleton.DisconnectClient(kick.actualClientId);
            if (kick.playerSteamId == 0)
            {
                return;
            }
            var s = StartOfRound.Instance;
            ulong playerSteamId = kick.playerSteamId;
            if (!s.KickedClientIds.Contains(playerSteamId) && !canJoin)
            {
                s.KickedClientIds.Add(playerSteamId);
            }
            var terminal = UnityEngine.Object.FindAnyObjectByType<Terminal>();
            AntiCheatPlugin.ManualLog.LogInfo("terminalInUse" + terminal.placeableObject.inUse);
            AntiCheatPlugin.ManualLog.LogInfo("whoUseTerminal:" + whoUseTerminal?.playerUsername);
            AntiCheatPlugin.ManualLog.LogInfo("playerUsername:" + kick?.playerUsername);
            if (whoUseTerminal == kick && terminal.placeableObject.inUse)
            {
                AntiCheatPlugin.ManualLog.LogInfo("SetTerminalInUseServerRpc");
                terminal.SetTerminalInUseServerRpc(false);
                terminal.terminalInUse = false;
            }
            ShowMessage($"{kick.playerUsername} 被踢出游戏");
        }

        [HarmonyPatch(typeof(HUDManager), "SetPlayerLevel")]
        [HarmonyPrefix]
        public static bool SetPlayerLevel(bool isDead, bool mostProfitable, bool allPlayersDead)
        {
            foreach (var item in HUDManager.Instance.playerLevels)
            {
                if (item.XPMax == 0)
                {
                    item.XPMax = 1;
                }
            }
            return true;
        }


        //[HarmonyPatch(typeof(Shovel), "HitShovelServerRpc")]
        //[HarmonyPrefix]
        //public static bool HitShovelServerRpc(Shovel __instance)
        //{
        //    if (!StartOfRound.Instance.localPlayerController.IsHost)
        //    {
        //        return true;
        //    }
        //    else if (AntiCheatPlugin.ItemCooldown.Value)
        //    {
        //        if (__instance.playerHeldBy == null)
        //        {
        //            return false;
        //        }
        //        PlayerControllerB playerHeldBy = __instance.playerHeldBy;
        //        var id = playerHeldBy.playerSteamId;
        //        DateTime dt = DateTime.Now;
        //        var m = dt.Ticks / 10000;
        //        AntiCheatPlugin.ManualLog.LogInfo(m);
        //        if (!czcd.ContainsKey(id))
        //        {
        //            czcd.Add(id, new List<long>());
        //        }
        //        if (czcd[id].Count > 200)
        //        {
        //            czcd[id].RemoveRange(0, czcd[id].Count - 1);
        //        }
        //        if (czcd[id].Contains(m))
        //        {
        //            ShowMessage($"玩家{playerHeldBy.playerUsername}使用铲子频率异常！");
        //            if (AntiCheatPlugin.ItemCooldown2.Value)
        //            {
        //                var index = StartOfRound.Instance.allPlayerScripts.ToList().IndexOf(playerHeldBy);
        //                StartOfRound.Instance.KickPlayer(index);
        //            }
        //            return false;
        //        }
        //        else if (czcd[id].Count(x => x + 100 > m) > 3)
        //        {
        //            czcd[id].Add(m);
        //            //ShowMessage($"玩家{playerHeldBy.playerUsername}使用铲子频率异常！");
        //            //if (AntiCheatPlugin.ItemCooldown2.Value)
        //            //{
        //            //    var index = StartOfRound.Instance.allPlayerScripts.ToList().IndexOf(playerHeldBy);
        //            //    StartOfRound.Instance.KickPlayer(index);
        //            //}
        //            return false;
        //        }
        //        else
        //        {
        //            czcd[id].Add(m);
        //        }
        //    }
        //    return true;
        //}

        [HarmonyPatch(typeof(StartMatchLever), "__rpc_handler_2406447821")]
        [HarmonyPrefix]
        public static bool __rpc_handler_2406447821(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {

            if (Check(rpcParams, out var p))
            {
                if (UnityEngine.Object.FindAnyObjectByType<StartMatchLever>().leverHasBeenPulled && !StartOfRound.Instance.shipHasLanded)
                {
                    return StartGameServerRpc(target, reader, rpcParams);
                }
                else
                {
                    return EndGameServerRpc(target, reader, rpcParams);
                }
            }
            else if (p == null)
            {
                return false;
            }

            return true;
        }


        [HarmonyPatch(typeof(DepositItemsDesk), "SellAndDisplayItemProfits")]
        [HarmonyPostfix]
        public static void SellAndDisplayItemProfits(int profit, int newGroupCredits)
        {
            if (!StartOfRound.Instance.localPlayerController.IsHost)
            {
                return;
            }
            Money = newGroupCredits;
            AntiCheatPlugin.ManualLog.LogInfo($"SetMoney:{Money}");
        }

        [HarmonyPatch(typeof(Turret), "__rpc_handler_4195711963")]
        [HarmonyPrefix]
        public static bool __rpc_handler_4195711963(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (Check(rpcParams, out var p))
            {
                if (AntiCheatPlugin.Turret.Value)
                {
                    var obj = p.ItemSlots[p.currentItemSlot];
                    AntiCheatPlugin.ManualLog.LogInfo("itemName:" + obj.GetType().ToString());
                    if (obj != null && isShovel(obj))
                    {
                        var t = (Turret)target;
                        float v = Vector3.Distance(t.transform.position, p.transform.position);
                        if (v > 10)
                        {
                            ShowMessage($"检测玩家 {p.playerUsername} 强制激怒机枪！(距离：{v})");
                            if (AntiCheatPlugin.Turret2.Value)
                            {
                                KickPlayer(p);
                            }
                            return false;
                        }
                    }
                    else
                    {
                        ShowMessage($"检测玩家 {p.playerUsername} 强制激怒机枪(空手)！");
                        if (AntiCheatPlugin.Turret2.Value)
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

        [HarmonyPatch(typeof(TimeOfDay), "__rpc_handler_543987598")]
        [HarmonyPrefix]
        public static bool __rpc_handler_543987598(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (Check(rpcParams, out var p))
            {
                TimeOfDay.Instance.votesForShipToLeaveEarly++;
                int num = StartOfRound.Instance.connectedPlayersAmount + 1 - StartOfRound.Instance.livingPlayers;
                if (TimeOfDay.Instance.votesForShipToLeaveEarly >= num)
                {
                    return EndGameServerRpc(target, reader, rpcParams);
                }
            }
            else if (p == null)
            {
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(StartOfRound), "__rpc_handler_2028434619")]
        [HarmonyPrefix]
        public static bool EndGameServerRpc(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (Check(rpcParams, out var p))
            {
                var hour = int.Parse(AntiCheatPlugin.ShipConfig3.Value.Split(':')[0]);
                var min = int.Parse(AntiCheatPlugin.ShipConfig3.Value.Split(':')[1]);
                //AntiCheatPlugin.ManualLog.LogInfo($"cfg{hour}:{min}");
                var time = (int)(TimeOfDay.Instance.normalizedTimeOfDay * (60f * TimeOfDay.Instance.numberOfHours)) + 360;
                int time2 = (int)Mathf.Floor((float)(time / 60));
                bool pm = false;
                if (time2 > 12)
                {
                    pm = true;
                    time2 %= 12;
                }
                time = time % 60;
                if (pm)
                {
                    time2 += 12;
                }
                //AntiCheatPlugin.ManualLog.LogInfo($"time {time}:{time2}");
                var live = (decimal)StartOfRound.Instance.allPlayerScripts.Count(x => x.isPlayerControlled && !x.isPlayerDead);

                if (live == 1)
                {
                    return true;
                }
                //AntiCheatPlugin.ManualLog.LogInfo($"live {live}");
                decimal p1 = Math.Round(live * (decimal)(AntiCheatPlugin.ShipConfig4.Value / 100m), 2);
                decimal p2 = StartOfRound.Instance.allPlayerScripts.Count(x => x.isPlayerControlled && x.isInHangarShipRoom); // 
                if (StartOfRound.Instance.currentLevel.PlanetName.Contains("Gordion"))
                {
                    time2 = hour;
                    time = min;
                }
                //decimal p3 = StartOfRound.Instance.allPlayerScripts.Count(x => x.isPlayerControlled && x.isPlayerDead) * (decimal)(AntiCheatPlugin.ShipConfig5.Value / 100);
                if (hour <= time2 && min <= time && p2 >= p1)
                {
                    return true;
                }
                else
                {
                    ShowMessage($"请等待飞船人数达到 {p1}({AntiCheatPlugin.ShipConfig4.Value}%) 并且 游戏时间在 {hour.ToString("00")}:{min.ToString("00")} 之后起飞(当前时间:{time2.ToString("00")}:{time.ToString("00")})", "现在还不能起飞！");

                    return false;
                }
            }
            else if (p == null)
            {
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(ShipBuildModeManager), "__rpc_handler_861494715")]
        [HarmonyPrefix]
        public static bool __rpc_handler_861494715(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (Check(rpcParams, out var p))
            {
                try
                {
                    NetworkManager networkManager = target.NetworkManager;
                    if (networkManager == null || !networkManager.IsListening)
                    {
                        return true;
                    }
                    Vector3 newPosition;
                    reader.ReadValueSafe(out newPosition);
                    Vector3 newRotation;
                    reader.ReadValueSafe(out newRotation);
                    reader.Seek(0);
                    var pl = GetPlayer(rpcParams);
                    AntiCheatPlugin.ManualLog.LogInfo($"newPosition:{newPosition.ToString()}|newRotation:{newRotation.ToString()}|playerWhoMoved:{pl.playerUsername}");
                    if (newRotation.x == 0 || newPosition.x > 11.2 || newPosition.x < -5 || newPosition.y > 5 || newPosition.y < 0 || newPosition.z > -10 || newPosition.z < -18)
                    {
                        ShowMessage($"检测到玩家 {pl.playerUsername} 将飞船物品摆放到异常位置({newPosition.ToString()})！");
                        if (AntiCheatPlugin.ShipBuild2.Value)
                        {
                            KickPlayer(pl);
                        }
                        return false;
                    }
                    return true;
                }
                catch (Exception)
                {

                }
            }
            else if (p == null)
            {
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(Landmine), "__rpc_handler_3032666565")]
        [HarmonyPrefix]
        public static bool __rpc_handler_3032666565(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (Check(rpcParams, out var p))
            {
                if (AntiCheatPlugin.Landmine.Value)
                {
                    var lm = (Landmine)target;
                    var ps = p.transform.position;
                    if (p.isPlayerDead)
                    {
                        ps = p.deadBody.transform.position;
                        AntiCheatPlugin.ManualLog.LogInfo($"玩家 {p.deadBody.transform.position}");
                    }
                    AntiCheatPlugin.ManualLog.LogInfo($"玩家 {ps}");
                    if (Vector3.Distance(ps, lm.transform.position) > 15)
                    {
                        if (lm.transform.position.y > 0 && p.isInsideFactory)
                        {
                            return true;
                        }
                        else if (lm.transform.position.y < 0 && !p.isInsideFactory)
                        {
                            return true;
                        }
                        ShowMessage($"检测到玩家 {p.playerUsername} 强制引爆地雷！");
                        if (AntiCheatPlugin.Landmine2.Value)
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

        [HarmonyPatch(typeof(DepositItemsDesk), "__rpc_handler_3230280218")]
        [HarmonyPrefix]
        public static bool __rpc_handler_3230280218(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (Check(rpcParams, out var p))
            {
                if (AntiCheatPlugin.Boss.Value)
                {
                    ShowMessage($"检测到玩家 {p.playerUsername} 召唤老板！");
                    if (AntiCheatPlugin.Boss2.Value)
                    {
                        KickPlayer(p);
                    }
                }
                return false;
            }
            else if (p == null)
            {
                return false;
            }
            return true;
        }


    }
}
