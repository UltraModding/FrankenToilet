using FrankenToilet.Core;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace FrankenToilet.prideunique;

// Fix enemies breaking once dead

[PatchOnEntry]
[HarmonyPatch(typeof(Machine))]
public static class MachinePatch
{
    [HarmonyPrefix]
    [HarmonyPatch( nameof(Machine.GoLimp), new Type[] { typeof(bool) } ) ]
    private static void Prefix(Machine __instance)
    {
        __instance.aud = __instance.gameObject.GetOrAddComponent<AudioSource>();
    }
}
