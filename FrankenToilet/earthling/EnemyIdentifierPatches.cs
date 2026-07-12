using System;
using UnityEngine;
using HarmonyLib;
using FrankenToilet.Core;

namespace FrankenToilet.earthling;

[PatchOnEntry]
[HarmonyPatch(typeof(EnemyIdentifier))]
public class EnemyIdentifierPatches
{
    [HarmonyPostfix]
    [HarmonyPatch("Start")]
    public static void MakeFilthBlue(EnemyIdentifier __instance) {
        if (__instance.enemyType != EnemyType.Filth) return;

        string texture1 = "Assets/Bundles/toiletonfire/filthmouthopen.png";
        string texture2 = "Assets/Bundles/toiletonfire/filthmouthclosed.png";

        Renderer renderer = __instance.transform.Find(
                "RotationTransform (PortalOffset)/BodyCenterRotation (PortalOffset)/ZombieFilth/Melee_Husk/"
            ).GetComponent<Renderer>(); // if it works it works
        ZombieMelee zm = __instance.gameObject.GetComponent<ZombieMelee>();

        renderer.material.mainTexture = AssetBundleHelper.LoadAsset<Texture2D>(texture1);
        zm.biteMaterial.mainTexture = AssetBundleHelper.LoadAsset<Texture2D>(texture2);
        zm.originalMaterial.mainTexture = AssetBundleHelper.LoadAsset<Texture2D>(texture1);
    }


    [HarmonyPrefix]
    [HarmonyPatch("Death", new Type[]{typeof(bool)})]
    public static void SpawnFishOnDeath(EnemyIdentifier __instance)
    {
        if (__instance.dead) return;

        FishObject fish = FishProvider.GetRandomFish();
        ItemIdentifier fishPickup = FishProvider.CreateFishPickup(fish);
        GameObject fishInstance = GameObject.Instantiate(fishPickup.gameObject, __instance.transform.position, __instance.transform.rotation, __instance.transform.parent);
        fishInstance.GetComponent<Rigidbody>().velocity += Vector3.up * 50;

        FishingHUD.Instance.ShowHUD();
        FishingHUD.Instance.ShowFishCaught(true, fish);
        FishManager.Instance.UnlockFish(fish);
    }
}