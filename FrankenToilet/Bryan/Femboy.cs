namespace FrankenToilet.Bryan;

using FrankenToilet.Core;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary> Class for setting up my part of this mod (oh god) </summary>
[EntryPoint]
public class Femboy
{
    /// <summary> Whether to fuck tghe text (used in TextFucker) </summary>
    public static bool fuckText = false;

    /// <summary> why did i sign up for this </summary>
    [EntryPoint]
    public static void Awake()
    {
        BundleLoader.Load();

        SceneManager.sceneLoaded += (_, _) =>
        {
            fuckText = Random.Range(0, 4) == 0;
            
            if (SceneHelper.CurrentScene == "Main Menu")
                FindObject<Image>("Canvas/Main Menu (1)/LeftSide/Title").sprite = BundleLoader.ulakill;

            if (SceneHelper.CurrentScene == "Level 0-1")
                FindObject<Image>("Canvas/HurtScreen/Title Sound/Image").sprite = BundleLoader.UlraKil;

            if (SceneHelper.CurrentScene == "Level 7-1")
                FindObject<Image>("Canvas/HurtScreen/White").sprite = BundleLoader.Flash;

            if (SceneHelper.CurrentScene == "Level 2-1")
            {
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
                {
                    Room5Lighting.GetChild(i).Find("Point Light").GetComponent<Light>().color = new(0.92f, 0.4f, 0.74f, 1f);
                }
            }

            FindObject<TextMeshProUGUI>("Canvas/Level Stats Controller/Level Stats (1)/Style Title")?.text = "AURA";
        };
    }

    /// <summary> Finds a GameObject based on name/path, doesnt matter if its enabled or not. </summary>
    public static T FindObject<T>(string Path, Scene? scene = null) where T : Component
    {
        scene ??= SceneManager.GetSceneAt(0);
        string rootSearchObj = Path;
        if (Path.IndexOf('/') != -1)
        {
            rootSearchObj = Path[..Path.IndexOf('/')];
            Path = Path[(Path.IndexOf('/') + 1)..];
        }

        var search = scene.Value.GetRootGameObjects().Where(g => g.name == rootSearchObj).FirstOrDefault();
        search = Path.IndexOf('/') == -1 ? search : search?.transform?.Find(Path)?.gameObject;
        return search?.GetComponent<T>();
    }
}
