using HarmonyLib;

using Unity.Netcode;

namespace AntiCheat.Patches
{
    [HarmonyPatch(typeof(DoorLock))]
    [HarmonyWrapSafe]
    public static class DoorLockPatch
    {
        [HarmonyPatch("__rpc_handler_184554516")]
        [HarmonyPrefix]
        public static bool UnlockDoorServerRpc(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (PatchHelper.Check(rpcParams, out var player))
            {
                AntiCheatPlugin.LogInfo(player, "DoorLock.UnlockDoorServerRpc");
            }
            else if (player == null)
            {
                return false;
            }

            return true;
        }

        [HarmonyPatch("__rpc_handler_2046162111")]
        [HarmonyPrefix]
        public static bool OpenDoorAsEnemyServerRpc(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (PatchHelper.Check(rpcParams, out var player))
            {
                AntiCheatPlugin.LogInfo(player, "DoorLock.OpenDoorAsEnemyServerRpc");
            }
            else if (player == null)
            {
                return false;
            }

            return true;
        }
    }
}
