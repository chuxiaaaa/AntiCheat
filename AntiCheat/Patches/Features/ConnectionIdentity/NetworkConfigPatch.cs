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
    [HarmonyPatch(typeof(NetworkConfig))]
    [HarmonyWrapSafe]
    public static class NetworkConfigPatch
    {


        /// <summary>
        /// 
        /// </summary>
        /// <param name="__result"></param>
        /// <returns></returns>
        [HarmonyPatch("CompareConfig")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static bool NetworkConnectionManagerInitialize(ref bool __result)
        {
            if (StartOfRound.Instance != null && StartOfRound.Instance.IsHost)
            {
                if (PluginConfig.IgnoreClientConfig.Value)
                {
                    __result = true;
                    return false;
                }
            }
            return true;
        }
    }
}
