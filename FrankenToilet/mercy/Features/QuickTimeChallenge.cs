using System;
using System.Collections.Generic;
using System.Diagnostics;
using FrankenToilet.Core;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

namespace FrankenToilet.mercy.Features;

public sealed class QuickTimeChallenge : MonoBehaviour
{
    public Stopwatch timer = new();
    public double timeLimit = Math.Round(Plugin.rand.NextDouble() * 10, 3)+5;
    public int actionLimit = Plugin.rand.Next(1, 5);
    public TextMeshProUGUI tmp;
    public static PlayerInput input = InputManager.Instance.InputSource;
    public static InputActionState[] inputActionList = [
        input.Dodge, input.Fire1, input.Fire2, input.Hook, input.Jump, input.NextVariation, input.PreviousVariation,
        input.Slot1, input.Slot2, input.Slot3, input.Slot4, input.Slot5, input.Slot6
    ];
    public List<InputActionState> actions = new();
    
    public static void Activate()
    {
        // guess who's back, back again
        GameObject gameObject = Instantiate(new GameObject("Quick Time Challenge"), Plugin.canvas.transform);
        gameObject.AddComponent<QuickTimeChallenge>();
    }

    public string inputText()
    {
        string text = "Please press the following inputs in order: ";
        foreach (InputActionState action in actions) text += $"{action.Action.name} ";
        text += $"in {timeLimit} seconds or DIE!!!";
        return text;
    }
    
    private void Awake()
    {
        timer.Start();
        for (int i = 0; i < actionLimit; ++i)
        {
            int index = Plugin.rand.Next(0, inputActionList.Length-1);
            actions.Add(inputActionList[index]);
        }
        tmp = gameObject.AddComponent<TextMeshProUGUI>();
        tmp.text = inputText();
        tmp.fontSize = 16;
        tmp.enableWordWrapping = false;
        tmp.alignment = TextAlignmentOptions.Center;
        gameObject.transform.position -= new Vector3(0, 300, 0);
    }

    private void Update()
    {
        if (actions[0].WasPerformedThisFrame)
        {
            actions.RemoveAt(0);
            tmp.text = inputText();
        }
        if (actions.Count == 0) Destroy(gameObject);
        if (timer.Elapsed.TotalSeconds >= timeLimit)
        {
            NewMovement.Instance.GetHurt(420, false);
            Destroy(gameObject);
        }
    }
}