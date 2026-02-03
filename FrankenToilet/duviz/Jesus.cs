#pragma warning disable CS8618
using UnityEngine;

namespace FrankenToilet.duviz;

public class Jesus : MonoBehaviour
{
    public GameObject jesus;
    public Animator animator;
    public AudioSource audio;

    float cooldown = 1;
    int deathCount = -0;

    public void Start()
    {
        GameObject canvas = Instantiate(Bundle.bundle.LoadAsset<GameObject>("JesusCanvas"));
        jesus = canvas.transform.Find("Jesus").gameObject;
        animator = jesus.GetComponent<Animator>();
        animator.updateMode = AnimatorUpdateMode.UnscaledTime;
        AudioClip bell = Bundle.bundle.LoadAsset<AudioClip>("Bell");
        audio = canvas.AddComponent<AudioSource>();
        audio.clip = bell;
        audio.playOnAwake = false;
        GameObject.DontDestroyOnLoad(canvas);
    }

    public void Update()
    {
        if (NewMovement.instance != null)
        {
            if (NewMovement.instance.hp <= 50) cooldown -= Time.unscaledDeltaTime;
            if (NewMovement.instance.hp <= 25) cooldown -= Time.unscaledDeltaTime;
            if (NewMovement.instance.hp <= 15) cooldown -= Time.unscaledDeltaTime;
            if (NewMovement.instance.hp <= 0) cooldown -= Time.unscaledDeltaTime;
            else deathCount = 0;
            if (NewMovement.instance.hp == 1) cooldown = 2;

            if (cooldown < 0)
            {
                cooldown = 2;
                if (NewMovement.instance.hp <= 0)
                {
                    deathCount += 1;
                    if (deathCount > 5)
                        return;
                }
                Flash();
            }
        }
    }

    public void Flash()
    {
        animator?.SetTrigger("jesus");
        audio?.Play();
    }
}