using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace FrankenToilet.Bananastudio;


public class Implosion : MonoBehaviour
{

    public static readonly List<Implosion> Active = new List<Implosion>();

    void OnEnable()
    {
        Active.Add(this);
    }

    void OnDisable()
    {
        Active.Remove(this);
    }

    public EnemyIdentifier origin;

    AudioSource source;

    public float size = 30;
    float t;
    bool imploded;
    public bool followUser;

    void Start()
    {
        source = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!imploded)
            t += Time.deltaTime;
        else
            t -= Time.deltaTime * 25;

        source.pitch = Mathf.Lerp(0, 0.35f, Mathf.Clamp01(t / 2));

        transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one * size, t / 2);
        if (t >= 3 && !imploded)
        {
            imploded = true;
            GameObject part = Instantiate(MainThingy.LoadAddress<GameObject>("Assets/Particles/BlackHoleExplosion.prefab"), 
                transform.position, Quaternion.identity);
            part.transform.localScale = Vector3.one * size;
            Implode();
        }
        if (t < -0.8f)
        {
            Destroy(gameObject);
        }

        if (followUser)
        {
            transform.position += (NewMovement.Instance.transform.position - transform.position) * 0.5f * Time.deltaTime;
        }
    }

    void Implode()
    {
        foreach (var enemy in getEnemiesInRadius(size / 2, origin))
        {
            enemy.InstaKill();
            Destroy(enemy.gameObject);
        }

        if (Vector3.Distance(NewMovement.Instance.transform.position, transform.position) <= size / 2)
        {
            NewMovement.Instance.GetHurt(100000, false);
        }

        CameraController.Instance.CameraShake(5);
    }

    List<EnemyIdentifier> getEnemiesInRadius(float range, EnemyIdentifier eid)
    {
        List<EnemyIdentifier> inRad = new List<EnemyIdentifier>();
        foreach (var enemy in EnemyTracker.Instance.GetCurrentEnemies())
        {
            if (enemy == eid) continue;
            if (Vector3.Distance(enemy.transform.position, eid.transform.position) <= range)
            {
                inRad.Add(enemy);
            }
        }
        return inRad;
    }

    public float GetEffectStrength(Vector3 playerPos)
    {
        float radius = transform.localScale.x / 2f;
        float distance = Vector3.Distance(playerPos, transform.position);

        if (distance <= radius)
            return 1f;

        float fadeRange = radius / 2;
        float t = Mathf.InverseLerp(radius + fadeRange, radius, distance);
        return Mathf.Clamp01(t);
    }

}
