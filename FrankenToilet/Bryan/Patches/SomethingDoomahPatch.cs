namespace FrankenToilet.Bryan.Patches;

using FrankenToilet.Core;
using HarmonyLib;
using UnityEngine;

/// <summary> Something Heavy this way comes, replaces something wicked with a png of Heavy. </summary>
[PatchOnEntry] [HarmonyPatch(typeof(Wicked))]
public static class SomethingHeavyPatch
{
    /// <summary> dooooooooooooomaaaaaaaaaah </summary>
    [HarmonyPrefix] [HarmonyPatch("Start")]
    public static void dooooooomahhhhh(Wicked __instance)
    {
        __instance.transform.Find("SomethingWicked").gameObject.SetActive(false);

        var Heavy = Object.Instantiate(Assets.HeavyReal, __instance.transform);
        Heavy.transform.localPosition = new(0f, 5f, 0f);
        foreach (var mat in Heavy.GetComponent<MeshRenderer>().materials)
            mat.shader = DefaultReferenceManager.Instance.masterShader;
    }
}