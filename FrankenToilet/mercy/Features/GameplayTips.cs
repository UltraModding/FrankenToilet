using System;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace FrankenToilet.mercy.Features;

public sealed class GameplayTips : MonoBehaviour
{
    public Stopwatch timer = new();
    public TextMeshProUGUI text;
    public float fontSize = 26;
    public float additionFactor = 0.05f;
    public string[] gameplayTips =
    [
        "Use accumulator for\nAOE Accumulator Damage",
        "Always switch to\ndefault piercer",
        "Play 4-3!",
        "Emu Otori is the\nmain character\nof ULTRAKILL!",
        "Also try CrossCode!"
    ];
    [MercyFeature]
    public static void Activate()
    {
        GameObject gameObject = Instantiate(new GameObject("Gameplay Tips"), Plugin.canvas.transform);
        gameObject.AddComponent<GameplayTips>();
    }

    private void GenerateGameplayTip()
    {
        int x = Plugin.rand.Next(0, Screen.width);
        int y = Plugin.rand.Next(0, Screen.height);
        gameObject.transform.position = new Vector3(x, y);
        int index = Plugin.rand.Next(0, gameplayTips.Length-1);
        string gameplayTip = $"GAMEPLAY TIP:\n{gameplayTips[index]}";
        text.SetCharArray(gameplayTip.ToCharArray());
    }
    
    private void Awake()
    {
        text = gameObject.AddComponent<TextMeshProUGUI>();
        GenerateGameplayTip();
        text.fontSize = fontSize;
        text.color = Color.yellow;
        text.alignment = TextAlignmentOptions.Center;
        text.enableWordWrapping = false;
        timer.Start();
    }

    private void Update()
    {
        if (fontSize >= 28 || fontSize <= 24) additionFactor = -additionFactor;
        fontSize += additionFactor;
        text.fontSize = fontSize;
        if (timer.Elapsed.TotalSeconds >= 20)
        {
            GenerateGameplayTip();
            timer.Restart();
        }
    }
}