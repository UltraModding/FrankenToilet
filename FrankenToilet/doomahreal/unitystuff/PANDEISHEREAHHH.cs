using UnityEngine;
using System.Collections;

public class PANDEISHEREAHHH : MonoBehaviour
{
    public RectTransform pointer;
    public RectTransform targetCircle;
    public RectTransform progressRect;
    public RectTransform canvasRect;

    public float pointerFloatiness = 6f;
    public float mouseInfluence = 120f;
    public float mouseJiggleMultiplier = 3f;
    public float velocityDamping = 6f;

    public float barDrainSpeed = 0.2f;
    public float barFillSpeed = 0.3f;

    public float flickStrength = 400f;
    public float doubleFlickChance = 0.15f;
    public float doubleFlickDelay = 0.1f;

    public float vibrationStrength = 8f;

    public AudioSource audioSource;
    public AudioClip flickSound;

    public float circleHitRadius = 50f;

    public GameObject loseSprite;
    public float loseDisplayDuration = 2f;

    Vector2 pointerPos;
    Vector2 pointerVelocity;
    Vector2 prevMouseLocal;

    float timer;
    bool active;
    float nextFlickTime;
    bool done;
    bool failed;
    float progress;
    Camera canvasCamera;

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        MonoSingleton<NewMovement>.Instance.enabled = false;
        MonoSingleton<NewMovement>.Instance.rb.isKinematic = true;
        MonoSingleton<CameraController>.Instance.activated = false;
        MonoSingleton<MusicManager>.Instance.StopMusic();
        pointerPos = pointer.anchoredPosition;
        pointerVelocity = Vector2.zero;

        Canvas parentCanvas = canvasRect.GetComponentInParent<Canvas>();

        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, Input.mousePosition, canvasCamera, out prevMouseLocal);

        loseSprite.SetActive(false);

        StartCoroutine(StartDelay());
        ScheduleNextFlick();
        targetCircle.sizeDelta = new Vector2(circleHitRadius * 5f, circleHitRadius * 5f);
        Invoke(nameof(EndMiniGame), 42f);
    }

    IEnumerator StartDelay()
    {
        yield return new WaitForSeconds(5f);
        progress = 1f;
        active = true;
    }

    void ScheduleNextFlick()
    {
        float t = Mathf.Lerp(8.5f, 3.5f, Mathf.Clamp01(timer / 42f));
        nextFlickTime = timer + Random.Range(t - 1f, t + 1f);
    }

    void Update()
    {
        if (done || failed) return;
        float dt = Time.deltaTime;
        if (dt <= 0f || float.IsNaN(dt)) return;

        timer += dt;

        Vector2 mouseLocal;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, Input.mousePosition, canvasCamera, out mouseLocal);
        Vector2 mouseDelta = mouseLocal - prevMouseLocal;
        prevMouseLocal = mouseLocal;

        pointerVelocity += mouseDelta * mouseInfluence * dt * pointerFloatiness;
        pointerPos += pointerVelocity * dt;
        pointerPos += Random.insideUnitCircle * vibrationStrength * dt;
        pointerVelocity = Vector2.Lerp(pointerVelocity, Vector2.zero, velocityDamping * dt);
        pointerPos = ClampToCanvas(pointerPos);
        pointer.anchoredPosition = pointerPos;

        if (active)
        {
            float distToTarget = GetAnchoredDistance(pointer, targetCircle);

            if (distToTarget <= circleHitRadius)
                progress = Mathf.MoveTowards(progress, 1f, barFillSpeed * dt);
            else
                progress = Mathf.MoveTowards(progress, 0f, barDrainSpeed * dt);

            progress = Mathf.Clamp01(progress);
            UpdateProgressBar(dt);

            if (progress <= 0f)
            {
                OnFail();
                return;
            }
        }

        if (timer >= nextFlickTime)
        {
            FlickPointer();
            ScheduleNextFlick();
        }
    }

    float GetAnchoredDistance(RectTransform a, RectTransform b)
    {
        Vector3 worldA = a.TransformPoint(a.rect.center);
        Vector3 worldB = b.TransformPoint(b.rect.center);
        Vector2 localA, localB;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, RectTransformUtility.WorldToScreenPoint(canvasCamera, worldA), canvasCamera, out localA);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, RectTransformUtility.WorldToScreenPoint(canvasCamera, worldB), canvasCamera, out localB);
        return Vector2.Distance(localA, localB);
    }

    Vector2 ClampToCanvas(Vector2 pos)
    {
        Vector2 min = -canvasRect.sizeDelta / 2f;
        Vector2 max = canvasRect.sizeDelta / 2f;
        float halfWidth = pointer.sizeDelta.x * 0.5f;
        float halfHeight = pointer.sizeDelta.y * 0.5f;
        pos.x = Mathf.Clamp(pos.x, min.x + halfWidth, max.x - halfWidth);
        pos.y = Mathf.Clamp(pos.y, min.y + halfHeight, max.y - halfHeight);
        return pos;
    }

    void UpdateProgressBar(float dt)
    {
        float targetX = Mathf.Lerp(0f, 2f, progress);
        float currentX = progressRect.localScale.x;
        float newX = Mathf.Lerp(currentX, targetX, dt * 10f);
        progressRect.localScale = new Vector3(newX, progressRect.localScale.y, progressRect.localScale.z);
    }

    void FlickPointer()
    {
        Vector2 dir = Random.insideUnitCircle.normalized;
        if (dir == Vector2.zero) dir = Vector2.right;
        pointerVelocity += dir * flickStrength;
        PlayFlickSound();

        if (Random.value <= doubleFlickChance)
            StartCoroutine(DelayedFlick(doubleFlickDelay));
    }

    IEnumerator DelayedFlick(float delay)
    {
        yield return new WaitForSeconds(delay);
        Vector2 dir = Random.insideUnitCircle.normalized;
        if (dir == Vector2.zero) dir = Vector2.up;
        pointerVelocity += dir * flickStrength;
        PlayFlickSound();
    }

    void PlayFlickSound()
    {
        audioSource.PlayOneShot(flickSound);
    }

    void EndMiniGame()
    {
        done = true;
        StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        foreach (GameObject obj in FindObjectsOfType<GameObject>())
        {
            if (obj.name.ToLower().Contains("pande"))
            {
                Destroy(obj);
                break;
            }
        }

        CanvasGroup cg = GetComponent<CanvasGroup>();

        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / 2f;
            cg.alpha = 1f - t;
            yield return null;
        }

        MonoSingleton<NewMovement>.Instance.enabled = true;
        MonoSingleton<NewMovement>.Instance.rb.isKinematic = false;
        MonoSingleton<CameraController>.Instance.activated = true;
        MonoSingleton<MusicManager>.Instance.StartMusic();
        Destroy(gameObject);
    }

    public void OnFail()
    {
        if (failed) return;
        failed = true;
        loseSprite.SetActive(true);
        StartCoroutine(KillAfterDelay());
    }

    IEnumerator KillAfterDelay()
    {
        yield return new WaitForSeconds(loseDisplayDuration);
        MonoSingleton<NewMovement>.Instance.GetHurt(9999, false);
        Cursor.lockState = CursorLockMode.Confined;
        MonoSingleton<NewMovement>.Instance.enabled = true;
        MonoSingleton<NewMovement>.Instance.rb.isKinematic = false;
        MonoSingleton<CameraController>.Instance.activated = true;
        MonoSingleton<MusicManager>.Instance.StartMusic();
        foreach (GameObject obj in FindObjectsOfType<GameObject>())
        {
            if (obj.name.ToLower().Contains("pande"))
            {
                Destroy(obj);
                break;
            }
        }
        Destroy(gameObject);
    }
}
