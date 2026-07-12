using UnityEngine;

namespace FrankenToilet.greycsont;

public class DestoryTimer : MonoBehaviour
{
    public float lifetime;
    private float timer = 0f;

    void Update()
    {
        timer += Time.unscaledDeltaTime;
        if (timer >= lifetime)
        {
            Destroy(gameObject);
        }
    }
}