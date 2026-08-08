using UnityEngine;
using HarmonyLib;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using FrankenToilet.Core;
using System;

/*
 * This piece of code is responsible for the jumpscare on scene load.
 * I am not responcible for any possible heart attacks caused from this.
 */
namespace FrankenToilet.alma;

[EntryPoint]
internal class InterruptSceneLoading
{
    static int chanceOfJumpscare = 15; //in percentage

    [EntryPoint]
    private static void Start()
    {
        try
        {
            AssetBundle bundle = Functions.GetBundle("FrankenToilet.alma.scenes.bundle");
            AssetBundle assetsBundle = Functions.GetBundle("FrankenToilet.alma.assets.bundle");
            string[] scenePaths = bundle.GetAllScenePaths();
            string[] assetsNames = assetsBundle.GetAllAssetNames();
        }
        catch (Exception ex)
        {
            LogHelper.LogError($"Failed to load the bundle:{ex}");
        }
    }

    [PatchOnEntry]
    [HarmonyPatch(typeof(SceneHelper), nameof(SceneHelper.LoadScene))]
    public class PatchSceneHelperLoadScene
    {
        public static bool Prefix()
        {
            int percentage = new System.Random().Next(1, 101);
            if (percentage <= chanceOfJumpscare)
            {
                if (SceneHelper.CurrentScene != "Bootstrap")
                {
                    if (SceneHelper.CurrentScene != "Intro")
                    {
                        try
                        {
                            LogHelper.LogInfo("Loading into 'fear' scene...");
                            Addressables.LoadAssetAsync<GameObject>("FirstRoom Player Only");
                            SceneManager.LoadScene("fear");
                            return false;
                        }
                        catch (Exception ex)
                        {
                            LogHelper.LogError($"Failed to load the scene:{ex}");
                            return true;
                        }
                    }
                }
            }
            return true;
        }
    }
}
