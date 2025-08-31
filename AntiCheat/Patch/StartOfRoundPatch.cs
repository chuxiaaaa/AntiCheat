using AntiCheat.Core;

using HarmonyLib;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Unity.Netcode;

using UnityEngine;

namespace AntiCheat
{
    [HarmonyPatch(typeof(StartOfRound))]
    [HarmonyWrapSafe]
    public static class StartOfRoundPatch
    {
        public static List<ulong> SyncShipUnlockablesServerRpcCalls = new List<ulong>();
        public static List<ulong> SyncAlreadyHeldObjectsServerRpcCalls = new List<ulong>();

        /// <summary>
        /// SyncAlreadyHeldObjectsServerRpc
        /// </summary>
        [HarmonyPrefix]
        [HarmonyPatch("__rpc_handler_682230258")]
        public static bool SyncAlreadyHeldObjectsServerRpc(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            var steamId = Patches.ConnectionIdtoSteamIdMap[Patches.ClientIdToTransportId(rpcParams.Server.Receive.SenderClientId)];
            AntiCheat.Core.AntiCheat.LogInfo($"StartOfRound.SyncShipUnlockablesServerRpc:{steamId}|32");
            if (SyncAlreadyHeldObjectsServerRpcCalls.Contains(steamId))
            {
                AntiCheat.Core.AntiCheat.LogInfo($"StartOfRound.SyncShipUnlockablesServerRpc:{steamId}|36");
                return false;
            }
            AntiCheat.Core.AntiCheat.LogInfo($"StartOfRound.SyncShipUnlockablesServerRpc:{steamId}|39");
            SyncAlreadyHeldObjectsServerRpcCalls.Add(steamId);
            return true;
        }

        public static List<ulong> CallPlayerHasRevivedServerRpc { get; set; } = new List<ulong>();
        public static List<ulong> CallPlayerLoadedServerRpc { get; set; } = new List<ulong>();


        [HarmonyPrefix]
        [HarmonyPatch("__rpc_handler_3083945322")]
        public static bool PlayerHasRevivedServerRpc(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (Patches.Check(rpcParams, out var p))
            {
                int playersRevived = (int)AccessTools.DeclaredField(typeof(StartOfRound), "playersRevived").GetValue(StartOfRound.Instance);
                AntiCheat.Core.AntiCheat.LogInfo(p, $"StartOfRound.PlayerHasRevivedServerRpc", $"playersRevived:{(playersRevived + 1)}", $"connectedPlayers:{GameNetworkManager.Instance.connectedPlayers}");
                CallPlayerHasRevivedServerRpc.Add(p.playerSteamId);
                if (playersRevived < GameNetworkManager.Instance.connectedPlayers && playersRevived + 5 > GameNetworkManager.Instance.connectedPlayers)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (var item in StartOfRound.Instance.allPlayerScripts)
                    {
                        if (!item.isPlayerControlled || item.isHostPlayerObject)
                        {
                            continue;
                        }
                        if (!CallPlayerHasRevivedServerRpc.Contains(item.playerSteamId))
                        {
                            sb.Append(item.playerUsername + "||");
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(sb.ToString()))
                    {
                        AntiCheat.Core.AntiCheat.LogInfo($"PlayerHasRevivedServerRpc Wait For:{sb.ToString()}");
                    }
                    
                }
                if ((playersRevived + 1) == GameNetworkManager.Instance.connectedPlayers)
                {
                    AntiCheat.Core.AntiCheat.LogInfo($"PlayerHasRevivedServerRpc All Players Loaded");
                    CallPlayerHasRevivedServerRpc = new List<ulong>();
                }
            }
            return true;
        }

        [HarmonyPostfix]
        [HarmonyPatch("StartGame")]
        public static void StartGame()
        {
            if (File.Exists("AntiCheat.log"))
            {
                File.Delete("AntiCheat.log");
            }
        }


        [HarmonyPrefix]
        [HarmonyPatch("__rpc_handler_4249638645")]
        public static bool PlayerLoadedServerRpc(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (Patches.Check(rpcParams, out var p))
            {
                int fullyLoadedPlayers = StartOfRound.Instance.fullyLoadedPlayers.Count;
                AntiCheat.Core.AntiCheat.LogInfo(p, $"StartOfRound.PlayerLoadedServerRpc", $"fullyLoadedPlayers:{(fullyLoadedPlayers + 1)}", $"connectedPlayers:{GameNetworkManager.Instance.connectedPlayers}");
                CallPlayerLoadedServerRpc.Add(p.playerSteamId);
                if (fullyLoadedPlayers < GameNetworkManager.Instance.connectedPlayers && fullyLoadedPlayers + 5 > GameNetworkManager.Instance.connectedPlayers)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (var item in StartOfRound.Instance.allPlayerScripts)
                    {
                        if (!item.isPlayerControlled || item.isHostPlayerObject)
                        {
                            continue;
                        }
                        if (!CallPlayerLoadedServerRpc.Contains(item.playerSteamId))
                        {
                            sb.Append(item.playerUsername + "||");
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(sb.ToString()))
                    {
                        AntiCheat.Core.AntiCheat.LogInfo($"PlayerLoadedServerRpc Wait For:{sb.ToString()}");
                    }
                }
                if ((fullyLoadedPlayers + 1) == GameNetworkManager.Instance.connectedPlayers)
                {
                    CallPlayerLoadedServerRpc = new List<ulong>();
                    AntiCheat.Core.AntiCheat.LogInfo($"PlayerLoadedServerRpc All Players Loaded");
                }
            }
            return true;
        }

        /// <summary>
        /// SyncShipUnlockablesServerRpc
        /// </summary>
        [HarmonyPrefix]
        [HarmonyPatch("__rpc_handler_744998938")]
        public static bool __rpc_handler_744998938(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (Patches.Check(rpcParams, out var p))
            {
                AntiCheat.Core.AntiCheat.LogInfo(p, $"StartOfRound.SyncShipUnlockablesServerRpc", "140");
                if (SyncShipUnlockablesServerRpcCalls.Contains(p.playerSteamId))
                {
                    AntiCheat.Core.AntiCheat.LogInfo(p, $"StartOfRound.SyncShipUnlockablesServerRpc", "144");
                    return false;
                }
                AntiCheat.Core.AntiCheat.LogInfo(p, $"StartOfRound.SyncShipUnlockablesServerRpc", "146");
                SyncShipUnlockablesServerRpcCalls.Add(p.playerSteamId);
                return true;
            }
            else if (p == null)
            {
                return false;
            }
            return true;
        }

    }
}
