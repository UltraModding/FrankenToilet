using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace FrankenToilet.prideunique;

public static class FoxyPopup
{
    private static GameObject MainPrefab;
    private static VideoClip FoxyVideoClip;

    public static void Init()
    {
        if (!AssetsController.AssetsLoaded)
            return;

        if (!CameraController.Instance || !OptionsMenuToManager.Instance)
            return;

        MainPrefab = Popups.MainPrefab;
        MainPrefab.SetActive(false);

        FoxyVideoClip = AssetsController.LoadAsset<VideoClip>("assets/aizoaizo/foxy.mp4");

        CoroutineRunner.Run(FoxyHandler());
    }

    private static IEnumerator FoxyHandler()
    {
        yield return new WaitForSeconds(RandomForMe.Next(30.0f, 40.0f));

        while (true)
        {
            if (AssetsController.IsSlopSafe)
            {
                SpawnFoxy();

                yield return new WaitForSeconds(RandomForMe.Next(15.0f, 50.0f));
            }
            else
            {
                GameObject go = SpawnFoxy();

                yield return new WaitForSeconds(((float)FoxyVideoClip.length));

                UnityEngine.Object.Destroy(go);

                yield return new WaitForSeconds(RandomForMe.Next(120.0f, 280.0f));
            }
        }
    }

    private static GameObject SpawnFoxy()
    {
        GameObject go = UnityEngine.Object.Instantiate(MainPrefab);
        go.SetActive(true);

        RenderTexture renderTexture = new RenderTexture(Popups.BaseRenderTexture);
        renderTexture.Create();

        Popups.RenderTextures.Add(go, renderTexture);

        VideoPlayer videoPlayer = go.GetComponentInChildren<VideoPlayer>();
        videoPlayer.targetTexture = renderTexture;

        RawImage rawImage = go.GetComponentInChildren<RawImage>();
        rawImage.rectTransform.sizeDelta = new Vector2(FoxyVideoClip.width * 2, FoxyVideoClip.height * 2);
        rawImage.texture = renderTexture;
        rawImage.raycastTarget = false;

        Popup pu = rawImage.gameObject.AddComponent<Popup>();
        pu.Parent = go;
        pu.CloseSound = Popups.VideoCloseSound;

        videoPlayer.source = VideoSource.VideoClip;
        videoPlayer.clip = FoxyVideoClip;
        videoPlayer.SetDirectAudioVolume(0, PrefsManager.Instance.GetFloat("allVolume", 0f));

        videoPlayer.Prepare();

        videoPlayer.prepareCompleted += (vp) =>
        {
            go.transform.position = new Vector3(0f, 0f, 0f);
            go.transform.rotation = Quaternion.identity;

            go.transform.SetParent(OptionsMenuToManager.Instance.transform, false);

            vp.Play();
        };

        return go;
    }
}
