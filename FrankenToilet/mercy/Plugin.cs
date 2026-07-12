using System.IO;
using System.Reflection;
using FrankenToilet.Core;
using UnityEngine;
using Random = System.Random;

namespace FrankenToilet.mercy;

[EntryPoint]
public static class Plugin
{
    public static AssetBundle? assetBundle;
    public static Random rand = new();
    public static GameObject? manager;
    public static GameObject? canvas;
    public const string ASSET_BUNDLE_NAME = "FrankenToilet.mercy.assets.bundle";
    [EntryPoint]
    public static void Entry()
    {
        LogHelper.LogInfo("MERCY HERE!");
        // loading asset bundle
        Assembly assembly = Assembly.GetExecutingAssembly();
        Stream? stream = assembly.GetManifestResourceStream(ASSET_BUNDLE_NAME);
        if (stream == null) assetBundle = null;
        else assetBundle = AssetBundle.LoadFromStream(stream);
        if (assetBundle == null) LogHelper.LogError("SORRY BRAH BUT THE ASSET BUNDLE IS MISSING :((((");
        else LogHelper.LogInfo($"LOADED {ASSET_BUNDLE_NAME}");
    }
}