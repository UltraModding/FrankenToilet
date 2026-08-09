using System;
using FrankenToilet.Core;
using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = System.Random;

/*
 * This piece of code is responsible for the jumpscare on scene load.
 * I am not responsible for any possible heart attacks caused from this.
 */
namespace FrankenToilet.alma;

[EntryPoint]
internal static class InterruptSceneLoading
{
    private static readonly int chanceOfJumpscare = 15; // in percentage

    [EntryPoint]
    private static void Start()
    {
        try
        {
            Functions.GetBundle("FrankenToilet.alma.scenes.bundle");
            Functions.GetBundle("FrankenToilet.alma.assets.bundle");
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
            var percentage = new Random().Next(1, 101);
            if (percentage <= chanceOfJumpscare && SceneHelper.CurrentScene != "Tutorial")
                if (SceneHelper.CurrentScene != "Bootstrap" && SceneHelper.CurrentScene != "Intro")
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

            return true;
        }
    }
}