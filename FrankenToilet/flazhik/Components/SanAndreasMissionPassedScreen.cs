using TMPro;
using UnityEngine;
using static FrankenToilet.flazhik.Utils.ReflectionUtils;
using static UnityEngine.GameObject;
namespace FrankenToilet.flazhik.Components;

public sealed class SanAndreasMissionPassedScreen : MonoBehaviour
{
    private const float fadeInDuration = 3.0f;
    private const float stayingDuration = 3.0f;
    private const float fadeOutDuration = 3.0f;

    private MusicManager muman;
    private string originalText;

    private void Awake()
    {
        muman = MusicManager.Instance;
        originalText = Find("MissionPassed").GetComponent<TextMeshProUGUI>().text;
        var fade = gameObject.AddComponent<FadeInAndOut>();
        fade.fadeInDuration = fadeInDuration;
        fade.stayingDuration = stayingDuration;
        fade.fadeOutDuration = fadeOutDuration;
        GetComponent<AudioSource>().outputAudioMixerGroup = AudioMixerController.Instance.allGroup;
        PauseTheMusicCallbacks(fade);
    }

    // Putting an extra effort just to undo the damage made by others contributors
    // Fucking hell
    private void Start()
    {
        Find("MissionPassed").GetComponent<TextMeshProUGUI>().SetCharArray(originalText.ToCharArray());
    }

    // Pause the music for the duration of "Mission passed" sequence
    private void PauseTheMusicCallbacks(FadeInAndOut fade)
    {
        var allThemes = GetPrivate<AudioSource[]>(muman, typeof(MusicManager), "allThemes");
        fade.FadeInCallback = () =>
        {
            foreach (var theme in allThemes)
                theme.Pause();
        };

        fade.FadeOutCallback = () =>
        {
            foreach (var theme in allThemes)
                theme.Play();
        };
    }
}