using AntiCheat.Utils;

using HarmonyLib;

using System;
using System.Collections.Generic;

using Unity.Netcode;

namespace AntiCheat.Patches
{
    [HarmonyPatch(typeof(HUDManager))]
    [HarmonyWrapSafe]
    public static class HUDManagerChatRealPatch
    {
        [HarmonyPatch(typeof(HUDManager), "__rpc_handler_2930587515")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static bool AddPlayerChatMessageServerRpc(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (PatchHelper.Check(rpcParams, out var player))
            {
                if (PluginConfig.ChatReal.Value)
                {
                    try
                    {
                        reader.ReadValueSafe(out bool flag, default);
                        string chatMessage = null;
                        if (flag)
                        {
                            reader.ReadValueSafe(out chatMessage, false);
                        }
                        ByteUnpacker.ReadValueBitPacked(reader, out int playerId);
                        reader.Seek(0);
                        PatchHelper.LogInfo(player, "HUDManager.AddPlayerChatMessageServerRpc", $"chatMessage:{chatMessage}");
                        if (playerId == -1)
                        {
                            return false;
                        }
                        if (playerId <= StartOfRound.Instance.allPlayerScripts.Length)
                        {
                            if (StartOfRound.Instance.allPlayerScripts[playerId].playerSteamId != player.playerSteamId)
                            {
                                PatchHelper.ShowMessage(PatchHelper.locale.Msg_GetString("ChatReal", new Dictionary<string, string>()
                                {
                                    { "{player}", player.playerUsername },
                                    { "{player2}", StartOfRound.Instance.allPlayerScripts[playerId].playerUsername },
                                }));
                                if (PluginConfig.ChatReal2.Value)
                                {
                                    PatchHelper.KickPlayer(player);
                                }
                                return false;
                            }

                            bool allowed = CooldownManager.CheckCooldown("Chat", player);
                            if (allowed && PatchHelper.locale.current_language == "zh_CN")
                            {
                                AccessTools.DeclaredMethod(typeof(HUDManager), "AddPlayerChatMessageClientRpc").Invoke(
                                    HUDManager.Instance,
                                    new object[]
                                    {
                                        chatMessage,
                                        playerId
                                    });
                                return false;
                            }
                            return allowed;
                        }

                        return false;
                    }
                    catch (Exception ex)
                    {
                        PatchHelper.LogInfo(ex.ToString());
                        return false;
                    }
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
