using GameNetcodeStuff;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

namespace AntiCheat
{
    public static class CooldownManager
    {
        private static readonly Dictionary<string, CooldownData> cooldownGroups = new Dictionary<string, CooldownData>();

        public static void Reset()
        {
            cooldownGroups.Clear();
        }

        public static void RegisterCooldownGroup(string groupName, Func<bool> isEnabled, Func<float> getCooldown)
        {
            cooldownGroups.Add(groupName, new CooldownData
            {
                IsEnabled = isEnabled,
                GetCooldown = getCooldown,
                CooldownList = new List<ulong>()
            });
        }

        public static IEnumerator HandleCooldown(string groupName, ulong playerId)
        {
            var data = cooldownGroups[groupName];
            data.CooldownList.Add(playerId);

            yield return new WaitForSeconds(data.GetCooldown());

            data.CooldownList.Remove(playerId);
        }

        public static bool CheckCooldown(string groupName, PlayerControllerB player)
        {
            var data = cooldownGroups[groupName];
            if (!data.IsEnabled() || data.GetCooldown() <= 0)
                return true;
            if (data.CooldownList.Contains(player.playerSteamId))
                return false;
            player.StartCoroutine(HandleCooldown(groupName, player.playerSteamId));
            return true;
        }

        private struct CooldownData
        {
            public Func<bool> IsEnabled;
            public Func<float> GetCooldown;
            public List<ulong> CooldownList;
        }
    }
}
