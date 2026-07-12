namespace FrankenToilet.Bryan;

using FrankenToilet.Bryan.Patches;
using FrankenToilet.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityObject = UnityEngine.Object;
using UnityRandom = UnityEngine.Random;

/// <summary> Class for setting up my part of this mod (oh god) </summary>
[EntryPoint]
public static class Femboy
{
    /// <summary> Whether to fuck tghe text (used in TextFucker) </summary>
    public static bool fuckText = false;

    /// <summary> why did i sign up for this </summary>
    [EntryPoint]
    public static void Load()
    {
        Assets.DelayedLoad();

        SceneManager.sceneLoaded += (_, _) =>
        {
            fuckText = UnityRandom.Range(0, 4) == 0;

            switch (SceneHelper.CurrentScene)
            {
                case "Main Menu": FindObject<Image>("Canvas/Main Menu (1)/LeftSide/Title").sprite = Assets.ulakill; break;
                case "Level 0-1": FindObject<Image>("Canvas/HurtScreen/Title Sound/Image").sprite = Assets.UlraKil; break;
                case "Level 7-1": FindObject<Image>("Canvas/HurtScreen/White").sprite = Assets.Flash; break;

                case "Level 2-1":
                    Transform Room1Lighting = FindObject<Transform>("1 - New Opener/1 Nonstuff/Lighting");
                    Room1Lighting.GetChild(0).GetComponent<Light>().color = new(0f, 0.8f, 1f, 1f);
                    Room1Lighting.GetChild(1).GetComponent<Light>().color = new(0.86f, 0f, 0.5f, 1f);

                    // room1 section 2
                    Room1Lighting.GetChild(2).GetComponent<Light>().color = new(0.96f, 0.63f, 1f, 1f);
                    Room1Lighting.GetChild(3).GetComponent<Light>().color = new(0.314159265f, 0.58f, 1f, 1f);
                    Room1Lighting.GetChild(5).GetComponent<Light>().color = new(1f, 0.86f, 1f, 1f);

                    Transform Room2Decor = FindObject<Transform>("2 - Tower 1/2 Nonstuff/Decorations");
                    Room2Decor.GetChild(0).Find("Point Light").GetComponent<Light>().color = new(0f, 0.8f, 1f, 1f);
                    Room2Decor.GetChild(1).Find("Point Light").GetComponent<Light>().color = new(1f, 0.86f, 1f, 1f);
                    Room2Decor.GetChild(2).Find("Point Light").GetComponent<Light>().color = new(0.86f, 0f, 0.5f, 1f);
                    Room2Decor.GetChild(3).Find("Point Light").GetComponent<Light>().color = new(0.314159265f, 0.58f, 1f, 1f);
                    Room2Decor.GetChild(4).Find("Point Light").GetComponent<Light>().color = new(0.86f, 0f, 0.5f, 1f);
                    Room2Decor.GetChild(5).Find("Point Light").GetComponent<Light>().color = new(0f, 0.81f, 1f, 1f);
                    Room2Decor.GetChild(6).Find("Point Light").GetComponent<Light>().color = new(0.96f, 0.63f, 1f, 1f);
                    Room2Decor.GetChild(7).Find("Point Light").GetComponent<Light>().color = new(0f, 1f, 1f, 1f);
                    Room2Decor.GetChild(8).Find("Point Light").GetComponent<Light>().color = new(1f, 0.86f, 1f, 1f);
                    Room2Decor.GetChild(9).Find("Point Light").GetComponent<Light>().color = new(0.314159265f, 0.58f, 1f, 1f);
                    Room2Decor.GetChild(10).Find("Point Light").GetComponent<Light>().color = new(0.86f, 0f, 0.5f, 1f);
                    Room2Decor.GetChild(11).Find("Point Light").GetComponent<Light>().color = new(0.96f, 0.63f, 1f, 1f);

                    Transform Room5Lighting = FindObject<Transform>("5 - Tower 2/5 Nonstuff/Lights");
                    for (int i = 0; i < 20; i++)
                        Room5Lighting.GetChild(i).Find("Point Light").GetComponent<Light>().color = new(0.92f, 0.4f, 0.74f, 1f);

                    break;
            }

            if (SceneHelper.CurrentScene != "Intro")
            {
                foreach (VideoPlayer vid in UnityObject.FindObjectsOfType<VideoPlayer>(true))
                    VideoPatch.ReplaceVideo(vid);
            }

            try
            {
                FindObject<TextMeshProUGUI>("Canvas/Level Stats Controller/Level Stats (1)/Style Title").text = "AURA";
            }
            catch { }
        };
    }

    extension(GameObject obj)
    {
        /// <summary> Finds a GameObject based on name/path, doesnt matter if its enabled or not. </summary>
        public static T FindObject<T>(string Path, Scene? scene = null) where T : Component
        {
            scene ??= SceneManager.GetActiveScene();

            string ogPath = Path;
            string rootSearchObj = Path;
            if (Path.IndexOf('/') != -1)
            {
                rootSearchObj = Path[..Path.IndexOf('/')];
                Path = Path[(Path.IndexOf('/') + 1)..];
            }

            GameObject search = scene.Value.GetRootGameObjects().FirstOrDefault(g => g.name == rootSearchObj)
                ?? throw new NullReferenceException($"Couldn't find root object named '{rootSearchObj}'.");

            // if path == rootsearchobj that means the path had no '/' and we're not searching for children of the root
            if (Path != rootSearchObj)
            {
                search = search.transform.Find(Path)?.gameObject 
                    ?? throw new NullReferenceException($"Couldn't find child in object named '{rootSearchObj}' at path '{Path}'.");
            }

            return search.GetComponent<T>() ?? throw new NullReferenceException($"Object at path '{ogPath}' does not have component '{typeof(T).Name}'.");
        }

        public GameObject ReplaceAllShaders(Shader shader)
        {
            if (!shader) throw new ArgumentNullException(nameof(shader));

            IEnumerable<Renderer> allRenderers = obj.GetComponents<Renderer>();
            allRenderers = allRenderers.Concat(obj.GetComponentsInChildren<Renderer>(true));

            foreach (Material mat in allRenderers.SelectMany(r => r.materials))
                mat?.shader = shader;

            return obj;
        }
    }
}