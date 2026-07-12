using System.Collections;
using System.Collections.Generic;
using FrankenToilet.Core;
using HarmonyLib;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace FrankenToilet.triggeredidiot;

[EntryPoint]
public class Tnt : MonoBehaviour
{
    private class ForceFieldTimer : MonoBehaviour
    {
        public GameObject forceField;

        public void Update()
        {
            if(forceField)
                forceField.SetActive(IsForcefield);
            ForcefieldTime -= Time.deltaTime;
        }
    }

    public static float ForcefieldTime = 0.0f;
    public const float UsualForcefieldTime = 4.0f;
    public static bool IsForcefield => ForcefieldTime > 0.0f;
    public static float MaxTntMult = 1;
    public static float NavMeshCheckRadius = 48.0f;

    // misc util stuff
    private static bool _getRandomPointOnNavMesh(Vector3 center, float radius, out Vector3 result)
    {
        for (int i = 0; i < 30; i++) // retry a few times
        {
            Vector3 randomPoint = center + (Random.insideUnitSphere * radius);

            if (NavMesh.SamplePosition(
                    randomPoint,
                    out NavMeshHit hit,
                    radius,
                    NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }
        }

        result = Vector3.zero;
        return false;
    }
    private static Transform _getRootParent(Transform t)
    {
        while (t.parent != null)
        {
            t = t.parent;
        }
        return t;
    }

    [EntryPoint]
    public static void Hook()
    {
        LogHelper.LogDebug("[triggeredidiot] Hooking tnt placer");

        bool hasCheckedSteam = false;
        SceneManager.sceneLoaded += (s, lcm) =>
        {
            if(s.name == "Bootstrap" || SceneHelper.CurrentScene == "Main Menu" || SceneHelper.CurrentScene == "Intro" || SceneHelper.CurrentScene == "Endless")
                return;

            if (!hasCheckedSteam)
            {
                hasCheckedSteam = true;
                if (AssetsController.IsSlopSafe)
                    MaxTntMult *= 1.25f;
            }

            if(MonoSingleton<PrefsManager>.Instance != null)
                MaxTntMult += (MonoSingleton<PrefsManager>.Instance.GetInt("difficulty") + 1.0f) / 16.0f;

            ForcefieldTime = UsualForcefieldTime;

            var doors = FindObjectsByType<Door>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            doors = doors ?? [];
            var objects = FindObjectsByType<MeshCollider>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            List<MeshCollider> floors = [];
            foreach (var obj in objects)
                if (obj != null && obj.CompareTag("Floor") && _getRootParent(obj.transform).name.ToLower() != "firstroom")
                    floors.Add(obj);

            int tntCap = Mathf.FloorToInt((doors.Length * MaxTntMult) + (floors.Count * MaxTntMult));
            float tntChance = (5.5f) / (tntCap * 0.0025f);

            int tntSpawned = 0;

            List<Vector3> searchPoints = [];
            foreach (var door in doors)
            {
                if (door != null)
                {
                    for (int i = 0; i < MaxTntMult + 2; i++)
                        searchPoints.Add(door.transform.position);
                }
            }

            foreach (var floor in floors)
            {
                for (int i = 0; i < MaxTntMult; i++)
                {
                    if(Random.Range(0, 100) < tntChance * MaxTntMult)
                        searchPoints.Add(floor.transform.position);
                }
            }

            int droppedTntCount = 0;
            foreach (var searchPoint in searchPoints)
            {
                if(tntSpawned > tntCap)
                    break;

                if (_getRandomPointOnNavMesh(searchPoint, NavMeshCheckRadius, out var result))
                {
                    var tnt = AssetsController.LoadAsset("TntRoot");
                    if(tnt == null)
                    {
                        LogHelper.LogError("[triggeredidiot] Unable to load tnt asset!");
                        break;
                    }
                    tnt.transform.position = result;
                    tnt.AddComponent<Tnt>();
                    var rb = tnt.GetComponent<Rigidbody>();
                    rb.isKinematic = true;
                    rb.useGravity = true;
                    rb.freezeRotation = true;
                    tntSpawned++;
                }
                else
                    droppedTntCount++;
            }

            if (droppedTntCount > 0)
                LogHelper.LogWarning($"[triggeredidiot] {droppedTntCount} misplaced tnt!");

            var fft = new GameObject("Forcefield Controller (for tnt)").AddComponent<ForceFieldTimer>();
            if (NewMovement.Instance != null)
            {
                var ffi = AssetsController.LoadAsset("ForceFieldIndic");
                if (ffi != null)
                {
                    ffi.transform.SetParent(NewMovement.Instance.transform);
                    ffi.transform.localPosition = Vector3.zero;
                    fft.forceField = ffi;
                }
            }
        };
    }

    public void Explode()
    {
        Tnt[] tnts = FindObjectsByType<Tnt>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        StartCoroutine(_explode(tnts));
    }

    private IEnumerator _explode(Tnt[]? cache = null)
    {
        if(IsForcefield)
        {
            Destroy(this.gameObject);
            yield break;
        }
        DeltaruneExplosion.ExplodePlayer();
        transform.Find("ExplodeEffect").gameObject.SetActive(true);
        GetComponentInChildren<MeshRenderer>().enabled = false;

        this.enabled = false;
        Destroy(this.gameObject, 1.0f);
        foreach (var tnt in cache)
        {
            if(tnt.enabled && Vector3.Distance(tnt.transform.position, this.transform.position) < 12.0f)
            {
                tnt.enabled = false;
                tnt.StartCoroutine(tnt._explode(cache));
                yield return new WaitForSecondsRealtime(0.5f);
            }
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInChildren<NewMovement>() != null)
            Explode();
    }

    private void Start()
    {
        StartCoroutine(_animLoop());
    }
    IEnumerator _animLoop()
    {
        yield return new WaitForSecondsRealtime(Random.Range(0.01f, 0.1f));
        Transform target = NewMovement.Instance.transform;
        Animator anim = GetComponentInChildren<Animator>(includeInactive:true);
        while (true)
        {
            float dist = Vector3.Distance(transform.position, target.position);
            anim.enabled = dist < 24.0f;

            yield return new WaitForSecondsRealtime(Random.Range(0.05f, 0.25f));
        }
    }
}

[PatchOnEntry]
[HarmonyPatch(typeof(NewMovement), "Respawn")]
public static class TntInjector_Respawn
{
    public static void Prefix(NewMovement __instance)
    {
        Tnt.ForcefieldTime = Tnt.UsualForcefieldTime;
    }
}