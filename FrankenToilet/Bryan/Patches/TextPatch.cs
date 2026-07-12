namespace FrankenToilet.Bryan.Patches;

using FrankenToilet.Core;
using HarmonyLib;
using TMPro;
using UnityEngine.UI;

/// <summary> Patch textMeshProUGUI components (text components) to make them funky. </summary>
[PatchOnEntry] [HarmonyPatch(typeof(TextMeshProUGUI))]
public static class TextMeshProUGUIPatch
{
    /// <summary> Replace font with sand + fucker </summary>
    [HarmonyPrefix] [HarmonyPatch("Awake")] [HarmonyPatch("OnEnable")]
    public static void ChangeFontAndFUck(TextMeshProUGUI __instance)
    { 
        __instance.font = Assets.ComicSands ?? __instance.font;
        
        if (__instance.gameObject.GetComponent<TextFucker>() == null)
            __instance.gameObject.AddComponent<TextFucker>();
    }
}

/// <summary> Patch Text components (legacy text components) to make them funky. </summary>
[PatchOnEntry] [HarmonyPatch(typeof(Text))]
public static class LegacyTextPatch
{
    /// <summary> Replace font with sand + fuck them </summary>
    [HarmonyPrefix] [HarmonyPatch("OnEnable")]
    public static void ChangeFontAndFuck(Text __instance)
    {
        __instance.font = Assets.L_ComicSands ?? __instance.font;

        if (__instance.gameObject.GetComponent<TextFucker>() == null)
            __instance.gameObject.AddComponent<TextFucker>();
    }
}