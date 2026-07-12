using System;
using System.Collections.Generic;
using System.Linq;
using FrankenToilet.Core;
using FrankenToilet.mercy.Features;
using HarmonyLib;
using plog.Models;

namespace FrankenToilet.mercy.Patches;

[PatchOnEntry]
[HarmonyPatch(typeof(EnemyIdentifier))]
public static class LevelingPatches
{
    // please someone tell me that there's a better way of doing this
    public static EnemyType[] strongEnemies = [
        EnemyType.BigJohnator, EnemyType.Cerberus, EnemyType.HideousMass, EnemyType.MaliciousFace, EnemyType.Mindflayer,
        EnemyType.Swordsmachine, EnemyType.Virtue, EnemyType.Turret, EnemyType.Idol, EnemyType.VeryCancerousRodent,
        EnemyType.Gutterman, EnemyType.Guttertank, EnemyType.Sisyphus, EnemyType.Ferryman, EnemyType.MirrorReaper,
        EnemyType.Providence, EnemyType.Deathcatcher, EnemyType.Power
    ];
    public static EnemyType[] bossEnemies = [
        EnemyType.V2, EnemyType.V2Second, EnemyType.Gabriel, EnemyType.GabrielSecond, EnemyType.MinosPrime, EnemyType.Minos,
        EnemyType.Leviathan, EnemyType.SisyphusPrime, EnemyType.FleshPanopticon, EnemyType.FleshPrison, EnemyType.Minotaur,
        EnemyType.Mandalore, EnemyType.Centaur, EnemyType.Geryon
    ];
    // are we deadass
    [HarmonyPatch("Death", new Type[]{typeof(bool)})]
    [HarmonyPostfix]
    public static void LevelOnDeath(EnemyIdentifier __instance)
    {
        if (Plugin.canvas.GetComponentInChildren<LevelingSystem>())
        {
            int expIncrease;
            if (bossEnemies.Contains(__instance.enemyType)) expIncrease = Plugin.rand.Next(50, 100);
            else if (strongEnemies.Contains(__instance.enemyType)) expIncrease = Plugin.rand.Next(10, 50);
            else expIncrease = Plugin.rand.Next(1, 10);
            Plugin.canvas.GetComponentInChildren<LevelingSystem>().IncreaseExp(expIncrease);
        }
    }

}
