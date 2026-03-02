using AntiCheat;

using HarmonyLib;

using System;
using System.Collections.Generic;

using Unity.Netcode;

using UnityEngine;

[HarmonyPatch(typeof(Landmine))]
[HarmonyWrapSafe]
public static class LandminePatch
{
    private static readonly HashSet<int> _landMines = new HashSet<int>();
    private static readonly List<ExplosionData> _explosions = new List<ExplosionData>();
    private static readonly object _explosionsLock = new object();

    private static readonly AccessTools.FieldRef<Landmine, bool> _mineActivatedField =
        AccessTools.FieldRefAccess<Landmine, bool>("mineActivated");

    private const int EXPLOSION_EXPIRE_SECONDS = 10;
    private const float MINE_TRIGGER_DISTANCE = 5f;

    [HarmonyPrefix]
    [HarmonyPatch("SpawnExplosion")]
    public static void SpawnExplosion(Vector3 explosionPosition)
    {
        AntiCheatPlugin.LogInfo($"Landmine.SpawnExplosion -> {explosionPosition}");

        lock (_explosionsLock)
        {
            _explosions.RemoveAll(x => x.CreateDateTime.AddSeconds(EXPLOSION_EXPIRE_SECONDS) < DateTime.UtcNow);

            _explosions.Add(new ExplosionData(explosionPosition));
        }
    }

    [HarmonyPrefix]
    [HarmonyPatch("OnTriggerExit")]
    public static bool TriggerMineOnLocalClientByExiting(Landmine __instance, Collider other)
    {
        if (!StartOfRound.Instance.IsHost ||
            __instance.hasExploded ||
            !__instance.gameObject.activeSelf)
        {
            return true;
        }

        if (!_mineActivatedField(__instance))
        {
            return true;
        }

        lock (_landMines)
        {
            _landMines.Add(__instance.GetInstanceID());
        }

        return true;
    }

    [HarmonyPatch("__rpc_handler_3032666565")]
    [HarmonyPrefix]
    [HarmonyWrapSafe]
    public static bool __rpc_handler_3032666565(NetworkBehaviour target, FastBufferReader reader, __RpcParams rpcParams)
    {
        if (!PluginConfig.Landmine.Enable || !Check(rpcParams, out var p) || p == null)
        {
            return p != null; // 如果 p 为 null 返回 false
        }

        var lm = (Landmine)target;
        int id = lm.GetInstanceID();

        lock (_landMines)
        {
            if (_landMines.Contains(id) || lm.hasExploded)
            {
                return true;
            }
        }

        // 使用距离平方比较，避免开方
        const float triggerDistanceSq = MINE_TRIGGER_DISTANCE * MINE_TRIGGER_DISTANCE;

        if (recentPlayerPositions.TryGetValue(p.playerSteamId, out var positions) &&
            !positions.Any(x => (x.pos - lm.transform.position).sqrMagnitude < triggerDistanceSq))
        {
            AntiCheatPlugin.ShowMessage(locale.Msg_GetString("Landmine", new Dictionary<string, string>
            {
                ["{player}"] = p.playerUsername
            }));

            if (PluginConfig.Landmine.Kick)
            {
                KickPlayer(p);
            }
        }

        return true;
    }
}

public class ExplosionData
{
    public List<ulong> CalledClient { get; } = new List<ulong>();
    public Vector3 ExplosionPostion { get; set; }
    public DateTime CreateDateTime { get; set; }

    public ExplosionData(Vector3 position)
    {
        ExplosionPostion = position;
        CreateDateTime = DateTime.UtcNow;
    }
}