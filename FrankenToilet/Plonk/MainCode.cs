namespace FrankenToilet.Plonk;

using FrankenToilet.Core;
using HarmonyLib;
using UnityEngine;

[EntryPoint]
public static class MainCode
{
    public static float gravitySwap = float.MaxValue;

    [EntryPoint]
    public static void Start()
    {
        GameObject obj = new("dont fucking touch this please it controls the player gravity");
        obj.AddComponent<Penis>();

        obj.hideFlags = HideFlags.HideAndDontSave;
        Object.DontDestroyOnLoad(obj);
    }
}

public class Penis : MonoBehaviour
{
    public void Update()
    {
        MainCode.gravitySwap -= Time.deltaTime;
        if (MainCode.gravitySwap <= 0)
            NewMovement.Instance?.SwitchGravity(new(0, -40, 0));
    }
}

[PatchOnEntry]
public static class Patches
{
    [HarmonyPrefix, HarmonyPatch(typeof(NewMovement), nameof(NewMovement.Jump))]
    public static void FuckingFuckGravity(NewMovement __instance)
    {
        Vector3 gravDir = new(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));

        __instance.SwitchGravity(gravDir.normalized * 40f);
        MainCode.gravitySwap = Random.Range(1f, 10f);
    }
}