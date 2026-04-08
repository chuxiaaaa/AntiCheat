using HarmonyLib;

using System.Collections.Generic;

using Unity.Netcode;

using static AntiCheat.Patches.PatchHelper;

namespace AntiCheat.Patches
{
    [HarmonyPatch(typeof(TimeOfDay))]
    [HarmonyWrapSafe]
    public static class TimeOfDayShipFlowPatch
    {
        [HarmonyPatch("__rpc_handler_543987598")]
        [HarmonyPostfix]
        [HarmonyWrapSafe]
        public static void SetShipLeaveEarlyServerRpcPostfix(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (StartOfRound.Instance.localPlayerController.IsHost &&
                PluginConfig.ShipSetting_OnlyOneVote.Value &&
                rpcParams.Server.Receive.SenderClientId == 0)
            {
                TimeOfDay.Instance.SetShipLeaveEarlyClientRpc(
                    TimeOfDay.Instance.normalizedTimeOfDay + 0.1f,
                    StartOfRound.Instance.allPlayerScripts.Length);
            }
        }

        [HarmonyPatch("__rpc_handler_543987598")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static bool SetShipLeaveEarlyServerRpc(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (Check(rpcParams, out var player))
            {
                int requiredVotes = StartOfRound.Instance.connectedPlayersAmount + 1 - StartOfRound.Instance.livingPlayers;
                string msg = locale.Msg_GetString("vote_player", new Dictionary<string, string>()
                {
                    { "{player}", player.playerUsername },
                    { "{now}", (TimeOfDay.Instance.votesForShipToLeaveEarly + 1).ToString() },
                    { "{max}", requiredVotes.ToString() }
                });
                ShowMessageHostOnly(msg);
                if (TimeOfDay.Instance.votesForShipToLeaveEarly + 1 >= requiredVotes)
                {
                    LogInfo("Vote EndGame");
                    return StartOfRoundShipFlowPatch.EndGameServerRpc(target, reader, rpcParams);
                }
            }
            else if (player == null)
            {
                return false;
            }
            return true;
        }
    }
}
