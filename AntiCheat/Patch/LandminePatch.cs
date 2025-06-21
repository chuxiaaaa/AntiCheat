using HarmonyLib;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

namespace AntiCheat.Patch
{
    [HarmonyPatch(typeof(Landmine))]
    [HarmonyWrapSafe]
    public static class LandminePatch
    {
        [HarmonyPrefix]
        [HarmonyPatch("SpawnExplosion")]
        public static void SpawnExplosion(Vector3 explosionPosition)
        {
            Patches.LogInfo($"Landmine.SpawnExplosion -> {explosionPosition}");
            Patches.explosions = Patches.explosions.Where(x => x.CreateDateTime.AddSeconds(10) > DateTime.Now).ToList();
            Patches.explosions.Add(new Patches.ExplosionData()
            {
                ExplosionPostion = explosionPosition,
                CalledClient = new List<ulong>(),
                CreateDateTime = DateTime.Now
            });
        }
    }
}
