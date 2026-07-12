namespace FrankenToilet.Bryan.Patches;

using FrankenToilet.Core;
using HarmonyLib;
using UnityEngine;

/// <summary> die </summary>
[PatchOnEntry] [HarmonyPatch(typeof(MaliciousFace))]
public static class MuaricePATCH
{
    /// <summary> low quality </summary>
    [HarmonyPrefix] [HarmonyPatch("Awake")]
    public static void lowquality(MaliciousFace __instance)
    {
        SkinnedMeshRenderer mr = __instance.transform.Find("MaliciousFace/MaliciousFace").GetComponent<SkinnedMeshRenderer>();
        mr.enabled = false;

        var mauriceBad = Object.Instantiate(Assets.MauriceBad, mr.transform);
        foreach (var mat in mauriceBad.GetComponent<MeshRenderer>().materials)
            mat.shader = DefaultReferenceManager.Instance.masterShader;
    }
}