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
    [HarmonyPatch(typeof(CentipedeAI))]
    [HarmonyWrapSafe]
    public static class CentipedeAIPatch
    {

        /// <summary>
        /// Prefix CentipedeAI.ClingToPlayerServerRpc
        /// </summary>
        [HarmonyPatch("__rpc_handler_2791977891")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static bool __rpc_handler_2791977891(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (!PatchHelper.Check(rpcParams, out var p))
                return p != null;
            ByteUnpacker.ReadValueBitPacked(reader, out int num);
            reader.Seek(0);
            LogInfo(p, "CentipedeAI.ClingToPlayerServerRpc", $"num:{num}");
            if ((int)p.playerClientId == num)
            {
                if (ClingTime.ContainsKey(p.playerSteamId))
                {
                    ClingTime[p.playerSteamId] = DateTime.Now;
                }
                else
                {
                    ClingTime.Add(p.playerSteamId, DateTime.Now);
                }
            }
            return true;
            //return KillPlayerServerRpc(target, reader, rpcParams, "CentipedeAI.ClingToPlayerServerRpc");
        }
    }
}
