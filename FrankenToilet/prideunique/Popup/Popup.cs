using UnityEngine;

namespace FrankenToilet.prideunique;

public class Popup : MonoBehaviour
{
    public GameObject Parent;
    public AudioClip CloseSound;

    public void Destroy()
    {
        if (CloseSound != null)
        {
            GameObject soundObj = new GameObject("PopupCloseSound");
            AudioSource source = soundObj.AddComponent<AudioSource>();
            soundObj.AddComponent<RemoveOnTime>().time = 2.0f;

            source.clip = CloseSound;
            source.playOnAwake = false;
            source.spatialBlend = 0f;
            source.Play();
        }
        Destroy(Parent);
    }

    private void OnDestroy()
    {
        if (Popups.RenderTextures.TryGetValue(Parent, out var renderTexture))
        {
            renderTexture.Release();
            Popups.RenderTextures.Remove(Parent);
        }
    }
}
