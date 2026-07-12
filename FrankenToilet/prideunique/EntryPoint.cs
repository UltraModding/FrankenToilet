using FrankenToilet.Core;
using UnityEngine.SceneManagement;
using static FrankenToilet.Core.LogHelper;

namespace FrankenToilet.prideunique;

[EntryPoint]
public static class EntryPoint
{
    [EntryPoint]
    public static void Start()
    {
        LogInfo("Start Called");

        AssetsController.Init();

        SceneManager.sceneLoaded += (scene, lcm) =>
        {
            if (SceneHelper.LastScene == "Bootstrap")
            {
                AudioClipsAddressableLoader.LoadSoundAndMusicAddressables();
            }

            if (SceneHelper.CurrentScene != "Bootstrap"
                && SceneHelper.CurrentScene != "Intro")
            {
                CoroutineRunner.RunDelayed(0.1f, () =>
                {
                    if (SceneHelper.CurrentScene != "Main Menu")
                    {
                        Popups.Init();
                        ImproveHookPoints.Init();
                    }

                    SoundRandomizer.SwitchSounds();
                });
            }
        };

        TurnDarkOverTime.Instance.Awake();
        LogInfo("Start End Reached");
    }
}
