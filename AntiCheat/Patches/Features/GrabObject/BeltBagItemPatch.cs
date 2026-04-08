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
    [HarmonyPatch(typeof(BeltBagItem))]
    [HarmonyWrapSafe]
    public static class BeltBagItemPatch
    {


        /// <summary>
        /// 添加物品到腰包事件
        /// Prefix BeltBagItem.TryAddObjectToBagServerRpc
        /// </summary>
        [HarmonyPatch("__rpc_handler_2988305002")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static bool __rpc_handler_2988305002(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (Check(rpcParams, out var p))
            {
                if (PluginConfig.GrabObject.Value && PluginConfig.GrabObject_BeltBag.Value)
                {
                    reader.ReadValueSafe(out NetworkObjectReference netObjectRef, default);
                    ByteUnpacker.ReadValueBitPacked(reader, out int playerWhoAdded);
                    reader.Seek(0);
                    if (netObjectRef.TryGet(out NetworkObject networkObject, null))
                    {
                        GrabbableObject component = networkObject.GetComponent<GrabbableObject>();
                        if (!component.itemProperties.isScrap && !component.isHeld && !component.isHeldByEnemy && component.itemProperties.itemId != 123984 && component.itemProperties.itemId != 819501)
                        {
                            return true;
                        }
                        ((BeltBagItem)target).CancelAddObjectToBagClientRpc(playerWhoAdded);
                        return false;
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
