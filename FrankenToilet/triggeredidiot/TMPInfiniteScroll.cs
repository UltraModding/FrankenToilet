using UnityEngine;

namespace FrankenToilet.triggeredidiot;

// fuckass class i don't even think this fixes the loop being visible with unity animators

public sealed class TMPInfiniteScroll : MonoBehaviour
{
    public float speed = 512f;

    private RectTransform rect;
    private float width;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        width = rect.rect.width;
    }

    void Update()
    {
        rect.anchoredPosition += Vector2.left * (speed * Time.deltaTime);
        if (rect.anchoredPosition.x <= -width)
        {
            rect.anchoredPosition += new Vector2(width * 2f, 0f);
        }
    }
}