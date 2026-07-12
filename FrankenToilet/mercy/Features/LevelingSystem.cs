using System;
using FrankenToilet.Core;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace FrankenToilet.mercy.Features;

public sealed class LevelingSystem : MonoBehaviour
{
    public static int level = 1;
    public static int experience = 0;
    [MercyFeature]
    public static void Activate()
    {
        GameObject gameObject = Instantiate(
            Plugin.assetBundle.LoadAsset<GameObject>("Assets/Features/LEVEL.prefab"), 
            Plugin.canvas.transform);
        gameObject.AddComponent<LevelingSystem>();
    }

    public static int GetExperienceRequired() => (int) Math.Round(10 * Math.Exp(level));
    
    public void IncreaseExp(int exp)
    {
        experience += exp;
        if (experience > GetExperienceRequired())
        {
            experience -= GetExperienceRequired();
            level++;
        }
    }
    
    private void Awake() => gameObject.transform.position = new Vector3(160, 1038);

    private void Update()
    {
        foreach (TextMeshProUGUI tmp in GetComponentsInChildren<TextMeshProUGUI>())
        {
            if (tmp.name == "LEVELCOUNTER") tmp.text = level.ToString();
            else if (tmp.name == "EXPERIENCECOUNTER") tmp.text = experience + "/" + GetExperienceRequired();
        }
    }
}