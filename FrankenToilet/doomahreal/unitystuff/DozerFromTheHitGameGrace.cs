using System.Collections;
using UnityEngine;

public class DozerFromTheHitGameGrace : MonoBehaviour
{
    public GameObject watching;
    public GameObject checkWindow;
    public GameObject attack;
    public GameObject deathSequence;
    public GameObject textPrefab;
    public AudioSource ralsey;
    public float chanceIntervalSeconds = 1f;
    [Range(1, 100)] public int chanceOutOf100 = 100;

    bool sequenceRunning;
    bool pausedAfterSurvive;

    Coroutine watchingJitter;
    Coroutine checkWindowJitter;

    void Start()
    {
        StartCoroutine(ChanceLoop());
    }

    IEnumerator ChanceLoop()
    {
        while (true)
        {
            if (!sequenceRunning && !pausedAfterSurvive)
            {
                yield return new WaitForSeconds(chanceIntervalSeconds);
                if (Random.Range(1, chanceOutOf100 + 1) == 1)
                    StartCoroutine(Godisnthereanymore());
            }
            else yield return null;
        }
    }

    IEnumerator Godisnthereanymore()
    {
        sequenceRunning = true;

        watching.SetActive(true);
        watching.GetComponent<AudioSource>().Play();
        watchingJitter = StartCoroutine(Tweakthefuckout(watching));

        yield return new WaitForSeconds(1.55f);

        StopCoroutine(watchingJitter);
        ResetTransform(watching);
        watching.SetActive(false);

        checkWindow.SetActive(true);
        checkWindowJitter = StartCoroutine(Tweakthefuckout(checkWindow));

        bool inputDetected = false;
        float t = 0f;

        while (t < 0.5f)
        {
            if (CheckInput())
            {
                inputDetected = true;
                break;
            }
            t += Time.deltaTime;
            yield return null;
        }

        StopCoroutine(checkWindowJitter);
        ResetTransform(checkWindow);
        checkWindow.SetActive(false);

        if (inputDetected)
        {
            attack.SetActive(true);
            StartCoroutine(SpawnClonesUntilLimit());
            yield return new WaitForSeconds(0.7f);
            attack.SetActive(false);
        }
        else
        {
            pausedAfterSurvive = true;
            yield return new WaitForSeconds(60f);
            pausedAfterSurvive = false;
        }

        sequenceRunning = false;
    }

    //yucky, could be better
    bool CheckInput()
    {
        var inputManager = MonoSingleton<InputManager>.Instance;
        if (inputManager != null)
        {
            var src = inputManager.InputSource;
            if (src != null)
            {
                if (src.Move.IsPressed || src.Move.WasPerformedThisFrame) return true;
                if (src.Jump.IsPressed || src.Jump.WasPerformedThisFrame) return true;
                if (src.Dodge.IsPressed || src.Dodge.WasPerformedThisFrame) return true;
                if (src.Slide.IsPressed || src.Slide.WasPerformedThisFrame) return true;
            }
        }

        return false;
    }

    IEnumerator Tweakthefuckout(GameObject obj)
    {
        Transform t = obj.transform;
        Vector3 basePos = t.localPosition;

        while (true)
        {
            t.localPosition = basePos + new Vector3(
                Random.Range(-1.5f, 1.5f),
                Random.Range(-1.5f, 1.5f),
                0f
            );

            t.localRotation = Quaternion.Euler(
                0f,
                0f,
                Random.Range(-2f, 2f)
            );

            yield return new WaitForSeconds(0.1f);
        }
    }

    void ResetTransform(GameObject obj)
    {
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localRotation = Quaternion.identity;
    }

    IEnumerator SpawnClonesUntilLimit()
    {
        RectTransform originalRt = textPrefab.GetComponent<RectTransform>();
        Vector2 basePos = originalRt.anchoredPosition;
        Vector2 offset = Vector2.zero;

        while (true)
        {
            GameObject clone = Instantiate(textPrefab, attack.transform);
            RectTransform cloneRt = clone.GetComponent<RectTransform>();
            cloneRt.anchoredPosition = basePos + offset;

            offset += new Vector2(20f, 20f);

            if (offset.x >= 300f || offset.y >= 300f)
            {
                attack.SetActive(false);
                deathSequence.SetActive(true);
                StartCoroutine(WaitForDeathAudio());
                yield break;
            }

            yield return new WaitForSeconds(0.05f);
        }
    }

    IEnumerator WaitForDeathAudio()
    {
        ralsey.Play();
        MonoSingleton<NewMovement>.Instance.enabled = false;
        MonoSingleton<NewMovement>.Instance.rb.isKinematic = true;
        MonoSingleton<CameraController>.Instance.activated = false;
        MonoSingleton<MusicManager>.Instance.StopMusic();

        while (ralsey.isPlaying)
            yield return null;

        MonoSingleton<NewMovement>.Instance.enabled = true;
        MonoSingleton<NewMovement>.Instance.rb.isKinematic = false;
        MonoSingleton<CameraController>.Instance.activated = true;
        MonoSingleton<NewMovement>.Instance.GetHurt(9999, false);
        MonoSingleton<MusicManager>.Instance.StartMusic();
        Destroy(gameObject);
    }
}
