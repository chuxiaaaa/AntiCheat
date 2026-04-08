using GameNetcodeStuff;

using HarmonyLib;

using System.Collections.Generic;

using Unity.Netcode;
using UnityEngine;

using static AntiCheat.Patches.PatchHelper;

namespace AntiCheat.Patches
{
    [HarmonyPatch(typeof(PlayerControllerB))]
    [HarmonyWrapSafe]
    public static class PlayerControllerCombatPatch
    {
        [HarmonyPatch("__rpc_handler_4121569671")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static bool KillPlayerServerRpc(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (Check(rpcParams, out var player))
            {
                if (rpcs.ContainsKey("KillPlayer"))
                {
                    rpcs["KillPlayer"].Remove(player.playerClientId);
                }
                ByteUnpacker.ReadValueBitPacked(reader, out int playerId);
                reader.ReadValueSafe(out bool spawnBody, default);
                reader.ReadValueSafe(out Vector3 bodyVelocity);
                ByteUnpacker.ReadValueBitPacked(reader, out int num);
                reader.Seek(0);
                LogInfo(player, "PlayerControllerB.KillPlayerServerRpc", $"playerId:{PlayerClientIdConvertName(playerId)}({playerId})", $"spawnBody:{spawnBody}", $"bodyVelocity:{bodyVelocity}", $"num:{(CauseOfDeath)num}({num})");
                if (playerId < 0)
                {
                    LogInfo($"KillPlayerServerRpc:Invalid PlayerId({playerId})");
                    return false;
                }
                if (StartOfRound.Instance.allPlayerScripts[playerId] != player)
                {
                    LogInfo("KillPlayerServerRpc:Can't kill other player!");
                    return false;
                }
                if (player.isPlayerDead)
                {
                    LogInfo("KillPlayerServerRpc:Player death can't kill!");
                    return false;
                }
                if ((CauseOfDeath)num == CauseOfDeath.Abandoned)
                {
                    string msg = locale.Msg_GetString("behind_player", new Dictionary<string, string>()
                    {
                        { "{player}", player.playerUsername }
                    });
                    LogInfo(msg);
                    AddTextMessageClientRpc(msg);
                }
            }
            else if (player == null)
            {
                return false;
            }
            return true;
        }

        [HarmonyPatch("__rpc_handler_638895557")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static bool DamagePlayerFromOtherClientServerRpc(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (Check(rpcParams, out var player))
            {
                ByteUnpacker.ReadValueBitPacked(reader, out int damageAmount);
                reader.ReadValueSafe(out Vector3 hitDirection);
                ByteUnpacker.ReadValueBitPacked(reader, out int playerWhoHit);
                reader.Seek(0);
                LogInfo(player, "PlayerControllerB.DamagePlayerFromOtherClientServerRpc", $"damageAmount:{damageAmount}", $"hitDirection:{hitDirection}", $"playerWhoHit:{PlayerClientIdConvertName(playerWhoHit)}({playerWhoHit})");
                var targetPlayer = (PlayerControllerB)target;
                return CheckDamage(targetPlayer, player, ref damageAmount);
            }
            else if (player == null)
            {
                return false;
            }
            return true;
        }

        [HarmonyPatch("__rpc_handler_2585603452")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static bool HealServerRpc(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (Check(rpcParams, out var player))
            {
                LogInfo(player, "PlayerControllerB.HealServerRpc", $"health:{player.health}", "newHealth:20");
                player.health = 20;
            }
            else if (player == null)
            {
                return false;
            }
            return true;
        }

        [HarmonyPatch("DamagePlayer")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static bool DamagePlayer(PlayerControllerB __instance, int damageNumber, bool hasDamageSFX = true, bool callRPC = true, CauseOfDeath causeOfDeath = CauseOfDeath.Unknown, int deathAnimation = 0, bool fallDamage = false, Vector3 force = default)
        {
            return true;
        }

        [HarmonyPatch("__rpc_handler_1084949295")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static bool DamagePlayerServerRpc(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (Check(rpcParams, out var player) || StartOfRound.Instance.IsHost)
            {
                if (rpcs.ContainsKey("Hit"))
                {
                    rpcs["Hit"].Remove(player.playerClientId);
                }
                ByteUnpacker.ReadValueBitPacked(reader, out int damageNumber);
                ByteUnpacker.ReadValueBitPacked(reader, out int newHealthAmount);
                reader.Seek(0);
                LogInfo(player, "PlayerControllerB.DamagePlayerServerRpc", $"damageNumber:{damageNumber}", $"newHealthAmount:{newHealthAmount}");
                var targetPlayer = (PlayerControllerB)target;
                if (targetPlayer == player)
                {
                    if (PluginConfig.Health_Recover.Value && damageNumber < 0)
                    {
                        string msg = locale.Msg_GetString("Health_Recover", new Dictionary<string, string>()
                        {
                            { "{player}", player.playerUsername },
                            { "{hp}", (damageNumber * -1).ToString() }
                        });
                        ShowMessage(msg);
                        if (PluginConfig.Health_Kick.Value)
                        {
                            KickPlayer(player);
                        }
                        return false;
                    }
                    return true;
                }
                return CheckDamage(targetPlayer, player, ref damageNumber);
            }
            else if (player == null)
            {
                return false;
            }
            return true;
        }
    }
}
