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
    [HarmonyPatch(typeof(InteractTrigger))]
    [HarmonyWrapSafe]
    public static class InteractTriggerPatch
    {

        [HarmonyPatch("__rpc_handler_1430497838")]
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

                    var terminalTrigger = (InteractTrigger)AccessTools.DeclaredField(typeof(Terminal), "terminalTrigger").GetValue(terminal);
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


        [HarmonyPatch("__rpc_handler_880620475")]
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
                    var terminalTrigger = (InteractTrigger)AccessTools.DeclaredField(typeof(Terminal), "terminalTrigger").GetValue(terminal);
                    if (terminalTrigger.GetInstanceID() == ((InteractTrigger)target).GetInstanceID())
                    {
                        lastWhoUseTerminal = new KeyValuePair<DateTime, ulong>(DateTime.Now, p.playerSteamId);
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
    }
}
