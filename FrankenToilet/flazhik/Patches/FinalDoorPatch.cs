using FrankenToilet.Core;
using FrankenToilet.flazhik.Components;
using HarmonyLib;
using UnityEngine;
using static UnityEngine.Object;

namespace FrankenToilet.flazhik.Patches;

[PatchOnEntry]
[HarmonyPatch(typeof(FinalDoor))]
public class FinalDoorPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(typeof(FinalDoor), "Open")]
    public static bool FinalDoor_Open_Prefix(FinalDoor __instance)
    {
        var finalRoom = __instance.transform.parent;
        if (finalRoom.GetComponentInChildren<FinalPit>(true) == null)
            return true;

        var missionPassedScreen = FindFirstObjectByType<SanAndreasMissionPassedScreen>(FindObjectsInactive.Include);
        missionPassedScreen.gameObject.SetActive(true);

        return true;
    }
}