using FrankenToilet.Core;
using System.IO;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.Video;

namespace FrankenToilet.Bryan;

/// <summary> Die </summary>
public static class BundleLoader
{
    /// <summary> Spooky scary asset bundle oooooo </summary>
    public static AssetBundle assetBundle;

    /// <summary> Amercia. </summary>
    public static VideoPlayer Amercia;

    /// <summary> Comic sands. </summary>
    public static TMP_FontAsset ComicSands;

    /// <summary> Comic sands. </summary>
    public static Font ComicSandsLegacy;

    /// <summary> Lakull. </summary>
    public static AudioClip MinosDime;

    /// <summary> silly </summary>
    public static Sprite UlraKil;

    /// <summary> Load the asset bundle. </summary>
    public static void Load()
    {
        GrabEmbeddedBundle();

        Amercia = assetBundle.LoadAsset<VideoPlayer>("assets/amercia.mp4");
        ComicSands = assetBundle.LoadAsset<TMP_FontAsset>("assets/comicsans.asset");
        ComicSandsLegacy = assetBundle.LoadAsset<Font>("assets/comicsanslegacy.ttf");
        MinosDime = assetBundle.LoadAsset<AudioClip>("assets/minos prime.wav");
        UlraKil = assetBundle.LoadAsset<Sprite>("assets/ultrakill wingdings.png");
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
