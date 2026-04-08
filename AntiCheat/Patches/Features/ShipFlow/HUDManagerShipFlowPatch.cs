using HarmonyLib;

using System;

namespace AntiCheat.Patches
{
    [HarmonyPatch(typeof(HUDManager))]
    [HarmonyWrapSafe]
    public static class HUDManagerShipFlowPatch
    {
        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        [HarmonyWrapSafe]
        public static void Update()
        {
            if (!StartOfRound.Instance.IsHost)
            {
                return;
            }
            if (GameNetworkManager.Instance == null || GameNetworkManager.Instance.localPlayerController == null)
            {
                return;
            }
            if (StartOfRound.Instance.shipIsLeaving || !StartOfRound.Instance.currentLevel.planetHasTime)
            {
                return;
            }
            if (PluginConfig.ShipSetting_OnlyOneVote.Value &&
                !TimeOfDay.Instance.shipLeavingAlertCalled &&
                GameNetworkManager.Instance.localPlayerController.isPlayerDead &&
                !string.IsNullOrEmpty(HUDManager.Instance.holdButtonToEndGameEarlyVotesText.text))
            {
                HUDManager.Instance.holdButtonToEndGameEarlyVotesText.text +=
                    Environment.NewLine + PatchHelper.locale.Msg_GetString("vote");
            }
        }
    }
}
