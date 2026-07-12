namespace FrankenToilet.Bryan.Patches;

using FrankenToilet.Core;
using HarmonyLib;
using UnityEngine;

/// <summary> Turns every skull into Heavy. </summary>
[PatchOnEntry] [HarmonyPatch(typeof(Skull))]
public static class HeavySkullPatch
{
    /// <summary> dooooooooooooomaaaaaaaaaah2 </summary>
    [HarmonyPrefix] [HarmonyPatch("Awake")]
    public static void heeeeavvvvvvvvvyyyy2(Skull __instance)
    {
        Transform oldSkull = __instance.transform.Find("NewSkull");
        oldSkull.gameObject.SetActive(false);

        ItemIdentifier id = __instance.GetComponent<ItemIdentifier>();
        GameObject Heavy = id.itemType switch
        {
            ItemType.SkullBlue => Object.Instantiate(Assets.HeavyBlue, __instance.transform),
            ItemType.SkullGreen => Object.Instantiate(Assets.HeavyGreen, __instance.transform),
            ItemType.SkullRed or _ => Object.Instantiate(Assets.HeavyRed, __instance.transform)
        };

        Heavy.transform.localPosition = new(-0.2f, 0f, -0.5f);
        Heavy.transform.localEulerAngles = new(0f, 20f, 0f);
    }
}