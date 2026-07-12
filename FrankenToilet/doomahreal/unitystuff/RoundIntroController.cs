using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundIntroController : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip startRouletteClip;
    public AudioClip evilJingleClip;
    public AudioClip spinSfxClip;
    public List<GameObject> entries = new List<GameObject>();
    public Animator animator;
    public GameObject evilScaryPrefab;
    public float waitBeforeScroll = 2f;
    public float scrollDuration = 5f;
    public float delayAfterLandingBeforeUnEvil = 5f;
    public float spawnAfterSeconds = 20f;
    public float spawnDistance = 100f;
    public float easingExponent = 3f;

    bool isSpinning;

    void Awake()
    {
        waitBeforeScroll = 2f;
        audioSource.clip = startRouletteClip;
        audioSource.Play();

        for (int i = 0; i < entries.Count; i++)
            entries[i].SetActive(false);

        StartCoroutine(SequenceCoroutine());
    }

    IEnumerator SequenceCoroutine()
    {
        yield return new WaitForSeconds(waitBeforeScroll);

        if (entries == null || entries.Count == 0)
            yield break;

        isSpinning = true;

        int index = 0;
        int prevIndex = -1;
        float startTime = Time.time;

        float minInterval = 0.02f;
        float maxInterval = 0.6f;

        while (Time.time - startTime < scrollDuration)
        {
            prevIndex = index;
            index = (index + 1) % entries.Count;

            if (prevIndex >= 0)
                entries[prevIndex].SetActive(false);

            entries[index].SetActive(true);

            audioSource.PlayOneShot(spinSfxClip);

            float elapsed = Time.time - startTime;
            float t = Mathf.Clamp01(elapsed / scrollDuration);
            float exp = Mathf.Max(0.01f, easingExponent);
            float easedT = 1f - Mathf.Pow(1f - t, exp);
            float interval = Mathf.Lerp(minInterval, maxInterval, easedT);

            float remaining = scrollDuration - elapsed;
            if (interval > remaining)
                interval = remaining;

            if (interval > 0f)
                yield return new WaitForSeconds(interval);
            else
                yield return null;
        }

        isSpinning = false;

        for (int i = 0; i < entries.Count - 1; i++)
            entries[i].SetActive(false);

        int lastIndex = entries.Count - 1;
        entries[lastIndex].SetActive(true);

        audioSource.clip = evilJingleClip;
        audioSource.volume = 0.5f;
        audioSource.Play();

        yield return new WaitForSeconds(delayAfterLandingBeforeUnEvil);

        animator.Play("UnEvil");
        StartCoroutine(SpawnCoroutine());
    }

    IEnumerator SpawnCoroutine()
    {
        yield return new WaitForSeconds(spawnAfterSeconds);

        Transform newMovementTransform = MonoSingleton<NewMovement>.Instance.gameObject.transform;
        Vector3 dir = newMovementTransform.forward;
        if (dir == Vector3.zero) dir = Vector3.forward;
        Vector3 spawnPos = newMovementTransform.position + dir.normalized * spawnDistance;
        Instantiate(evilScaryPrefab, spawnPos, Quaternion.identity);
        Destroy(gameObject);
    }
}
