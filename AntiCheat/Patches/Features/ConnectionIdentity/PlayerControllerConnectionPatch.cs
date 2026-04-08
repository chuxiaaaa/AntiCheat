using GameNetcodeStuff;

using HarmonyLib;

using Steamworks;
using Steamworks.Data;

using Unity.Netcode;

namespace AntiCheat.Patches
{
    [HarmonyPatch(typeof(PlayerControllerB))]
    [HarmonyWrapSafe]
    public static class PlayerControllerConnectionPatch
    {
        [HarmonyPatch("__rpc_handler_2504133785")]
        [HarmonyPrefix]
        public static bool SendNewPlayerValuesServerRpc(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (!StartOfRound.Instance.localPlayerController.IsHost || rpcParams.Server.Receive.SenderClientId == 0)
            {
                return true;
            }

            ByteUnpacker.ReadValueBitPacked(reader, out ulong newPlayerSteamId);
            reader.Seek(0);

            ulong steamId = PatchHelper.ConnectionIdtoSteamIdMap[
                PatchHelper.ClientIdToTransportId(rpcParams.Server.Receive.SenderClientId)];
            var friend = new Friend(steamId);

            if (newPlayerSteamId != steamId)
            {
                NetworkManager.Singleton.DisconnectClient(rpcParams.Server.Receive.SenderClientId);
                AntiCheatPlugin.LogInfo($"Player {friend.Name}({steamId}) spoofed SteamID({newPlayerSteamId}) while joining");
                return false;
            }

            var msg = PluginConfig.PlayerJoin.Value.Replace("{player}", friend.Name);
            PatchHelper.AddTextMessageClientRpc(msg);
            return true;
        }
    }
}
