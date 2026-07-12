using FrankenToilet.Core;
using HarmonyLib;
using UnityEngine;

namespace FrankenToilet.doomahreal.patches;

//[PatchOnEntry]
//[HarmonyPatch]
//public static class headspin
//{
//    private static int _evilStartCounter = 0;

//    [HarmonyPatch(typeof(CameraController), "Update")]
//    [HarmonyPostfix]
//    static void Postfix(CameraController __instance)
//    {
//        Vector3 euler = __instance.transform.localEulerAngles;

//        float z = euler.z;
//        if (z > 180f) z -= 360f;

//        z += 6f;

//        __instance.transform.localEulerAngles = new Vector3(euler.x, euler.y, z);
//    }
//}

[PatchOnEntry]
[HarmonyPatch]
public static class headspin
{
    private static int _evilStartCounter = 0;

    [HarmonyPatch(typeof(CameraController), "Update")]
    [HarmonyPostfix]
    static void Postfix(CameraController __instance)
    {
        __instance.tilt = false;
        __instance.transform.localEulerAngles = new Vector3(0f - __instance.rotationX, 0f, 5f);
    }
}