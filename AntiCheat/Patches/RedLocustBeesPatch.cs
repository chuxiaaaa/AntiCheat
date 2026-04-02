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
    [HarmonyPatch(typeof(RedLocustBees))]
    [HarmonyWrapSafe]
    public static class RedLocustBeesPatch
    {

        ///// <summary>
        ///// Prefix MouthDogAI.KillPlayerServerRpc
        ///// </summary>
        //[HarmonyPatch(typeof(MouthDogAI), "__rpc_handler_998670557")]
        //[HarmonyPrefix]
        //[HarmonyWrapSafe]
        //public static bool __rpc_handler_998670557(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        //{
        //    return KillPlayerServerRpc(target, reader, rpcParams, "MouthDogAI.KillPlayerServerRpc");
        //}

        ///// <summary>
        ///// Prefix ForestGiantAI.GrabPlayerServerRpc
        ///// </summary>
        //[HarmonyPatch(typeof(ForestGiantAI), "__rpc_handler_2965927486")]
        //[HarmonyPrefix]
        //[HarmonyWrapSafe]
        //public static bool __rpc_handler_2965927486(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        //{
        //    return KillPlayerServerRpc(target, reader, rpcParams, "ForestGiantAI.GrabPlayerServerRpc");
        //}

        /// <summary>
        /// Prefix RedLocustBees.BeeKillPlayerServerRpc
        /// </summary>
        [HarmonyPatch("__rpc_handler_3246315153")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static bool __rpc_handler_3246315153(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            return KillPlayerServerRpc(target, reader, rpcParams, "RedLocustBees.BeeKillPlayerServerRpc");
        }
    }
}
