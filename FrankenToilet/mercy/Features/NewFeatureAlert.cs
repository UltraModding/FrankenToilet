using System.Diagnostics;
using FrankenToilet.Core;
using UnityEngine;
using UnityEngine.UI;

namespace FrankenToilet.mercy.Features;

public sealed class NewFeatureAlert : MonoBehaviour
{
    public Sprite[]? frames;
    public Stopwatch deathTimer = new();
    public Stopwatch frameTimer = new();
    public int index = 0;
    public Image? image;
    public const int FRAMECOUNT = 11;

    private void Awake()
    {
        frames = Helper.LoadFrames(Plugin.assetBundle, "NewFeatureAlert", FRAMECOUNT);
        image = gameObject.AddComponent<Image>();
        deathTimer.Start();
        frameTimer.Start();
    }

    private void Update()
    {
        if (deathTimer.Elapsed.TotalSeconds >= 2) Destroy(gameObject);
        Helper.Animate(ref image, frames, frameTimer, ref index, FRAMECOUNT, 0.05);
    }
}