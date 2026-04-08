using HarmonyLib;

using static AntiCheat.Patches.PatchHelper;

namespace AntiCheat.Patches
{
    [HarmonyPatch(typeof(Terminal))]
    [HarmonyWrapSafe]
    public static class TerminalCreditTrackingPatch
    {
        [HarmonyPatch("SyncGroupCreditsClientRpc")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static bool SyncGroupCreditsClientRpc(int newGroupCredits, int numItemsInShip)
        {
            if (!StartOfRound.Instance.localPlayerController.IsHost)
            {
                return true;
            }
            Money = newGroupCredits;
            return true;
        }

        [HarmonyPatch("BeginUsingTerminal")]
        [HarmonyPrefix]
        public static bool BeginUsingTerminal(Terminal __instance)
        {
            if (!StartOfRound.Instance.IsHost)
            {
                return true;
            }
            Money = __instance.groupCredits;
            LogInfo($"SetMoney:{Money}");
            return true;
        }
    }
}
