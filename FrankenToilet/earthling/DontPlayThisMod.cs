using UnityEngine;
using UnityEngine.SceneManagement;
using FrankenToilet.Core;

namespace FrankenToilet.earthling;

[EntryPoint]
public static class DontPlayThisMod
{
    [EntryPoint]
    public static void ChangePlayButtonText()
    {
        SceneManager.sceneLoaded += (scene, lcm) => {
            if (SceneHelper.CurrentScene == "Main Menu")
            {
                GameObject playButtonText = GameObject.Find("Canvas/Main Menu (1)/LeftSide/Continue/Text");
                playButtonText.GetComponent<TMPro.TextMeshProUGUI>().text = "don't play";
            }
        };
    }
}
