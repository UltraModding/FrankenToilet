using FrankenToilet.Core;
using FrankenToilet.flazhik.Components;
using HarmonyLib;
using UnityEngine;
using static UnityEngine.Object;

namespace FrankenToilet.flazhik.Patches;

[PatchOnEntry]
[HarmonyPatch(typeof(PlayerActivatorRelay))]
public class PlayerActivatorRelayPatch
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(PlayerActivatorRelay), "Activate")]
    public static void PlayerActivatorRelay_Activate_Postfix()
    {
        var hud = FindFirstObjectByType<SanAndreasHud>(FindObjectsInactive.Include);
        if (hud != null)
            hud.gameObject.SetActive(true);

        var levelName = FindFirstObjectByType<SanAndreasLevelName>(FindObjectsInactive.Include);
        if (levelName != null)
            levelName.gameObject.SetActive(true);
    }
}