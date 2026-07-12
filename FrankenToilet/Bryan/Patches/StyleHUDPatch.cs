namespace FrankenToilet.Bryan.Patches;

using FrankenToilet.Core;
using HarmonyLib;
using System.Collections.Generic;

/// <summary> Patch the silly lil style hud :3 </summary>
[PatchOnEntry] [HarmonyPatch(typeof(StyleHUD))]
public static class StyleHUDPatch
{
    /// <summary> Goofed style ranks :3 </summary>
    public static Dictionary<string, string> styleEdits = new()
    {
        ["ultrakill.kill"] = "DEAD",
        ["ultrakill.doublekill"] = "<color=orange>DEADx2</color>",
        ["ultrakill.triplekill"] = "<color=orange>DEADx3</color>",
        ["ultrakill.bigkill"] = "BIG DEAD",
        ["ultrakill.bigfistkill"] = "BIG ARM DEAD",
        ["ultrakill.headshot"] = "NECKSHOT",
        ["ultrakill.bigheadshot"] = "BIG NECKSHOT",
        ["ultrakill.limbhit"] = "LIMB SHOT",
        ["ultrakill.interruption"] = "<color=#f1f>SUPRISE</color>",
        ["ultrakill.arsenal"] = "<color=#3c3>WEAPONS</color>",
        ["ultrakill.splattered"] = "SPLAT",
        ["ultrakill.instakill"] = "<color=#f1f>QUICKLYDEAD</color>",
        ["ultrakill.fireworks"] = "<color=#3c3>MAKE IT RAIN</color>",
        ["ultrakill.airslam"] = "<color=#3c3>AIR POUND</color>",
        ["ultrakill.airshot"] = "<color=#3c3>AIRSHIT</color>",
        ["ultrakill.groundslam"] = "GROUND POUND",
        ["ultrakill.overkill"] = "MEGAMURDER",
        ["ultrakill.exploded"] = "BOOM",
        ["ultrakill.fried"] = "SIZLE",
        ["ultrakill.mauriced"] = "ROCK CRUSH",
        ["ultrakill.multikill"] = "<color=orange>DEADxALOT</color>",
        ["ultrakill.finishedoff"] = "<color=#3c3>FINISH HIM</color>",
        ["ultrakill.iconoclasm"] = "BOOM!!!",
        ["ultrakill.roundtrip"] = "VOYAGE",
    };

    /// <summary> use my style edits first 3:< </summary>
    [HarmonyPrefix] [HarmonyPatch(typeof(StyleHUD), "GetLocalizedName")]
    public static bool USEMINEGRRR(string id, ref string __result)
    {
        if (styleEdits.TryGetValue(id, out var replacement))
        {
            __result = replacement;
            return false;
        }

        return true;
    }
}