using FrankenToilet.Core;
using FrankenToilet.flazhik.Assets;
using FrankenToilet.flazhik.Components;
using HarmonyLib;
using UnityEngine;
using static UnityEngine.Object;

namespace FrankenToilet.flazhik.Patches;

[PatchOnEntry]
[HarmonyPatch(typeof(StyleHUD))]
public class StyleHUDPatch
{
    [ExternalAsset("Assets/UltrakillSanAndreas/Prefabs/StylePanelCanvas.prefab", typeof(GameObject))]
    public static GameObject styleHudCanvas;

    [HarmonyPostfix]
    [HarmonyPatch(typeof(StyleHUD), "Start")]
    public static void StyleHUD_Start_Postfix(StyleHUD __instance, ref Vector3 ___defaultPos, GameObject ___styleHud)
    {
        // Adjust StyleHUD position
        const int adjustment = 50;
        var saPanel = Instantiate(styleHudCanvas, __instance.transform);
        saPanel.AddComponent<SanAndreasHud>();
        var saPanelHeight = saPanel.GetComponent<RectTransform>().rect.height;
        ___styleHud.transform.localPosition -= new Vector3(0, saPanelHeight - adjustment, 0);
        saPanel.transform.localPosition -= new Vector3(0, saPanelHeight - adjustment, 0);

        // Only activate on PlayerActivatorRelay#Activate()
        saPanel.SetActive(false);
    }
}