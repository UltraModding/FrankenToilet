#pragma warning disable CS8618
using FrankenToilet.Core;
using UnityEngine;
using UnityObject = UnityEngine.Object;
using UnityEngine.SceneManagement;
using UnityEngine.Events;


namespace FrankenToilet.somebilly {
    [EntryPoint]
    // THIS IS THE PLUGIN.
    public class Plugin {
        public static Bib bib;

        [EntryPoint]
        public static void Initialize() {
            LogHelper.LogInfo("SOMETHING MALICIOUS IS BREWING...");
            SceneManager.sceneLoaded += new UnityAction<Scene, LoadSceneMode>(OnSceneLoaded);
        }

        public static void OnSceneLoaded(Scene scene, LoadSceneMode lsm) {
            if (Bib.Instance != null) {
                return;
            }
            AddThings();
        }
        public static void AddThings() {
            GameObject theStuff = new GameObject("frankenstuff");
            Bib.Instance = theStuff.AddComponent<Bib>();
            UnityObject.DontDestroyOnLoad(theStuff);
        }
    }
}