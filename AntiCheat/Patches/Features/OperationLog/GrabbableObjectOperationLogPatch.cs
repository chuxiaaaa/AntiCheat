using AntiCheat.Utils;

using HarmonyLib;

using System.Collections.Generic;
using System.Linq;

namespace AntiCheat.Patches
{
    [HarmonyPatch(typeof(GrabbableObject))]
    [HarmonyWrapSafe]
    public static class GrabbableObjectOperationLogPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(LungProp), "EquipItem")]
        public static bool EquipItem(LungProp __instance)
        {
            if (PluginConfig.OperationLog.Value &&
                __instance.isLungDocked &&
                StartOfRound.Instance.shipHasLanded)
            {
                PatchHelper.ShowMessageHostOnly(
                    PatchHelper.locale.OperationLog_GetString(
                        "GrabLungProp",
                        new Dictionary<string, string>
                        {
                            ["{player}"] = StartOfRound.Instance
                                .allPlayerScripts
                                .First(x => x.OwnerClientId == __instance.OwnerClientId)
                                .playerUsername,
                        }));
            }

            return true;
        }
    }
}
