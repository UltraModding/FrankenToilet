using FrankenToilet.Core;
using HarmonyLib;
using UnityEngine;
using Object = UnityEngine.Object;

namespace FrankenToilet.doomahreal.patches;

[PatchOnEntry]
[HarmonyPatch]
public static class Startlevelpatch
{
    private static int _evilStartCounter = 0;

    [HarmonyPatch(typeof(PlayerTracker), "LevelStart")]
    [HarmonyPostfix]
    public static void Postfix()
    {
        _evilStartCounter++;
        LogHelper.LogInfo($"Evil Scary Round Start Counter: {_evilStartCounter}");
        if (_evilStartCounter < 6) return; //This is jank, level loading calls this twice and i want it to happen after 3 times, Fuck you hakita

        var bundle = IMLOADINGITSOHARDDDD.thegrundle;
        GameObject? prefab = bundle.LoadAsset<GameObject>("Assets/Custom/imfrakeninmykill/evilscary/round start canvas/EvilScary 1.prefab");

        // evil scary could spawn camp you so delete chaser

        if (Chaser.Instance != null)
            Object.Destroy(Chaser.Instance.gameObject);

        Object.Instantiate(prefab);
        _evilStartCounter = 0;
    }
}
