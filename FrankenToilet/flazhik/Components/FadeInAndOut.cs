using System;
using System.Collections;
using UnityEngine;

namespace FrankenToilet.flazhik.Components;

public sealed class FadeInAndOut : MonoBehaviour
{
    public CanvasGroup panelGroup;

    public float fadeInDuration;
    public float stayingDuration;
    public float fadeOutDuration;

    public Action FadeInCallback;
    public Action FadeOutCallback;

    private Coroutine _currentRoutine;

    private void OnEnable()
    {
        panelGroup = GetComponent<CanvasGroup>();
        if (_currentRoutine != null)
            StopCoroutine(_currentRoutine);
        _currentRoutine = StartCoroutine(FadeInAndOutRoutine());
    }

    private IEnumerator FadeInAndOutRoutine()
    {
        yield return FadeIn();
        yield return Await();
        yield return FadeOut();
    }

    private IEnumerator FadeIn()
    {
        FadeInCallback?.Invoke();
        var time = 0.0f;
        while (time < (double)fadeInDuration)
        {
            time += Time.unscaledDeltaTime;
            panelGroup.alpha = time / fadeInDuration;
            yield return null;
        }
        panelGroup.alpha = 1;
    }

    private IEnumerator Await()
    {
        var time = stayingDuration;
        while (time > 0.0)
        {
            panelGroup.alpha = 1;
            if (time > 0.0)
                time -= Time.unscaledDeltaTime;
            yield return null;
        }
    }

    private IEnumerator FadeOut()
    {
        var time = fadeOutDuration;
        while (time > 0.0)
        {
            time -= Time.unscaledDeltaTime;
            panelGroup.alpha = time / fadeOutDuration;
            yield return null;
        }
        gameObject.SetActive(false);
        FadeOutCallback?.Invoke();
    }
}