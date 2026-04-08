using GameNetcodeStuff;

using HarmonyLib;

using Unity.Netcode;

namespace AntiCheat.Patches
{
    [HarmonyPatch(typeof(PlayerControllerB))]
    [HarmonyWrapSafe]
    public static class PlayerControllerOperationLogPatch
    {
        [HarmonyPatch("__rpc_handler_1748753755")]
        [HarmonyPrefix]
        public static bool DropAllHeldItemsServerRpc(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (!PatchHelper.Check(rpcParams, out var player))
            {
                return player != null;
            }
            AntiCheatPlugin.LogInfo(player, "PlayerControllerB.DropAllHeldItemsRpc");
            return true;
        }
    }
}
