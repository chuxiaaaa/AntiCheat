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
    [HarmonyPatch(typeof(FacepunchTransport))]
    [HarmonyWrapSafe]
    public static class FacepunchTransportPatch
    {

        /// <summary>
        /// 代码来源 @Charlese2 HostFixes
        /// 客户端连接事件，获取真实的SteamID
        /// </summary>
        [HarmonyPatch("Steamworks.ISocketManager.OnConnecting")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static bool FacepunchTransportOnConnecting(ref Connection connection, ref ConnectionInfo info)
        {
            NetIdentity identity = Traverse.Create(info).Field<NetIdentity>("identity").Value;
            //if (StartOfRound.Instance.KickedClientIds.Contains(identity.SteamId.Value))
            //{
            //    LogInfo(locale.Log_GetString("refuse_connect", new Dictionary<string, string>() {
            //        {"{steamId}",identity.SteamId.Value.ToString() }
            //    }));
            //    return false;
            //}
            if (StartOfRound.Instance.allPlayerScripts.Any(x => x.isPlayerControlled && x.playerSteamId == identity.SteamId.Value))
            {
                LogInfo("{steamId} repeatedly joins the game.");
                return false;
            }
            if (ConnectionIdtoSteamIdMap.ContainsKey(connection.Id))
            {
                ConnectionIdtoSteamIdMap[connection.Id] = identity.SteamId.Value;
            }
            else
            {
                ConnectionIdtoSteamIdMap.Add(connection.Id, identity.SteamId.Value);
            }
            if (PluginConfig.OperationLog.Value)
            {
                ShowMessageHostOnly(locale.OperationLog_GetString("JoinLobby", new Dictionary<string, string>() {
                    { "{player}",new Friend(ConnectionIdtoSteamIdMap[connection.Id]).Name }
                }));
            }
            return true;
        }


        /// <summary>
        /// 代码来源 @Charlese2 HostFixes
        /// 客户端断开连接事件
        /// Prefix FacepunchTransport.OnDisconnected
        /// </summary>
        [HarmonyPatch("Steamworks.ISocketManager.OnDisconnected")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static void FacepunchTransportOnDisconnected(ref Connection connection, ref ConnectionInfo info)
        {
            if (NetworkManager.Singleton?.IsListening == true)
            {
                NetIdentity identity = Traverse.Create(info).Field<NetIdentity>("identity").Value;
                HUDManagerRoundSyncPatch.SyncAllPlayerLevelsServerRpcCalls.Remove(ConnectionIdtoSteamIdMap[connection.Id]);
                StartOfRoundPatch.SyncShipUnlockablesServerRpcCalls.Remove(ConnectionIdtoSteamIdMap[connection.Id]);
                StartOfRoundPatch.SyncAlreadyHeldObjectsServerRpcCalls.Remove(ConnectionIdtoSteamIdMap[connection.Id]);
                ConnectionIdtoSteamIdMap.Remove(connection.Id);
            }
        }
    }
}
