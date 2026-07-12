using FrankenToilet.Core;
using System;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;
using static FrankenToilet.Core.LogHelper;

namespace FrankenToilet.prideunique;

public static class AssetsController
{
    private static AssetBundle _assets;
    public static bool AssetsLoaded = false;

    // IsSlopTuber errored once so im paranoid now
    public static bool IsSlopSafe
    {
        get
        {
            try
            {
                return SteamHelper.IsSlopTuber;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }

    public static void Init()
    {
        LogInfo("Loading assets");
        byte[] data;
        try
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string resourceName = $"FrankenToilet.prideunique.aizoaizo";
            var s = assembly.GetManifestResourceStream(resourceName);
            s = s ?? throw new FileNotFoundException($"Could not find embedded resource '{resourceName}'.");
            using var ms = new MemoryStream();
            s.CopyTo(ms);
            data = ms.ToArray();
        }
        catch (Exception ex)
        {
            LogError($"Error loading assets: " + ex.Message);
            return;
        }

        SceneManager.sceneLoaded += (scene, lcm) =>
        {
            if (_assets != null) return;

            _assets = AssetBundle.LoadFromMemory(data);
            AssetsLoaded = true;
            LogInfo("Loaded assets");
        };
    }

    public static GameObject? LoadAsset(string assetName)
    {
        if (_assets == null)
            return null;

        var go = _assets.LoadAsset<GameObject>(assetName);
        if (go == null)
            return null;

        return Object.Instantiate<GameObject>(go);
    }

    public static T? LoadAsset<T>(string assetName) where T : UnityEngine.Object
    {
        if (_assets == null)
            return null;

        var go = _assets.LoadAsset<T>(assetName);
        if (go == null)
            return null;

        return Object.Instantiate<T>(go);
    }
}