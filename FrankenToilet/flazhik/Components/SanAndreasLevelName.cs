using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static FrankenToilet.flazhik.Utils.ReflectionUtils;

namespace FrankenToilet.flazhik.Components;

public sealed class SanAndreasLevelName : MonoBehaviour
{
    private const float fadeInDuration = 3.0f;
    private const float stayingDuration = 3.0f;
    private const float fadeOutDuration = 3.0f;

    private static readonly Dictionary<string, string> HardcodedLevelNames = new()
    {
        {"Endless", "Cybergrind"},
        {"Tutorial", "Tutorial"}
    };

    private void Awake()
    {
        var fade = gameObject.AddComponent<FadeInAndOut>();
        fade.fadeInDuration = fadeInDuration;
        fade.stayingDuration = stayingDuration;
        fade.fadeOutDuration = fadeOutDuration;
    }

    private void Start()
    {
        SetupLevelName();
    }

    private void SetupLevelName()
    {
        string sceneName;
        if (HardcodedLevelNames.TryGetValue(SceneHelper.CurrentScene, out var hardcodedLevelName))
            sceneName = hardcodedLevelName;
        else
        {
            var levelNamePopup = LevelNamePopup.Instance;
            if (levelNamePopup == null)
                return;

            sceneName = FixLevelNameCase(GetPrivate<string>(levelNamePopup, typeof(LevelNamePopup), "nameString"));
        }

        var levelName = transform.Find("LevelName").GetComponent<TMP_Text>();
        levelName.SetCharArray(sceneName.ToCharArray());
    }

    private static string FixLevelNameCase(string src)
    {
        if (string.IsNullOrEmpty(src))
            return string.Empty;

        return char.ToUpper(src[0]) + src[1..].ToLower();
    }
}