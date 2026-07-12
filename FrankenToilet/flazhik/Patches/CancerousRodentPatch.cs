using FrankenToilet.Core;
using FrankenToilet.flazhik.Assets;
using FrankenToilet.flazhik.Components;
using HarmonyLib;
using UnityEngine;
using static UnityEngine.Object;

namespace FrankenToilet.flazhik.Patches;

[PatchOnEntry]
[HarmonyPatch(typeof(CancerousRodent))]
public class CancerousRodentPatch
{
    [ExternalAsset("Assets/UltrakillSanAndreas/Prefabs/Big Smoke.prefab", typeof(GameObject))]
    private static GameObject bigSmoke;

    [ExternalAsset("Assets/UltrakillSanAndreas/Prefabs/Little Smoke.prefab", typeof(GameObject))]
    private static GameObject littleSmoke;

    [HarmonyPrefix]
    [HarmonyPatch(typeof(CancerousRodent), "Start")]
    public static void CancerousRodent_Start_Postfix(CancerousRodent __instance)
    {
        SubtitleController.Instance.DisplaySubtitle("You picked the wrong house, fool!");
        var harmless = __instance.harmless;

        // Renaming poor rodent
        var smokeRank = harmless ? "LITTLE SMOKE" : "BIG SMOKE";
        __instance.eid.overrideFullName = smokeRank;
        var bossHealthBar = __instance.GetComponent<BossHealthBar>();
        if (bossHealthBar != null)
            bossHealthBar.bossName = smokeRank;

        if (harmless)
        {
            var machine = __instance.GetComponent<Machine>();
            machine.chest.GetComponent<MeshRenderer>().enabled = false;
            machine.chest = Instantiate(littleSmoke, __instance.transform);
            machine.chest.GetComponent<AudioSource>().outputAudioMixerGroup = AudioMixerController.Instance.allGroup;
        }
        else
        {
            var statue = __instance.GetComponent<Statue>();
            statue.chest.GetComponent<MeshRenderer>().enabled = false;
            statue.chest = Instantiate(bigSmoke, __instance.transform);
            statue.chest.GetComponent<AudioSource>().outputAudioMixerGroup = AudioMixerController.Instance.allGroup;
        }
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(CancerousRodent), "OnGoLimp")]
    public static void CancerousRodent_OnGoLimp_Prefix(CancerousRodent __instance)
    {
        var bigSmokeDeathHelper = new GameObject
        {
            name = "BigSmokeDeathHelper"
        };
        bigSmokeDeathHelper.AddComponent<BigSmokeDeath>();

        if (__instance.harmless)
            Destroy(__instance.GetComponent<Machine>().chest);
    }
}