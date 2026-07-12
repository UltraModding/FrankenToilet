using UnityEngine;

namespace FrankenToilet.dolfelive;

public sealed class ApplyShader : MonoBehaviour
{
    private static readonly int Tint = Shader.PropertyToID("_Tint");
    public Material overlayMaterial = null!;
    public bool includeInactive = true;

    public Color redColor = new Color(251/255f, 51/255f, 132/255f);
    public Color blueColor= new Color(34/255f, 7/255f, 179/255f);
    public Color blackColor = new Color(0f/255f, 0f/255f, 0f/255f);

    public float blackDuration = 0.08f;
    public float colorDuration = 0.5f;

    private int _cycleIndex = 0;
    private float _cycleTimer = 0f;

    void Start()
    {
        var renderers = FindObjectsOfType<Renderer>(includeInactive);

        for (int i = 0; i < renderers.Length; i++)
        {
            var r = renderers[i];
            if (!r) continue;
            if (r.GetComponentInParent<Camera>()) continue;

            var original = r.sharedMaterials;
            int len = original.Length;

            Material[] combined = new Material[len + 1];

            for (int j = 0; j < len; j++)
                combined[j] = original[j];

            combined[len] = overlayMaterial;

            r.materials = combined;
        }
    }

    void Update()
    {
        if (!overlayMaterial) return;

        _cycleTimer += Time.deltaTime;
        float currentDuration = (_cycleIndex == 0 || _cycleIndex == 3) ? blackDuration : colorDuration;

        if (_cycleTimer >= currentDuration)
        {
            _cycleTimer -= currentDuration;
            _cycleIndex = (_cycleIndex + 1) % 6;
        }

        Color fromColor = GetColorForIndex(_cycleIndex);
        Color toColor = GetColorForIndex((_cycleIndex + 1) % 6);

        float t = _cycleTimer / currentDuration;
        Color currentColor = Color.Lerp(fromColor, toColor, t);

        overlayMaterial.SetColor(Tint, currentColor);
    }

    Color GetColorForIndex(int index)
    {
        return index == 0 ? blackColor :
            index == 1 ? blueColor :
            index == 2 ? redColor :
            index == 3 ? blackColor :
            index == 4 ? redColor :
            blueColor;
    }
}
