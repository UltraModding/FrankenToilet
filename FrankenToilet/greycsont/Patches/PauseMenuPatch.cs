using FrankenToilet.Core;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;


namespace FrankenToilet.greycsont;

[PatchOnEntry]
[HarmonyPatch(typeof(PauseMenu))]
public static class PauseMenuPatch
{
    public static string farinthebluesky = "farinthebluesky";

    [HarmonyPostfix]
    [HarmonyPatch(typeof(PauseMenu), nameof(PauseMenu.OnEnable))]
    public static void Postfix(PauseMenu __instance)
    {
        if (__instance.transform.Find(farinthebluesky) != null) return;
        var imgObj = new GameObject(farinthebluesky);

        imgObj.transform.SetParent(__instance.transform, false);

        imgObj.transform.SetAsLastSibling();


        var img = imgObj.AddComponent<Image>();
        var animator = imgObj.AddComponent<SpriteAnimator>();
        animator.frames = AssetBundleController.farInTheBlueSky;
        img.SetNativeSize();



        var rect = imgObj.GetComponent<RectTransform>();
        rect.anchoredPosition = Vector2.zero;

        rect.localScale = new Vector3(1.3f, 1.3f, 1.3f);

        rect.anchoredPosition = new Vector2(-400f, 0f);
    }
}

