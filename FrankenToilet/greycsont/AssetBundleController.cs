using UnityEngine;
using System.Reflection;
using System.Collections.Generic;
using FrankenToilet.Core;


namespace FrankenToilet.greycsont;

[EntryPoint]
public static class AssetBundleController
{
    const string noteSkinPath = "FrankenToilet.greycsont.assetstealfromrhythmgame.bundle";

    public static AssetBundle assetBundle;

    public static Sprite[] arrowSprites;

    public static Sprite[] farInTheBlueSky;

    public static Dictionary<string, AudioClip> audioCaches = new Dictionary<string, AudioClip>();

    [EntryPoint]
    public static void Initialize()
    {
        assetBundle = LoadAssetBundle(noteSkinPath);
        arrowSprites = assetBundle.LoadAssetWithSubAssets<Sprite>("arrow");
        farInTheBlueSky = assetBundle.LoadAssetWithSubAssets<Sprite>("farinthebluesky");
        var clips = assetBundle.LoadAllAssets<AudioClip>();
        
        foreach (var clip in clips)
            audioCaches[clip.name] = clip;
        LogHelper.LogDebug($"[greycsont] audioClip count: {audioCaches.Count}");
        
        foreach (var kvp in audioCaches)
            LogHelper.LogDebug($"[greycsont] key: {kvp.Key}");
        
        if (farInTheBlueSky == null)
            LogHelper.LogError($"[greycsont] farInTheBlueSky not found");
        else
            LogHelper.LogDebug($"[greycsont] farInTheBlueSky found");
    }

    public static AssetBundle LoadAssetBundle(string assetBundlePath)
    {
        var assembly = Assembly.GetExecutingAssembly();
        using var stream = assembly.GetManifestResourceStream(assetBundlePath);
        if (stream == null)
        {
            LogHelper.LogError($"[greycsont] FUCK YOU UNITY");
        }
        
        LogHelper.LogInfo($"[greycsont] Loaded AssetBundle: {assetBundlePath}");
        
        return AssetBundle.LoadFromStream(stream);;
    }
}