using FrankenToilet.Core;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace FrankenToilet.prideunique;

[PatchOnEntry]
[HarmonyPatch(typeof(NewMovement))]
public static class SlidePatch
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(NewMovement.CheckForGasoline))]
    private static void Prefix(NewMovement __instance)
    {
        Vector3Int vector3Int = StainVoxelManager.WorldToVoxelPosition(__instance.transform.position + Vector3.down * 1.83333337f);
        __instance.lastCheckedGasolineVoxel = vector3Int;
        __instance.modForcedFrictionMultip = 0f;
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(NewMovement.FixedUpdate))]
    private static void Prefix1(NewMovement __instance)
    {
        __instance.slideSafety = 20.0f;
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(NewMovement.Dodge))]
    private static void Prefix2(NewMovement __instance)
    {
        if (AssetsController.IsSlopSafe)
        {
            __instance.movementDirection2 *= 1.01f;
            __instance.dodgeDirection *= 1.01f;
        }
        else
        {
            __instance.movementDirection2 *= 1.005f;
            __instance.dodgeDirection *= 1.005f;
        }
    }
}