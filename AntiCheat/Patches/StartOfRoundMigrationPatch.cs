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
    [HarmonyPatch(typeof(StartOfRound))]
    [HarmonyWrapSafe]
    public static class StartOfRoundMigrationPatch
    {

        /// <summary>
        /// 游戏结束时重置所有变量
        /// StartOfRound.EndOfGame
        /// </summary>
        [HarmonyPatch("EndOfGame")]
        [HarmonyPostfix]
        [HarmonyWrapSafe]
        public static void EndOfGame()
        {
            jcs = new List<ulong>();
            //chcs = new Dictionary<int, Dictionary<ulong, List<DateTime>>>();
            if (rpcs.ContainsKey("Hit"))
            {
                rpcs["Hit"] = new List<ulong>();
            }
            if (rpcs.ContainsKey("KillPlayer"))
            {
                rpcs["KillPlayer"] = new List<ulong>();
            }
        }


        /// <summary>
        /// Prefix StartOfRound.ChangeLevelServerRpc
        /// </summary>
        [HarmonyPatch("__rpc_handler_1134466287")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static bool __rpc_handler_1134466287(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (Check(rpcParams, out var p))
            {
                //LogInfo($"{p.playerUsername}|StartOfRound.ChangeLevelServerRpc");
                if (PluginConfig.RemoteTerminal.Value)
                {
                    if (!CheckRemoteTerminal(p, "StartOfRound.ChangeLevelServerRpc"))
                    {
                        return false;
                    }
                }
                ByteUnpacker.ReadValueBitPacked(reader, out int levelID);
                ByteUnpacker.ReadValueBitPacked(reader, out int newGroupCreditsAmount);
                reader.Seek(0);
                if (PluginConfig.FreeBuy.Value)
                {
                    if (newGroupCreditsAmount > Money || Money < 0)
                    {
                        ShowMessage(locale.Msg_GetString("FreeBuy_SetMoney", new Dictionary<string, string>() {
                            { "{player}",p.playerUsername },
                            { "{Money}",(newGroupCreditsAmount - Money).ToString() }
                        }));
                        if (PluginConfig.FreeBuy2.Value)
                        {
                            KickPlayer(p);
                        }
                        return false;
                    }
                    if (levelID > StartOfRound.Instance.levels.Length)
                    {
                        LogInfo(p, "StartOfRound.ChangeLevelServerRpc", "levelID > StartOfRound.Instance.levels.Length");
                        return false;
                    }
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
                        LogInfo(p, "StartOfRound.ChangeLevelServerRpc", $"levelID:{levelID}", $"itemCost:{itemCost}");
                        if (itemCost != 0)
                        {
                            int newValue = Money - itemCost;
                            if (newValue != newGroupCreditsAmount || Money == 0)
                            {
                                ShowMessage(locale.Msg_GetString("FreeBuy_Level", new Dictionary<string, string>() {
                                    { "{player}",p.playerUsername }
                                }));
                                if (PluginConfig.FreeBuy2.Value)
                                {
                                    KickPlayer(p);
                                }
                                return false;
                            }
                        }
                    }
                }
                if (PluginConfig.OperationLog.Value)
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
        [HarmonyPatch("__rpc_handler_3953483456")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static bool __rpc_handler_3953483456(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (Check(rpcParams, out var p))
            {
                if (PluginConfig.RemoteTerminal.Value)
                {
                    if (!CheckRemoteTerminal(p, "StartOfRound.BuyShipUnlockableServerRpc"))
                    {
                        return false;
                    }
                }
                ByteUnpacker.ReadValueBitPacked(reader, out int unlockableID);
                ByteUnpacker.ReadValueBitPacked(reader, out int newGroupCreditsAmount);
                reader.Seek(0);
                if (PluginConfig.FreeBuy.Value)
                {
                    //LogInfo($"__rpc_handler_3953483456|newGroupCreditsAmount:{newGroupCreditsAmount}");
                    if (Money == newGroupCreditsAmount || Money == 0)
                    {
                        //LogInfo($"Money:{Money}|newGroupCreditsAmount:{newGroupCreditsAmount}");
                        ShowMessage(locale.Msg_GetString("FreeBuy_unlockable", new Dictionary<string, string>() {
                            { "{player}",p.playerUsername }
                        }));
                        if (PluginConfig.FreeBuy2.Value)
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
                        if (PluginConfig.FreeBuy2.Value)
                        {
                            KickPlayer(p);
                        }
                        return false;
                    }
                }
                if (PluginConfig.OperationLog.Value)
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






        [HarmonyPatch("BuyShipUnlockableClientRpc")]
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


        [HarmonyPatch("ChangeLevelClientRpc")]
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



        /// <summary>
        /// 玩家拉杆事件
        /// Prefix StartOfRound.__rpc_handler_1089447320
        /// </summary>
        [HarmonyPatch("__rpc_handler_1089447320")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static bool StartGameServerRpc(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            StartMatchLever startMatchLever = UnityEngine.Object.FindObjectOfType<StartMatchLever>();
            if (Check(rpcParams, out var p))
            {
                if (PluginConfig.ShipConfig.Value && !GameNetworkManager.Instance.gameHasStarted)
                {
                    ShowMessage(locale.Msg_GetString("ShipConfig5", new Dictionary<string, string>() {
                        { "{player}",p.playerUsername }
                    }));
                    startMatchLever.triggerScript.interactable = true;
                    if (PluginConfig.Ship_Kick.Value)
                    {
                        KickPlayer(p);
                        return false;
                    }
                    return false;
                }
                else if (StartOfRound.Instance.allPlayerScripts.Count(x => x.isPlayerControlled) >= PluginConfig.ShipConfig2.Value)
                {
                    return true;
                }
                else
                {
                    startMatchLever.triggerScript.interactable = true;
                    ShowMessage(locale.Msg_GetString("ShipConfig2", new Dictionary<string, string>() {
                        { "{player}",p.playerUsername },
                        { "{cfg}",PluginConfig.ShipConfig2.Value.ToString() }
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
        [HarmonyPatch("OnPlayerDC")]
        [HarmonyPostfix]
        [HarmonyWrapSafe]
        public static void OnPlayerDC(int playerObjectNumber, ulong clientId)
        {
            PlayerControllerB component = StartOfRound.Instance.allPlayerObjects[playerObjectNumber].GetComponent<PlayerControllerB>();
            component.playerSteamId = 0;
            return;
        }


        /// <summary>
        /// 起飞拉杆事件
        /// Prefix StartOfRound.EndGameServerRpc
        /// </summary>
        [HarmonyPatch("__rpc_handler_2028434619")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static bool EndGameServerRpc(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (Check(rpcParams, out var p))
            {
                ByteUnpacker.ReadValueBitPacked(reader, out int num);
                reader.Seek(0);
                LogInfo(p, "StartOfRound.EndGameServerRpc", $"num:{num}", $"shipHasLanded:{StartOfRound.Instance.shipHasLanded}");
                if (num == 0 && p.playerClientId != 0)
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
                var hour = int.Parse(PluginConfig.ShipConfig3.Value.Split(':')[0]);
                var min = int.Parse(PluginConfig.ShipConfig3.Value.Split(':')[1]);
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
                decimal p1 = Math.Round((decimal)live.Count() * (decimal)(PluginConfig.ShipConfig4.Value / 100m), 2);
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
                        { "{cfg4}",PluginConfig.ShipConfig4.Value.ToString() },
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
    }
}
