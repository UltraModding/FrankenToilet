using FrankenToilet.Core;
using HarmonyLib;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace FrankenToilet.triggeredidiot;

public sealed class DeltaruneExplosion : MonoBehaviour
{
    private static DeltaruneExplosion? _instance = null;

    private bool _thirdPersonActive = false;
    public Camera hudCamera = null!;
    public GameObject effect = null!;
    public float thirdPersonDist = 4.5f;
    public static GameObject? LastEffect = null;

    public static void EnableThirdPerson()
    {
        if (_instance == null) return;

        _instance._thirdPersonActive = true;
        _instance.hudCamera.gameObject.SetActive(false);
    }
    public static void DisableThirdPerson()
    {
        if (_instance == null) return;

        _instance._thirdPersonActive = false;
        _instance.hudCamera.gameObject.SetActive(true);
    }

    private void Awake()
    {
        _instance = this;
    }

    public float orbitYaw = 0f;
    public float orbitPitch = 20f;

    private void LateUpdate()
    {
        if (!_thirdPersonActive) return;

        Quaternion rotation = Quaternion.Euler(orbitPitch, orbitYaw, 0f);
        Vector3 offset = rotation * Vector3.back * thirdPersonDist;

        var pos = transform.position + Vector3.up * 1.4f;
        CameraController.Instance.transform.position = pos + offset;
        CameraController.Instance.transform.LookAt(pos);
    }

    public void Explode()
    {
        if(LastEffect != null)
            Destroy(LastEffect);
        if(NewMovement.Instance.dead)
            return;

        var effectInst = Object.Instantiate(effect, effect.transform.parent);
        effectInst.transform.localPosition = effect.transform.localPosition;
        effectInst.SetActive(true);
        Destroy(effectInst, 2.15f);
        LastEffect = effectInst;

        orbitYaw = Random.Range(0f, 360f);
        orbitPitch = Random.Range(45f, 80f);
        EnableThirdPerson();

        NewMovement.Instance.GetHurt(
            int.MaxValue,
            false,
            1f,
            false,
            false,
            1f,
            true
        );
    }

    public static void ExplodePlayer()
    {
        if (_instance == null)
        {
            LogHelper.LogError("[triggeredidiot] ExplodePlayer() called but there is no DeltaruneExplosion premade!");
            return;
        }
        if(NewMovement.Instance.dead)
            return;

        _instance.Explode();
    }
}

[PatchOnEntry]
[HarmonyPatch(typeof(NewMovement), "Awake")]
public static class DeltaruneExplosionInjector_Awake
{
    public static void Prefix(NewMovement __instance)
    {
        var effect = AssetsController.LoadAsset("KaboomEffect");
        if (effect != null)
        {
            effect.transform.SetParent(__instance.transform);
            effect.transform.localPosition = Vector3.zero;

            HudController hc = __instance.GetComponentInChildren<HudController>(includeInactive: true);
            var de = __instance.gameObject.AddComponent<DeltaruneExplosion>();
            de.effect = effect;
            de.hudCamera = hc.GetComponentInParent<Camera>();
        }
    }
}
[PatchOnEntry]
[HarmonyPatch(typeof(NewMovement), "Respawn")]
public static class DeltaruneExplosionInjector_Respawn
{
    public static void Prefix(NewMovement __instance)
    {
        DeltaruneExplosion.DisableThirdPerson();
        if (DeltaruneExplosion.LastEffect != null)
            Object.Destroy(DeltaruneExplosion.LastEffect);
    }
}