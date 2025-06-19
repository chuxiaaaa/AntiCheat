using BepInEx;
using BepInEx.Configuration;

using GameNetcodeStuff;

using HarmonyLib;

using Netcode.Transports.Facepunch;

using Steamworks;
using Steamworks.Data;

using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using TMPro;

using Unity.Netcode;

using UnityEngine;
using UnityEngine.Events;

using static UnityEngine.GraphicsBuffer;

namespace AntiCheat
{
    public static class Patches
    {
        public static Locale.LocalizationManager locale { get => Core.AntiCheat.localizationManager; } 

        public static List<ulong> jcs = new List<ulong>();

        public static Dictionary<ulong, List<string>> czcd = new Dictionary<ulong, List<string>>();
        public static Dictionary<ulong, List<string>> sdqcd = new Dictionary<ulong, List<string>>();

        public static Dictionary<uint, ulong> ConnectionIdtoSteamIdMap { get; set; } = new Dictionary<uint, ulong>();

        public static Dictionary<int, Dictionary<ulong, List<DateTime>>> chcs = new Dictionary<int, Dictionary<ulong, List<DateTime>>>();

        public static List<long> mjs { get; set; } = new List<long>();
        public static List<long> lhs { get; set; } = new List<long>();

        public static List<int> landMines { get; set; }

        public static Dictionary<string, List<ulong>> rpcs { get; set; } = new Dictionary<string, List<ulong>>();

        public static void LogInfo(string info) => Core.AntiCheat.LogInfo(info);

        public static void LogError(string info)
        {
            if (Core.AntiCheat.Log.Value)
            {
                Core.AntiCheat.ManualLog.LogError($"{info}");
            }
        }

        [HarmonyPatch(typeof(PlayerControllerB), "__rpc_handler_4121569671")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static bool __rpc_handler_4121569671(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (Check(rpcParams, out var p))
            {
                if (rpcs.ContainsKey("KillPlayer"))
                {
                    rpcs["KillPlayer"].Remove(p.actualClientId);
                }
                ByteUnpacker.ReadValueBitPacked(reader, out int playerId);
                reader.ReadValueSafe(out bool spawnBody, default);
                reader.ReadValueSafe(out Vector3 bodyVelocity);
                ByteUnpacker.ReadValueBitPacked(reader, out int num);
                reader.Seek(0);
                if ((CauseOfDeath)num == CauseOfDeath.Abandoned)
                {
                    bypass = true;
                    string msg = locale.Msg_GetString("behind_player", new Dictionary<string, string>() {
                        { "{player}",p.playerUsername }
                    });
                    LogInfo(msg);
                    HUDManager.Instance.AddTextToChatOnServer(msg, -1);
                    bypass = false;
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
        [HarmonyPatch(typeof(PlayerControllerB), "__rpc_handler_638895557")]
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
                LogInfo($"{p.playerUsername} call PlayerControllerB.DamagePlayerFromOtherClientServerRpc|damageAmount:{damageAmount}|hitDirection:{hitDirection}|playerWhoHit:{playerWhoHit}");
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
        [HarmonyPatch(typeof(PlayerControllerB), "__rpc_handler_2585603452")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static bool __rpc_handler_2585603452(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (Check(rpcParams, out var p))
            {
                LogInfo($"{p.playerUsername} call PlayerControllerB.HealServerRpc|health:{p.health}|newHealth:20");
                p.health = 20;
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
            LogInfo($"{p.playerUsername} hit {p2.playerUsername} damageAmount:{damageAmount}|p2:{p2.playerUsername}");
            try
            {
                if (Core.AntiCheat.Shovel.Value)
                {
                    var distance = Vector3.Distance(p.transform.position, p2.transform.position);
                    var obj = p.ItemSlots[p.currentItemSlot];
                    string playerUsername = p.playerUsername;
                    if (jcs.Contains(p.playerSteamId))
                    {
                        damageAmount = 0;
                    }
                    else if (damageAmount != 20 && obj != null && (isShovel(obj) || isKnife(obj)))
                    {
                        if (!jcs.Contains(p.playerSteamId))
                        {
                            ShowMessage(locale.Msg_GetString("Shovel", new Dictionary<string, string>() {
                                { "{player}",p.playerUsername },
                                { "{player2}",p2.playerUsername },
                                { "{damageAmount}",damageAmount.ToString() },
                                { "{item}", isShovel(obj) ? locale.Item_GetString("Shovel") : locale.Item_GetString("Knife") }
                            }));
                            jcs.Add(p.playerSteamId);
                            if (Core.AntiCheat.Shovel2.Value)
                            {
                                KickPlayer(p);
                            }
                        }
                        damageAmount = 0;
                    }
                    else if (distance > 11 && obj != null && (isShovel(obj) || isKnife(obj)))
                    {
                        if (p2.isPlayerDead)
                        {
                            return true;
                        }
                        if (!jcs.Contains(p.playerSteamId))
                        {
                            ShowMessage(locale.Msg_GetString("Shovel2", new Dictionary<string, string>() {
                                { "{player}",p.playerUsername },
                                { "{player2}",p2.playerUsername },
                                { "{distance}",distance.ToString() },
                                { "{damageAmount}",damageAmount.ToString() },
                                { "{item}", isShovel(obj) ? locale.Item_GetString("Shovel") : locale.Item_GetString("Knife") }
                            }));
                            jcs.Add(p.playerSteamId);
                            if (Core.AntiCheat.Shovel2.Value)
                            {
                                KickPlayer(p);
                            }
                        }
                        damageAmount = 0;
                    }
                    else if (obj == null || (!isGun(obj) && !isShovel(obj) && !isKnife(obj)))
                    {
                        LogInfo($"currentItemSlot:{p.currentItemSlot}");
                        LogInfo($"currentlyHeldObjectServer:{p.currentlyHeldObjectServer}");
                        LogInfo($"obj:{obj}");
                        if (!jcs.Contains(p.playerSteamId))
                        {
                            if (Core.AntiCheat.Shovel3.Value && (damageAmount == 10 || damageAmount == 20 || damageAmount == 30 || damageAmount == 100))
                            {
                                LogInfo(locale.Msg_GetString("Shovel3", new Dictionary<string, string>() {
                                    { "{player}",p.playerUsername },
                                    { "{player2}",p2.playerUsername },
                                    { "{damageAmount}",damageAmount.ToString() }
                                }));
                                return true;
                            }
                            else
                            {
                                ShowMessage(locale.Msg_GetString("Shovel3", new Dictionary<string, string>() {
                                    { "{player}",p.playerUsername },
                                    { "{player2}",p2.playerUsername },
                                    { "{damageAmount}",damageAmount.ToString() }
                                }));
                                jcs.Add(p.playerSteamId);
                                if (Core.AntiCheat.Shovel2.Value)
                                {
                                    KickPlayer(p);
                                }
                                damageAmount = 0;
                            }
                        }
                    }
                    if (damageAmount == 0)
                    {
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
                LogInfo($"{ex.ToString()}");
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

        public static bool isKnife(GrabbableObject item)
        {
            return item is KnifeItem;
        }

        public static bool isJetpack(GrabbableObject item)
        {
            return item is JetpackItem;
        }

        public static ulong lastClientId { get; set; }


        //[HarmonyPatch(typeof(StartOfRound), "StartTrackingAllPlayerVoices")]
        //[HarmonyPostfix]
        //[HarmonyWrapSafe]
        //public static void StartTrackingAllPlayerVoices()
        //{
        //    if (!StartOfRound.Instance.localPlayerController.IsHost)
        //    {
        //        return;
        //    }
        //    foreach (var item in StartOfRound.Instance.allPlayerScripts)
        //    {
        //        if (!item.isPlayerControlled)
        //        {
        //            continue;
        //        }
        //        var playerName = item.playerUsername;
        //        if (playerName == "Player #0")
        //        {
        //            continue;
        //        }
        //        if (StartOfRound.Instance.KickedClientIds.Contains(item.playerSteamId))
        //        {
        //            KickPlayer(item);
        //            return;
        //        }
        //        LogInfo(playerName);
        //        if (Regex.IsMatch(playerName, "Nameless\\d*") || Regex.IsMatch(playerName, "Unknown\\d*") || Regex.IsMatch(playerName, "Player #\\d*"))
        //        {
        //            if (Core.AntiCheat.Nameless.Value)
        //            {
        //                ShowMessage(locale.Msg_GetString("Nameless"));
        //                if (Core.AntiCheat.Nameless2.Value)
        //                {
        //                    KickPlayer(item, true, locale.Msg_GetString("Kick_Nameless"));
        //                }
        //            }
        //        }
        //    }
        //    var p2 = StartOfRound.Instance.allPlayerScripts.OrderByDescending(x => x.actualClientId).FirstOrDefault();
        //    if (p2.actualClientId != lastClientId)
        //    {
        //        if (p2.isPlayerControlled && p2.playerSteamId == 0)
        //        {
        //            KickPlayer(p2);
        //            return;
        //        }
        //        else if (!p2.isPlayerControlled)
        //        {
        //            return;
        //        }
        //        bypass = true;
        //        string msg = Core.AntiCheat.PlayerJoin.Value.Replace("{player}", p2.playerUsername);
        //        LogInfo(msg);
        //        HUDManager.Instance.AddTextToChatOnServer(msg, -1);
        //        lastClientId = p2.actualClientId;
        //        bypass = false;
        //    }
        //}



        [HarmonyPatch(typeof(GameNetworkManager), "SteamMatchmaking_OnLobbyMemberJoined")]
        [HarmonyPostfix]
        [HarmonyWrapSafe]
        public static void SteamMatchmaking_OnLobbyMemberJoined()
        {
            if (StartOfRound.Instance == null || StartOfRound.Instance.localPlayerController == null || !StartOfRound.Instance.localPlayerController.IsHost)
            {
                return;
            }
            LogInfo($"SetMoney:{Money}");
            Money = UnityEngine.Object.FindObjectOfType<Terminal>().groupCredits;
        }

        /// <summary>
        /// 游戏结束时重置所有变量
        /// StartOfRound.EndOfGame
        /// </summary>
        [HarmonyPatch(typeof(StartOfRound), "EndOfGame")]
        [HarmonyPostfix]
        [HarmonyWrapSafe]
        public static void EndOfGame()
        {
            jcs = new List<ulong>();
            chcs = new Dictionary<int, Dictionary<ulong, List<DateTime>>>();
            if (rpcs.ContainsKey("Hit"))
            {
                rpcs["Hit"] = new List<ulong>();
            }
            if (rpcs.ContainsKey("KillPlayer"))
            {
                rpcs["KillPlayer"] = new List<ulong>();
            }
        }

        public static int Money = -1;

        /// <summary>
        /// Prefix StartOfRound.ChangeLevelServerRpc
        /// </summary>
        [HarmonyPatch(typeof(StartOfRound), "__rpc_handler_1134466287")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static bool __rpc_handler_1134466287(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (Check(rpcParams, out var p))
            {
                //LogInfo($"{p.playerUsername}|StartOfRound.ChangeLevelServerRpc");
                if (Core.AntiCheat.RemoteTerminal.Value)
                {
                    if (!CheckRemoteTerminal(p, "StartOfRound.ChangeLevelServerRpc"))
                    {
                        return false;
                    }
                }
                ByteUnpacker.ReadValueBitPacked(reader, out int levelID);
                ByteUnpacker.ReadValueBitPacked(reader, out int newGroupCreditsAmount);
                reader.Seek(0);
                if (Core.AntiCheat.FreeBuy.Value)
                {
                    if (newGroupCreditsAmount > Money || Money < 0)
                    {
                        ShowMessage(locale.Msg_GetString("FreeBuy_SetMoney", new Dictionary<string, string>() {
                            { "{player}",p.playerUsername },
                            { "{Money}",(newGroupCreditsAmount - Money).ToString() }
                        }));
                        if (Core.AntiCheat.FreeBuy2.Value)
                        {
                            KickPlayer(p);
                        }
                        return false;
                    }
                    if (levelID > StartOfRound.Instance.levels.Length)
                    {
                        LogInfo($"{p.playerUsername}|ChangeLevelServerRpc|levelID > StartOfRound.Instance.levels.Length");
                        return false;
                    }
                    var terminal = UnityEngine.Object.FindObjectOfType<Terminal>();
                    var Route = terminal.terminalNodes.allKeywords.FirstOrDefault(x => x.word.ToLower() == "route");//Route
                    var level = StartOfRound.Instance.levels[levelID].PlanetName.Split(' ')[0];
                    var compatibleNoun = Route.compatibleNouns.Where(x => x.result.name == level + "route");
                    if (compatibleNoun.Any())
                    {
                        int itemCost = compatibleNoun.First().result.itemCost;
                        level = StartOfRound.Instance.currentLevel.PlanetName.Split(' ')[0];
                        var nowCompatibleNoun = Route.compatibleNouns.Where(x => x.result.name == level + "route");
                        if (itemCost == 0 && nowCompatibleNoun.Any() && nowCompatibleNoun.First().result.itemCost != 0)
                        {
                            ShowMessage(locale.Msg_GetString("ChangeToFreeLevel", new Dictionary<string, string>() {
                                { "{player}",p.playerUsername }
                            }));
                            return false;
                        }
                        LogInfo($"{p.playerUsername}|ChangeLevelServerRpc|levelID:{levelID}|itemCost:{itemCost}");
                        if (itemCost != 0)
                        {
                            int newValue = Money - itemCost;
                            if (newValue != newGroupCreditsAmount || Money == 0)
                            {
                                ShowMessage(locale.Msg_GetString("FreeBuy_Level", new Dictionary<string, string>() {
                                    { "{player}",p.playerUsername }
                                }));
                                if (Core.AntiCheat.FreeBuy2.Value)
                                {
                                    KickPlayer(p);
                                }
                                return false;
                            }
                        }
                    }
                }
                if (Core.AntiCheat.OperationLog.Value)
                {
                    if (levelID < StartOfRound.Instance.levels.Length)
                    {
                        ShowMessageHostOnly(locale.OperationLog_GetString("ChangeLevel", new Dictionary<string, string>() {
                            { "{player}",p.playerUsername },
                            { "{planet}",StartOfRound.Instance.levels[levelID].PlanetName }
                        }));
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
        /// StartOfRound.BuyShipUnlockableServerRpc
        /// </summary>
        /// <returns></returns>
        [HarmonyPatch(typeof(StartOfRound), "__rpc_handler_3953483456")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static bool __rpc_handler_3953483456(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (Check(rpcParams, out var p))
            {
                if (Core.AntiCheat.RemoteTerminal.Value)
                {
                    if (!CheckRemoteTerminal(p, "StartOfRound.BuyShipUnlockableServerRpc"))
                    {
                        return false;
                    }
                }
                ByteUnpacker.ReadValueBitPacked(reader, out int unlockableID);
                ByteUnpacker.ReadValueBitPacked(reader, out int newGroupCreditsAmount);
                reader.Seek(0);
                if (Core.AntiCheat.FreeBuy.Value)
                {
                    //LogInfo($"__rpc_handler_3953483456|newGroupCreditsAmount:{newGroupCreditsAmount}");
                    if (Money == newGroupCreditsAmount || Money == 0)
                    {
                        //LogInfo($"Money:{Money}|newGroupCreditsAmount:{newGroupCreditsAmount}");
                        ShowMessage(locale.Msg_GetString("FreeBuy_unlockable", new Dictionary<string, string>() {
                            { "{player}",p.playerUsername }
                        }));
                        if (Core.AntiCheat.FreeBuy2.Value)
                        {
                            KickPlayer(p);
                        }
                        return false;
                    }
                    else if (newGroupCreditsAmount > Money || Money < 0)
                    {
                        ShowMessage(locale.Msg_GetString("FreeBuy_SetMoney", new Dictionary<string, string>() {
                            { "{player}",p.playerUsername },
                            { "{Money}",(newGroupCreditsAmount - Money).ToString() }
                        }));
                        if (Core.AntiCheat.FreeBuy2.Value)
                        {
                            KickPlayer(p);
                        }
                        return false;
                    }
                }
                if (Core.AntiCheat.OperationLog.Value)
                {
                    if (unlockableID < StartOfRound.Instance.unlockablesList.unlockables.Count)
                    {
                        ShowMessageHostOnly(locale.OperationLog_GetString("BuyShipUnlockable", new Dictionary<string, string>() {
                            { "{player}",p.playerUsername },
                            { "{unlockable}",StartOfRound.Instance.unlockablesList.unlockables[unlockableID].unlockableName }
                        }));
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
        [HarmonyWrapSafe]
        public static bool BuyShipUnlockableClientRpc(int newGroupCreditsAmount, int unlockableID = -1)
        {
            if (!StartOfRound.Instance.localPlayerController.IsHost)
            {
                return true;
            }
            Money = newGroupCreditsAmount;
            return true;
        }

       

        [HarmonyPatch(typeof(TimeOfDay), "SyncNewProfitQuotaClientRpc")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static bool SyncNewProfitQuotaClientRpc(int newProfitQuota, int overtimeBonus, int fulfilledQuota)
        {
            if (!StartOfRound.Instance.localPlayerController.IsHost)
            {
                return true;
            }
            Terminal terminal = UnityEngine.Object.FindObjectOfType<Terminal>();
            Money = Mathf.Clamp(terminal.groupCredits + overtimeBonus, terminal.groupCredits, 100000000);
            return true;
        }

        [HarmonyPatch(typeof(Terminal), "SyncGroupCreditsClientRpc")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
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
        [HarmonyWrapSafe]
        public static bool ChangeLevelClientRpc(int levelID, int newGroupCreditsAmount)
        {
            if (!StartOfRound.Instance.localPlayerController.IsHost)
            {
                return true;
            }
            Money = newGroupCreditsAmount;
            return true;
        }


        [HarmonyWrapSafe]
        [HarmonyPatch(typeof(Terminal), "BeginUsingTerminal")]
        [HarmonyPrefix]
        public static bool BeginUsingTerminal(Terminal __instance)
        {
            if (!StartOfRound.Instance.IsHost)
            {
                return true;
            }
            Money = __instance.groupCredits;
            LogInfo($"SetMoney:{Money}");
            //if (__instance.terminalNodes != null && __instance.terminalNodes.allKeywords != null)
            //{
            //    for (int i = 0; i < __instance.terminalNodes.allKeywords.Length; i++)
            //    {
            //        var item = __instance.terminalNodes.allKeywords[i];
            //        //LogInfo($"allKeywords:{i}|name:{item.name}|specialKeywordResult?:{item.specialKeywordResult?.name}");
            //        foreach (var item2 in item.compatibleNouns)
            //        {
            //            LogInfo("item2.result.name:"+ item2.result.name);
            //            if (item2.result.name.Trim() == "RadMechFile" || item2.result.name.Trim() == "TulipSnakeFile" || item2.result.name.Trim() == "ButlerFile" || item2.result.name.Trim() == "ArtificeInfo")
            //            {
            //                LogInfo($"displayText:{item2.result.displayText}");
            //            }

            //            //    LogInfo($"{item2.noun.}");
            //            //LogInfo($"compatibleNouns|noun:{item2.noun.ToString()}|name:{item2.result.name}|buyItemIndex:{item2.result.buyItemIndex}|buyRerouteToMoon:{item2.result.buyRerouteToMoon}|itemCost:{item2.result.itemCost}");
            //        }
            //    }
            //}
            return true;
        }

        /// <summary>
        /// Prefix PlayerControllerB.DamagePlayer
        /// </summary>
        /// <returns></returns>
        [HarmonyPatch(typeof(PlayerControllerB), "DamagePlayer")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static bool DamagePlayer(PlayerControllerB __instance, int damageNumber, bool hasDamageSFX = true, bool callRPC = true, CauseOfDeath causeOfDeath = CauseOfDeath.Unknown, int deathAnimation = 0, bool fallDamage = false, Vector3 force = default(Vector3))
        {
            //LogInfo($"PlayerControllerB.DamagePlayer|{__instance.playerUsername}|damageNumber:{damageNumber}|hasDamageSFX:{hasDamageSFX}|callRPC:{callRPC}");
            return true;
        }

        [HarmonyPatch(typeof(PlayerControllerB), "IHittable.Hit")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static bool Hit(PlayerControllerB __instance, int force, Vector3 hitDirection, PlayerControllerB playerWhoHit, bool playHitSFX = false, int hitID = -1)
        {
            if (!rpcs.ContainsKey("Hit"))
            {
                rpcs.Add("Hit", new List<ulong>());
            }
            if (__instance.AllowPlayerDeath())
            {
                rpcs["Hit"].Add(__instance.actualClientId);
                LogInfo($"PlayerControllerB.Hit|{__instance.playerUsername}|force:{force}|playerWhoHit:{playerWhoHit.playerUsername}|playHitSFX:{playHitSFX}|hitID:{hitID}");
                __instance.StartCoroutine(CheckRpc(__instance, "Hit"));
            }
            return true;
        }



        public static IEnumerator CheckRpc(PlayerControllerB __instance, string RPC)
        {
            if (RPC == "Hit" && !Core.AntiCheat.RPCReport_Hit.Value)
            {
                yield break;
            }
            if (RPC == "KillPlayer" && !Core.AntiCheat.RPCReport_KillPlayer.Value)
            {
                yield break;
            }
            yield return new WaitForSeconds(Core.AntiCheat.RPCReport_Delay.Value);
            if (__instance.isPlayerDead)
            {
                yield break;
            }
            if (rpcs[RPC].Contains(__instance.actualClientId))
            {
                rpcs[RPC].Remove(__instance.actualClientId);
                ShowMessage(locale.Msg_GetString("RPCReport", new Dictionary<string, string>() {
                  { "{player}", __instance.playerUsername },
                  { "{RPC}", RPC },

                }));
                if (Core.AntiCheat.RPCReport_Kick.Value)
                {
                    KickPlayer(__instance);
                }
            }
            yield break;
        }


        /// <summary>
        /// Prefix PlayerControllerB.DamagePlayerServerRpc
        /// </summary>
        /// <returns></returns>
        [HarmonyPatch(typeof(PlayerControllerB), "__rpc_handler_1084949295")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static bool __rpc_handler_1084949295(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (Check(rpcParams, out var p) || StartOfRound.Instance.IsHost)
            {
                if (rpcs.ContainsKey("Hit"))
                {
                    rpcs["Hit"].Remove(p.actualClientId);
                }
                ByteUnpacker.ReadValueBitPacked(reader, out int damageNumber);
                ByteUnpacker.ReadValueBitPacked(reader, out int newHealthAmount);
                reader.Seek(0);
                LogInfo($"{p.playerUsername} call PlayerControllerB.DamagePlayerServerRpc|damageNumber:{damageNumber}|newHealthAmount:{newHealthAmount}");
                var p2 = (PlayerControllerB)target;
                if (p2 == p)
                {
                    if (Core.AntiCheat.Health_Recover.Value)
                    {
                        if (damageNumber < 0)
                        {
                            string msg = locale.Msg_GetString("Health_Recover", new Dictionary<string, string>() {
                                { "{player}", p.playerUsername },
                                { "{hp}", (damageNumber * -1).ToString() }
                            });
                            ShowMessage(msg);
                            if (Core.AntiCheat.Health_Kick.Value)
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
        /// Prefix EnemyAI.SwitchToBehaviourServerRpc
        /// </summary>
        [HarmonyPatch(typeof(EnemyAI), "__rpc_handler_2081148948")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static bool __rpc_handler_2081148948(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (Check(rpcParams, out var p))
            {
                ByteUnpacker.ReadValueBitPacked(reader, out int stateIndex);
                reader.Seek(0);
                LogInfo($"{p.playerUsername} call EnemyAI.SwitchToBehaviourServerRpc||stateIndex:{stateIndex}");
                if (Core.AntiCheat.Enemy.Value)
                {
                    var e = (EnemyAI)target;
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
                            LogInfo($"{p.playerUsername} call EnemyAI.SwitchToBehaviourServerRpc||currentBehaviourStateIndex:{j.currentBehaviourStateIndex}|stateIndex:{stateIndex}|popUpTimer:{j.popUpTimer}");
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

        #region Enemy Kill Player(Only Player Self)

        /// <summary>
        /// Prefix JesterAI.KillPlayerServerRpc
        /// </summary>
        [HarmonyPatch(typeof(JesterAI), "__rpc_handler_3446243450")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static bool __rpc_handler_3446243450(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            return KillPlayerServerRpc(target, reader, rpcParams, "JesterAI.KillPlayerServerRpc");
        }

        /// <summary>
        /// Prefix MouthDogAI.KillPlayerServerRpc
        /// </summary>
        [HarmonyPatch(typeof(MouthDogAI), "__rpc_handler_998670557")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static bool __rpc_handler_998670557(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            return KillPlayerServerRpc(target, reader, rpcParams, "MouthDogAI.KillPlayerServerRpc");
        }

        /// <summary>
        /// Prefix ForestGiantAI.GrabPlayerServerRpc
        /// </summary>
        [HarmonyPatch(typeof(ForestGiantAI), "__rpc_handler_2965927486")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static bool __rpc_handler_2965927486(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            return KillPlayerServerRpc(target, reader, rpcParams, "ForestGiantAI.GrabPlayerServerRpc");
        }

        /// <summary>
        /// Prefix RedLocustBees.BeeKillPlayerServerRpc
        /// </summary>
        [HarmonyPatch(typeof(RedLocustBees), "__rpc_handler_3246315153")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static bool __rpc_handler_3246315153(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            return KillPlayerServerRpc(target, reader, rpcParams, "RedLocustBees.BeeKillPlayerServerRpc");
        }

        /// <summary>
        /// Prefix MaskedPlayerEnemy.KillPlayerAnimationServerRpc
        /// </summary>
        [HarmonyPatch(typeof(MaskedPlayerEnemy), "__rpc_handler_3192502457")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static bool __rpc_handler_3192502457(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            return KillPlayerServerRpc(target, reader, rpcParams, "MaskedPlayerEnemy.KillPlayerAnimationServerRpc");
        }

        /// <summary>
        /// Prefix NutcrackerEnemyAI.LegKickPlayerServerRpc
        /// </summary>
        [HarmonyPatch(typeof(NutcrackerEnemyAI), "__rpc_handler_3881699224")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static bool __rpc_handler_3881699224(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            return KillPlayerServerRpc(target, reader, rpcParams, "NutcrackerEnemyAI.LegKickPlayerServerRpc");
        }

        /// <summary>
        /// Prefix BlobAI.SlimeKillPlayerEffectServerRpc
        /// </summary>
        [HarmonyPatch(typeof(BlobAI), "__rpc_handler_3848306567")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static bool __rpc_handler_3848306567(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            return KillPlayerServerRpc(target, reader, rpcParams, "BlobAI.SlimeKillPlayerEffectServerRpc");
        }

        /// <summary>
        /// Prefix CentipedeAI.ClingToPlayerServerRpc
        /// </summary>
        [HarmonyPatch(typeof(CentipedeAI), "__rpc_handler_2791977891")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static bool __rpc_handler_2791977891(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            return KillPlayerServerRpc(target, reader, rpcParams, "CentipedeAI.ClingToPlayerServerRpc");
        }

        /// <summary>
        /// Prefix RadMechAI.GrabPlayerServerRpc
        /// </summary>
        [HarmonyPatch(typeof(RadMechAI), "__rpc_handler_3707286996")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static bool __rpc_handler_3707286996(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            return KillPlayerServerRpc(target, reader, rpcParams, "RadMechAI.GrabPlayerServerRpc");
        }

        /// <summary>
        /// Prefix CaveDwellerAI.GrabPlayerServerRpc
        /// </summary>
        /// <returns></returns>
        [HarmonyPatch(typeof(CaveDwellerAI), "__rpc_handler_3591556954")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static bool __rpc_handler_3591556954(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            return KillPlayerServerRpc(target, reader, rpcParams, "CaveDwellerAI.GrabPlayerServerRpc");
        }



        private static bool KillPlayerServerRpc(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams,string call)
        {
            if (Check(rpcParams, out var p))
            {
                var e = (EnemyAI)target;
                ByteUnpacker.ReadValueBitPacked(reader, out int playerId);
                reader.Seek(0);
                LogInfo($"{p.playerUsername} call {e.GetType()}.KillPlayerServerRpc|playerId:{playerId}");
                if (playerId <= StartOfRound.Instance.allPlayerScripts.Length && StartOfRound.Instance.allPlayerScripts[playerId] != p)
                {
                    LogInfo($"{p.playerUsername} call {e.GetType()}.KillPlayerServerRpc|playerUsername:{StartOfRound.Instance.allPlayerScripts[playerId].playerUsername}");
                    return false;
                }
                if (!rpcs.ContainsKey("KillPlayer"))
                {
                    rpcs.Add("KillPlayer", new List<ulong>());
                }
                if (p.AllowPlayerDeath())
                {
                    LogInfo($"StartCoroutine:CheckRpc|{p.playerUsername}|{call}");
                    rpcs["KillPlayer"].Add(p.actualClientId);
                    p.StartCoroutine(CheckRpc(p, "KillPlayer"));
                }
            }
            else if (p == null)
            {
                return false;
            }
            return true;
        }

        #endregion

        /// <summary>
        /// Prefix EnemyAI.UpdateEnemyPositionServerRpc
        /// </summary>
        [HarmonyPatch(typeof(EnemyAI), "__rpc_handler_255411420")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static bool __rpc_handler_255411420(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (Check(rpcParams, out var p))
            {
                reader.ReadValueSafe(out Vector3 newPos);
                reader.Seek(0);
                LogInfo($"{p.playerUsername} call EnemyAI.UpdateEnemyPositionServerRpc|newPos:{newPos}");
            }
            else if (p == null)
            {
                return false;
            }
            return true;
        }


        [HarmonyPatch(typeof(EnemyAI), "__rpc_handler_1810146992")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static bool __rpc_handler_1810146992(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (Check(rpcParams, out var p))
            {
                LogInfo($"{p.playerUsername} call EnemyAI.KillEnemyServerRpc");
                if (Core.AntiCheat.KillEnemy.Value)
                {
                    var e = (EnemyAI)target;
                    foreach (var item in bypassKill)
                    {
                        if (item.EnemyInstanceId == e.GetInstanceID() && !item.CalledClient.Contains(p.actualClientId))
                        {
                            LogInfo($"{e.GetInstanceID()} bypass|actualClientId:{p.actualClientId}");
                            item.CalledClient.Add(p.actualClientId);
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
                        if (Core.AntiCheat.KillEnemy2.Value)
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
        [HarmonyWrapSafe]
        public static bool __rpc_handler_3079913705(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (Check(rpcParams, out var p))
            {
                if (Core.AntiCheat.Enemy.Value)
                {
                    LogInfo($"{p.playerUsername} call EnemyAI.UpdateEnemyRotationServerRpc");
                    return true;
                }
            }
            return true;
        }

        //[HarmonyPatch(typeof(EnemyAI), "__rpc_handler_255411420")]
        //[HarmonyPrefix]
        //public static bool __rpc_handler_255411420(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        //{
        //    if (Check(rpcParams, out var p))
        //    {
        //        if (Core.AntiCheatPlugin.Enemy.Value)
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
                if (Core.AntiCheat.Enemy.Value)
                {
                    ByteUnpacker.ReadValueBitPacked(reader, out int clientId);
                    reader.Seek(0);
                    var enemy = target.GetComponent<EnemyAI>();
                    if (enemy is MouthDogAI || enemy is DressGirlAI)
                    {
                        return true;
                    }
                    float v = Vector3.Distance(p.transform.position, enemy.transform.position);
                    LogInfo($"Distance:{v}");
                    LogInfo($"{p.playerUsername}|{p.actualClientId} called ChangeOwnershipOfEnemy|enemy:{enemy.enemyType.enemyName}|clientId:{clientId}");
                    LogInfo($"OwnerClientId:{enemy.OwnerClientId}");
                    int key = enemy.GetInstanceID();
                    if (!chcs.ContainsKey(key))
                    {
                        chcs.Add(key, new Dictionary<ulong, List<DateTime>>());
                    }
                    if (!chcs[key].ContainsKey(p.actualClientId))
                    {
                        chcs[key].Add(p.actualClientId, new List<DateTime>());
                    }
                    if (chcs[key][p.actualClientId].Count(x => x.AddSeconds(1) > DateTime.Now) > 5)
                    {
                        ShowMessage(locale.Msg_GetString("Enemy_ChangeOwnershipOfEnemy", new Dictionary<string, string>() {
                            { "{player}",p.playerUsername }
                        }));
                        //KickPlayer(p); 
                        return false;
                    }
                    chcs[key][p.actualClientId].Add(DateTime.Now);
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
                            LogInfo("client ChangeOwnershipOfEnemy:" + clientId);
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
                            LogInfo("client ChangeOwnershipOfEnemy:" + clientId);
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
        [HarmonyWrapSafe]
        public static bool __rpc_handler_3663112878(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (Check(rpcParams, out var p))
            {
                if (Core.AntiCheat.Jetpack.Value)
                {
                    var jp = (JetpackItem)target;
                    if (jp.playerHeldBy != null && jp.playerHeldBy.actualClientId != p.actualClientId)
                    {
                        ShowMessage(locale.Msg_GetString("Jetpack", new Dictionary<string, string>() {
                             { "{player}",p.playerUsername }
                        }));
                        if (Core.AntiCheat.Jetpack2.Value)
                        {
                            KickPlayer(p);
                        }
                        return false;
                    }
                    else if (jp.playerHeldBy == null)
                    {
                        ShowMessage(locale.Msg_GetString("Jetpack", new Dictionary<string, string>() {
                             { "{player}",p.playerUsername }
                        }));
                        if (Core.AntiCheat.Jetpack2.Value)
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

        public static List<HitData> bypassHit = new List<HitData>();
        public static List<HitData> bypassKill = new List<HitData>();

        [HarmonyPatch(typeof(EnemyAI), "KillEnemyOnOwnerClient")]
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


        [HarmonyPatch(typeof(EnemyAI), "HitEnemyOnLocalClient")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static void HitEnemyOnLocalClient(EnemyAI __instance, int force, Vector3 hitDirection, PlayerControllerB playerWhoHit, bool playHitSFX, int hitID)
        {
            if (StartOfRound.Instance.localPlayerController.IsHost)
            {
                if (bypassHit.Any(x => x.EnemyInstanceId == __instance.GetInstanceID()))
                {
                    return;
                }
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


        public class HitData
        {
            public int EnemyInstanceId { get; set; }

            public int force { get; set; }

            public List<ulong> CalledClient { get; set; }
        }

        public static PlayerControllerB lastDriver { get; set; }


        [HarmonyPatch(typeof(ButlerEnemyAI), "KillEnemy")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static void ButlerBlowUpAndPop(ButlerEnemyAI __instance)
        {
            if (StartOfRound.Instance.localPlayerController.IsHost)
            {
                LogInfo($"bypassHit -> {__instance.enemyType.enemyName}({__instance.GetInstanceID()})");
                if (!(bool)AccessTools.DeclaredField(typeof(ButlerEnemyAI), "startedButlerDeathAnimation").GetValue(__instance))
                {
                    bypassHit.Add(new HitData()
                    {
                        EnemyInstanceId = __instance.GetInstanceID(),
                        force = 6,
                        CalledClient = new List<ulong>()
                    });
                }
            }
        }
        //public static void HitEnemy(int force = 1, PlayerControllerB playerWhoHit = null, bool playHitSFX = false, int hitID = -1)
        //{
        //    LogInfo($"force:{force}|playerWhoHit:{playerWhoHit?.playerUsername}|playHitSFX:{playHitSFX}|hitID:{hitID}");
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
                LogInfo($"{p.playerUsername} call ({((EnemyAI)target).enemyType.enemyName})EnemyAI.HitEnemyServerRpc|force:{force}|playerWhoHit:{playerWhoHit}");
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
                var e = (EnemyAI)target;
                //if (e.isEnemyDead)
                //{
                //    return true;
                //}
                foreach (var item in bypassHit)
                {
                    if (item.EnemyInstanceId == e.GetInstanceID() && item.force == force && !item.CalledClient.Contains(p.actualClientId))
                    {
                        LogInfo($"{e.GetInstanceID()} bypass|actualClientId:{p.actualClientId}");
                        item.CalledClient.Add(p.actualClientId);
                        return true;
                    }
                }
                if (Core.AntiCheat.Shovel.Value)
                {
                    //if (playerWhoHit == -1 && Vector3.Distance(p.transform.position, e.transform.position) < 30)
                    //{
                    //    return true;
                    //}
                    var obj = p.ItemSlots[p.currentItemSlot];
                    string playerUsername = p.playerUsername;
                    if (force != 1 && obj != null && (isShovel(obj) || isKnife(obj)))
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
                            if (Core.AntiCheat.Shovel2.Value)
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
                            LogInfo($"currentItemSlot:{p.currentItemSlot}");
                            LogInfo($"currentlyHeldObjectServer:{p.currentlyHeldObjectServer}");
                            LogInfo($"obj:{obj}");
                            if (!Core.AntiCheat.Shovel3.Value && (force == 1 || force == 2 || force == 3 || force == 5))
                            {
                                ShowMessage(locale.Msg_GetString("Shovel6", new Dictionary<string, string>() {
                                    { "{player}",p.playerUsername },
                                    { "{enemyName}",e.enemyType.enemyName },
                                    { "{damageAmount}",force.ToString() }
                                }));
                                jcs.Add(p.playerSteamId);
                                if (Core.AntiCheat.Shovel2.Value)
                                {
                                    KickPlayer(p);
                                }
                            }
                            else
                            {
                                LogInfo(locale.Msg_GetString("Shovel6", new Dictionary<string, string>() {
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

        private static string lastMessage = string.Empty;

        /// <summary>
        /// 在游戏中输出信息
        /// </summary>
        public static void ShowMessage(string msg, string lastmsg = null)
        {
            string showmsg = string.Empty;
            var type = Core.AntiCheat.DetectedMessageType.Value;
            showmsg = locale.MessageFormat(new Dictionary<string, string>() {
                { "{Prefix}",locale.Prefix() },
                { "{msg}",msg }
            });
            if (lastMessage == msg || (lastmsg == lastMessage && lastmsg != null))
            {
                //LogInfo($"RepeatMessage|{showmsg}");
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
            if (type == Core.AntiCheat.MessageType.PublicChat)
            {
                AccessTools.DeclaredMethod(typeof(HUDManager), "AddPlayerChatMessageClientRpc").Invoke(HUDManager.Instance,new object[] {
                    showmsg, -1
                });
            }
            else if (type == Core.AntiCheat.MessageType.HostChat)
            {
                LogInfo($"AddChatMessage|{showmsg}");
                ShowMessageHostOnly(showmsg);
            }
            else
            {
                LogInfo($"ShowGUI|{showmsg}");
            }
        }

        public static bool bypass { get; set; }

        /// <summary>
        /// 开礼物盒事件(一个礼物盒只能开一次)
        /// Prefix GiftBoxItem.OpenGiftBoxServerRpc
        /// </summary>
        [HarmonyPatch(typeof(GiftBoxItem), "__rpc_handler_2878544999")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static bool __rpc_handler_2878544999(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (Check(rpcParams, out var p))
            {
                if (Core.AntiCheat.Gift.Value)
                {
                    if (lhs.Contains(target.GetInstanceID()))
                    {
                        ShowMessage(locale.Msg_GetString("Gift", new Dictionary<string, string>() {
                            { "{player}",p.playerUsername }
                        }));
                        if (Core.AntiCheat.Gift2.Value)
                        {
                            KickPlayer(p);
                        }
                        return false;
                    }
                    else
                    {
                        lhs.Add(target.GetInstanceID());
                        var item = (GiftBoxItem)target;
                        item.OnNetworkDespawn();
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
        /// 使用面具生成假人事件(一个面具只能生成一个假人)
        /// Prefix HauntedMaskItem.CreateMimicServerRpc
        /// </summary>
        [HarmonyPatch(typeof(HauntedMaskItem), "__rpc_handler_1065539967")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static bool __rpc_handler_1065539967(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (Check(rpcParams, out var p))
            {
                if (Core.AntiCheat.Mask.Value)
                {
                    if (mjs.Contains(target.GetInstanceID()))
                    {
                        ShowMessage(locale.Msg_GetString("Mask", new Dictionary<string, string>() {
                            { "{player}",p.playerUsername }
                        }));
                        if (Core.AntiCheat.Mask2.Value)
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

        /// <summary>
        /// 检测是否需要处理事件(顺带处理掉SteamID为0的玩家)
        /// </summary>
        public static bool Check(__RpcParams rpcParams, out PlayerControllerB p)
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
            var tmp = GetPlayer(rpcParams);
            p = tmp;
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
            else if (StartOfRound.Instance.KickedClientIds.Contains(p.playerSteamId))//如果被踢
            {
                NetworkManager.Singleton.DisconnectClient(rpcParams.Server.Receive.SenderClientId);
                return false;
            }
            else if (p.playerSteamId == 0)
            {
                uint clientId = ClientIdToTransportId(rpcParams.Server.Receive.SenderClientId);
                if (clientId == 0)
                {
                    return false;
                }
                ulong steamId = ConnectionIdtoSteamIdMap[clientId];
                Friend f = new Friend(steamId);
                NetworkManager.Singleton.DisconnectClient(rpcParams.Server.Receive.SenderClientId);
                StartOfRound.Instance.KickedClientIds.Add(steamId);
                LogInfo($"检测玩家 {f.Name}({steamId}) 使用AntiKick功能，已自动踢出！");
                return false;
            }
            return true;
        }

        public static uint ClientIdToTransportId(ulong SenderClientId)
        {
            if (SenderClientId == 0)
            {
                return 0;
            }
            NetworkConnectionManager networkConnectionManager = Traverse.Create(NetworkManager.Singleton).Field("ConnectionManager").GetValue<NetworkConnectionManager>();
            ulong transportId = Traverse.Create(networkConnectionManager).Method("ClientIdToTransportId", new object[] { SenderClientId }).GetValue<ulong>();
            return (uint)transportId;
        }

        /// <summary>
        /// 降落时加载地图事件(显示反作弊信息)
        /// Prefix RoundManager.LoadNewLevel
        /// </summary>
        [HarmonyPatch(typeof(RoundManager), "LoadNewLevel")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static bool LoadNewLevel(SelectableLevel newLevel)
        {
            if (!StartOfRound.Instance.localPlayerController.IsHost)//非主机
            {
                return true;
            }
            landMines = new List<int>();
            bypassHit = new List<HitData>();
            bypassKill = new List<HitData>();
            HUDManager.Instance.AddTextToChatOnServer(locale.Msg_GetString("game_start", new Dictionary<string, string>() {
                { "{ver}",Core.AntiCheat.Version }
            }), -1);
            return true;
        }

        /// <summary>
        /// 玩家切换格子事件(拿了双手物品无法切换格子，他们本地客户端依旧可以)
        /// Prefix PlayerControllerB.SwitchItemSlotsServerRpc
        /// </summary>
        [HarmonyPatch(typeof(PlayerControllerB), "__rpc_handler_412259855")]
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
        /// 添加物品到腰包事件
        /// Prefix BeltBagItem.TryAddObjectToBagServerRpc
        /// </summary>
        [HarmonyPatch(typeof(BeltBagItem), "__rpc_handler_2988305002")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static bool __rpc_handler_2988305002(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (Check(rpcParams, out var p))
            {
                if (Core.AntiCheat.GrabObject.Value && Core.AntiCheat.GrabObject_BeltBag.Value)
                {
                    reader.ReadValueSafe(out NetworkObjectReference netObjectRef, default);
                    ByteUnpacker.ReadValueBitPacked(reader, out int playerWhoAdded);
                    reader.Seek(0);
                    if (netObjectRef.TryGet(out NetworkObject networkObject, null))
                    {
                        GrabbableObject component = networkObject.GetComponent<GrabbableObject>();
                        if (!component.itemProperties.isScrap && !component.isHeld && !component.isHeldByEnemy && component.itemProperties.itemId != 123984 && component.itemProperties.itemId != 819501)
                        {
                            return true;
                        }
                        ((BeltBagItem)target).CancelAddObjectToBagClientRpc(playerWhoAdded);
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
        /// 玩家捡起物品事件(用于检测多格子，单手拿双手物品，隔空取物)
        /// Prefix PlayerControllerB.GrabObjectServerRpc
        /// </summary>
        [HarmonyPatch(typeof(PlayerControllerB), "__rpc_handler_1554282707")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static bool __rpc_handler_1554282707(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (Check(rpcParams, out var p))
            {
                if (Core.AntiCheat.GrabObject.Value)
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
                            bool ban = false;
                            if (Core.AntiCheat.GrabObject_TwoHand.Value)
                            {
                                if (g.itemProperties.twoHanded && hastwohand)
                                {
                                    ban = true;
                                }
                                else if (g.itemProperties.twoHanded && jetpack)
                                {
                                    ban = true;
                                }
                                else if(hastwohand && jetpack)
                                {
                                    ban = true;
                                }
                            }
                            if (Core.AntiCheat.GrabObject_MoreSlot.Value)
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
                                if (Core.AntiCheat.GrabObject_MoreSlot.Value)
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
        //[HarmonyPatch(typeof(PlayerControllerB), "__rpc_handler_2013428264")]
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
                //var Jumping = PlayerJumping[p];
                //LogInfo($"{p.playerUsername} call PlayerControllerB.UpdatePlayerPositionServerRpc|newPos:{newPos}|Jumping:{Jumping}");
                //if (!Jumping)
                //{
                //    if (Physics.Raycast(p.transform.position, Vector3.down, out var raycastHit, 80f, 268437760, QueryTriggerInteraction.Ignore))
                //    {
                //        if (raycastHit.distance > 0.1)
                //        {
                //            ShowMessage($"检测到玩家 {p.playerUsername} 使用noclip！{raycastHit.distance}");
                //        }
                //    }
                //}
                if (Core.AntiCheat.Invisibility.Value)
                {
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
                        if (Core.AntiCheat.Invisibility2.Value)
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
        /// 记录本机SteamID
        /// Prefix NetworkManager.Awake
        /// </summary>
        [HarmonyPatch(typeof(NetworkManager), "Awake")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static void NetworkManagerAwake()
        {
            if (!GameNetworkManager.Instance.disableSteam)
            {
                ConnectionIdtoSteamIdMap[0] = SteamClient.SteamId;
            }
        }

        /// <summary>
        /// 代码来源 @Charlese2 HostFixes
        /// 客户端连接事件，获取真实的SteamID
        /// </summary>
        [HarmonyPatch(typeof(FacepunchTransport), "Steamworks.ISocketManager.OnConnecting")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static bool FacepunchTransportOnConnecting(ref Connection connection, ref ConnectionInfo info)
        {
            NetIdentity identity = Traverse.Create(info).Field<NetIdentity>("identity").Value;
            if (StartOfRound.Instance.KickedClientIds.Contains(identity.SteamId.Value))
            {
                LogInfo(locale.Log_GetString("refuse_connect", new Dictionary<string, string>() {
                    {"{steamId}",identity.SteamId.Value.ToString() }
                }));
                return false;
            }
            if (StartOfRound.Instance.allPlayerScripts.Any(x => x.isPlayerControlled && x.playerSteamId == identity.SteamId.Value))
            {
                LogInfo("{steamId} repeatedly joins the game.");
                return false;
            }
            if (ConnectionIdtoSteamIdMap.ContainsKey(connection.Id))
            {
                ConnectionIdtoSteamIdMap[connection.Id] = identity.SteamId.Value;
            }
            else
            {
                ConnectionIdtoSteamIdMap.Add(connection.Id, identity.SteamId.Value);
            }
            if (Core.AntiCheat.OperationLog.Value)
            {
                ShowMessageHostOnly(locale.OperationLog_GetString("JoinLobby", new Dictionary<string, string>() {
                    { "{player}",new Friend(ConnectionIdtoSteamIdMap[connection.Id]).Name }
                }));
            }
            return true;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="__result"></param>
        /// <returns></returns>
        [HarmonyPatch(typeof(NetworkConfig), "CompareConfig")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static bool NetworkConnectionManagerInitialize(ref bool __result)
        {
            if (StartOfRound.Instance != null && StartOfRound.Instance.IsHost)
            {
                if (Core.AntiCheat.IgnoreClientConfig.Value)
                {
                    __result = true;
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 代码来源 @Charlese2 HostFixes
        /// 客户端断开连接事件
        /// Prefix FacepunchTransport.OnDisconnected
        /// </summary>
        [HarmonyPatch(typeof(FacepunchTransport), "Steamworks.ISocketManager.OnDisconnected")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static void FacepunchTransportOnDisconnected(ref Connection connection, ref ConnectionInfo info)
        {
            if (NetworkManager.Singleton?.IsListening == true)
            {
                NetIdentity identity = Traverse.Create(info).Field<NetIdentity>("identity").Value;
                HUDManagerPatch.SyncAllPlayerLevelsServerRpcCalls.Remove(ConnectionIdtoSteamIdMap[connection.Id]);
                StartOfRoundPatch.SyncShipUnlockablesServerRpcCalls.Remove(ConnectionIdtoSteamIdMap[connection.Id]);
                StartOfRoundPatch.SyncAlreadyHeldObjectsServerRpcCalls.Remove(ConnectionIdtoSteamIdMap[connection.Id]);
                ConnectionIdtoSteamIdMap.Remove(connection.Id);
            }
        }

        ///// <summary>
        ///// 玩家发送SteamID事件(目前还没遇到过伪造)
        ///// Prefix PlayerControllerB.SendNewPlayerValuesServerRpc
        ///// </summary>
        //[HarmonyPatch(typeof(PlayerControllerB), "__rpc_handler_2504133785")]
        //[HarmonyPrefix]
        //[HarmonyWrapSafe]
        //public static bool __rpc_handler_2504133785(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        //{
        //    if (!StartOfRound.Instance.localPlayerController.IsHost)//非主机
        //    {
        //        return true;
        //    }
        //    if (rpcParams.Server.Receive.SenderClientId == 0)
        //    {
        //        return true;
        //    }
        //    ByteUnpacker.ReadValueBitPacked(reader, out ulong newPlayerSteamId);
        //    reader.Seek(0);
        //    ulong steamId = ConnectionIdtoSteamIdMap[ClientIdToTransportId(rpcParams.Server.Receive.SenderClientId)];
        //    if (newPlayerSteamId != steamId)
        //    {
        //        Friend f = new Friend(steamId);
        //        NetworkManager.Singleton.DisconnectClient(rpcParams.Server.Receive.SenderClientId);
        //        LogInfo($"玩家 {f.Name}({steamId}) 伪造SteamID({newPlayerSteamId})加入游戏");
        //        return false;
        //    }
        //    return true;
        //}

      

        /// <summary>
        /// Prefix HUDManager.AddTextMessageServerRpc
        /// </summary>
        [HarmonyPatch(typeof(HUDManager), "__rpc_handler_2787681914")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static bool __rpc_handler_2787681914(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (Check(rpcParams, out var p))
            {
                reader.ReadValueSafe(out bool flag, default);
                string chatMessage = null;
                if (flag)
                {
                    reader.ReadValueSafe(out chatMessage, false);
                }
                reader.Seek(0);
                LogInfo($"HUDManager.AddTextMessageServerRpc|{p.playerUsername}|{chatMessage}");
                if (chatMessage.Contains("<color") || chatMessage.Contains("<size"))
                {
                    if (Core.AntiCheat.Map.Value)
                    {
                        if (chatMessage.Contains("<size=0>Tyzeron.Minimap"))
                        {
                            ShowMessage(locale.Msg_GetString("Map", new Dictionary<string, string>() {
                                { "{player}",p.playerUsername }
                            }));
                            if (Core.AntiCheat.Map2.Value)
                            {
                                KickPlayer(p);
                            }
                            return false;
                        }
                    }
                    if (bypass)
                    {
                        LogInfo("bypass");
                        return true;
                    }
                    return false;
                }
                else if (chatMessage.StartsWith("[morecompanycosmetics]"))//bypass MoreCompany
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (p == null)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// HUDManager.AddPlayerChatMessageClientRpc
        /// </summary>
        [HarmonyPatch(typeof(HUDManager), "__rpc_handler_168728662")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static bool __rpc_handler_168728662(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            return __rpc_handler_2930587515(target, reader, rpcParams);
        }

        /// <summary>
        /// HUDManager.AddPlayerChatMessageServerRpc
        /// </summary>
        [HarmonyPatch(typeof(HUDManager), "__rpc_handler_2930587515")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static bool __rpc_handler_2930587515(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (Check(rpcParams, out var p))
            {
                if (Core.AntiCheat.ChatReal.Value)
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
                        LogInfo($"{p.playerUsername} call HUDManager.AddPlayerChatMessageServerRpc|chatMessage:{chatMessage}");
                        if (playerId == -1)
                        {
                            return false;
                        }
                        if (playerId <= StartOfRound.Instance.allPlayerScripts.Length)
                        {
                            if (StartOfRound.Instance.allPlayerScripts[(int)playerId].playerSteamId != p.playerSteamId)
                            {
                                ShowMessage(locale.Msg_GetString("ChatReal", new Dictionary<string, string>() {
                                    { "{player}",p.playerUsername },
                                    { "{player2}",StartOfRound.Instance.allPlayerScripts[playerId].playerUsername },
                                }));
                                if (Core.AntiCheat.ChatReal2.Value)
                                {
                                    KickPlayer(p);
                                }
                                return false;
                            }
                            else
                            {
                               return  CooldownManager.CheckCooldown("Chat", p);
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }
                    catch (Exception ex)
                    {
                        LogInfo(ex.ToString());
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

        public static T2 GetField<T, T2>(this T obj, string name)
        {
            return (T2)obj.GetType().GetField(name, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.GetField).GetValue(obj);
        }

        public static void SetField<T>(this T obj, string name, object value)
        {
            obj.GetType().GetField(name, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.GetField).SetValue(obj, value);
        }

        public static bool CheckRemoteTerminal(PlayerControllerB p,string call)
        {
            if (whoUseTerminal == null && lastWhoUseTerminal == p)
            {
                return true;
            }
            if (whoUseTerminal != p)
            {
                if (whoUseTerminal == null)
                {
                    LogInfo($"no player use terminal|request player:{p.playerUsername}|call:{call}");
                }
                else
                {
                    LogInfo($"whoUseTerminal:{whoUseTerminal.playerUsername}|p:{p.playerUsername}|call:{call}");
                }
                ShowMessage(locale.Msg_GetString("RemoteTerminal", new Dictionary<string, string>() {
                    { "{player}",p.playerUsername }
                }));
                if (Core.AntiCheat.RemoteTerminal2.Value)
                {
                    KickPlayer(p);
                }
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(InteractTrigger), "__rpc_handler_1430497838")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static bool __rpc_handler_1430497838(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (Check(rpcParams, out var p) || StartOfRound.Instance.IsHost)
            {
                ByteUnpacker.ReadValueBitPacked(reader, out int playerNum);
                reader.Seek(0);
                if (playerNum == (int)p.playerClientId)
                {
                    lastWhoUseTerminal = whoUseTerminal;
                    
                    var terminal = UnityEngine.Object.FindObjectOfType<Terminal>();
                    var terminalTrigger = terminal.GetField<Terminal, InteractTrigger>("terminalTrigger");
                    if (terminalTrigger.GetInstanceID() == ((InteractTrigger)target).GetInstanceID())
                    {
                        whoUseTerminal = p;
                        LogInfo($"player {p.playerUsername} use Terminal");
                    }
                }
            }
            else if (p == null)
            {
                return false;
            }
            return true;
        }

        [HarmonyPatch(typeof(InteractTrigger), "__rpc_handler_880620475")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static bool __rpc_handler_880620475(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (Check(rpcParams, out var p) || StartOfRound.Instance.IsHost)
            {
                ByteUnpacker.ReadValueBitPacked(reader, out int playerNum);
                reader.Seek(0);
                if (playerNum == (int)p.playerClientId)
                {
                    if (p.isPlayerDead)
                    {
                        LogInfo($"player {p.playerUsername} death can't use terminal");
                        return false;
                    }
                    var terminal = UnityEngine.Object.FindObjectOfType<Terminal>();
                    var terminalTrigger = terminal.GetField<Terminal, InteractTrigger>("terminalTrigger");
                    if (terminalTrigger.GetInstanceID() == ((InteractTrigger)target).GetInstanceID())
                    {
                        whoUseTerminal = null;
                        LogInfo($"player {p.playerUsername} stop use Terminal");
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
        /// 终端噪音限制
        /// Prefix Terminal.PlayTerminalAudioServerRpc
        /// </summary>
        [HarmonyPatch(typeof(Terminal), "__rpc_handler_1713627637")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static bool __rpc_handler_1713627637(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (Check(rpcParams, out var p))
            {
                if (Core.AntiCheat.RemoteTerminal.Value)
                {
                    if (!CheckRemoteTerminal(p, "Terminal.PlayTerminalAudioServerRpc"))
                    {
                        return false;
                    }
                }
                return CooldownManager.CheckCooldown("TerminalNoise", p);
            }
            else if (p == null)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 飞船灯开关速度限制
        /// Prefix ShipLights.SetShipLightsServerRpc
        /// </summary>
        [HarmonyPatch(typeof(ShipLights), "__rpc_handler_1625678258")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static bool __rpc_handler_1625678258(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (Check(rpcParams, out var p))
            {
                return CooldownManager.CheckCooldown("ShipLight", p);
            }
            else if (p == null)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 通过ClientId找到调用RPC的玩家
        /// </summary>
        /// <param name="rpcParams"></param>
        /// <returns></returns>
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

        public static Dictionary<ulong, bool> ReloadGun = new Dictionary<ulong, bool>();

        /// <summary>
        /// 上弹事件
        /// Prefix ShotgunItem.ReloadGunEffectsServerRpc
        /// </summary>
        [HarmonyPatch(typeof(ShotgunItem), "__rpc_handler_3349119596")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static bool __rpc_handler_3349119596(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (Check(rpcParams, out var p))
            {
                if (Core.AntiCheat.InfiniteAmmo.Value)
                {
                    reader.ReadValueSafe(out bool start, default);
                    reader.Seek(0);
                    LogInfo($"{p.playerUsername} call ShotgunItem.ReloadGunEffectsServerRpc({start})");
                    if (!ReloadGun.ContainsKey(p.playerSteamId))
                    {
                        ReloadGun.Add(p.playerSteamId, start);
                    }
                    else
                    {
                        ReloadGun[p.playerSteamId] = start;
                    }
                    if (start)
                    {
                        var shot = (ShotgunItem)target;
                        var ammo = shot.playerHeldBy.ItemSlots.FirstOrDefault(x => x is GunAmmo ga && ga.ammoType == shot.gunCompatibleAmmoID);
                        if (ammo != default)
                        {
                            shot.StartCoroutine(CheckAmmo(p, shot, ammo));
                            return true;
                        }
                        else
                        {
                            ShowMessage(locale.Msg_GetString("InfiniteAmmo", new Dictionary<string, string>() {
                                { "{player}",p.playerUsername }
                            }));
                            if (Core.AntiCheat.InfiniteAmmo2.Value)
                            {
                                KickPlayer(p);
                            }
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

        public static IEnumerator CheckAmmo(PlayerControllerB p, ShotgunItem shot, GrabbableObject ammo)
        {
            yield return new WaitForSeconds(0.95f + 0.3f);
            yield return new WaitForSeconds(3f);//delay
            if (ReloadGun.ContainsKey(p.playerSteamId) && !ReloadGun[p.playerSteamId])
            {
                if (ammo != null && ammo.NetworkObject != null && ammo.NetworkObject.IsSpawned)
                {
                    ShowMessage(locale.Msg_GetString("InfiniteAmmo", new Dictionary<string, string>() {
                    { "{player}",p.playerUsername }
                }));
                    if (Core.AntiCheat.InfiniteAmmo2.Value)
                    {
                        KickPlayer(p);
                    }
                }
            }
        }



        /// <summary>
        /// 开枪事件
        /// Prefix ShotgunItem.ShootGunServerRpc
        /// </summary>
        [HarmonyPatch(typeof(ShotgunItem), "__rpc_handler_1329927282")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static bool __rpc_handler_1329927282(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (Check(rpcParams, out var p) || StartOfRound.Instance.localPlayerController.IsHost)
            {
                if (Core.AntiCheat.InfiniteAmmo.Value)
                {
                    var s = (ShotgunItem)target;
                    var localClientSendingShootGunRPC = (bool)typeof(ShotgunItem).GetField("localClientSendingShootGunRPC", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(s);
                    if (localClientSendingShootGunRPC)
                    {
                        return true;
                    }
                    else
                    {
                        LogInfo($"{p.playerUsername} call ShotgunItem.ShootGunServerRpc|ShotgunItem.shellsLoaded:{s.shellsLoaded}");
                        if (s.shellsLoaded == 0)
                        {
                            ShowMessage(locale.Msg_GetString("InfiniteAmmo", new Dictionary<string, string>() {
                                { "{player}",p.playerUsername }
                            }));
                            if (Core.AntiCheat.InfiniteAmmo2.Value)
                            {
                                KickPlayer(p);
                            }
                            return false;
                        }
                    }
                }
                if (Core.AntiCheat.ItemCooldown.Value)
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
                        ShowMessage(locale.Msg_GetString("ItemCooldown", new Dictionary<string, string>() {
                            { "{player}",p.playerUsername },
                            { "{item}",locale.Item_GetString("Shotgun") }
                        }));
                        if (Core.AntiCheat.ItemCooldown2.Value)
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
            }
            else if (p == null)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 挥刀事件
        /// Prefix KnifeItem.HitShovelServerRpc
        /// </summary>
        [HarmonyPatch(typeof(KnifeItem), "__rpc_handler_2696735117")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static bool __rpc_handler_2696735117(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            return CheckCoolDownMethod(rpcParams, 10);
        }

        /// <summary>
        /// 挥铲事件
        /// Prefix Shovel.HitShovelServerRpc
        /// </summary>
        [HarmonyPatch(typeof(Shovel), "__rpc_handler_2096026133")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static bool __rpc_handler_2096026133(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            return CheckCoolDownMethod(rpcParams, 3);
        }

        private static bool CheckCoolDownMethod(__RpcParams rpcParams, int cd)
        {
            if (Check(rpcParams, out var p))
            {
                if (Core.AntiCheat.ItemCooldown.Value)
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
                    if (czcd[id].Count(x => x == m) >= cd)
                    {
                        ShowMessage(locale.Msg_GetString("ItemCooldown", new Dictionary<string, string>() {
                            { "{player}",p.playerUsername },
                            { "{item}",locale.Item_GetString("Shovel") }
                        }));
                        if (Core.AntiCheat.ItemCooldown2.Value)
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
            }
            else if (p == null)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 玩家拉杆事件
        /// Prefix StartOfRound.__rpc_handler_1089447320
        /// </summary>
        [HarmonyPatch(typeof(StartOfRound), "__rpc_handler_1089447320")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static bool StartGameServerRpc(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            StartMatchLever startMatchLever = UnityEngine.Object.FindObjectOfType<StartMatchLever>();
            if (Check(rpcParams, out var p))
            {
                if (Core.AntiCheat.ShipConfig.Value && !GameNetworkManager.Instance.gameHasStarted)
                {
                    ShowMessage(locale.Msg_GetString("ShipConfig5", new Dictionary<string, string>() {
                        { "{player}",p.playerUsername }
                    }));
                    startMatchLever.triggerScript.interactable = true;
                    if (Core.AntiCheat.Ship_Kick.Value)
                    {
                        KickPlayer(p);
                        return false;
                    }
                    return false;
                }
                else if (StartOfRound.Instance.allPlayerScripts.Count(x => x.isPlayerControlled) >= Core.AntiCheat.ShipConfig2.Value)
                {
                    return true;
                }
                else
                {
                    startMatchLever.triggerScript.interactable = true;
                    ShowMessage(locale.Msg_GetString("ShipConfig2", new Dictionary<string, string>() {
                        { "{player}",p.playerUsername },
                        { "{cfg}",Core.AntiCheat.ShipConfig2.Value.ToString() }
                    }));
                    return false;
                }
            }
            else if (p == null)
            {
                startMatchLever.triggerScript.interactable = true;
                return false;
            }
            return true;
        }

        /// <summary>
        /// 玩家断开连接时清空SteamId(防止游戏缓存)
        /// Postfix StartOfRound.OnPlayerDC
        /// </summary>
        [HarmonyPatch(typeof(StartOfRound), "OnPlayerDC")]
        [HarmonyPostfix]
        [HarmonyWrapSafe]
        public static void OnPlayerDC(int playerObjectNumber, ulong clientId)
        {
            PlayerControllerB component = StartOfRound.Instance.allPlayerObjects[playerObjectNumber].GetComponent<PlayerControllerB>();
            component.playerSteamId = 0;
            return;
        }

        public static PlayerControllerB lastWhoUseTerminal { get; set; } = null;
        public static PlayerControllerB whoUseTerminal { get; set; }

        /// <summary>
        /// 踢出玩家
        /// </summary>
        /// <param name="kick">玩家</param>
        /// <param name="canJoin">重新加入</param>
        public static void KickPlayer(PlayerControllerB kick, bool canJoin = false, string Reason = null)
        {
            if (kick.actualClientId == 0)
            {
                return;
            }
            NetworkManager.Singleton.DisconnectClient(kick.actualClientId, $"[{locale.Prefix()}] {locale.Msg_GetString("KickPlayer")}！");
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
            LogInfo("terminalInUse" + terminal.placeableObject.inUse);
            LogInfo("whoUseTerminal:" + whoUseTerminal?.playerUsername);
            LogInfo("playerUsername:" + kick?.playerUsername);
            if (whoUseTerminal == kick && terminal.placeableObject.inUse)
            {
                LogInfo("SetTerminalInUseServerRpc");
                terminal.SetTerminalInUseClientRpc(false);
                terminal.terminalInUse = false;
            }
            ShowMessage(locale.Msg_GetString("Kick", new Dictionary<string, string>() {
                { "{player}",kick.playerUsername }
            }));
        }



        /// <summary>
        /// 拉杆事件(正常拉杆都要经过这一层)
        /// Prefix StartMatchLever.PlayLeverPullEffectsServerRpc
        /// </summary>
        [HarmonyPatch(typeof(StartMatchLever), "__rpc_handler_2406447821")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static bool __rpc_handler_2406447821(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (File.Exists("AntiCheat.log"))
            {
                File.Delete("AntiCheat.log");
            }
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
                UnityEngine.Object.FindObjectOfType<StartMatchLever>().triggerScript.interactable = true;
                return false;
            }
            return true;
        }


        /// <summary>
        /// 出售完货物事件(用于更新反作弊的记录金钱)
        /// Postfix DepositItemsDesk.SellAndDisplayItemProfits
        /// </summary>
        [HarmonyPatch(typeof(DepositItemsDesk), "SellAndDisplayItemProfits")]
        [HarmonyPostfix]
        [HarmonyWrapSafe]
        public static void SellAndDisplayItemProfits(int profit, int newGroupCredits)
        {
            if (!StartOfRound.Instance.localPlayerController.IsHost)
            {
                return;
            }
            Money = newGroupCredits;
            LogInfo($"SetMoney:{Money}");
        }

        /// <summary>
        /// 玩家销毁物品事件(允许销毁礼物盒和钥匙)
        /// Prefix PlayerControllerB.DespawnHeldObjectServerRpc
        /// </summary>
        [HarmonyPatch(typeof(PlayerControllerB), "__rpc_handler_1786952262")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static bool __rpc_handler_1786952262(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (Check(rpcParams, out var p))
            {
                if (Core.AntiCheat.DespawnItem.Value)
                {
                    if (p.currentlyHeldObjectServer != null && !(p.currentlyHeldObjectServer is GiftBoxItem) && !(p.currentlyHeldObjectServer is KeyItem))
                    {
                        ShowMessage(locale.Msg_GetString("DespawnItem", new Dictionary<string, string>() {
                            { "{player}",p.playerUsername },
                            { "{item}",p.currentlyHeldObjectServer.itemProperties.itemName }
                        }));
                        if (Core.AntiCheat.DespawnItem2.Value)
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
        /// UI更新事件(房主死亡时加上一票起飞提示)
        /// Postfix HUDManager.Update
        /// </summary>
        [HarmonyPatch(typeof(HUDManager), "Update")]
        [HarmonyPostfix]
        [HarmonyWrapSafe]
        public static void Update()
        {
            if (!StartOfRound.Instance.IsHost)
            {
                return;
            }
            if (GameNetworkManager.Instance == null || GameNetworkManager.Instance.localPlayerController == null)
            {
                return;
            }
            if (StartOfRound.Instance.shipIsLeaving || !StartOfRound.Instance.currentLevel.planetHasTime)
            {
                return;
            }
            if (Core.AntiCheat.ShipSetting_OnlyOneVote.Value)
            {
                if (!TimeOfDay.Instance.shipLeavingAlertCalled)
                {
                    if (GameNetworkManager.Instance.localPlayerController.isPlayerDead && !string.IsNullOrEmpty(HUDManager.Instance.holdButtonToEndGameEarlyVotesText.text))
                    {
                        HUDManager.Instance.holdButtonToEndGameEarlyVotesText.text += Environment.NewLine + locale.Msg_GetString("vote");
                    }
                }
            }
        }


        /// <summary>
        /// 死亡玩家投票事件(这里处理房主一票起飞)
        /// Postfix TimeOfDay.SetShipLeaveEarlyServerRpc
        /// </summary>
        [HarmonyPatch(typeof(TimeOfDay), "__rpc_handler_543987598")]
        [HarmonyPostfix]
        [HarmonyWrapSafe]
        public static void Postfix__rpc_handler_543987598(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (StartOfRound.Instance.localPlayerController.IsHost)
            {
                if (Core.AntiCheat.ShipSetting_OnlyOneVote.Value)
                {
                    if (rpcParams.Server.Receive.SenderClientId == 0)
                    {
                        TimeOfDay.Instance.SetShipLeaveEarlyClientRpc(TimeOfDay.Instance.normalizedTimeOfDay + 0.1f, StartOfRound.Instance.allPlayerScripts.Length);
                    }
                }
            }
        }

        /// <summary>
        /// 死亡玩家投票事件(将事件转发到起飞拉杆事件)
        /// Prefix TimeOfDay.SetShipLeaveEarlyServerRpc
        /// </summary>
        [HarmonyPatch(typeof(TimeOfDay), "__rpc_handler_543987598")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static bool __rpc_handler_543987598(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (Check(rpcParams, out var p))
            {
                int num = StartOfRound.Instance.connectedPlayersAmount + 1 - StartOfRound.Instance.livingPlayers;
                string msg = locale.Msg_GetString("vote_player", new Dictionary<string, string>() {
                    { "{player}",p.playerUsername },
                    { "{now}",(TimeOfDay.Instance.votesForShipToLeaveEarly + 1).ToString() },
                    { "{max}",num.ToString() }
                });
                ShowMessageHostOnly(msg);
                if (TimeOfDay.Instance.votesForShipToLeaveEarly + 1 >= num)
                {
                    LogInfo("Vote EndGame");
                    return EndGameServerRpc(target, reader, rpcParams);
                }
            }
            else if (p == null)
            {
                return false;
            }
            return true;
        }

        public static void ShowMessageHostOnly(string msg)
        {
            AccessTools.DeclaredMethod(typeof(HUDManager), "AddChatMessage").Invoke(HUDManager.Instance, new object[] { msg, "", -1, false });
        }

        /// <summary>
        /// 起飞拉杆事件
        /// Prefix StartOfRound.EndGameServerRpc
        /// </summary>
        [HarmonyPatch(typeof(StartOfRound), "__rpc_handler_2028434619")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static bool EndGameServerRpc(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (Check(rpcParams, out var p))
            {
                ByteUnpacker.ReadValueBitPacked(reader, out int num);
                reader.Seek(0);
                LogInfo($"StartOfRound.EndGameServerRpc({num})|StartOfRound.Instance.shipHasLanded:{StartOfRound.Instance.shipHasLanded}");
                if (num == 0 && p.actualClientId != 0)
                {
                    UnityEngine.Object.FindObjectOfType<StartMatchLever>().triggerScript.interactable = true;
                    return false;
                }
                if (!StartOfRound.Instance.shipHasLanded)
                {
                    UnityEngine.Object.FindObjectOfType<StartMatchLever>().triggerScript.interactable = true;
                    return false;
                }
                if (TimeOfDay.Instance.shipLeavingAlertCalled)
                {
                    return true;
                }
                var hour = int.Parse(Core.AntiCheat.ShipConfig3.Value.Split(':')[0]);
                var min = int.Parse(Core.AntiCheat.ShipConfig3.Value.Split(':')[1]);
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
                var live = StartOfRound.Instance.allPlayerScripts.Where(x => x.isPlayerControlled && !x.isPlayerDead);

                if (live.Count() == 1 && live.FirstOrDefault().isInHangarShipRoom)
                {
                    return true;
                }
                decimal p1 = Math.Round((decimal)live.Count() * (decimal)(Core.AntiCheat.ShipConfig4.Value / 100m), 2);
                decimal p2 = StartOfRound.Instance.allPlayerScripts.Count(x => x.isPlayerControlled && x.isInHangarShipRoom); // 
                if (StartOfRound.Instance.currentLevel.PlanetName.Contains("Gordion"))
                {
                    time2 = hour;
                    time = min;
                }
                if (hour <= time2 && min <= time && p2 >= p1)
                {
                    return true;
                }
                else
                {
                    ShowMessage(locale.Msg_GetString("ShipConfig4", new Dictionary<string, string>() {
                        { "{player}",p.playerUsername },
                        { "{player_count}",p1.ToString() },
                        { "{cfg4}",Core.AntiCheat.ShipConfig4.Value.ToString() },
                        { "{cfg3}",$"{hour.ToString("00")}:{min.ToString("00")}" },
                        { "{game_time}",$"{time2.ToString("00")}:{time.ToString("00")}" }
                    }));
                    UnityEngine.Object.FindObjectOfType<StartMatchLever>().triggerScript.interactable = true;
                    return false;
                }
            }
            else if (p == null)
            {
                UnityEngine.Object.FindObjectOfType<StartMatchLever>().triggerScript.interactable = true;
                return false;
            }
            return true;
        }

        /// <summary>
        /// 放置飞船装饰事件(检测)
        /// Prefix ShipBuildModeManager.PlaceShipObjectServerRpc
        /// </summary>
        [HarmonyPatch(typeof(ShipBuildModeManager), "__rpc_handler_861494715")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static bool __rpc_handler_861494715(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (Check(rpcParams, out var p) || StartOfRound.Instance.IsHost)
            {
                if (Core.AntiCheat.ShipBuild.Value)
                {
                    Vector3 newPosition;
                    reader.ReadValueSafe(out newPosition);
                    Vector3 newRotation;
                    reader.ReadValueSafe(out newRotation);
                    reader.ReadValueSafe(out NetworkObjectReference objectRef, default);
                    reader.Seek(0);
                    if (objectRef.TryGet(out var networkObject, null))
                    {
                        PlaceableShipObject placingObject = networkObject.gameObject.GetComponentInChildren<PlaceableShipObject>();
                        //LogInfo($"newRotation:{newRotation}|mainMesh:{placingObject.mainMesh.transform.eulerAngles.ToString()}");
                        if (Math.Floor(newRotation.x) != Math.Floor(placingObject.mainMesh.transform.eulerAngles.x) || Math.Floor(newRotation.z) != Math.Floor(placingObject.mainMesh.transform.eulerAngles.z))
                        {
                            ShowMessage(locale.Msg_GetString("ShipBuild", new Dictionary<string, string>() {
                                    { "{player}",p.playerUsername },
                                    { "{position}",newRotation.ToString() },
                                    { "{object}",placingObject.parentObject.name }
                            }), locale.Msg_GetString("ShipBuild"));
                            if (Core.AntiCheat.ShipBuild2.Value)
                            {
                                KickPlayer(p);
                            }
                            return false;
                        }
                        var ShipBuildModeManager = (ShipBuildModeManager)target;
                        //LogInfo($"{p.playerUsername}|ShipBuildModeManager.PlaceShipObjectServerRpc|placingObject:{placingObject.parentObject.name},{placingObject.unlockableID}|newPosition:{newPosition}|newRotation:{newRotation}");
                        if (!StartOfRound.Instance.shipInnerRoomBounds.bounds.Contains(newPosition))
                        {
                            ShowMessage(locale.Msg_GetString("ShipBuild", new Dictionary<string, string>() {
                                    { "{player}",p.playerUsername },
                                    { "{position}",newPosition.ToString() }
                            }), locale.Msg_GetString("ShipBuild"));
                            if (Core.AntiCheat.ShipBuild2.Value)
                            {
                                KickPlayer(p);
                            }
                            return false;
                        }
                    }
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
        /// 离开地雷事件(记录是否触发过)
        /// Prefix Landmine.OnTriggerExit
        /// </summary>
        [HarmonyPatch(typeof(Landmine), "OnTriggerExit")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static bool OnTriggerExit(Landmine __instance, Collider other)
        {
            if (!StartOfRound.Instance.IsHost)
            {
                return true;
            }
            if (__instance.hasExploded)
            {
                return true;
            }
            bool mineActivated = (bool)typeof(Landmine).GetField("mineActivated", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(__instance);
            if (!mineActivated)
            {
                return true;
            }
            int id = __instance.GetInstanceID();
            if (!landMines.Contains(id))
            {
                landMines.Add(id);
            }
            LogInfo($"Landmine.OnTriggerExit({id})");
            return true;
        }

        /// <summary>
        /// 地雷引爆事件(检测)
        /// Prefix Landmine.ExplodeMineServerRpc
        /// </summary>
        [HarmonyPatch(typeof(Landmine), "__rpc_handler_3032666565")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static bool __rpc_handler_3032666565(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (Check(rpcParams, out var p))
            {
                if (Core.AntiCheat.Landmine.Value)
                {
                    var lm = (Landmine)target;
                    int id = lm.GetInstanceID();
                    if (!landMines.Contains(id))
                    {
                        ShowMessage(locale.Msg_GetString("Landmine", new Dictionary<string, string>() {
                             { "{player}",p.playerUsername }
                        }));
                        if (Core.AntiCheat.Landmine2.Value)
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
        /// 老板激怒事件(正常被激怒只有主机才会调用SeverRpc，客户端调用就是有问题)
        /// Prefix DepositItemsDesk.AttackPlayersServerRpc
        /// </summary>
        [HarmonyPatch(typeof(DepositItemsDesk), "__rpc_handler_3230280218")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static bool __rpc_handler_3230280218(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (Check(rpcParams, out var p))
            {
                if (Core.AntiCheat.Boss.Value)
                {
                    ShowMessage(locale.Msg_GetString("Boss", new Dictionary<string, string>() {
                         { "{player}",p.playerUsername }
                    }));
                    if (Core.AntiCheat.Boss2.Value)
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

        /// <summary>
        /// 自动添加AC标识
        /// Prefix GameNetworkManager.StartHost
        /// </summary>
        [HarmonyPatch(typeof(GameNetworkManager), "StartHost")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static void StartHost()
        {
            if (Core.AntiCheat.Prefix.Value.IsNullOrWhiteSpace())
            {
                return;
            }
            var setting = GameNetworkManager.Instance.lobbyHostSettings;
            string rawText = setting.lobbyName;
            rawText = rawText.Replace("【", "[").Replace("】", "]");
            List<string> labels = new List<string>();
            var match = Regex.Match(rawText, "^\\[(.*?)\\]");
            if (match.Success)
            {
                var txt = match.Groups[1].Value;
                labels.AddRange(txt.Split('/'));
                rawText = rawText.Remove(0, match.Groups[0].Value.Length).TrimStart();
            }
            if (!labels.Any(x => x == Core.AntiCheat.Prefix.Value))
            {
                labels.Add(Core.AntiCheat.Prefix.Value);
            }
            setting.lobbyName = "[" + string.Join("/", labels) + "]" + " " + rawText;
        }
    }
}
