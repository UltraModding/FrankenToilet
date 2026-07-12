using UnityEngine;

namespace FrankenToilet.Bananastudio;

public class ImplosionGrayscaleController : MonoBehaviour
{
    private Material material;

    float currentStrength;

    void Start()
    {
        material = MainThingy.bundle.LoadAsset<Material>("BlackwhiteScreenShader");
    }

    void Update()
    {
        float targetStrength = 0f;
        Vector3 playerPos = CameraController.Instance.GetDefaultPos();

        var list = Implosion.Active;
        for (int i = 0; i < list.Count; i++)
        {
            float s = list[i].GetEffectStrength(playerPos);
            if (s > targetStrength)
                targetStrength = s;
        }

        currentStrength = Mathf.Lerp(
            currentStrength,
            targetStrength,
            Time.deltaTime * 5f
        );
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (material == null)
        {
            Graphics.Blit(src, dest);
            return;
        }

        material.SetFloat("_Strength", currentStrength);
        Graphics.Blit(src, dest, material);
    }
}