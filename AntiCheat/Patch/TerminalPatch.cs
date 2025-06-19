using AntiCheat.Locale;

using HarmonyLib;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Unity.Netcode;

using static Unity.IO.LowLevel.Unsafe.AsyncReadManagerMetrics;

namespace AntiCheat.Patch
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
            if (Patches.Check(rpcParams, out var p))
            {
                if (Core.AntiCheat.RemoteTerminal.Value)
                {
                    if (!Patches.CheckRemoteTerminal(p, "Terminal.BuyItemsServerRpc"))
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
                if (Core.AntiCheat.FreeBuy.Value)
                {
                    //LogInfo("__rpc_handler_4003509079|boughtItems:" + string.Join(",", boughtItems) + "|newGroupCredits:" + newGroupCredits + "|Money:" + Money);
                    if (Patches.Money == newGroupCredits || Patches.Money == 0)
                    {
                        var terminal = (Terminal)target;
                        Patches.ShowMessage(Patches.locale.Msg_GetString("FreeBuy_Item", new Dictionary<string, string>() {
                            { "{player}",p.playerUsername },
                            { "{items}", string.Join(",", boughtItems.GroupBy(x => terminal.buyableItemsList[x].itemName).Select(g => g.Count() == 1 ? g.Key : $"{g.Key}*{g.Count()}")) }
                        }));
                        if (Core.AntiCheat.FreeBuy2.Value)
                        {
                            Patches.KickPlayer(p);
                        }
                        return false;
                    }
                    else if (newGroupCredits > Patches.Money || Patches.Money < 0)
                    {
                        Patches.ShowMessage(Patches.locale.Msg_GetString("FreeBuy_SetMoney", new Dictionary<string, string>() {
                            { "{player}",p.playerUsername },
                            { "{Money}",(newGroupCredits - Patches.Money).ToString() }
                        }));
                        if (Core.AntiCheat.FreeBuy2.Value)
                        {
                            Patches.KickPlayer(p);
                        }
                        return false;
                    }
                }
                if (Core.AntiCheat.OperationLog.Value)
                {
                    var terminal = (Terminal)target;
                    if (boughtItems.Count(x => x < terminal.buyableItemsList.Length) == boughtItems.Count())
                    {
                        Patches.ShowMessageHostOnly(Patches.locale.OperationLog_GetString("BuyItem", new Dictionary<string, string>() {
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
            if (Patches.Check(rpcParams, out var p) || true)
            {
                if (Core.AntiCheat.RemoteTerminal.Value)
                {
                    if (!Patches.CheckRemoteTerminal(p, "Terminal.BuyVehicleServerRpc"))
                    {
                        return false;
                    }
                }
                ByteUnpacker.ReadValueBitPacked(reader, out int vehicleID);
                ByteUnpacker.ReadValueBitPacked(reader, out int newGroupCredits);
                reader.ReadValueSafe(out bool useWarranty, default);
                reader.Seek(0);
                if (Core.AntiCheat.FreeBuy.Value)
                {
                    if(useWarranty && (bool)AccessTools.DeclaredField(typeof(Terminal), "hasWarrantyTicket").GetValue(target))
                    {
                        //可以免费购买
                    }
                    else if (Patches.Money == newGroupCredits || Patches.Money == 0)
                    {
                        Patches.ShowMessage(Patches.locale.Msg_GetString("FreeBuy_Item", new Dictionary<string, string>() {
                            { "{player}",p.playerUsername },
                            { "{items}", Patches.locale.Item_GetString("Cruiser") }
                        }));
                        if (Core.AntiCheat.FreeBuy2.Value)
                        {
                            Patches.KickPlayer(p);
                        }
                        return false;
                    }
                    else if (newGroupCredits > Patches.Money || Patches.Money < 0)
                    {
                        Patches.ShowMessage(Patches.locale.Msg_GetString("FreeBuy_SetMoney", new Dictionary<string, string>() {
                            { "{player}",p.playerUsername },
                            { "{Money}",(newGroupCredits - Patches.Money).ToString() }
                        }));
                        if (Core.AntiCheat.FreeBuy2.Value)
                        {
                            Patches.KickPlayer(p);
                        }
                        return false;
                    }
                }
                if (Core.AntiCheat.OperationLog.Value)
                {
                    var terminal = (Terminal)target;
                    Patches.ShowMessageHostOnly(Patches.locale.OperationLog_GetString("BuyItem", new Dictionary<string, string>() {
                        { "{player}", p.playerUsername },
                        { "{items}", Patches.locale.Item_GetString("Cruiser") }
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
