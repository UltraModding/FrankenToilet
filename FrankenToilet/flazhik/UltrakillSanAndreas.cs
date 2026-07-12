using FrankenToilet.Core;
using FrankenToilet.flazhik.Assets;
using FrankenToilet.flazhik.Components;
using FrankenToilet.flazhik.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEngine.Object;
using static UnityEngine.GameObject;

namespace FrankenToilet.flazhik;

[EntryPoint]
public class UltrakillSanAndreas
{
    [ExternalAsset("Assets/UltrakillSanAndreas/Prefabs/ScreenCanvas.prefab", typeof(GameObject))]
    private static GameObject screenCanvas;

    [EntryPoint]
    public static void Init()
    {
        AssetsManager.Instance.LoadAssets("FrankenToilet.flazhik.ultrakill_sa.bundle");
        AssetsManager.Instance.RegisterPrefabs();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (SceneHelper.CurrentScene == null || scene != SceneManager.GetActiveScene() 
                                             || SceneHelper.CurrentScene.IsOneOf("Bootstrap", "Main Menu", "Intro"))
            return;

        SetupCanvas();
    }

    private static void SetupCanvas()
    {
        var canvas = Find("/Canvas");
        var saCanvas = Instantiate(screenCanvas, canvas.transform, false);
        saCanvas.transform.SetAsFirstSibling();
        saCanvas.AddComponent<SanAndreasUi>();
    }
}