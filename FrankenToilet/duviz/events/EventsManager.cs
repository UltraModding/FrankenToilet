using FrankenToilet.Core;
using FrankenToilet.duviz;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace FrankenToilet.duviz.events;

/// <summary> You can see an example of an event in the EventsCreator.cs </summary>
public class EventsManager : MonoBehaviour
{
    /// <summary> Has every event, contains a name and a UnityEvent to activate it</summary>
    public static List<(string eventName, UnityEvent unityEvent)> events = [];

    /// <summary> Time between events (10 random seconds range) </summary>
    static float eventCycle = 15;

    /// <summary> Adds a event with that name and returns the UnityEvent of it </summary>
    /// <returns> UnityEvent of the created event </returns>
    public static UnityEvent AddEvent(string eName)
    {
        (string, UnityEvent) e = new(eName, new());
        events.Add(e);
        return e.Item2;
    }

    float timer = 0;

    public void Update()
    {
        if (NewMovement.instance == null) return;
        if (!NewMovement.instance.activated) { timer = 0; return; }
        if (GunControl.instance.noWeapons) { return; }

        timer += Time.deltaTime;

        if (timer > eventCycle)
        {
            timer = 0 - Random.Range(0, 10);

            if (events.Count == 0) { LogHelper.LogError("No events found!"); return; }

            (string, UnityEvent) e = events[Random.Range(0, events.Count)];
            e.Item2.Invoke();
            CreatePopup(e.Item1);
        }
    }

    static void CreatePopup(string eName)
    {
        GameObject canvas = Instantiate(Bundle.bundle.LoadAsset<GameObject>("EventCanvas"));
        canvas.AddComponent<DestroyInTime>();
        canvas.GetComponentInChildren<TMP_Text>().text = $"EVENT: {eName.ToUpper()}";
    }
}