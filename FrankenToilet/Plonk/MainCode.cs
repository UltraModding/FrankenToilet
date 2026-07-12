using FrankenToilet.Core;
using HarmonyLib;
using UnityEngine;

namespace FrankenToilet.Plonk;

[EntryPoint]
public static class MainCode
{
    [EntryPoint]
    public static void Start()
    {
        GameObject obj = new GameObject("dont fucking touch this please");
        obj.AddComponent<Penis>();
    }
    public static float gravitySwap = 0f;
    [PatchOnEntry]
    public static class Patches 
    {
        [HarmonyPrefix, HarmonyPatch(typeof(NewMovement), nameof(NewMovement.Jump))]
        public static void FuckingFuckGravity(NewMovement __instance)
        {
            Vector3 gravDir = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
            Physics.gravity = gravDir * 40f;
            gravitySwap = Random.Range(1f, 10f);
        }
    }
} 

public class Penis : MonoBehaviour
{  
    public void Start() { gameObject.hideFlags = HideFlags.HideAndDontSave; DontDestroyOnLoad(this.gameObject); }

    public void Update() => MainCode.gravitySwap -= Time.deltaTime;
}
