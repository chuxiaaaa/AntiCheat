using HarmonyLib;

using System.Collections.Generic;

using static AntiCheat.Patches.PatchHelper;

namespace AntiCheat.Patches
{
    [HarmonyPatch(typeof(StartOfRound))]
    [HarmonyWrapSafe]
    public static class StartOfRoundSessionPatch
    {
        [HarmonyPatch("EndOfGame")]
        [HarmonyPostfix]
        [HarmonyWrapSafe]
        public static void EndOfGame()
        {
            jcs = new List<ulong>();
            if (rpcs.ContainsKey("Hit"))
            {
                rpcs["Hit"] = new List<ulong>();
            }
            if (rpcs.ContainsKey("KillPlayer"))
            {
                rpcs["KillPlayer"] = new List<ulong>();
            }
        }
    }
}
