namespace FrankenToilet.Bryan.Patches;

using FrankenToilet.Core;
using HarmonyLib;

[PatchOnEntry] [HarmonyPatch(typeof(Projectile))]
public static class ProjectilePatch
{
    /// <summary> Add a projectile fucker to the projectile and that will handle all the fancy shit </summary>
    [HarmonyPrefix] [HarmonyPatch("Start")]
    public static void meow(Projectile __instance)
    {
        if (__instance.GetComponent<ProjectileFucker>() == null) // check if it already has a ProjecttileFucker
            __instance.gameObject.AddComponent<ProjectileFucker>(); // since this will make it constantly create more projectiles when it dupes itself
    }
}