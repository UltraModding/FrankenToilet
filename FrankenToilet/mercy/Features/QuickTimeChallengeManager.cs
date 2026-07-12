using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace FrankenToilet.mercy.Features;

public sealed class QuickTimeChallengeManager : MonoBehaviour
{
    // wouldn't be a feature without one of these
    public Stopwatch stopwatch = new();
    [MercyFeature]
    public static void Activate() => Plugin.manager.AddComponent<QuickTimeChallengeManager>();
    private void Awake() => stopwatch.Start();
    private void Update()
    {
        if (stopwatch.Elapsed.TotalSeconds >= 1)
        {
            if (Plugin.rand.Next(1, 120) == 1 && !Plugin.canvas.GetComponent<QuickTimeChallenge>()) 
                QuickTimeChallenge.Activate();
            stopwatch.Restart();
        }
    }
}