using GameNetcodeStuff;

using HarmonyLib;

using System.Collections.Generic;
using System.IO;

namespace AntiCheat.Patches
{
    [HarmonyPatch(typeof(PlayerControllerB))]
    [HarmonyWrapSafe]
    public static class PlayerControllerSessionPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch("ConnectClientToPlayerObject")]
        public static void ConnectClientToPlayerObject(PlayerControllerB __instance)
        {
            if (__instance.isHostPlayerObject && StartOfRound.Instance.IsHost)
            {
                if (File.Exists("AntiCheat.log"))
                {
                    File.Delete("AntiCheat.log");
                }

                PatchHelper.explosions = new List<PatchHelper.ExplosionData>();
                PatchHelper.ConnectionIdtoSteamIdMap = new Dictionary<uint, ulong>();
                HUDManagerRoundSyncPatch.SyncAllPlayerLevelsServerRpcCalls = new List<ulong>();
                StartOfRoundPatch.SyncShipUnlockablesServerRpcCalls = new List<ulong>();
                StartOfRoundPatch.SyncAlreadyHeldObjectsServerRpcCalls = new List<ulong>();
            }
        }
    }
}
