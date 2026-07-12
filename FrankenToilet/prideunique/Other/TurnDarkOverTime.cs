using UnityEngine;
using UnityEngine.UI;

namespace FrankenToilet.prideunique;

[ConfigureSingleton(SingletonFlags.DestroyDuplicates | SingletonFlags.PersistAutoInstance)]
public class TurnDarkOverTime : MonoSingleton<TurnDarkOverTime>
{
    public bool CreatedCanvas = false;
    private RawImage rawImage;

    public float fadeDuration = 60f * 60f; //1min(60s) * 60 = 1 hour
    public float maxAlpha = 0.5f;   // highest alpha allowed
    private float elapsed = 0f;
    private bool fading = false;

    public void Awake()
    {
        gameObject.hideFlags = HideFlags.HideAndDontSave;
    }

    public void Update()
    {
        if (!CreatedCanvas)
        {
            if (AssetsController.AssetsLoaded)
            {
                CreatedCanvas = true;

                GameObject go = Object.Instantiate(AssetsController.LoadAsset<GameObject>("assets/aizoaizo/darkovertime.prefab"));
                Object.DontDestroyOnLoad(go);
                if (!go)
                    return;
                
                var canvas = go.GetComponentInChildren<Canvas>();
                canvas.sortingOrder = 1114; // The "LOADING" screen's sorting order is 1000

                rawImage = go.GetComponentInChildren<RawImage>();
                if (!rawImage)
                    return;

                var startCol = rawImage.color;
                startCol.a = 0f;
                rawImage.color = startCol;

                elapsed = 0f;
                fading = true;
            }

            return;
        }

        if (!rawImage)
            return;

        if (fading && rawImage != null)
        {
            if (fadeDuration <= 0f)
            {
                SetAlpha(1f);
                fading = false;
            }
            else
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / fadeDuration);
                float a = Mathf.Lerp(0f, maxAlpha, t);
                SetAlpha(a);

                if (t >= 1f)
                    fading = false;
            }
        }
    }

    private void SetAlpha(float a)
    {
        var c = rawImage.color;
        c.a = a;
        rawImage.color = c;
    }
}
