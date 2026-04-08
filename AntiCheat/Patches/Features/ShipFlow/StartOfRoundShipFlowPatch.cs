using HarmonyLib;

using System;
using System.Collections.Generic;
using System.Linq;

using Unity.Netcode;
using UnityEngine;

using static AntiCheat.Patches.PatchHelper;

namespace AntiCheat.Patches
{
    [HarmonyPatch(typeof(StartOfRound))]
    [HarmonyWrapSafe]
    public static class StartOfRoundShipFlowPatch
    {
        [HarmonyPatch("__rpc_handler_1089447320")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static bool StartGameServerRpc(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            StartMatchLever startMatchLever = UnityEngine.Object.FindObjectOfType<StartMatchLever>();
            if (Check(rpcParams, out var player))
            {
                if (PluginConfig.ShipConfig.Value && !GameNetworkManager.Instance.gameHasStarted)
                {
                    ShowMessage(locale.Msg_GetString("ShipConfig5", new Dictionary<string, string>()
                    {
                        { "{player}", player.playerUsername }
                    }));
                    startMatchLever.triggerScript.interactable = true;
                    if (PluginConfig.Ship_Kick.Value)
                    {
                        KickPlayer(player);
                    }
                    return false;
                }
                else if (StartOfRound.Instance.allPlayerScripts.Count(x => x.isPlayerControlled) >= PluginConfig.ShipConfig2.Value)
                {
                    return true;
                }
                else
                {
                    startMatchLever.triggerScript.interactable = true;
                    ShowMessage(locale.Msg_GetString("ShipConfig2", new Dictionary<string, string>()
                    {
                        { "{player}", player.playerUsername },
                        { "{cfg}", PluginConfig.ShipConfig2.Value.ToString() }
                    }));
                    return false;
                }
            }
            else if (player == null)
            {
                startMatchLever.triggerScript.interactable = true;
                return false;
            }
            return true;
        }

        [HarmonyPatch("__rpc_handler_2028434619")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static bool EndGameServerRpc(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (Check(rpcParams, out var player))
            {
                ByteUnpacker.ReadValueBitPacked(reader, out int num);
                reader.Seek(0);
                LogInfo(player, "StartOfRound.EndGameServerRpc", $"num:{num}", $"shipHasLanded:{StartOfRound.Instance.shipHasLanded}");
                if (num == 0 && player.playerClientId != 0)
                {
                    UnityEngine.Object.FindObjectOfType<StartMatchLever>().triggerScript.interactable = true;
                    return false;
                }
                if (!StartOfRound.Instance.shipHasLanded)
                {
                    UnityEngine.Object.FindObjectOfType<StartMatchLever>().triggerScript.interactable = true;
                    return false;
                }
                if (TimeOfDay.Instance.shipLeavingAlertCalled)
                {
                    return true;
                }
                var hour = int.Parse(PluginConfig.ShipConfig3.Value.Split(':')[0]);
                var min = int.Parse(PluginConfig.ShipConfig3.Value.Split(':')[1]);
                var time = (int)(TimeOfDay.Instance.normalizedTimeOfDay * (60f * TimeOfDay.Instance.numberOfHours)) + 360;
                int timeHours = (int)Mathf.Floor((float)(time / 60));
                bool pm = false;
                if (timeHours > 12)
                {
                    pm = true;
                    timeHours %= 12;
                }
                time = time % 60;
                if (pm)
                {
                    timeHours += 12;
                }
                var livePlayers = StartOfRound.Instance.allPlayerScripts.Where(x => x.isPlayerControlled && !x.isPlayerDead);

                if (livePlayers.Count() == 1 && livePlayers.First().isInHangarShipRoom)
                {
                    return true;
                }
                decimal minPlayers = Math.Round((decimal)livePlayers.Count() * (decimal)(PluginConfig.ShipConfig4.Value / 100m), 2);
                decimal playersInShip = StartOfRound.Instance.allPlayerScripts.Count(x => x.isPlayerControlled && x.isInHangarShipRoom);
                if (StartOfRound.Instance.currentLevel.PlanetName.Contains("Gordion"))
                {
                    timeHours = hour;
                    time = min;
                }
                if (hour <= timeHours && min <= time && playersInShip >= minPlayers)
                {
                    return true;
                }

                ShowMessage(locale.Msg_GetString("ShipConfig4", new Dictionary<string, string>()
                {
                    { "{player}", player.playerUsername },
                    { "{player_count}", minPlayers.ToString() },
                    { "{cfg4}", PluginConfig.ShipConfig4.Value.ToString() },
                    { "{cfg3}", $"{hour:00}:{min:00}" },
                    { "{game_time}", $"{timeHours:00}:{time:00}" }
                }));
                UnityEngine.Object.FindObjectOfType<StartMatchLever>().triggerScript.interactable = true;
                return false;
            }
            else if (player == null)
            {
                UnityEngine.Object.FindObjectOfType<StartMatchLever>().triggerScript.interactable = true;
                return false;
            }
            return true;
        }
    }
}
