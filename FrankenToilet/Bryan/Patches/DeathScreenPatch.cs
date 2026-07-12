namespace FrankenToilet.Bryan.Patches;

using FrankenToilet.Core;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;

[PatchOnEntry] [HarmonyPatch(typeof(LaughingSkull))]
public static class DeathScreenPatch
{
    [HarmonyPostfix] [HarmonyPatch("Start")]
    public static void oiweoufjds(LaughingSkull __instance)
    {
        __instance.GetComponent<Animator>().runtimeAnimatorController = Assets.LaughingAnim;
        __instance.gameObject.AddComponent<LaughLaugh>();

        var blackScreen = __instance.transform.parent.GetComponent<Image>();
        blackScreen.sprite = Assets.Trans;
    }
}

/// <summary> Makes the skull play the laughing sfx and edit the background :P </summary>
public class LaughLaugh : MonoBehaviour
{
    /// <summary> Audio source that does the hahahaha </summary>
    public AudioSource Aud;

    /// <summary> Background image, yknow the one replaced with the trans flag cuz funi </summary>
    public Image Background;

    /// <summary> Skull image so we can edit its color </summary>
    public Image Skull;

    /// <summary> Set stuff up :P </summary>
    public void Awake()
    {
        Background = transform.parent.GetComponent<Image>();
        Skull = GetComponent<Image>();

        Aud = GetComponent<AudioSource>();
        Aud.clip = Assets.Laughing;
        Aud.loop = true;
    }

    /// <summary> rawr </summary>
    public void OnEnable()
    {
        Aud.Play();

        bool trans = Random.Range(0, 4) == 0;
        Skull.color = trans ? Color.black : Color.white;
        Background.color = trans ? Color.white : Color.black;
    }
}