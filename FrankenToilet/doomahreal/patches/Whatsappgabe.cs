using FrankenToilet.Core;
using HarmonyLib;
using UnityEngine;

namespace FrankenToilet.doomahreal.patches;

/// 
/// this file's length was almost reaching 150 lines
///

[PatchOnEntry]
[HarmonyPatch(typeof(GabrielVoice), "Start")]
public static class WHATSUPSTAIRS
{
    [HarmonyPostfix]
    public static void Postfix(GabrielVoice __instance)
    {
        var b = IMLOADINGITSOHARDDDD.thegrundle;
        var w = b.LoadAsset<AudioClip>("Assets/Custom/imfrakeninmykill/whatsappgabe/wt.mp3");

        for (int i = 0; i < __instance.hurt.Length; i++) __instance.hurt[i] = w;
        for (int i = 0; i < __instance.bigHurt.Length; i++) __instance.bigHurt[i] = w;
        for (int i = 0; i < __instance.taunt.Length; i++) __instance.taunt[i] = w;
        for (int i = 0; i < __instance.tauntSecondPhase.Length; i++) __instance.tauntSecondPhase[i] = w;

        for (int i = 0; i < __instance.taunts.Length; i++) __instance.taunts[i] = "*Whatsapp notification*";
        for (int i = 0; i < __instance.tauntsSecondPhase.Length; i++) __instance.tauntsSecondPhase[i] = "*Whatsapp notification*";

        __instance.phaseChange = w;
        __instance.phaseChangeSubtitle = "*Whatsapp notification*";

        var tg = b.LoadAsset<Texture2D>("Assets/Custom/imfrakeninmykill/whatsappgabe/gabenormal.png");
        var tw = b.LoadAsset<Texture2D>("Assets/Custom/imfrakeninmykill/whatsappgabe/normal.png");

        var eb = b.LoadAsset<Texture2D>("Assets/Custom/imfrakeninmykill/whatsappgabe/gameenraged.png");
        var ew = b.LoadAsset<Texture2D>("Assets/Custom/imfrakeninmykill/whatsappgabe/enraged.png");

        var g = __instance.GetComponent<GabrielBase>();
        if (g != null) { g.enrageBody.mainTexture = eb; g.enrageWing.mainTexture = ew; g.origBody.mainTexture = tg; g.origWing.mainTexture = tw; }
    }
}
