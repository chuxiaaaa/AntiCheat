using HarmonyLib;

using Unity.Netcode;

namespace AntiCheat.Patches
{
    [HarmonyPatch(typeof(HUDManager))]
    [HarmonyWrapSafe]
    public static class HUDManagerTerminalAccessPatch
    {
        [HarmonyPatch("__rpc_handler_2436660286")]
        [HarmonyPrefix]
        public static bool UseSignalTranslatorServerRpc(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (!PatchHelper.Check(rpcParams, out var player))
            {
                return player != null;
            }

            if (PluginConfig.RemoteTerminal.Enable &&
                !PatchHelper.CheckRemoteTerminal(player, "HUDManager.UseSignalTranslatorServerRpc"))
            {
                return false;
            }

            return true;
        }
    }
}
