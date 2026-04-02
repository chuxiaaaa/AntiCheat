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
    [HarmonyPatch(typeof(RoundManager))]
    [HarmonyWrapSafe]
    public static class RoundManagerMigrationPatch
    {

        /// <summary>
        /// 降落时加载地图事件(显示反作弊信息)
        /// Prefix RoundManager.LoadNewLevel
        /// </summary>
        [HarmonyPatch("LoadNewLevel")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static bool LoadNewLevel(SelectableLevel newLevel)
        {
            if (!StartOfRound.Instance.localPlayerController.IsHost)//非主机
            {
                return true;
            }
            landMines = new List<int>();
            bypassHit = new List<HitData>();
            bypassKill = new List<HitData>();
            HUDManager.Instance.AddTextToChatOnServer(locale.Msg_GetString("game_start", new Dictionary<string, string>() {
                { "{ver}",LCMPluginInfo.PLUGIN_VERSION }
            }), -1);
            return true;
        }
    }
}
