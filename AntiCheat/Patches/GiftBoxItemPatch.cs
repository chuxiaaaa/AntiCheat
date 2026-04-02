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
    [HarmonyPatch(typeof(GiftBoxItem))]
    [HarmonyWrapSafe]
    public static class GiftBoxItemPatch
    {

        /// <summary>
        /// 开礼物盒事件(一个礼物盒只能开一次)
        /// Prefix GiftBoxItem.OpenGiftBoxServerRpc
        /// </summary>
        [HarmonyPatch("__rpc_handler_2878544999")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static bool __rpc_handler_2878544999(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (Check(rpcParams, out var p))
            {
                if (PluginConfig.Gift.Value)
                {
                    var item = (GiftBoxItem)target;
                    if ((bool)AccessTools.DeclaredField(typeof(GiftBoxItem), "hasUsedGift").GetValue(item))
                    {
                        ShowMessage(locale.Msg_GetString("Gift", new Dictionary<string, string>() {
                            { "{player}",p.playerUsername }
                        }));
                        if (PluginConfig.Gift2.Value)
                        {
                            KickPlayer(p);
                        }
                        return false;
                    }
                    else
                    {
                        StartOfRound.Instance.localPlayerController.StartCoroutine(DestroySelf(item.gameObject));
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
