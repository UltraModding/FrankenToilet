using FrankenToilet.Core;
using HarmonyLib;

namespace FrankenToilet.mercy.Patches;

[PatchOnEntry]
[HarmonyPatch(typeof(Guttertank))]
public static class GuttertankSpeed
{
    public static bool active = false;
    [MercyFeature]
    public static void Activate() => active = true;
    [HarmonyPostfix]
    [HarmonyPatch("Start")]
    public static void Start(Guttertank __instance)
    {
        if (active) __instance.anim.speed *= 2;
    }
}
