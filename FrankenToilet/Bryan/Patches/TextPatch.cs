namespace FrankenToilet.Bryan.Patches;

using FrankenToilet.Core;
using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary> Patch textMeshProUGUI components (text components) to make them funky. </summary>
[PatchOnEntry]
[HarmonyPatch(typeof(TextMeshProUGUI))]
public static class TextMeshProUGUIPatch
{
    /// <summary> Replace font with sand </summary>
    [HarmonyPrefix]
    [HarmonyPatch("Awake")]
    public static void ChangeFont(TextMeshProUGUI __instance) =>
        __instance.font = BundleLoader.ComicSands ?? __instance.font;
}

/// <summary> Patch Text components (legacy text components) to make them funky. </summary>
[PatchOnEntry]
[HarmonyPatch(typeof(Text))]
public static class LegacyTextPatch
{
    /// <summary> Replace font with sand </summary>
    [HarmonyPrefix]
    [HarmonyPatch("OnEnable")]
    public static void ChangeFont(Text __instance) =>
        __instance.font = BundleLoader.ComicSandsLegacy ?? __instance.font;
}