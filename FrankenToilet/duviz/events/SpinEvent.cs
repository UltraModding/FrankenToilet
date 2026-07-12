using UnityEngine;

namespace FrankenToilet.duviz.events;

public class SpinEvent : MonoBehaviour
{
    public static Vector3 offset;
    public static Vector3 visibleOffset;

    static AudioSource source;

    public void Awake()
    {
        source = gameObject.AddComponent<AudioSource>();
        source.playOnAwake = false;
        source.clip = Bundle.bundle.LoadAsset<AudioClip>("goofy-ahh-car-horn-sound-effect");
    }

    public void LateUpdate()
    {
        if (CameraController.instance == null) return;
        visibleOffset = Vector3.Lerp(visibleOffset, offset, Mathf.Clamp01(Time.deltaTime * 5));
        offset = Vector3.Lerp(offset, Vector3.zero, Mathf.Clamp01(Time.deltaTime));

        CameraController.instance.transform.localEulerAngles += visibleOffset;
    }

    public static void Play()
    {
        source.Play();
        offset = new Vector3(UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360));
    }
}