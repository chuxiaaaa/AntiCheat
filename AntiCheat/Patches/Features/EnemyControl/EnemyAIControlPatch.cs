using HarmonyLib;

using Unity.Netcode;
using UnityEngine;

using static AntiCheat.Patches.PatchHelper;

namespace AntiCheat.Patches
{
    [HarmonyPatch(typeof(EnemyAI))]
    [HarmonyWrapSafe]
    public static class EnemyAIControlPatch
    {
        [HarmonyPatch("__rpc_handler_2081148948")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static bool SwitchToBehaviourServerRpc(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (Check(rpcParams, out var player))
            {
                ByteUnpacker.ReadValueBitPacked(reader, out int stateIndex);
                reader.Seek(0);
                var enemy = (EnemyAI)target;
                LogInfo(player, $"({enemy.enemyType.enemyName})EnemyAI.SwitchToBehaviourServerRpc", $"stateIndex:{stateIndex}");
                if (PluginConfig.Enemy.Value && enemy is JesterAI jester)
                {
                    if (jester.currentBehaviourStateIndex == 0 && stateIndex == 1)
                    {
                        if (jester.targetPlayer != null)
                        {
                            return true;
                        }
                    }
                    else if (jester.currentBehaviourStateIndex == 1 && stateIndex == 2 && jester.popUpTimer <= 0)
                    {
                        return true;
                    }
                    else if (jester.currentBehaviourStateIndex == 2 && stateIndex == 0)
                    {
                        return true;
                    }
                    else
                    {
                        LogInfo(player, $"({enemy.enemyType.enemyName})EnemyAI.SwitchToBehaviourServerRpc", $"stateIndex:{stateIndex}", $"popUpTimer:{jester.popUpTimer}");
                        return true;
                    }
                }
            }
            else if (player == null)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Prefix EnemyAI.UpdateEnemyPositionRpc
        /// </summary>
        [HarmonyPatch("__rpc_handler_255411420")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static bool UpdateEnemyPositionServerRpc(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (!Check(rpcParams, out var player) && player == null)
            {
                return false;
            }

            return true;
        }

        [HarmonyPatch("__rpc_handler_3079913705")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static bool UpdateEnemyRotationServerRpc(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (Check(rpcParams, out var player))
            {
                if (PluginConfig.Enemy.Value)
                {
                    return true;
                }
            }
            return true;
        }

        [HarmonyPatch(typeof(EnemyAI), "__rpc_handler_3587030867")]
        [HarmonyPrefix]
        [HarmonyWrapSafe]
        public static bool ChangeEnemyOwnerServerRpc(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
        {
            if (Check(rpcParams, out var player))
            {
                if (PluginConfig.Enemy.Value)
                {
                    ByteUnpacker.ReadValueBitPacked(reader, out int clientId);
                    reader.Seek(0);
                    var enemy = target.GetComponent<EnemyAI>();
                    if (enemy is MouthDogAI || enemy is DressGirlAI)
                    {
                        return true;
                    }
                    float distance = Vector3.Distance(player.transform.position, enemy.transform.position);
                    LogInfo(player, $"({enemy.enemyType.enemyName})EnemyAI.ChangeEnemyOwnerServerRpc", $"Distance:{distance}", $"CallClientId:{player.playerClientId}", $"OwnerClientId:{enemy.OwnerClientId}", $"NewClientId:{clientId}");
                    return true;
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
