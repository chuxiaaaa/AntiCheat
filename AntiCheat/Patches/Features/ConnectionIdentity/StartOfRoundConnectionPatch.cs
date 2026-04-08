using GameNetcodeStuff;

using HarmonyLib;

namespace AntiCheat.Patches
{
    [HarmonyPatch(typeof(StartOfRound))]
    [HarmonyWrapSafe]
    public static class StartOfRoundConnectionPatch
    {
        [HarmonyPatch("OnPlayerDC")]
        [HarmonyPostfix]
        [HarmonyWrapSafe]
        public static void OnPlayerDC(int playerObjectNumber, ulong clientId)
        {
            PlayerControllerB component = StartOfRound.Instance.allPlayerObjects[playerObjectNumber].GetComponent<PlayerControllerB>();
            component.playerSteamId = 0;
        }
    }
}
