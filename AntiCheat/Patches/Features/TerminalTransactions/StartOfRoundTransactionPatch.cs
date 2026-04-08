using HarmonyLib;

using System.Collections.Generic;
using System.Linq;

using Unity.Netcode;

using static AntiCheat.Patches.PatchHelper;

namespace AntiCheat.Patches
{
    [HarmonyPatch(typeof(StartOfRound))]
    [HarmonyWrapSafe]
    public static class StartOfRoundTransactionPatch
    {
        [HarmonyPatch("__rpc_handler_1134466287")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static bool ChangeLevelServerRpc(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (Check(rpcParams, out var player))
            {
                if (PluginConfig.RemoteTerminal.Value &&
                    !CheckRemoteTerminal(player, "StartOfRound.ChangeLevelServerRpc"))
                {
                    return false;
                }

                ByteUnpacker.ReadValueBitPacked(reader, out int levelID);
                ByteUnpacker.ReadValueBitPacked(reader, out int newGroupCreditsAmount);
                reader.Seek(0);
                if (PluginConfig.FreeBuy.Value)
                {
                    if (newGroupCreditsAmount > Money || Money < 0)
                    {
                        ShowMessage(locale.Msg_GetString("FreeBuy_SetMoney", new Dictionary<string, string>()
                        {
                            { "{player}", player.playerUsername },
                            { "{Money}", (newGroupCreditsAmount - Money).ToString() }
                        }));
                        if (PluginConfig.FreeBuy2.Value)
                        {
                            KickPlayer(player);
                        }
                        return false;
                    }
                    if (levelID > StartOfRound.Instance.levels.Length)
                    {
                        LogInfo(player, "StartOfRound.ChangeLevelServerRpc", "levelID > StartOfRound.Instance.levels.Length");
                        return false;
                    }
                    var route = terminal.terminalNodes.allKeywords.FirstOrDefault(x => x.word.ToLower() == "route");
                    var level = StartOfRound.Instance.levels[levelID].PlanetName.Split(' ')[0];
                    var compatibleNoun = route.compatibleNouns.Where(x => x.result.name == level + "route");
                    if (compatibleNoun.Any())
                    {
                        int itemCost = compatibleNoun.First().result.itemCost;
                        level = StartOfRound.Instance.currentLevel.PlanetName.Split(' ')[0];
                        var currentCompatibleNoun = route.compatibleNouns.Where(x => x.result.name == level + "route");
                        if (itemCost == 0 && currentCompatibleNoun.Any() && currentCompatibleNoun.First().result.itemCost != 0)
                        {
                            ShowMessage(locale.Msg_GetString("ChangeToFreeLevel", new Dictionary<string, string>()
                            {
                                { "{player}", player.playerUsername }
                            }));
                            return false;
                        }
                        LogInfo(player, "StartOfRound.ChangeLevelServerRpc", $"levelID:{levelID}", $"itemCost:{itemCost}");
                        if (itemCost != 0)
                        {
                            int newValue = Money - itemCost;
                            if (newValue != newGroupCreditsAmount || Money == 0)
                            {
                                ShowMessage(locale.Msg_GetString("FreeBuy_Level", new Dictionary<string, string>()
                                {
                                    { "{player}", player.playerUsername }
                                }));
                                if (PluginConfig.FreeBuy2.Value)
                                {
                                    KickPlayer(player);
                                }
                                return false;
                            }
                        }
                    }
                }
                if (PluginConfig.OperationLog.Value && levelID < StartOfRound.Instance.levels.Length)
                {
                    ShowMessageHostOnly(locale.OperationLog_GetString("ChangeLevel", new Dictionary<string, string>()
                    {
                        { "{player}", player.playerUsername },
                        { "{planet}", StartOfRound.Instance.levels[levelID].PlanetName }
                    }));
                }
            }
            else if (player == null)
            {
                return false;
            }
            return true;
        }

        [HarmonyPatch("__rpc_handler_3953483456")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static bool BuyShipUnlockableServerRpc(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (Check(rpcParams, out var player))
            {
                if (PluginConfig.RemoteTerminal.Value &&
                    !CheckRemoteTerminal(player, "StartOfRound.BuyShipUnlockableServerRpc"))
                {
                    return false;
                }
                ByteUnpacker.ReadValueBitPacked(reader, out int unlockableID);
                ByteUnpacker.ReadValueBitPacked(reader, out int newGroupCreditsAmount);
                reader.Seek(0);
                if (PluginConfig.FreeBuy.Value)
                {
                    if (Money == newGroupCreditsAmount || Money == 0)
                    {
                        ShowMessage(locale.Msg_GetString("FreeBuy_unlockable", new Dictionary<string, string>()
                        {
                            { "{player}", player.playerUsername }
                        }));
                        if (PluginConfig.FreeBuy2.Value)
                        {
                            KickPlayer(player);
                        }
                        return false;
                    }
                    else if (newGroupCreditsAmount > Money || Money < 0)
                    {
                        ShowMessage(locale.Msg_GetString("FreeBuy_SetMoney", new Dictionary<string, string>()
                        {
                            { "{player}", player.playerUsername },
                            { "{Money}", (newGroupCreditsAmount - Money).ToString() }
                        }));
                        if (PluginConfig.FreeBuy2.Value)
                        {
                            KickPlayer(player);
                        }
                        return false;
                    }
                }
                if (PluginConfig.OperationLog.Value &&
                    unlockableID < StartOfRound.Instance.unlockablesList.unlockables.Count)
                {
                    ShowMessageHostOnly(locale.OperationLog_GetString("BuyShipUnlockable", new Dictionary<string, string>()
                    {
                        { "{player}", player.playerUsername },
                        { "{unlockable}", StartOfRound.Instance.unlockablesList.unlockables[unlockableID].unlockableName }
                    }));
                }
            }
            else if (player == null)
            {
                return false;
            }
            return true;
        }

        [HarmonyPatch("BuyShipUnlockableClientRpc")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static bool BuyShipUnlockableClientRpc(int newGroupCreditsAmount, int unlockableID = -1)
        {
            if (!StartOfRound.Instance.localPlayerController.IsHost)
            {
                return true;
            }
            Money = newGroupCreditsAmount;
            return true;
        }

        [HarmonyPatch("ChangeLevelClientRpc")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static bool ChangeLevelClientRpc(int levelID, int newGroupCreditsAmount)
        {
            if (!StartOfRound.Instance.localPlayerController.IsHost)
            {
                return true;
            }
            Money = newGroupCreditsAmount;
            return true;
        }
    }
}
