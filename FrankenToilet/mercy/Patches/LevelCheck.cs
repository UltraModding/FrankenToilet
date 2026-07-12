using System.Linq;
using FrankenToilet.Core;
using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FrankenToilet.mercy.Patches;

[PatchOnEntry]
[HarmonyPatch(typeof(SceneHelper))]
public static class LevelCheck
{
    public static string[] blacklistedScenes = ["Bootstrap", "Intro", "Main Menu"];
    [HarmonyPatch("OnSceneLoaded")]
    [HarmonyPostfix]
    public static void SceneCheck() {
        GameObject[] gameObjects = SceneManager.GetActiveScene().GetRootGameObjects();
        foreach (GameObject go in gameObjects)
            if (go.GetComponent<Canvas>() != null) Plugin.canvas = go;
        // ACTIVATE OUR FEATURES,,,
        if (!blacklistedScenes.Contains(SceneHelper.CurrentScene))
        {
            Plugin.manager = new GameObject("Manager");
            Plugin.manager.AddComponent<ActivateFeatures>();
        }
    }
}