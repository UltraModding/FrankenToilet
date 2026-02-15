using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace FrankenToilet.dolfelive;

public sealed class Sin : MonoBehaviour
{
    // Audio
    public AudioClip zenRelease = null!;
    public AudioClip actionsNBanger = null!;
    public float audioDelay = 1.5f;

    // Visuals
    public Image image = null!;
    public GameObject trailPrefab = null!;

    // Trail
    public float trailDuration = 1f;
    public float trailGap = 4.18f;
    public float trailBehindDistance = 3.5f;
    public int trailSpawnSpeed = 30; // x a sec,
    public Vector2 trailSizeRange = new Vector2(9f, 13.1f);
    private float trailSpawnTimer = 0.1f;
    private float trailSpawnDelay => 1f / trailSpawnSpeed;
    private Transform trailParent = null!;

    // Follow
    public float baseFollowSpeed = 50f;
    public float speedMultiplier = 1.01f;
    public float recordInterval = 0.1f;
    private Queue<Vector3> pathPoints = new Queue<Vector3>();
    private float lastRecordTime;

    // Circle motion
    public float circleRadius = 10f;
    public float circleSpeed = 15f;
    public float descentSpeed = 14f;

    // Animation
    public float animFPS = 13f;
    public Sprite[] frames = null!;

    // Countdown
    public DolfeCountdown? countdown = null!;

    // State
    private bool beginChase = false;
    private bool playerKilled = false;

    // refs
    private AudioSource _audioSource = null!;
    private Transform? cam => NewMovement.instance?.cc?.cam.transform;

    // Animation state
    private int _index;
    private float _timer;


    void Start()
    {
        trailParent = Instantiate(new GameObject("trailParent")).transform;

        if (frames.Length > 0) image.sprite = frames[0];

        _audioSource = GetComponent<AudioSource>();
        _audioSource.volume = PrefsManager.Instance.GetFloat("allVolume");

        AudioSource[] audioChildren = transform.Find("AudioSources").GetComponents<AudioSource>();
        foreach (AudioSource child in audioChildren)
        {
            child.maxDistance *= 1.2f;
            child.volume = PrefsManager.Instance.GetFloat("allVolume");
        }

        StartCoroutine(playSounds());
        StartCoroutine(SpawnCircles());
    }

    IEnumerator playSounds()
    {
        _audioSource.PlayOneShot(zenRelease);
        yield return new WaitForSeconds(audioDelay);
        _audioSource.PlayOneShot(actionsNBanger);
    }

    IEnumerator SpawnCircles()
    {
        Vector3 startPos = transform.position;
        float angle = 0f;
        int circlesCompleted = 0;

        while (circlesCompleted < 9)
        {
            float x = Mathf.Cos(angle) * circleRadius;
            float z = Mathf.Sin(angle) * circleRadius;

            float y = startPos.y - (descentSpeed * Time.time);

            transform.position = new Vector3(startPos.x + x, y, startPos.z + z);

            angle += circleSpeed * Time.deltaTime;

            if (angle >= Mathf.PI * 2)
            {
                angle -= Mathf.PI * 2;
                circlesCompleted++;
            }

            yield return null;
        }

        beginChase = true;
    }

    void AnimateEye()
    {
        if (frames.Length == 0) return;
        _timer += Time.deltaTime;
        if (_timer >= 1f / animFPS)
        {
            _timer -= 1f / animFPS;
            _index = (_index + 1) % frames.Length;
            image.sprite = frames[_index];
        }
    }

    void DoTrail()
    {
        trailSpawnTimer -= Time.deltaTime;
        if (trailSpawnTimer <= 0)
        {
            trailSpawnTimer = trailSpawnDelay;
            Vector3 randomPosOffset = new Vector3(Random.Range(-trailGap, trailGap), Random.Range(-trailGap, trailGap), 0f);
            Vector3 trailOrigin = transform.position - transform.forward * trailBehindDistance;
            if (cam != null)
            {
                Vector3 awayFromCamera = (transform.position - cam.position).normalized;
                if (awayFromCamera.sqrMagnitude > 0.0001f)
                {
                    trailOrigin = transform.position + awayFromCamera * trailBehindDistance;
                }
            }
            GameObject trail = Instantiate(trailPrefab, trailOrigin + randomPosOffset, Quaternion.identity, trailParent);
            SinTrail sTrail = trail.AddComponent<SinTrail>();
            sTrail.duration = trailDuration;
            sTrail.trailSizeRange = trailSizeRange;
            Destroy(trail, trailDuration);
        }
    }

    void RecordCameraPath()
    {
        if (Time.time - lastRecordTime >= recordInterval)
        {
            pathPoints.Enqueue(cam!.position);
            lastRecordTime = Time.time;

            if (pathPoints.Count > 100)
                pathPoints.Dequeue();
        }
    }

    private void FollowPath()
    {
        if (pathPoints.Count == 0) return;
        float distanceToCam = Vector3.Distance(transform.position, NewMovement.instance.transform.position);
        if (distanceToCam < 2f)
        {
            NewMovement.Instance.GetHurt(999, false, ignoreInvincibility: true);
            SHUTUP();
            playerKilled = true;
        }

        Vector3 targetPoint;
        if (distanceToCam < 10f)
        {
            targetPoint = NewMovement.instance.transform.position;
        }
        else
        {
            targetPoint = pathPoints.Peek();
        }
        countdown?.timeLeft = distanceToCam;

        float dynamicSpeed = baseFollowSpeed + ((distanceToCam > 60f ? distanceToCam - 60f : 0f) * speedMultiplier);

        Vector3 direction = (targetPoint - transform.position).normalized;
        transform.position += direction * (dynamicSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPoint) < 2f)
        {
            pathPoints.Dequeue();
        }
    }

    private void LookAtCamera()
    {
        if (cam == null) return;

        Vector3 toCamera = cam!.position - transform.position;
        if (toCamera.sqrMagnitude < 0.0001f) return;

        transform.rotation = Quaternion.LookRotation(toCamera, Vector3.up);
    }

    private void Update()
    {
        AnimateEye();
        DoTrail();
        LookAtCamera();

        if (!beginChase) return;
        if (playerKilled) return;

        RecordCameraPath();
        FollowPath();
    }


    void SHUTUP()
    {
        AudioSource[] audioChildren = this.transform.Find("AudioSources").GetComponents<AudioSource>();
        foreach (AudioSource child in audioChildren)
        {
            child.loop = false;
            child.volume = 0f;
            child.Stop();
        }
    }
}

public sealed class SinTrail : MonoBehaviour
{
    public float duration = 2f;
    public Vector2 trailSizeRange = new Vector2(1.8f, 2.2f);

    private float _size = 1f;
    private float _startSize;
    private float _elapsed = 0f;

    private void Start()
    {
        _size = Random.Range(trailSizeRange.x, trailSizeRange.y);
        _startSize = _size;
    }

    private void Update()
    {
        _elapsed += Time.deltaTime;
        float t = duration > 0f ? Mathf.Clamp01(_elapsed / duration) : 1f;
        _size = Mathf.Lerp(_startSize, 0f, t);
        transform.localScale = new Vector3(_size, _size, _size);
    }
}
