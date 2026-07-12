namespace FrankenToilet.Bryan.Patches;

using FrankenToilet.Core;
using HarmonyLib;
using UnityEngine;

/// <summary> Assigns outdoors light colors for 2-1. </summary>
[PatchOnEntry] [HarmonyPatch(typeof(OutdoorLightMaster))]
public static class OutdoorLightMasterPatch
{
    /// <summary> assign colors cuz they look cool :3 </summary>
    [HarmonyPostfix] [HarmonyPatch("Start")]
    public static void raaaaaaawruwu(OutdoorLightMaster __instance)
    {
        if (SceneHelper.CurrentScene == "Level 2-1")
        {
            __instance.outdoorLights[0].color = new Color(0.86f, 0f, 0.5f, 1f);
            __instance.outdoorLights[1].color = new Color(0f, 0.81f, 1f, 1f);
            __instance.outdoorLights[2].color = new Color(0f, 1f, 1f, 1f);
        }
    }
}