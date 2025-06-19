using GameNetcodeStuff;
using HarmonyLib;

using Steamworks.Data;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace AntiCheat.Patch
{
    [HarmonyPatch(typeof(PlayerControllerB))]
    [HarmonyWrapSafe]
    public static class PlayerControllerBPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch("ConnectClientToPlayerObject")]
        public static void ConnectClientToPlayerObject()
        {
            Core.AntiCheat.LogInfo("ConnectClientToPlayerObject");
            HUDManagerPatch.SyncAllPlayerLevelsServerRpcCalls = new List<ulong>();
            StartOfRoundPatch.SyncShipUnlockablesServerRpcCalls = new List<ulong>();
            StartOfRoundPatch.SyncAlreadyHeldObjectsServerRpcCalls = new List<ulong>();
            CooldownManager.Reset();
        }
    }
}
