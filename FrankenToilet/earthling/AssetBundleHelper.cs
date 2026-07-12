using System;
using System.IO;
using System.Reflection;
using FrankenToilet.Core;
using UnityEngine;
using System.Collections.Generic;
using Object = System.Object;

namespace FrankenToilet.earthling;

[EntryPoint]
public static class AssetBundleHelper
{
    private static AssetBundle? bundle;
    private static Dictionary<string, Object> assetCache = new Dictionary<string, Object>();

    [EntryPoint]
    public static void Init()
    {
        LogHelper.LogInfo("[earthling on fire] Loading assets");
        byte[] data;
        try
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string resourceName = $"FrankenToilet.earthling.toiletonfire.bundle";
            var s = assembly.GetManifestResourceStream(resourceName);
            s = s ?? throw new FileNotFoundException($"Could not find embedded resource '{resourceName}'.");
            using var ms = new MemoryStream();
            s.CopyTo(ms);
            data = ms.ToArray();
        }
        catch (Exception ex)
        {
            LogHelper.LogError($"[earthling on fire] Error loading assets: " + ex.Message);
            return;
        }

        bundle = AssetBundle.LoadFromMemory(data);

        if (bundle != null)
        {
            LogHelper.LogInfo("[earthling on fire] Loaded assets");
            FishProvider.LoadFishes();
        }
        else
        {
            LogHelper.LogError("[earthling on fire] Error loading bundle");
        }
    }

    public static T? LoadAsset<T>(string asset)
    {
        if (bundle == null) return default(T);

        if (assetCache.ContainsKey(asset))
        {
            if (!(assetCache[asset] == null) && !assetCache[asset].Equals(null))
            {
                return (T)assetCache[asset];
            }
            assetCache.Remove(asset);
        }
        Object loadedAsset = bundle.LoadAsset(asset);
        assetCache.Add(asset, loadedAsset);
        return (T)loadedAsset;
    }
}
