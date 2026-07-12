using System.Collections;
using System.Linq;
using FrankenToilet.Core;
using FrankenToilet.flazhik.Assets;
using HarmonyLib;
using TMPro;
using UnityEngine;

namespace FrankenToilet.flazhik.Patches;

[PatchOnEntry]
[HarmonyPatch(typeof(HudMessageReceiver))]
public class HudMessageReceiverPatch
{
    [ExternalAsset("Assets/UltrakillSanAndreas/Prefabs/SubtitleFontPreset.prefab", typeof(GameObject))]
    private static GameObject fontPreset;

    [ExternalAsset("Assets/UltrakillSanAndreas/Sounds/Notification.mp3", typeof(AudioClip))]
    private static AudioClip notification;

    [HarmonyPostfix]
    [HarmonyPatch(typeof(HudMessageReceiver), "Start")]
    public static void HudMessageReceiver_Start_Postfix(HudMessageReceiver __instance)
    {
        var text = fontPreset.GetComponent<TMP_Text>();
        __instance.text.font = text.font;
        __instance.aud.clip = notification;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(HudMessageReceiver), "PrepText")]
    public static bool HudMessageReceiver_PrepText_Prefix(HudMessageReceiver __instance, ref IEnumerator __result)
    {
        __instance.text.enabled = true;
        __instance.img.enabled = false;
        __instance.text.SetCharArray(__instance.fullMessage.ToCharArray());
        __result = Enumerable.Empty<int>().GetEnumerator();
        return false;
    }
}