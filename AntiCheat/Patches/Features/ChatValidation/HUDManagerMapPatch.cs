using HarmonyLib;

using System.Collections.Generic;

using Unity.Netcode;

namespace AntiCheat.Patches
{
    [HarmonyPatch(typeof(HUDManager))]
    [HarmonyWrapSafe]
    public static class HUDManagerMapPatch
    {
        [HarmonyPatch("__rpc_handler_2787681914")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static bool AddTextMessageServerRpc(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (PatchHelper.Check(rpcParams, out var player))
            {
                reader.ReadValueSafe(out bool flag, default);
                string chatMessage = null;
                if (flag)
                {
                    reader.ReadValueSafe(out chatMessage, false);
                }
                reader.Seek(0);
                PatchHelper.LogInfo(player, "HUDManager.AddTextMessageServerRpc", $"chatMessage:{chatMessage}");
                if (chatMessage.Contains("<color") || chatMessage.Contains("<size"))
                {
                    if (PluginConfig.Map.Value && chatMessage.Contains("<size=0>Tyzeron.Minimap"))
                    {
                        PatchHelper.ShowMessage(PatchHelper.locale.Msg_GetString("Map", new Dictionary<string, string>()
                        {
                            { "{player}", player.playerUsername }
                        }));
                        if (PluginConfig.Map2.Value)
                        {
                            PatchHelper.KickPlayer(player);
                        }
                    }

                    return false;
                }

                return chatMessage.StartsWith("[morecompanycosmetics]");
            }
            else if (player == null)
            {
                return false;
            }

            return true;
        }
    }
}
