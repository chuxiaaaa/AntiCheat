using HarmonyLib;

using System.Collections.Generic;

using static AntiCheat.Patches.PatchHelper;

namespace AntiCheat.Patches
{
    [HarmonyPatch(typeof(RoundManager))]
    [HarmonyWrapSafe]
    public static class RoundManagerSessionPatch
    {
        [HarmonyPatch("LoadNewLevel")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static bool LoadNewLevel(SelectableLevel newLevel)
        {
            if (!StartOfRound.Instance.localPlayerController.IsHost)
            {
                return true;
            }
            landMines = new List<int>();
            bypassHit = new List<HitData>();
            bypassKill = new List<HitData>();
            HUDManager.Instance.AddTextToChatOnServer(locale.Msg_GetString("game_start", new Dictionary<string, string>()
            {
                { "{ver}", LCMPluginInfo.PLUGIN_VERSION }
            }), -1);
            return true;
        }
    }
}
