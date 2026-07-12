using System.Diagnostics;
using System.IO;
using System.Reflection;
using FrankenToilet.Core;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace FrankenToilet.mercy;

public static class Helper {
    public static Sprite[]? LoadFrames(AssetBundle? assetBundle, string path, int frames)
    {
        AssetBundle? bundle = Plugin.assetBundle;
        if (bundle == null)
        {
            LogHelper.LogError("NO ASSET BUNDLE");
            return null;
        }
        // ADDS ALL OF THE FRAMES TO MAKE AN ANIMATED IMAGE, WAHOO
        Sprite[] sprites = new Sprite[frames];
        for (int i = 1; i <= frames; i++)
        {
            string filePath = $"Assets/Features/{path}/{i.ToString()}.png";
            Sprite? currentSprite = bundle.LoadAsset<Sprite>(filePath);
            if (currentSprite == null)
            {
                LogHelper.LogError($"FRAME {i} COULDN'T BE LOADED");
                return null;
            }
            sprites[i-1] = currentSprite;
        } return sprites;
    }

    public static GameObject? GetRootCanvas()
    {
        
        return null;
    }

    public static void Animate(ref Image image, Sprite[]? frames, Stopwatch frameTimer, ref int index, int frameCount, double delay)
    {
        if (frames == null) return;
        // ANIMATION
        if (frameTimer.Elapsed.TotalSeconds >= delay)
        {
            image.sprite = frames[index];
            if (index < frameCount-1) ++index;
            else index = 0;
            frameTimer.Restart();
        }
    }

    public static void CreateImage<T>(string name, int width, int height) where T : MonoBehaviour
    {
        GameObject imageObj = GameObject.Instantiate(new GameObject(name), Plugin.canvas.transform);
        imageObj.AddComponent<T>();
        RectTransform rectTransform = imageObj.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(width, height);
    }
}