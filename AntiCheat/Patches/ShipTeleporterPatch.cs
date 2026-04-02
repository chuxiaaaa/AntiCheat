using HarmonyLib;

using System;
using System.Collections.Generic;
using System.Text;

using Unity.Netcode;

namespace AntiCheat.Patches
{
    [HarmonyPatch(typeof(ShipTeleporter))]
    [HarmonyWrapSafe]
    public static class ShipTeleporterPatch
    {
        /// <summary>
        /// PressTeleportButtonServerRpc
        /// </summary>
        [HarmonyPrefix]
        [HarmonyPatch("__rpc_handler_389447712")]
        public static bool PressTeleportButtonServerRpc(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (PatchHelper.Check(rpcParams, out var p))
            {
                var shiptp = (ShipTeleporter)target;
                AntiCheatPlugin.LogInfo(p, "ShipTeleporter.PressTeleportButtonServerRpc", $"isInverseTeleporter:{shiptp.isInverseTeleporter}");
                if (shiptp.isInverseTeleporter && !(bool)AccessTools.DeclaredMethod(typeof(ShipTeleporter), "CanUseInverseTeleporter").Invoke(shiptp, null))
                {
                    return false;
                }
                var cooldownTime = (float)AccessTools.DeclaredField(typeof(ShipTeleporter), "cooldownTime").GetValue(shiptp);
                if (cooldownTime > 0)
                {
                    AntiCheatPlugin.LogInfo(p, "ShipTeleporter.PressTeleportButtonServerRpc", $"cooldownAmount:{cooldownTime}");
                    return false;
                }
                return true;
            }
            else if (p == null)
            {
                return false;
            }
            return true;
        }
    }
}
