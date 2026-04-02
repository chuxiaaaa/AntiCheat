using HarmonyLib;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Unity.Netcode;

namespace AntiCheat.Patches
{
    [HarmonyPatch(typeof(Terminal))]
    [HarmonyWrapSafe]
    public static class TerminalPatch
    {

        /// <summary>
        /// Terminal.BuyItemsServerRpc
        /// </summary>
        /// <returns></returns>
        [HarmonyPatch("__rpc_handler_4003509079")]
        [HarmonyPrefix]
        public static bool BuyItemsServerRpc(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (PatchHelper.Check(rpcParams, out var p))
            {
                if (PluginConfig.RemoteTerminal.Value)
                {
                    if (!PatchHelper.CheckRemoteTerminal(p, "Terminal.BuyItemsServerRpc"))
                    {
                        return false;
                    }
                }
                reader.ReadValueSafe(out bool flag, default);
                int[] boughtItems = null;
                if (flag)
                {
                    reader.ReadValueSafe(out boughtItems, default);
                }
                ByteUnpacker.ReadValueBitPacked(reader, out int newGroupCredits);
                reader.Seek(0);
                if (PluginConfig.FreeBuy.Value)
                {
                    //LogInfo("__rpc_handler_4003509079|boughtItems:" + string.Join(",", boughtItems) + "|newGroupCredits:" + newGroupCredits + "|Money:" + Money);
                    if (PatchHelper.Money == newGroupCredits || PatchHelper.Money == 0)
                    {
                        var terminal = (Terminal)target;
                        PatchHelper.ShowMessage(PatchHelper.locale.Msg_GetString("FreeBuy_Item", new Dictionary<string, string>() {
                            { "{player}",p.playerUsername },
                            { "{items}", string.Join(",", boughtItems.GroupBy(x => terminal.buyableItemsList[x].itemName).Select(g => g.Count() == 1 ? g.Key : $"{g.Key}*{g.Count()}")) }
                        }));
                        if (PluginConfig.FreeBuy_Kick.Value)
                        {
                            PatchHelper.KickPlayer(p);
                        }
                        return false;
                    }
                    else if (newGroupCredits > PatchHelper.Money || PatchHelper.Money < 0)
                    {
                        PatchHelper.ShowMessage(PatchHelper.locale.Msg_GetString("FreeBuy_SetMoney", new Dictionary<string, string>() {
                            { "{player}",p.playerUsername },
                            { "{Money}",(newGroupCredits - PatchHelper.Money).ToString() }
                        }));
                        if (PluginConfig.FreeBuy_Kick.Value)
                        {
                            PatchHelper.KickPlayer(p);
                        }
                        return false;
                    }
                }
                if (PluginConfig.OperationLog.Value)
                {
                    var terminal = (Terminal)target;
                    if (boughtItems.Count(x => x < terminal.buyableItemsList.Length) == boughtItems.Count())
                    {
                        PatchHelper.ShowMessageHostOnly(PatchHelper.locale.OperationLog_GetString("BuyItem", new Dictionary<string, string>() {
                            { "{player}", p.playerUsername },
                            { "{items}", string.Join(",", boughtItems.GroupBy(x => terminal.buyableItemsList[x].itemName).Select(g => g.Count() == 1 ? g.Key : $"{g.Key}*{g.Count()}")) }
                        }));
                    }
                }
            }
            else if (p == null)
            {
                return false;
            }
            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch("__rpc_handler_2452398197")]
        public static bool BuyVehicleServerRpc(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (PatchHelper.Check(rpcParams, out var p) || true)
            {
                if (PluginConfig.RemoteTerminal.Value)
                {
                    if (!PatchHelper.CheckRemoteTerminal(p, "Terminal.BuyVehicleServerRpc"))
                    {
                        return false;
                    }
                }
                ByteUnpacker.ReadValueBitPacked(reader, out int vehicleID);
                ByteUnpacker.ReadValueBitPacked(reader, out int newGroupCredits);
                reader.ReadValueSafe(out bool useWarranty, default);
                reader.Seek(0);
                if (PluginConfig.FreeBuy.Value)
                {
                    if (useWarranty && (bool)AccessTools.DeclaredField(typeof(Terminal), "hasWarrantyTicket").GetValue(target))
                    {
                        //可以免费购买
                    }
                    else if (PatchHelper.Money == newGroupCredits || PatchHelper.Money == 0)
                    {
                        PatchHelper.ShowMessage(PatchHelper.locale.Msg_GetString("FreeBuy_Item", new Dictionary<string, string>() {
                            { "{player}",p.playerUsername },
                            { "{items}", PatchHelper.locale.Item_GetString("Cruiser") }
                        }));
                        if (PluginConfig.FreeBuy_Kick.Value)
                        {
                            PatchHelper.KickPlayer(p);
                        }
                        return false;
                    }
                    else if (newGroupCredits > PatchHelper.Money || PatchHelper.Money < 0)
                    {
                        PatchHelper.ShowMessage(PatchHelper.locale.Msg_GetString("FreeBuy_SetMoney", new Dictionary<string, string>() {
                            { "{player}",p.playerUsername },
                            { "{Money}",(newGroupCredits - PatchHelper.Money).ToString() }
                        }));
                        if (PluginConfig.FreeBuy_Kick.Value)
                        {
                            PatchHelper.KickPlayer(p);
                        }
                        return false;
                    }
                }
                if (PluginConfig.OperationLog.Value)
                {
                    var terminal = (Terminal)target;
                    PatchHelper.ShowMessageHostOnly(PatchHelper.locale.OperationLog_GetString("BuyItem", new Dictionary<string, string>() {
                        { "{player}", p.playerUsername },
                        { "{items}", PatchHelper.locale.Item_GetString("Cruiser") }
                    }));
                }
            }
            else if (p == null)
            {
                return false;
            }
            return true;
        }

    }
}
