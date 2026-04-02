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
    [HarmonyPatch(typeof(Terminal))]
    [HarmonyWrapSafe]
    public static class TerminalMigrationPatch
    {

        [HarmonyPatch("SyncGroupCreditsClientRpc")]
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

        [HarmonyPatch("BeginUsingTerminal")]
        [HarmonyPrefix]
        public static bool BeginUsingTerminal(Terminal __instance)
        {
            if (!StartOfRound.Instance.IsHost)
            {
                return true;
            }
            Money = __instance.groupCredits;
            LogInfo($"SetMoney:{Money}");
            return true;
        }


        /// <summary>
        /// 终端噪音限制
        /// Prefix Terminal.PlayTerminalAudioServerRpc
        /// </summary>
        [HarmonyPatch("__rpc_handler_1713627637")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static bool __rpc_handler_1713627637(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (Check(rpcParams, out var p))
            {
                if (PluginConfig.RemoteTerminal.Value)
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
    }
}
