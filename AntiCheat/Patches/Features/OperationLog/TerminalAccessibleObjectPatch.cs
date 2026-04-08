using HarmonyLib;

using Unity.Netcode;

namespace AntiCheat.Patches
{
    [HarmonyPatch(typeof(TerminalAccessibleObject))]
    [HarmonyWrapSafe]
    public static class TerminalAccessibleObjectPatch
    {
        [HarmonyPatch("__rpc_handler_1181174413")]
        [HarmonyPrefix]
        public static bool Prefix(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (PatchHelper.Check(rpcParams, out var player))
            {
                AntiCheatPlugin.LogInfo(player, "TerminalAccessibleObject.SetDoorOpenServerRpc");
            }
            else if (player == null)
            {
                return false;
            }

            return true;
        }
    }
}
