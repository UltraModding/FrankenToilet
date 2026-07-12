using UnityEngine;


namespace FrankenToilet.greycsont;

public static class UnityPathHelper
{
    public static Canvas? FindCanvas()
    {
        var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        foreach (var root in scene.GetRootGameObjects())
        {
            var canvas = root.GetComponent<Canvas>();
            if (canvas != null)
                return canvas;
        }
        return null;
    }
}