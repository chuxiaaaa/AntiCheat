using AntiCheat.Utils;

using BepInEx;
using BepInEx.Configuration;

using GameNetcodeStuff;

using HarmonyLib;

using Netcode.Transports.Facepunch;

using Steamworks;
using Steamworks.Data;
using Steamworks.ServerList;

using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using TMPro;

using Unity.Collections.LowLevel.Unsafe;
using Unity.Netcode;

using UnityEngine;
using UnityEngine.Events;

using static UnityEngine.GraphicsBuffer;

namespace AntiCheat.Patches
{
    public static class PatchHelper
    {
        public static Locale.LocalizationManager locale { get => AntiCheatPlugin.localizationManager; }

        public static List<ulong> jcs = new List<ulong>();

        public static Dictionary<ulong, List<string>> czcd = new Dictionary<ulong, List<string>>();
        public static Dictionary<ulong, List<string>> sdqcd = new Dictionary<ulong, List<string>>();

        public static Dictionary<uint, ulong> ConnectionIdtoSteamIdMap { get; set; } = new Dictionary<uint, ulong>();

        //public static Dictionary<int, Dictionary<ulong, List<DateTime>>> chcs = new Dictionary<int, Dictionary<ulong, List<DateTime>>>();

        public static List<long> mjs { get; set; } = new List<long>();

        public static List<int> landMines { get; set; }

        public static Dictionary<string, List<ulong>> rpcs { get; set; } = new Dictionary<string, List<ulong>>();

        internal static Dictionary<ulong, List<(Vector3 pos, float time)>> recentPlayerPositions = new Dictionary<ulong, List<(Vector3 pos, float time)>>();

        public static void LogInfo(string info) => AntiCheatPlugin.LogInfo(info);

        public static void LogInfo(PlayerControllerB p, string rpc, params object[] param) => AntiCheatPlugin.LogInfo(p, rpc, param);

        public static void LogError(string info)
        {
            if (PluginConfig.Log.Value)
            {
                AntiCheatPlugin.Log.LogError($"{info}");
            }
        }
        internal static bool CheckDamage(PlayerControllerB p2, PlayerControllerB p, ref int damageAmount)
        {
            if (damageAmount == 0)
            {
                return true;
            }
            LogInfo($"{p.playerUsername} hit {p2.playerUsername} damageAmount:{damageAmount}|p2:{p2.playerUsername}");
            try
            {
                if (PluginConfig.Shovel.Value)
                {
                    var distance = Vector3.Distance(p.transform.position, p2.transform.position);
                    var obj = p.ItemSlots[p.currentItemSlot];
                    string playerUsername = p.playerUsername;
                    if (jcs.Contains(p.playerSteamId))
                    {
                        damageAmount = 0;
                    }
                    else if (damageAmount != 20 && obj != null && (isShovel(obj) || isKnife(obj)))
                    {
                        if (!jcs.Contains(p.playerSteamId))
                        {
                            ShowMessage(locale.Msg_GetString("Shovel", new Dictionary<string, string>() {
                                { "{player}",p.playerUsername },
                                { "{player2}",p2.playerUsername },
                                { "{damageAmount}",damageAmount.ToString() },
                                { "{item}", isShovel(obj) ? locale.Item_GetString("Shovel") : locale.Item_GetString("Knife") }
                            }));
                            jcs.Add(p.playerSteamId);
                            if (PluginConfig.Shovel2.Value)
                            {
                                KickPlayer(p);
                            }
                        }
                        damageAmount = 0;
                    }
                    else if (distance > 11 && obj != null && (isShovel(obj) || isKnife(obj)))
                    {
                        if (p2.isPlayerDead)
                        {
                            return true;
                        }
                        if (!jcs.Contains(p.playerSteamId))
                        {
                            ShowMessage(locale.Msg_GetString("Shovel2", new Dictionary<string, string>() {
                                { "{player}",p.playerUsername },
                                { "{player2}",p2.playerUsername },
                                { "{distance}",distance.ToString() },
                                { "{damageAmount}",damageAmount.ToString() },
                                { "{item}", isShovel(obj) ? locale.Item_GetString("Shovel") : locale.Item_GetString("Knife") }
                            }));
                            jcs.Add(p.playerSteamId);
                            if (PluginConfig.Shovel2.Value)
                            {
                                KickPlayer(p);
                            }
                        }
                        damageAmount = 0;
                    }
                    else
                    {
                        if (ClingTime.ContainsKey(p.playerSteamId) && ClingTime[p.playerSteamId].AddSeconds(5) > DateTime.Now)
                        {
                            return true;
                        }
                        if (p.ItemSlots.Any(x => isShovel(x)) && damageAmount == 10)
                        {
                            return true;
                        }
                        //for (int i = 0; i <.Length; i++)
                        //{
                        //    LogInfo($"p:{p.playerUsername}|i:{i}|itemName:{p.ItemSlots[i]?.itemProperties?.itemName}");
                        //}
                        //LogInfo($"currentItemSlot:{p.currentItemSlot}");
                        //LogInfo($"currentlyHeldObjectServer:{p.currentlyHeldObjectServer?.itemProperties?.itemName}");
                        //LogInfo($"obj:{obj}");
                        //if (!jcs.Contains(p.playerSteamId))
                        //{
                        //    if (PluginConfig.Shovel3.Value && (damageAmount == 10 || damageAmount == 20 || damageAmount == 30 || damageAmount == 100))
                        //    {
                        //        ShowMessage(locale.Msg_GetString("Shovel3", new Dictionary<string, string>() {
                        //            { "{player}",p.playerUsername },
                        //            { "{player2}",p2.playerUsername },
                        //            { "{damageAmount}",damageAmount.ToString() }
                        //        }));
                        //        return true;
                        //    }
                        //    else
                        //    {
                        //        ShowMessage(locale.Msg_GetString("Shovel3", new Dictionary<string, string>() {
                        //            { "{player}",p.playerUsername },
                        //            { "{player2}",p2.playerUsername },
                        //            { "{damageAmount}",damageAmount.ToString() }
                        //        }));
                        //        jcs.Add(p.playerSteamId);
                        //        if (PluginConfig.Shovel2.Value)
                        //        {
                        //            KickPlayer(p);
                        //        }
                        //        damageAmount = 0;
                        //    }
                        //}
                    }
                    if (damageAmount == 0)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                LogInfo($"{ex.ToString()}");
            }
            return true;
        }
        public static bool isGun(GrabbableObject item)
        {
            return item is ShotgunItem;
        }

        public static bool isShovel(GrabbableObject item)
        {
            return item is Shovel;
        }

        public static bool isKnife(GrabbableObject item)
        {
            return item is KnifeItem;
        }

        public static bool isJetpack(GrabbableObject item)
        {
            return item is JetpackItem;
        }

        public static ulong lastClientId { get; set; }
        public static Terminal terminal
        {
            get
            {
                if (_terminal == null)
                {
                    _terminal = UnityEngine.Object.FindObjectOfType<Terminal>();
                }
                return _terminal;
            }
        }

        internal static Terminal _terminal { get; set; }
        public static int Money = -1;
        [HarmonyWrapSafe]



        public static IEnumerator CheckRpc(PlayerControllerB __instance, string RPC)
        {
            if (RPC == "Hit" && !PluginConfig.RPCReport_Hit.Value)
            {
                yield break;
            }
            if (RPC == "KillPlayer" && !PluginConfig.RPCReport_KillPlayer.Value)
            {
                yield break;
            }
            LogInfo("700");
            yield return new WaitForSeconds(PluginConfig.RPCReport_Delay.Value / 1000);
            LogInfo("702");
            if (__instance.isPlayerDead)
            {
            LogInfo("705");
                yield break;
            }
            LogInfo("708");
            if (rpcs[RPC].Contains(__instance.playerClientId))
            {
                LogInfo("711");
                rpcs[RPC].Remove(__instance.playerClientId);
                LogInfo($"{__instance.playerUsername}:{__instance.isPlayerDead}");
                ShowMessage(locale.Msg_GetString("RPCReport", new Dictionary<string, string>() {
                  { "{player}", __instance.playerUsername },
                  { "{RPC}", RPC },

                }));
                if (PluginConfig.RPCReport_Kick.Value)
                {
                    KickPlayer(__instance);
                }
            }
            yield break;
        }
        #region Enemy Kill Player(Only Player Self)

        public static Dictionary<ulong, DateTime> ClingTime { get; set; } = new Dictionary<ulong, DateTime>();



        internal static bool KillPlayerServerRpc(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams, string call)
        {
            if (Check(rpcParams, out var p))
            {
                var e = (EnemyAI)target;
                ByteUnpacker.ReadValueBitPacked(reader, out int playerId);
                reader.Seek(0);
                LogInfo(p, $"{e.GetType()}.KillPlayerServerRpc", $"playerId:{playerId}");
                if (playerId <= StartOfRound.Instance.allPlayerScripts.Length && StartOfRound.Instance.allPlayerScripts[playerId] != p)
                {
                    LogInfo(p, $"{e.GetType()}.KillPlayerServerRpc", $"playerUsername:{StartOfRound.Instance.allPlayerScripts[playerId].playerUsername}");
                    return false;
                }
                if (!rpcs.ContainsKey("KillPlayer"))
                {
                    rpcs.Add("KillPlayer", new List<ulong>());
                }
                if (p.AllowPlayerDeath())
                {
                    LogInfo(p, "StartCoroutine:CheckRpc(KillPlayer)", $"call:{call}");
                    rpcs["KillPlayer"].Add(p.playerClientId);
                    p.StartCoroutine(CheckRpc(p, "KillPlayer"));
                }
            }
            else if (p == null)
            {
                return false;
            }
            return true;
        }

        #endregion
        public static List<HitData> bypassHit = new List<HitData>();
        public static List<HitData> bypassKill = new List<HitData>();
        public static List<ExplosionData> explosions { get; set; } = new List<ExplosionData>();

        public class ExplosionData
        {
            public List<ulong> CalledClient { get; set; }

            public Vector3 ExplosionPostion { get; set; }

            public DateTime CreateDateTime { get; set; }
        }

        public class HitData
        {
            public int EnemyInstanceId { get; set; }

            public int force { get; set; }


            public List<ulong> CalledClient { get; set; }
        }

        public static PlayerControllerB lastDriver { get; set; }
        //public static void HitEnemy(int force = 1, PlayerControllerB playerWhoHit = null, bool playHitSFX = false, int hitID = -1)
        //{
        //    LogInfo($"force:{force}|playerWhoHit:{playerWhoHit?.playerUsername}|playHitSFX:{playHitSFX}|hitID:{hitID}");
        //}

        public static string PlayerClientIdConvertName(int index)
        {
            if (index < 0 || index >= StartOfRound.Instance.allPlayerScripts.Length)
            {
                return "Unknown";
            }
            return StartOfRound.Instance.allPlayerScripts[index].playerUsername;
        }
        /// <summary>
        /// Broadcast a formatted anti-cheat message.
        /// </summary>
        public static void ShowMessage(string msg, string lastmsg = null)
        {
            AntiCheatPlugin.ShowMessage(msg, lastmsg ?? msg);
        }

        public static void AddTextMessageClientRpc(string showmsg)
        {
            AntiCheatPlugin.AddTextMessageClientRpc(showmsg);
        }

        public static bool bypass { get; set; }
        public static IEnumerator DestroySelf(GameObject self)
        {
            yield return new WaitForSeconds(5f);
            UnityEngine.Object.Destroy(self);
        }
        /// <summary>
        /// 检测是否需要处理事件(顺带处理掉SteamID为0的玩家)
        /// </summary>
        public static bool Check(__RpcParams rpcParams, out PlayerControllerB p)
        {
            if (StartOfRound.Instance.localPlayerController == null)
            {
                p = default;
                return false;
            }
            else if (!StartOfRound.Instance.localPlayerController.IsHost)//非主机
            {
                p = StartOfRound.Instance.localPlayerController;
                return false;
            }
            var tmp = GetPlayer(rpcParams);
            p = tmp;
            if (p == null)//没玩家
            {
                NetworkManager.Singleton.DisconnectClient(rpcParams.Server.Receive.SenderClientId);
                return false;
            }
            else if (rpcParams.Server.Receive.SenderClientId == GameNetworkManager.Instance.localPlayerController.actualClientId)//非本地
            {
                p = StartOfRound.Instance.localPlayerController;
                return false;
            }
            else if (StartOfRound.Instance.KickedClientIds.Contains(p.playerSteamId))//如果被踢
            {
                Task.Delay(-100);
                NetworkManager.Singleton.DisconnectClient(rpcParams.Server.Receive.SenderClientId);
                return false;
            }
            else if (p.playerSteamId == 0)
            {
                uint clientId = ClientIdToTransportId(rpcParams.Server.Receive.SenderClientId);
                if (clientId == 0)
                {
                    return false;
                }
                ulong steamId = ConnectionIdtoSteamIdMap[clientId];
                Friend f = new Steamworks.Friend(steamId);
                NetworkManager.Singleton.DisconnectClient(rpcParams.Server.Receive.SenderClientId);
                StartOfRound.Instance.KickedClientIds.Add(steamId);
                LogInfo($"检测玩家 {f.Name}({steamId}) 使用AntiKick功能，已自动踢出！");
                return false;
            }
            return true;
        }

        public static uint ClientIdToTransportId(ulong SenderClientId)
        {
            if (SenderClientId == 0)
            {
                return 0;
            }
            NetworkConnectionManager networkConnectionManager = Traverse.Create(NetworkManager.Singleton).Field("ConnectionManager").GetValue<NetworkConnectionManager>();
            ulong transportId = Traverse.Create(networkConnectionManager).Method("ClientIdToTransportId", new object[] { SenderClientId }).GetValue<ulong>();
            return (uint)transportId;
        }

        public static bool CheckRemoteTerminal(PlayerControllerB p, string call)
        {
            LogInfo(p, "CheckRemoteTerminal", $"Call:{call}");
            if (whoUseTerminal == null && lastWhoUseTerminal.Value == p.playerSteamId)
            {
                LogInfo($"whoUseTerminal == null && lastWhoUseTerminal == {p.playerSteamId}||Time:{Math.Round((DateTime.Now - lastWhoUseTerminal.Key).TotalSeconds, 2)}s");
                if (lastWhoUseTerminal.Key.AddSeconds(10) > DateTime.Now)
                {
                    return true;
                }
            }
            if (whoUseTerminal != p)
            {
                if (whoUseTerminal == null)
                {
                    LogInfo($"no player use terminal|request player:{p.playerUsername}|call:{call}");
                }
                else
                {
                    LogInfo($"whoUseTerminal:{whoUseTerminal.playerUsername}|p:{p.playerUsername}|call:{call}");
                }
                ShowMessage(locale.Msg_GetString("RemoteTerminal", new Dictionary<string, string>() {
                    { "{player}",p.playerUsername }
                }));
                if (PluginConfig.RemoteTerminal2.Value)
                {
                    KickPlayer(p);
                }
                return false;
            }
            return true;
        }
        /// <summary>
        /// 通过ClientId找到调用RPC的玩家
        /// </summary>
        /// <param name="rpcParams"></param>
        /// <returns></returns>
        internal static PlayerControllerB GetPlayer(__RpcParams rpcParams)
        {
            foreach (var item in StartOfRound.Instance.allPlayerScripts)
            {
                if (item.actualClientId == rpcParams.Server.Receive.SenderClientId)
                {
                    return item;
                }
            }
            return null;//??
        }

        public static Dictionary<ulong, bool> ReloadGun = new Dictionary<ulong, bool>();
        public static IEnumerator CheckAmmo(PlayerControllerB p, ShotgunItem shot, GrabbableObject ammo)
        {
            yield return new WaitForSeconds(0.95f + 0.3f);
            yield return new WaitForSeconds(3f);//delay
            if (ReloadGun.ContainsKey(p.playerSteamId) && !ReloadGun[p.playerSteamId])
            {
                if (ammo != null && ammo.NetworkObject != null && ammo.NetworkObject.IsSpawned)
                {
                    ShowMessage(locale.Msg_GetString("InfiniteAmmo", new Dictionary<string, string>() {
                        { "{player}",p.playerUsername }
                    }));
                    if (PluginConfig.InfiniteAmmo2.Value)
                    {
                        KickPlayer(p);
                    }
                }
            }
        }
        internal static bool CheckCoolDownMethod(__RpcParams rpcParams, int cd)
        {
            if (Check(rpcParams, out var p))
            {
                if (PluginConfig.ItemCooldown.Value)
                {
                    var id = p.playerSteamId;
                    var m = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    if (!czcd.ContainsKey(id))
                    {
                        czcd.Add(id, new List<string>());
                    }
                    if (czcd[id].Count > 200)
                    {
                        czcd[id].RemoveRange(0, czcd[id].Count - 1);
                    }
                    if (czcd[id].Count(x => x == m) >= cd)
                    {
                        ShowMessage(locale.Msg_GetString("ItemCooldown", new Dictionary<string, string>() {
                            { "{player}",p.playerUsername },
                            { "{item}",locale.Item_GetString("Shovel") }
                        }));
                        if (PluginConfig.ItemCooldown2.Value)
                        {
                            KickPlayer(p);
                        }
                        return false;
                    }
                    else
                    {
                        czcd[id].Add(m);
                    }
                }
            }
            else if (p == null)
            {
                return false;
            }
            return true;
        }
        public static KeyValuePair<DateTime, ulong> lastWhoUseTerminal { get; set; }
        public static PlayerControllerB whoUseTerminal { get; set; }

        /// <summary>
        /// 踢出玩家
        /// </summary>
        /// <param name="kick">玩家</param>
        /// <param name="canJoin">重新加入</param>
        public static void KickPlayer(PlayerControllerB kick, bool canJoin = false, string Reason = null)
        {
            if (kick.playerClientId == 0)
            {
                return;
            }
            NetworkManager.Singleton.DisconnectClient(kick.playerClientId, $"[{locale.Prefix()}] {locale.Msg_GetString("KickPlayer")}！");
            if (kick.playerSteamId == 0)
            {
                return;
            }
            var s = StartOfRound.Instance;
            ulong playerSteamId = kick.playerSteamId;
            if (!s.KickedClientIds.Contains(playerSteamId) && !canJoin)
            {
                s.KickedClientIds.Add(playerSteamId);
            }
            var terminal = UnityEngine.Object.FindAnyObjectByType<Terminal>();
            LogInfo("terminalInUse" + terminal.placeableObject.inUse);
            LogInfo("whoUseTerminal:" + whoUseTerminal?.playerUsername);
            LogInfo("playerUsername:" + kick?.playerUsername);
            if (whoUseTerminal == kick && terminal.placeableObject.inUse)
            {
                LogInfo("SetTerminalInUseServerRpc");
                terminal.SetTerminalInUseClientRpc(false);
                terminal.terminalInUse = false;
            }
            ShowMessage(locale.Msg_GetString("Kick", new Dictionary<string, string>() {
                { "{player}",kick.playerUsername }
            }));
        }
        public static void ShowMessageHostOnly(string msg)
        {
            AntiCheatPlugin.ShowMessageHostOnly(msg);
        }
    }
}
