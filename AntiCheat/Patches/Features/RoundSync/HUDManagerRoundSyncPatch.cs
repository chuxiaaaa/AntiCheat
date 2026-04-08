using HarmonyLib;

using System.Collections.Generic;

using Unity.Netcode;

namespace AntiCheat.Patches
{
    [HarmonyPatch(typeof(HUDManager))]
    [HarmonyWrapSafe]
    public static class HUDManagerRoundSyncPatch
    {
        public static List<ulong> SyncAllPlayerLevelsServerRpcCalls { get; set; } = new List<ulong>();

        [HarmonyPrefix]
        [HarmonyPatch("__rpc_handler_4217433937")]
        public static bool SyncAllPlayerLevelsServerRpc(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (!PatchHelper.Check(rpcParams, out var player))
            {
                return player != null;
            }

            if (SyncAllPlayerLevelsServerRpcCalls.Contains(player.playerSteamId))
            {
                return false;
            }

            ByteUnpacker.ReadValueBitPacked(reader, out int _);
            ByteUnpacker.ReadValueBitPacked(reader, out int playerClientId);
            reader.Seek(0);

            if (playerClientId != (int)player.playerClientId)
            {
                return false;
            }

            SyncAllPlayerLevelsServerRpcCalls.Add(player.playerSteamId);
            return true;
        }
    }
}
