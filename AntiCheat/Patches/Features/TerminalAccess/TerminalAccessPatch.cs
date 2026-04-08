using AntiCheat.Utils;

using HarmonyLib;

using Unity.Netcode;

namespace AntiCheat.Patches
{
    [HarmonyPatch(typeof(Terminal))]
    [HarmonyWrapSafe]
    public static class TerminalAccessPatch
    {
        [HarmonyPatch("__rpc_handler_1713627637")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static bool PlayTerminalAudioServerRpc(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (PatchHelper.Check(rpcParams, out var player))
            {
                if (PluginConfig.RemoteTerminal.Value &&
                    !PatchHelper.CheckRemoteTerminal(player, "Terminal.PlayTerminalAudioServerRpc"))
                {
                    return false;
                }
                return CooldownManager.CheckCooldown("TerminalNoise", player);
            }
            else if (player == null)
            {
                return false;
            }
            return true;
        }
    }
}
