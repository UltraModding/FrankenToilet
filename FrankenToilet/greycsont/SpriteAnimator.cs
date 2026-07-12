using UnityEngine;
using UnityEngine.UI;


namespace FrankenToilet.greycsont;


public class SpriteAnimator : MonoBehaviour
{
    public Sprite[] frames;
    public float fps = 25f; // That's the fps for far in the blue sky only

    private Image _image;
    private int _frameIndex;
    private float _timer;

    void Awake()
    {
        _image = GetComponent<Image>();
    }

    void Update()
    {
        if (frames == null || frames.Length == 0) return;

        _timer += Time.unscaledDeltaTime;

        if (_timer >= (1f / fps))
        {
            _timer = 0;
            _frameIndex = (_frameIndex + 1) % frames.Length;
            _image.sprite = frames[_frameIndex];
        }
    }
}