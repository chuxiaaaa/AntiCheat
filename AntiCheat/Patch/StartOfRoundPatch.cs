using AntiCheat.Core;

using HarmonyLib;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Unity.Netcode;

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
        public static bool __rpc_handler_682230258(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            var steamId = Patches.ConnectionIdtoSteamIdMap[Patches.ClientIdToTransportId(rpcParams.Server.Receive.SenderClientId)];
            if (SyncAlreadyHeldObjectsServerRpcCalls.Contains(steamId))
            {
                return false;
            }
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
                    else
                    {
                        AntiCheat.Core.AntiCheat.LogInfo($"PlayerHasRevivedServerRpc All Players Loaded");
                    }
                }
            }
            return true;
        }

        [HarmonyPostfix]
        [HarmonyPatch("StartGame")]
        public static void StartGame()
        {
            CallPlayerLoadedServerRpc = new List<ulong>();
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
                    else
                    {
                        AntiCheat.Core.AntiCheat.LogInfo($"PlayerLoadedServerRpc All Players Loaded");
                    }
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
                if (SyncShipUnlockablesServerRpcCalls.Contains(p.playerSteamId))
                {
                    return false;
                }
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
