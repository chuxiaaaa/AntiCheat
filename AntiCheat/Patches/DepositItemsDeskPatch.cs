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
    [HarmonyPatch(typeof(DepositItemsDesk))]
    [HarmonyWrapSafe]
    public static class DepositItemsDeskPatch
    {


        /// <summary>
        /// 出售完货物事件(用于更新反作弊的记录金钱)
        /// Postfix DepositItemsDesk.SellAndDisplayItemProfits
        /// </summary>
        [HarmonyPatch("SellAndDisplayItemProfits")]
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
        /// 老板激怒事件(正常被激怒只有主机才会调用SeverRpc，客户端调用就是有问题)
        /// Prefix DepositItemsDesk.AttackPlayersServerRpc
        /// </summary>
        [HarmonyPatch("__rpc_handler_3230280218")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static bool __rpc_handler_3230280218(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (Check(rpcParams, out var p))
            {
                if (PluginConfig.Boss.Value)
                {
                    ShowMessage(locale.Msg_GetString("Boss", new Dictionary<string, string>() {
                         { "{player}",p.playerUsername }
                    }));
                    if (PluginConfig.Boss2.Value)
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
