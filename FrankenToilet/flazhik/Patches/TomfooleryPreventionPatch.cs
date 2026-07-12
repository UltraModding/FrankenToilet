using System;
using System.Collections.Generic;
using System.Linq;
using FrankenToilet.Core;
using FrankenToilet.flazhik.Components;
using HarmonyLib;
using TMPro;
using UnityEngine;
using static UnityEngine.Object;

namespace FrankenToilet.flazhik.Patches;

[PatchOnEntry]
[HarmonyPatch(typeof(TextMeshProUGUI))]
public static class TomfooleryPreventionPatch
{
    private static readonly HashSet<Type> WhitelistedComponents =
    [
        typeof(RectTransform),
        typeof(TextMeshProUGUI),
        typeof(CanvasRenderer),
        typeof(FontProtector),
        typeof(TMP_Text)
    ];

    [HarmonyPostfix]
    [HarmonyPatch("OnEnable")]
    public static void DontFuckAroundWithMyFonts(TextMeshProUGUI __instance)
    {
        if (!__instance.gameObject.TryGetComponent<FontProtector>(out var fontProtector))
            return;

        foreach (var component in __instance.gameObject.GetComponents<Component>().Where(static component =>
                     !WhitelistedComponents.Contains(component.GetType())))
        {
            if (__instance.font != fontProtector.originalFont)
                __instance.font = fontProtector.originalFont;
            if (__instance.color != fontProtector.originalColor)
                __instance.color = fontProtector.originalColor;
            Destroy(component);
        }
    }
}