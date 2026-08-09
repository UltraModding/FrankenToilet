using System;
using System.Collections.Generic;
using System.Reflection;
using FrankenToilet.Core;
using UnityEngine;

namespace FrankenToilet.alma;

internal static class Functions
{
    public static Dictionary<string, AssetBundle> assetBundles = new();

    public static AssetBundle? GetBundle(string bundleToLoad)
    {
        if (assetBundles.ContainsKey(bundleToLoad))
        {
            LogHelper.LogWarning("Tried to load an already loaded bundle, skipping.");
            return assetBundles[bundleToLoad];
        }

        var assembly = Assembly.GetExecutingAssembly();
        try
        {
            using (var stream = assembly.GetManifestResourceStream(bundleToLoad))
            {
                var bundle = AssetBundle.LoadFromStream(stream);
                assetBundles.Add(bundleToLoad, bundle);
                return bundle;
            }
        }
        catch (Exception e)
        {
            LogHelper.LogError($"Failed to load bundle: {bundleToLoad}. Error: {e}");
            return null;
        }
    }
}