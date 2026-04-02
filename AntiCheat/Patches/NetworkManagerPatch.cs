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
    [HarmonyPatch(typeof(NetworkManager))]
    [HarmonyWrapSafe]
    public static class NetworkManagerPatch
    {

        /// <summary>
        /// 记录本机SteamID
        /// Prefix NetworkManager.Awake
        /// </summary>
        [HarmonyPatch("Awake")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static void NetworkManagerAwake()
        {
            if (!GameNetworkManager.Instance.disableSteam)
            {
                ConnectionIdtoSteamIdMap[0] = SteamClient.SteamId;
            }
        }
    }
}
