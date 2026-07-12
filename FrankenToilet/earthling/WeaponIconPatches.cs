using FrankenToilet.Core;
using HarmonyLib;

namespace FrankenToilet.earthling;

[PatchOnEntry]
[HarmonyPatch(typeof(WeaponIcon))]
public static class WeaponIconPatches
{
    [HarmonyPrefix]
    [HarmonyPatch("Start")]
    public static void ReplaceIcon(WeaponIcon __instance)
    {
        WeaponDescriptor? awesomeGun = AssetBundleHelper.LoadAsset<WeaponDescriptor>("Assets/Bundles/toiletonfire/awesomegun.asset");
        WeaponDescriptor newDescriptor = new WeaponDescriptor();

        if (awesomeGun == null)
        {
            LogHelper.LogError("Could not load awesome weapon icons");
            return;
        }

        newDescriptor.weaponName = __instance.weaponDescriptor.weaponName;
        newDescriptor.icon = awesomeGun.icon;
        newDescriptor.glowIcon = awesomeGun.glowIcon;

        switch (__instance.weaponDescriptor.variationColor)
        {
            case WeaponVariant.RedVariant:
                newDescriptor.variationColor = WeaponVariant.BlueVariant;
                break;
            case WeaponVariant.BlueVariant:
                newDescriptor.variationColor = WeaponVariant.GreenVariant;
                break;
            case WeaponVariant.GreenVariant:
                newDescriptor.variationColor = WeaponVariant.RedVariant;
                break;
            default:
                newDescriptor.variationColor = WeaponVariant.GoldVariant;
                break;
        }

        __instance.weaponDescriptor = newDescriptor;
    }
}