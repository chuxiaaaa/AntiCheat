using HarmonyLib;

using static AntiCheat.Patches.PatchHelper;

namespace AntiCheat.Patches
{
    [HarmonyPatch(typeof(DepositItemsDesk))]
    [HarmonyWrapSafe]
    public static class DepositItemsDeskTransactionPatch
    {
        [HarmonyPatch("SellAndDisplayItemProfits")]
        [HarmonyPostfix]
        [HarmonyWrapSafe]
        public static void SellAndDisplayItemProfits(int profit, int newGroupCredits)
        {
            if (!StartOfRound.Instance.localPlayerController.IsHost)
            {
                return;
            }
            Money = newGroupCredits;
            LogInfo($"SetMoney:{Money}");
        }
    }
}
