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
    [HarmonyPatch(typeof(TimeOfDay))]
    [HarmonyWrapSafe]
    public static class TimeOfDayMigrationPatch
    {



        [HarmonyPatch("SyncNewProfitQuotaClientRpc")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static bool SyncNewProfitQuotaClientRpc(int newProfitQuota, int overtimeBonus, int fulfilledQuota)
        {
            if (!StartOfRound.Instance.localPlayerController.IsHost)
            {
                return true;
            }
            Money = Mathf.Clamp(terminal.groupCredits + overtimeBonus, terminal.groupCredits, 100000000);
            return true;
        }



        /// <summary>
        /// 死亡玩家投票事件(这里处理房主一票起飞)
        /// Postfix TimeOfDay.SetShipLeaveEarlyServerRpc
        /// </summary>
        [HarmonyPatch("__rpc_handler_543987598")]
        [HarmonyPostfix]
        [HarmonyWrapSafe]
        public static void Postfix__rpc_handler_543987598(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (StartOfRound.Instance.localPlayerController.IsHost)
            {
                if (PluginConfig.ShipSetting_OnlyOneVote.Value)
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
        [HarmonyPatch("__rpc_handler_543987598")]
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
                    return StartOfRoundMigrationPatch.EndGameServerRpc(target, reader, rpcParams);
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
