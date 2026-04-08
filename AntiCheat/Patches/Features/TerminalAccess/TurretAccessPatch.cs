using HarmonyLib;

using Unity.Netcode;

namespace AntiCheat.Patches
{
    [HarmonyPatch(typeof(Turret))]
    [HarmonyWrapSafe]
    public static class TurretAccessPatch
    {
        [HarmonyPatch("__rpc_handler_2339273208")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static bool ToggleTurretServerRpc(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (PatchHelper.Check(rpcParams, out var player))
            {
                if (PluginConfig.RemoteTerminal.Enable &&
                    !PatchHelper.CheckRemoteTerminal(player, "Turret.ToggleTurretServerRpc"))
                {
                    return false;
                }
            }
            else if (player == null)
            {
                return false;
            }
            return true;
        }
    }
}
