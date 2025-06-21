using HarmonyLib;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Unity.Netcode;

namespace AntiCheat.Patch
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
            if (Patches.Check(rpcParams, out var p))
            {
                var shiptp = (ShipTeleporter)target;
                Core.AntiCheat.LogInfo(p, "ShipTeleporter.PressTeleportButtonServerRpc", $"isInverseTeleporter:{shiptp.isInverseTeleporter}");
                var cooldownTime = (float)AccessTools.DeclaredField(typeof(ShipTeleporter), "cooldownTime").GetValue(shiptp);
                if (cooldownTime > 0)
                {
                    Core.AntiCheat.LogInfo(p, "ShipTeleporter.PressTeleportButtonServerRpc", $"cooldownAmount:{cooldownTime}");
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
