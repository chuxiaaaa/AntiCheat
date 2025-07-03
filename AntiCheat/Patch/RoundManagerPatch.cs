using HarmonyLib;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Unity.Netcode;

namespace AntiCheat.Patch
{
    [HarmonyPatch(typeof(RoundManager))]
    [HarmonyWrapSafe]
    public static class RoundManagerPatch
    {
        public static List<ulong> CallFinishedGeneratingLevelServerRpc { get; set; } = new List<ulong>();

        [HarmonyPrefix]
        [HarmonyPatch("__rpc_handler_192551691")]
        public static bool FinishedGeneratingLevelServerRpc(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (Patches.Check(rpcParams, out var p))
            {

                int playersFinishedGeneratingFloorCount = (RoundManager.Instance.playersFinishedGeneratingFloor.Count + 1);
                AntiCheat.Core.AntiCheat.LogInfo(p, $"RoundManager.FinishedGeneratingLevelServerRpc", $"playersFinishedGeneratingFloor:{playersFinishedGeneratingFloorCount}", $"connectedPlayers:{GameNetworkManager.Instance.connectedPlayers}");
                CallFinishedGeneratingLevelServerRpc.Add(p.playerSteamId);
                if (playersFinishedGeneratingFloorCount < GameNetworkManager.Instance.connectedPlayers && playersFinishedGeneratingFloorCount + 5 > GameNetworkManager.Instance.connectedPlayers)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (var item in StartOfRound.Instance.allPlayerScripts)
                    {
                        if (!item.isPlayerControlled || item.isHostPlayerObject)
                        {
                            continue;
                        }
                        if (!CallFinishedGeneratingLevelServerRpc.Contains(item.playerSteamId))
                        {
                            sb.Append(item.playerUsername + "||");
                        }
                    }
                    if (!string.IsNullOrWhiteSpace(sb.ToString()))
                    {
                        AntiCheat.Core.AntiCheat.LogInfo($"FinishedGeneratingLevelServerRpc Wait For:{sb.ToString()}");
                    }
                }
                if (playersFinishedGeneratingFloorCount == GameNetworkManager.Instance.connectedPlayers)
                {
                    AntiCheat.Core.AntiCheat.LogInfo($"FinishedGeneratingLevelServerRpc All Players Loaded");
                    CallFinishedGeneratingLevelServerRpc = new List<ulong>();
                }
            }
            return true;
        }
    }
}
