namespace FrankenToilet.Bryan;

using FrankenToilet.Core;
using System;
using System.IO;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using UnityObject = UnityEngine.Object;

/// <summary> Die </summary>
public static class Assets
{
    /// <summary> Spooky scary asset bundle oooooo </summary>
    public static AssetBundle assetBundle;

    /// <summary> amercia </summary>
    public static VideoClip Amercia;

    /// <summary> Comic sands. </summary>
    public static TMP_FontAsset ComicSands;

    /// <summary> Comic sands. </summary>
    public static Font L_ComicSands;

    /// <summary> silly </summary>
    public static Sprite UlraKil, ulakill, Flash, HeavyImg, Trans;

    /// <summary> the budget was dropped for maurice </summary>
    public static GameObject MauriceBad;

    /// <summary> Real Heavy trust </summary>
    public static GameObject HeavyReal, HeavyRed, HeavyBlue, HeavyGreen;
    public static GameObject rawimage;

    /// <summary> ha ha ha ha </summary>
    public static RuntimeAnimatorController LaughingAnim;

    /// <summary> such an evil lil fella :3 </summary>
    public static AudioClip Laughing;

    /// <summary> Load the asset bundle when we enter the main menu. </summary>
    public static void DelayedLoad()
    {
        SceneManager.sceneLoaded += StartLoadIfMainMenu;
        void StartLoadIfMainMenu(Scene _, LoadSceneMode __)
        {
            if (SceneHelper.CurrentScene == "Main Menu")
            {
                Load();
                SceneManager.sceneLoaded -= StartLoadIfMainMenu;
            }
        }
    }

    /// <summary> Load the asset bundle. </summary>
    public static void Load()
    {
        GrabEmbeddedBundle();

        Trans    = LoadAsset<Sprite>("Assets/trans.png");
        ulakill  = LoadAsset<Sprite>("Assets/title.png");
        HeavyImg = LoadAsset<Sprite>("Assets/heavy.png");
        Flash    = LoadAsset<Sprite>("Assets/flash.jpeg");
        UlraKil  = LoadAsset<Sprite>("Assets/ultrakill wingdings.png");

        Amercia  = LoadAsset<VideoClip>("Assets/amercia.mp4");
        Laughing = LoadAsset<AudioClip>("Assets/1bitahh.mp3");

        L_ComicSands = LoadAsset<Font>("Assets/comicsanslegacy.ttf");
        ComicSands   = LoadAsset<TMP_FontAsset>("Assets/comicsans.asset");

        LaughingAnim = LoadAsset<RuntimeAnimatorController>("Assets/bad laughing skull.controller");

        HeavyRed   = LoadAsset<GameObject>("Assets/Heavy.prefab").ReplaceAllShaders(DefaultReferenceManager.Instance.masterShader);
        HeavyReal  = LoadAsset<GameObject>("Assets/heavyreal.prefab").ReplaceAllShaders(DefaultReferenceManager.Instance.masterShader);
        HeavyBlue  = LoadAsset<GameObject>("Assets/HeavyBlue.prefab").ReplaceAllShaders(DefaultReferenceManager.Instance.masterShader);
        HeavyGreen = LoadAsset<GameObject>("Assets/HeavyGreen.prefab").ReplaceAllShaders(DefaultReferenceManager.Instance.masterShader);
        MauriceBad = LoadAsset<GameObject>("Assets/mauricebad.prefab").ReplaceAllShaders(DefaultReferenceManager.Instance.masterShader);

        rawimage = LoadAsset<GameObject>("Assets/RawImage.prefab");
    }

    /// <summary> Loads an asset from the asset bundle with the provided name and checks if it's null. </summary>
    public static T LoadAsset<T>(string name) where T : UnityObject
    {
        LogHelper.LogInfo("loading asset " + name);
        T result = assetBundle.LoadAsset<T>(name);
        LogHelper.LogInfo("loaded asset " + (result?.name ?? "<null>"));
        result = result ?? throw new NullReferenceException($"Assetbundle doesn't have an asset called {name}");
        LogHelper.LogInfo("didnt throw :P");
        return result;
    }

    /// <summary> Grabs the embedded asset bundle. </summary>
    public static void GrabEmbeddedBundle()
    {
        // get the stream for the embedded asset bundle
        Stream bundleStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("FrankenToilet.Bryan.fuckyou.bundle");

        // load the asset bundle (ty unity for adding loadfromstream)
        assetBundle = AssetBundle.LoadFromStream(bundleStream);
    }

#region Debug

    /// <summary> Gets all the asset names in the asset bundle and logs it. </summary>
    public static void GetAllAssetNames() => // assets/amercia.mp4, assets/comicsans.asset, assets/minos prime.wav, assets/ultrakill wingdings.png
        LogHelper.LogInfo(string.Join(", ", assetBundle.GetAllAssetNames()));

    /// <summary> Grabs all the paths to all embedded assets and logs it. </summary>
    public static void GrabEmbeddedAssetPaths() =>
        LogHelper.LogInfo($"Embedded Assets: {string.Join(", ", Assembly.GetExecutingAssembly().GetManifestResourceNames())}");

#endregion
}