using System;
using System.Diagnostics;
using TMPro;
using UnityEngine;

namespace FrankenToilet.mercy.Features;

public sealed class DefaultPiercer : MonoBehaviour
{
    public Stopwatch stopwatch = new();
    public TextMeshProUGUI tmp;
    public static double mult;
    [MercyFeature]
    public static void Activate()
    {
        GameObject gameObject = Instantiate(new GameObject("Default Piercer Manager"), Plugin.canvas.transform);
        gameObject.AddComponent<DefaultPiercer>();
    }

    private void Awake()
    {
        int x = Plugin.rand.Next(0, Screen.width);
        int y = Plugin.rand.Next(0, Screen.height);
        gameObject.transform.position = new Vector3(x, y);
        tmp = gameObject.AddComponent<TextMeshProUGUI>();
        tmp.fontSize = 24;
        mult = Math.Round(Plugin.rand.NextDouble()+1, 3);
        stopwatch.Start();
        tmp.text = "Default Piercer Damage Multiplier: "+mult+"x";
    }

    private void Update()
    {
        if (stopwatch.Elapsed.TotalSeconds >= 1)
        {
            mult = Math.Round(Plugin.rand.NextDouble()+1, 3);
            tmp.text = "Default Piercer Damage Multiplier: "+mult+"x";
            stopwatch.Restart();
        }
    }
}