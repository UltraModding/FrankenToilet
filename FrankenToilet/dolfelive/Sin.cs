using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
// ReSharper disable UnnecessaryWhitespace

namespace FrankenToilet.dolfelive;

public sealed class Sin : MonoBehaviour
{
    public bool disabledMusic = false;
    
    // Audio
    public AudioClip zenRelease = null!;
    public AudioClip actionsNBanger = null!;
    public float audioDelay = 0.75f;

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
    public float baseFollowSpeed = 60f;
    public float speedMultiplier = 1.02f;
    public float recordInterval = 0.1f;
    
    struct PathEvent
    {
        public Vector3 position;
        public bool isPortal;
        public Vector3 portalDest;
    }
    
    private Queue<PathEvent> pathPoints = new Queue<PathEvent>();
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
    private AudioSource _musicSource = null!;
    private Transform? cam => NewMovement.instance?.cc?.cam.transform;

    // Animation state
    private int _index;
    private float _timer;
    
    void Start()
    {
        Camera.main.useOcclusionCulling = false;
        
        trailParent = Instantiate(new GameObject("trailParent")).transform;

        if (frames.Length > 0) image.sprite = frames[0];

        _audioSource = GetComponent<AudioSource>();
        _audioSource.volume = PrefsManager.Instance.GetFloat("allVolume");

        GameObject musicPlayerGO = new GameObject("Music player");
        musicPlayerGO.transform.parent = this.transform;
        _musicSource = musicPlayerGO.AddComponent<AudioSource>();
        _musicSource.volume = PrefsManager.Instance.GetFloat("musicVolume");
        _musicSource.clip = actionsNBanger;
        
        AudioSource[] audioChildren = transform.Find("AudioSources").GetComponents<AudioSource>();
        foreach (AudioSource child in audioChildren)
        {
            child.maxDistance *= 1.2f;
            child.volume = PrefsManager.Instance.GetFloat("allVolume");
            
            if (child.clip.name == "sinBells")
            {
                child.maxDistance = 15;
            }
            
            if (child.clip.name == "sinHORN")
            {
                child.volume *= 1.5f; 
            }
        }
        _musicSource.loop = true;
        
        StartCoroutine(playSounds());
        StartCoroutine(SpawnCircles());
    }
    
    IEnumerator playSounds()
    {
        disabledMusic = true;
        MusicManager.Instance.FadeOut(1f);
        _audioSource.PlayOneShot(zenRelease, tracked: true);
        yield return new WaitForSeconds(audioDelay);
        _musicSource.Play(tracked: true);
        
    }

    IEnumerator SpawnCircles()
    {
        Vector3 startPos = transform.position;
        float angle = 0f;
        int circlesCompleted = 0;
        float startTime = Time.time;
        
        while (circlesCompleted < 9)
        {
            float x = Mathf.Cos(angle) * circleRadius;
            float z = Mathf.Sin(angle) * circleRadius;
            
            float elapsed = Time.time - startTime;
            float y = startPos.y - (descentSpeed * elapsed);
            
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
            sTrail.GetComponent<Renderer>().material.renderQueue = 3999;
            sTrail.duration = trailDuration;
            sTrail.trailSizeRange = trailSizeRange;
            Destroy(trail, trailDuration);
        }
    }
    
    public void OnTeleport(Vector3 StartPos, Vector3 EndPos)
    {
        PathEvent pEvent = new();
        pEvent.position = StartPos;
        pEvent.isPortal = true;
        pEvent.portalDest = EndPos;
        pathPoints.Enqueue(pEvent);
    }
    
    void RecordCameraPath()
    {
        if (Time.time - lastRecordTime >= recordInterval)
        {
            PathEvent pEvent = new();
            pEvent.position = cam!.position;
            pathPoints.Enqueue(pEvent);
            lastRecordTime = Time.time;
            
            if (pathPoints.Count > 100)
            {
                PathEvent pe = pathPoints.Dequeue();
            }
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
            targetPoint = pathPoints.Peek().position;
        }
        countdown?.timeLeft = distanceToCam - 3f;

        float dynamicSpeed = baseFollowSpeed + ((distanceToCam > 60f ? distanceToCam - 60f : 0f) * speedMultiplier);

        Vector3 direction = (targetPoint - transform.position).normalized;
        transform.position += direction * (dynamicSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPoint) < 2f)
        {
            PathEvent current = pathPoints.Dequeue();
            if (current.isPortal)
                transform.position = current.portalDest;
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
        // return;
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
