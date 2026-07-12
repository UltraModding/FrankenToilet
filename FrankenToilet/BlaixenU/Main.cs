
using FrankenToilet.Core;
using System;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using HarmonyLib;
using static FrankenToilet.Core.LogHelper;

namespace FrankenToilet.BlaixenU;

// Organization? I barely know nation!

// GRAAAAAAAAAAAAAAAAAAAAHHHHHHHHHHHHHHHHHHH FUCKING HELP MEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEEE

public static class AssetMan
{

    private static AssetBundle _assets;

    public static bool AssetsLoaded = false;

    public static GameObject Popup1 => _assets.LoadAsset<GameObject>("Popup1");
    public static GameObject Popup2 => _assets.LoadAsset<GameObject>("Popup2"); // 2 and 3 prefab variants
    public static GameObject Popup3 => _assets.LoadAsset<GameObject>("Popup3");


    public static void Load()
    {
        LogInfo("Loading assets");
        byte[] data;
        try
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string resourceName = $"FrankenToilet.BlaixenU.assetblaixundle";
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
}

[EntryPoint]
public static class Main
{
    [EntryPoint]
    public static void Start()
    {
        AssetMan.Load();
    }
}

[PatchOnEntry]
[HarmonyPatch]
public static class FUCKMEEEEEEEEEEEEEEEHEEEEEEEELP
{
    public static GameObject PopupManObject = new();

    [HarmonyPostfix, HarmonyPatch(typeof(NewMovement), nameof(NewMovement.Start))]
    public static void HEEEEEEEEEEEEEEEEEEEEEEEEEEEEEELP()
    {
        if (!AssetMan.AssetsLoaded)
        {
            return;
        }

        if (PopupManObject == null)
        {
            PopupManObject = new();
            PopupManObject.AddComponent<UnityScripts.PopupManager>();
            return;
        }

        PopupManObject.AddComponent<UnityScripts.PopupManager>();
    }
}