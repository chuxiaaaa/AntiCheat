using HarmonyLib;

using static AntiCheat.Patches.PatchHelper;

namespace AntiCheat.Patches
{
    [HarmonyPatch(typeof(GameNetworkManager))]
    [HarmonyWrapSafe]
    public static class GameNetworkManagerTransactionPatch
    {
        [HarmonyPatch("SteamMatchmaking_OnLobbyMemberJoined")]
        [HarmonyPostfix]
        [HarmonyWrapSafe]
        public static void SteamMatchmaking_OnLobbyMemberJoined()
        {
            if (StartOfRound.Instance == null ||
                StartOfRound.Instance.localPlayerController == null ||
                !StartOfRound.Instance.localPlayerController.IsHost)
            {
                return;
            }
            LogInfo($"SetMoney:{Money}");
            Money = terminal.groupCredits;
        }
    }
}
