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
    [HarmonyPatch(typeof(ShotgunItem))]
    [HarmonyWrapSafe]
    public static class ShotgunItemPatch
    {

        /// <summary>
        /// 上弹事件
        /// Prefix ShotgunItem.ReloadGunEffectsServerRpc
        /// </summary>
        [HarmonyPatch("__rpc_handler_3349119596")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static bool __rpc_handler_3349119596(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (Check(rpcParams, out var p))
            {
                if (PluginConfig.InfiniteAmmo.Value)
                {
                    reader.ReadValueSafe(out bool start, default);
                    reader.Seek(0);
                    var shot = (ShotgunItem)target;
                    LogInfo(p, $"ShotgunItem.ReloadGunEffectsServerRpc", $"start:{start}", $"shellsLoaded:{shot.shellsLoaded}");
                    if (!ReloadGun.ContainsKey(p.playerSteamId))
                    {
                        ReloadGun.Add(p.playerSteamId, start);
                    }
                    else
                    {
                        ReloadGun[p.playerSteamId] = start;
                    }
                    if (start)
                    {
                        var ammo = shot.playerHeldBy.ItemSlots.FirstOrDefault(x => x is GunAmmo ga && ga.ammoType == shot.gunCompatibleAmmoID);
                        if (ammo != default)
                        {
                            shot.StartCoroutine(CheckAmmo(p, shot, ammo));
                            return true;
                        }
                        else
                        {
                            ShowMessage(locale.Msg_GetString("InfiniteAmmo", new Dictionary<string, string>() {
                                { "{player}",p.playerUsername }
                            }));
                            if (PluginConfig.InfiniteAmmo2.Value)
                            {
                                KickPlayer(p);
                            }
                        }
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




        /// <summary>
        /// 开枪事件
        /// Prefix ShotgunItem.ShootGunServerRpc
        /// </summary>
        [HarmonyPatch("__rpc_handler_1329927282")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static bool __rpc_handler_1329927282(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (Check(rpcParams, out var p) || StartOfRound.Instance.localPlayerController.IsHost)
            {
                if (PluginConfig.InfiniteAmmo.Value)
                {
                    var s = (ShotgunItem)target;
                    var localClientSendingShootGunRPC = (bool)typeof(ShotgunItem).GetField("localClientSendingShootGunRPC", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(s);
                    if (localClientSendingShootGunRPC)
                    {
                        return true;
                    }
                    else
                    {
                        LogInfo(p, "ShotgunItem.ShootGunServerRpc", $"shellsLoaded:{s.shellsLoaded}");
                        if (s.shellsLoaded == 0)
                        {
                            ShowMessage(locale.Msg_GetString("InfiniteAmmo", new Dictionary<string, string>() {
                                { "{player}",p.playerUsername }
                            }));
                            if (PluginConfig.InfiniteAmmo2.Value)
                            {
                                KickPlayer(p);
                            }
                            return false;
                        }
                    }
                }
                if (PluginConfig.ItemCooldown.Value)
                {
                    var id = p.playerSteamId;
                    var m = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    if (!sdqcd.ContainsKey(id))
                    {
                        sdqcd.Add(id, new List<string>());
                    }
                    if (sdqcd[id].Count > 200)
                    {
                        sdqcd[id].RemoveRange(0, sdqcd[id].Count - 1);
                    }
                    if (sdqcd[id].Count(x => x == m) >= 2)
                    {
                        ShowMessage(locale.Msg_GetString("ItemCooldown", new Dictionary<string, string>() {
                            { "{player}",p.playerUsername },
                            { "{item}",locale.Item_GetString("Shotgun") }
                        }));
                        if (PluginConfig.ItemCooldown2.Value)
                        {
                            KickPlayer(p);
                        }
                        return false;
                    }
                    else
                    {
                        sdqcd[id].Add(m);
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
