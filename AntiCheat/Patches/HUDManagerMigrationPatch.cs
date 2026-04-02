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
    [HarmonyPatch(typeof(HUDManager))]
    [HarmonyWrapSafe]
    public static class HUDManagerMigrationPatch
    {





        /// <summary>
        /// Prefix HUDManager.AddTextMessageServerRpc
        /// </summary>
        [HarmonyPatch("__rpc_handler_2787681914")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static bool __rpc_handler_2787681914(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (Check(rpcParams, out var p))
            {
                reader.ReadValueSafe(out bool flag, default);
                string chatMessage = null;
                if (flag)
                {
                    reader.ReadValueSafe(out chatMessage, false);
                }
                reader.Seek(0);
                LogInfo(p, "HUDManager.AddTextMessageServerRpc", $"chatMessage:{chatMessage}");
                if (chatMessage.Contains("<color") || chatMessage.Contains("<size"))
                {
                    if (PluginConfig.Map.Value)
                    {
                        if (chatMessage.Contains("<size=0>Tyzeron.Minimap"))
                        {
                            ShowMessage(locale.Msg_GetString("Map", new Dictionary<string, string>() {
                                { "{player}",p.playerUsername }
                            }));
                            if (PluginConfig.Map2.Value)
                            {
                                KickPlayer(p);
                            }
                            return false;
                        }
                    }
                    return false;
                }
                else if (chatMessage.StartsWith("[morecompanycosmetics]"))//bypass MoreCompany
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (p == null)
            {
                return false;
            }
            return true;
        }


        ///// <summary>
        ///// HUDManager.AddPlayerChatMessageClientRpc
        ///// </summary>
        //[HarmonyPatch("__rpc_handler_168728662")]
        //[HarmonyPrefix]
        //[HarmonyWrapSafe]
        //public static bool __rpc_handler_168728662(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        //{
        //    return __rpc_handler_2930587515(target, reader, rpcParams);
        //}

        /// <summary>
        /// HUDManager.AddPlayerChatMessageServerRpc
        /// </summary>
        [HarmonyPatch(typeof(HUDManager), "__rpc_handler_2930587515")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static bool __rpc_handler_2930587515(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (Check(rpcParams, out var p))
            {
                if (PluginConfig.ChatReal.Value)
                {
                    try
                    {
                        reader.ReadValueSafe(out bool flag, default);
                        string chatMessage = null;
                        if (flag)
                        {
                            reader.ReadValueSafe(out chatMessage, false);
                        }
                        ByteUnpacker.ReadValueBitPacked(reader, out int playerId);
                        reader.Seek(0);
                        LogInfo(p, $"HUDManager.AddPlayerChatMessageServerRpc", $"chatMessage:{chatMessage}");
                        if (playerId == -1)
                        {
                            return false;
                        }
                        if (playerId <= StartOfRound.Instance.allPlayerScripts.Length)
                        {
                            if (StartOfRound.Instance.allPlayerScripts[(int)playerId].playerSteamId != p.playerSteamId)
                            {
                                ShowMessage(locale.Msg_GetString("ChatReal", new Dictionary<string, string>() {
                                    { "{player}",p.playerUsername },
                                    { "{player2}",StartOfRound.Instance.allPlayerScripts[playerId].playerUsername },
                                }));
                                if (PluginConfig.ChatReal2.Value)
                                {
                                    KickPlayer(p);
                                }
                                return false;
                            }
                            else
                            {
                                bool ret = CooldownManager.CheckCooldown("Chat", p);
                                if (ret && locale.current_language == "zh_CN")
                                {

                                    AccessTools.DeclaredMethod(typeof(HUDManager), "AddPlayerChatMessageClientRpc").Invoke(HUDManager.Instance, new object[] {
                                        chatMessage,
                                        playerId
                                    });
                                    return false;

                                }
                                return ret;
                            }
                        }
                        else
                        {
                            return false;
                        }
                    }
                    catch (Exception ex)
                    {
                        LogInfo(ex.ToString());
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
        /// UI更新事件(房主死亡时加上一票起飞提示)
        /// Postfix HUDManager.Update
        /// </summary>
        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        [HarmonyWrapSafe]
        public static void Update()
        {
            if (!StartOfRound.Instance.IsHost)
            {
                return;
            }
            if (GameNetworkManager.Instance == null || GameNetworkManager.Instance.localPlayerController == null)
            {
                return;
            }
            if (StartOfRound.Instance.shipIsLeaving || !StartOfRound.Instance.currentLevel.planetHasTime)
            {
                return;
            }
            if (PluginConfig.ShipSetting_OnlyOneVote.Value)
            {
                if (!TimeOfDay.Instance.shipLeavingAlertCalled)
                {
                    if (GameNetworkManager.Instance.localPlayerController.isPlayerDead && !string.IsNullOrEmpty(HUDManager.Instance.holdButtonToEndGameEarlyVotesText.text))
                    {
                        HUDManager.Instance.holdButtonToEndGameEarlyVotesText.text += Environment.NewLine + locale.Msg_GetString("vote");
                    }
                }
            }
        }
    }
}
