using System.Collections.Generic;
using FrankenToilet.Core;
using HarmonyLib;
using UnityEngine;
using Object = UnityEngine.Object;

namespace FrankenToilet.doomahreal.patches
{
    public static class StatsManagerMonitor
    {
        public static GameObject SpawnedInstance;
        public static bool Spawned;
        public static EnemyIdentifier lastFILTHstanding;

        static readonly List<EnemyIdentifier> trackedEids = new List<EnemyIdentifier>();

        public static void addtothecbtlistt(EnemyIdentifier eid)
        {
            if (eid == null) return;
            if (!trackedEids.Contains(eid)) trackedEids.Add(eid);
        }

        public static void TrySpawn(StatsManager stats)
        {
            if (lastFILTHstanding != null && lastFILTHstanding.dead)
            {
                CleanupSpawn();
                lastFILTHstanding = null;
            }
            if (Spawned) return;
            if (stats == null) return;
            if (stats.killRanks == null || stats.killRanks.Length <= 3) return;

            int kills = stats.kills;
            int sThreshold = stats.killRanks[3];
            if (kills != sThreshold - 1) return;

            for (int i = trackedEids.Count - 1; i >= 0; i--)
            {
                var eid = trackedEids[i];
                if (eid == null)
                {
                    trackedEids.RemoveAt(i);
                    continue;
                }
                if (eid.dead)
                {
                    trackedEids.RemoveAt(i);
                    continue;
                }

                bool anyBuff = eid.healthBuff || eid.speedBuff || eid.damageBuff;

                if (anyBuff)
                    eid.radianceTier *= 1.35f;
                else
                    eid.radianceTier = 2f;

                eid.healthBuff = eid.speedBuff = eid.damageBuff = true;
                eid.UpdateModifiers();

                var prefab = IMLOADINGITSOHARDDDD.thegrundle.LoadAsset<GameObject>("Assets/Custom/imfrakeninmykill/forsakenlms/soulcursedasymrobloxgames.prefab");
                SpawnedInstance = Object.Instantiate(prefab);
                lastFILTHstanding = eid;

                Spawned = true;
                break;
            }
        }

        public static void CleanupSpawn()
        {
            if (SpawnedInstance != null) Object.Destroy(SpawnedInstance);
            SpawnedInstance = null;
            Spawned = false;
            lastFILTHstanding = null;
        }
    }

    [PatchOnEntry]
    [HarmonyPatch(typeof(EnemyIdentifier), "Start")]
    public static class EnemyIdentifier_StartPatch
    {
        [HarmonyPostfix]
        public static void Postfix(EnemyIdentifier __instance)
        {
            StatsManagerMonitor.addtothecbtlistt(__instance);
        }
    }

    [PatchOnEntry]
    [HarmonyPatch(typeof(StatsManager), "Update")]
    public static class StatsManager_UpdatePatch
    {
        [HarmonyPostfix]
        public static void Postfix(StatsManager __instance)
        {
            StatsManagerMonitor.TrySpawn(__instance);
        }
    }

    [PatchOnEntry]
    [HarmonyPatch(typeof(NewMovement), "Respawn")]
    public static class NewMovement_RespawnPatch
    {
        [HarmonyPrefix]
        public static void Prefix()
        {
            StatsManagerMonitor.CleanupSpawn();
        }
    }
}
