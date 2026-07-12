using HarmonyLib;
using UnityEngine;
using FrankenToilet.Core;
using Random = UnityEngine.Random;
using Object = UnityEngine.Object;

namespace FrankenToilet.doomahreal.patches;

[PatchOnEntry]
[HarmonyPatch]
public static class OpenPatch
{
    private static GameObject _dozerInstance;
    private static GameObject _pandeInstance;
    private static float _nextSpawnTime = 0f;
    private static readonly float SpawnCooldown = 180f;

    [HarmonyPatch(typeof(Door), "Open")]
    [HarmonyPostfix]
    public static void Postfix(Door __instance, bool enemy, bool skull)
    {
        if (Time.time < _nextSpawnTime || _dozerInstance != null && _pandeInstance != null) return;

        var bundle = IMLOADINGITSOHARDDDD.thegrundle;
        var dozerPrefab = bundle.LoadAsset<GameObject>("Assets/Custom/imfrakeninmykill/dozer/IKEEPONGROOVINGIKEEPONMOVING.prefab");
        var pandePrefab = bundle.LoadAsset<GameObject>("Assets/Custom/imfrakeninmykill/pande/Pandepande.prefab");

        bool pickDozer = Random.value > 0.5f;

        if (pickDozer && _dozerInstance == null)
        {
            _dozerInstance = Object.Instantiate(dozerPrefab);
            LogHelper.LogInfo("Spawned Dozer");
            _nextSpawnTime = Time.time + SpawnCooldown;
        }
        else if (!pickDozer && _pandeInstance == null)
        {
            _pandeInstance = Object.Instantiate(pandePrefab);
            LogHelper.LogInfo("Spawned Pande");
            _nextSpawnTime = Time.time + SpawnCooldown;
        }
    }
}
