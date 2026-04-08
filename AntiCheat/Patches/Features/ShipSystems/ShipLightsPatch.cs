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
    [HarmonyPatch(typeof(ShipLights))]
    [HarmonyWrapSafe]
    public static class ShipLightsPatch
    {

        /// <summary>
        /// 飞船灯开关速度限制
        /// Prefix ShipLights.SetShipLightsServerRpc
        /// </summary>
        [HarmonyPatch("__rpc_handler_1625678258")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static bool __rpc_handler_1625678258(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (Check(rpcParams, out var p))
            {
                bool canUse = CooldownManager.CheckCooldown("ShipLight", p);
                if (!canUse)
                {
                    ((ShipLights)target).SetShipLightsClientRpc(true);
                }
                return canUse;
            }
            else if (p == null)
            {
                return false;
            }
            return true;
        }
    }
}
