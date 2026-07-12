using System;
using System.Diagnostics;
using TMPro;
using ULTRAKILL.Cheats;
using UnityEngine;
using UnityEngine.UI;

namespace FrankenToilet.mercy.Features;

public sealed class PartyMode : MonoBehaviour
{
    public Sprite[]? frames;
    public Stopwatch frameTimer = new();
    public Stopwatch deathTimer = new();
    public int index = 0;
    public Image? image;
    public const int FRAMECOUNT = 155;
    public const int WIDTH = 264;
    public const int HEIGHT = 280;
    public readonly NoWeaponCooldown noWeaponCooldown = new();
    
    public static void Activate() => Helper.CreateImage<PartyMode>("Party Mode", WIDTH, HEIGHT);
    private void Awake()
    {
        frames = Helper.LoadFrames(Plugin.assetBundle, "PartyTime", FRAMECOUNT);
        image = gameObject.AddComponent<Image>();
        GameObject textObj = Instantiate(new GameObject("Text"), transform);
        TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
        tmp.text = "PARTY TIME!!!!";
        tmp.enableWordWrapping = false;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.magenta;
        Vector3 pos = transform.position;
        textObj.transform.position = new Vector3(pos.x, pos.y - HEIGHT, pos.z);
        frameTimer.Start();
        deathTimer.Start();
        noWeaponCooldown.Enable(CheatsManager.instance);
    }

    private void Update()
    {
        if (deathTimer.Elapsed.TotalSeconds >= 5) Destroy(gameObject);
        Helper.Animate(ref image, frames, frameTimer, ref index, FRAMECOUNT, 0.01);
        transform.Rotate(Vector3.up, 1);
    }

    private void OnDestroy()
    {
        noWeaponCooldown.Disable();
    }
}