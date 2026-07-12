using System;
using System.Diagnostics;
using FrankenToilet.Core;
using UnityEngine;
using UnityEngine.UI;

namespace FrankenToilet.mercy.Features;

public sealed class EmuOtori : MonoBehaviour
{
    public Sprite[]? frames;
    public Stopwatch frameTimer = new();
    public Stopwatch duplicateTimer = new();
    public int index = 0;
    public Image? image;
    public const int FRAMECOUNT = 22;
    public const int WIDTH = 253;
    public const int HEIGHT = 332;

    [MercyFeature]
    public static void Activate() => Helper.CreateImage<EmuOtori>("Emu Otori", WIDTH, HEIGHT);
    private void Awake()
    {
        frames = Helper.LoadFrames(Plugin.assetBundle, "EmuOtori", FRAMECOUNT);
        image = gameObject.AddComponent<Image>();
        frameTimer.Start();
        duplicateTimer.Start();
    }

    private void Update()
    {
        Helper.Animate(ref image, frames, frameTimer, ref index, FRAMECOUNT, 0.01);
        if (index == 0)
        {
            float x = Plugin.rand.Next(0, Screen.width);
            float y = Plugin.rand.Next(0, Screen.height);
            gameObject.transform.position = new Vector3(x, y);
        }

        if (duplicateTimer.Elapsed.TotalSeconds >= 60)
        {
            Helper.CreateImage<EmuOtori>("Emu Otori", WIDTH, HEIGHT);
            duplicateTimer.Restart();
        }
        if (NewMovement.instance.dead) Destroy(gameObject);
    }
}