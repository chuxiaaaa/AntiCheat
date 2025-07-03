using AntiCheat.Core;

using GameNetcodeStuff;

using HarmonyLib;

using Steamworks;
using Steamworks.Data;

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.Remoting.Messaging;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

using Unity.Netcode;

using UnityEngine;

namespace AntiCheat.Patch
{
    [HarmonyPatch(typeof(PlayerControllerB))]
    [HarmonyWrapSafe]
    public static class PlayerControllerBPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch("ConnectClientToPlayerObject")]
        public static void ConnectClientToPlayerObject(PlayerControllerB __instance)
        {
            if (__instance.isHostPlayerObject && StartOfRound.Instance.IsHost)
            {
                if (File.Exists("AntiCheat.log"))
                {
                    File.Delete("AntiCheat.log");
                }
                Patches.explosions = new List<Patches.ExplosionData>();
                Patches.ConnectionIdtoSteamIdMap = new Dictionary<uint, ulong>();
                HUDManagerPatch.SyncAllPlayerLevelsServerRpcCalls = new List<ulong>();
                StartOfRoundPatch.SyncShipUnlockablesServerRpcCalls = new List<ulong>();
                StartOfRoundPatch.SyncAlreadyHeldObjectsServerRpcCalls = new List<ulong>();
            }
        }

        /// <summary>
        /// DropAllHeldItemsServerRpc
        /// </summary>
        /// <returns></returns>
        [HarmonyPatch("__rpc_handler_760742013")]
        [HarmonyPrefix]
        public static bool DropAllHeldItemsServerRpc(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (!Patches.Check(rpcParams, out var p))
                return p != null;
            AntiCheat.Core.AntiCheat.LogInfo(p, $"PlayerControllerB.DropAllHeldItemsServerRpc");
            return true;
        }

        /// <summary>
        /// 玩家发送SteamID事件(目前还没遇到过伪造)
        /// Prefix PlayerControllerB.SendNewPlayerValuesServerRpc
        /// </summary>
        [HarmonyPatch("__rpc_handler_2504133785")]
        [HarmonyPrefix]
        public static bool SendNewPlayerValuesServerRpc(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (!StartOfRound.Instance.localPlayerController.IsHost)//非主机
            {
                return true;
            }
            if (rpcParams.Server.Receive.SenderClientId == 0)
            {
                return true;
            }
            ByteUnpacker.ReadValueBitPacked(reader, out ulong newPlayerSteamId);
            reader.Seek(0);
            ulong steamId = Patches.ConnectionIdtoSteamIdMap[Patches.ClientIdToTransportId(rpcParams.Server.Receive.SenderClientId)];
            Friend friend = new Friend(steamId);
            if (newPlayerSteamId != steamId)
            {
                NetworkManager.Singleton.DisconnectClient(rpcParams.Server.Receive.SenderClientId);
                AntiCheat.Core.AntiCheat.LogInfo($"玩家 {friend.Name}({steamId}) 伪造SteamID({newPlayerSteamId})加入游戏");
                return false;
            }
            else
            {
                string msg = Core.AntiCheat.PlayerJoin.Value.Replace("{player}", friend.Name);
                Patches.AddTextMessageClientRpc(msg);
            }
            return true;
        }
    }
}
