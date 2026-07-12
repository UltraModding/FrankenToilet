using System;
using System.Diagnostics;
using UnityEngine;

namespace FrankenToilet.mercy.Features;

public class PartyModeController : MonoBehaviour
{
    public Stopwatch stopwatch = new();
    [MercyFeature]
    public static void Activate() => Plugin.manager.AddComponent<PartyModeController>();
    private void Awake() => stopwatch.Start();
    private void Update()
    {
        if (stopwatch.Elapsed.TotalSeconds >= 1)
        {
            if (Plugin.rand.Next(1, 60) == 1) PartyMode.Activate();
            stopwatch.Restart();
        }
    }
}