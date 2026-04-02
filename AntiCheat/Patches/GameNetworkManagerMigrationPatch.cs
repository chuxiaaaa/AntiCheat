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
    [HarmonyPatch(typeof(GameNetworkManager))]
    [HarmonyWrapSafe]
    public static class GameNetworkManagerMigrationPatch
    {


        //[HarmonyPatch(typeof(StartOfRound), "StartTrackingAllPlayerVoices")]
        //[HarmonyPostfix]
        //[HarmonyWrapSafe]
        //public static void StartTrackingAllPlayerVoices()
        //{
        //    if (!StartOfRound.Instance.localPlayerController.IsHost)
        //    {
        //        return;
        //    }
        //    foreach (var item in StartOfRound.Instance.allPlayerScripts)
        //    {
        //        if (!item.isPlayerControlled)
        //        {
        //            continue;
        //        }
        //        var playerName = item.playerUsername;
        //        if (playerName == "Player #0")
        //        {
        //            continue;
        //        }
        //        if (StartOfRound.Instance.KickedClientIds.Contains(item.playerSteamId))
        //        {
        //            KickPlayer(item);
        //            return;
        //        }
        //        LogInfo(playerName);
        //        if (Regex.IsMatch(playerName, "Nameless\\d*") || Regex.IsMatch(playerName, "Unknown\\d*") || Regex.IsMatch(playerName, "Player #\\d*"))
        //        {
        //            if (PluginConfig.Nameless.Value)
        //            {
        //                ShowMessage(locale.Msg_GetString("Nameless"));
        //                if (PluginConfig.Nameless2.Value)
        //                {
        //                    KickPlayer(item, true, locale.Msg_GetString("Kick_Nameless"));
        //                }
        //            }
        //        }
        //    }
        //    var p2 = StartOfRound.Instance.allPlayerScripts.OrderByDescending(x => x.playerClientId).FirstOrDefault();
        //    if (p2.playerClientId != lastClientId)
        //    {
        //        if (p2.isPlayerControlled && p2.playerSteamId == 0)
        //        {
        //            KickPlayer(p2);
        //            return;
        //        }
        //        else if (!p2.isPlayerControlled)
        //        {
        //            return;
        //        }
        //        bypass = true;
        //        string msg = PluginConfig.PlayerJoin.Value.Replace("{player}", p2.playerUsername);
        //        LogInfo(msg);
        //        HUDManager.Instance.AddTextToChatOnServer(msg, -1);
        //        lastClientId = p2.playerClientId;
        //        bypass = false;
        //    }
        //}



        [HarmonyPatch("SteamMatchmaking_OnLobbyMemberJoined")]
        [HarmonyPostfix]
        [HarmonyWrapSafe]
        public static void SteamMatchmaking_OnLobbyMemberJoined()
        {
            if (StartOfRound.Instance == null || StartOfRound.Instance.localPlayerController == null || !StartOfRound.Instance.localPlayerController.IsHost)
            {
                return;
            }
            LogInfo($"SetMoney:{Money}");
            Money = terminal.groupCredits;
        }


        /// <summary>
        /// 自动添加AC标识
        /// Prefix GameNetworkManager.StartHost
        /// </summary>
        [HarmonyPatch("StartHost")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static void StartHost()
        {
            if (PluginConfig.Prefix.Value.IsNullOrWhiteSpace())
            {
                return;
            }
            var setting = GameNetworkManager.Instance.lobbyHostSettings;
            string rawText = setting.lobbyName;
            rawText = rawText.Replace("【", "[").Replace("】", "]");
            List<string> labels = new List<string>();
            var match = Regex.Match(rawText, "^\\[(.*?)\\]");
            if (match.Success)
            {
                var txt = match.Groups[1].Value;
                labels.AddRange(txt.Split('/'));
                rawText = rawText.Remove(0, match.Groups[0].Value.Length).TrimStart();
            }
            if (!labels.Any(x => x == PluginConfig.Prefix.Value))
            {
                labels.Add(PluginConfig.Prefix.Value);
            }
            setting.lobbyName = "[" + string.Join("/", labels) + "]" + " " + rawText;
        }
    }
}
