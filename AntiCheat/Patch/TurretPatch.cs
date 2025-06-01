using AntiCheat.Locale;

using HarmonyLib;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Unity.Netcode;

using UnityEngine;

namespace AntiCheat.Patch
{
    [HarmonyPatch(typeof(Turret))]
    [HarmonyWrapSafe]
    public static class TurretPatch
    {

        /// <summary>
        /// 机枪激怒事件(检测)
        /// Prefix Turret.EnterBerserkModeServerRpc
        /// </summary>
        [HarmonyPatch("__rpc_handler_4195711963")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static bool EnterBerserkModeServerRpc(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (Patches.Check(rpcParams, out var p))
            {
                if (Core.AntiCheat.Turret.Value)
                {
                    var obj = p.ItemSlots[p.currentItemSlot];
                    if (obj != null && (Patches.isShovel(obj) || Patches.isKnife(obj)))
                    {
                        var t = (Turret)target;
                        float v = Vector3.Distance(t.transform.position, p.transform.position);
                        if (v > 12)
                        {
                            Patches.ShowMessage(Patches.locale.Msg_GetString("Turret", new Dictionary<string, string>() {
                                { "{player}",p.playerUsername },
                                { "{Distance}",v.ToString() }
                            }));
                            if (Core.AntiCheat.Turret2.Value)
                            {
                                Patches.KickPlayer(p);
                            }
                            return false;
                        }
                    }
                    else
                    {
                        Patches.ShowMessage(Patches.locale.Msg_GetString("Turret2", new Dictionary<string, string>() {
                             { "{player}",p.playerUsername }
                        }));
                        if (Core.AntiCheat.Turret2.Value)
                        {
                            Patches.KickPlayer(p);
                        }
                        return false;
                    }
                }
            }
            else if (p == null)
            {
                return false;
            }
            return true;
        }

        [HarmonyPatch("__rpc_handler_2339273208")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static bool ToggleTurretServerRpc(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (Patches.Check(rpcParams, out var p))
            {
                bool remote = Patches.CheckRemoteTerminal(p);
                Core.AntiCheat.LogInfo(p,"Turret.ToggleTurretServerRpc", $"remote:{(!remote)}");
                if (!remote)
                {
                    return false;
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
