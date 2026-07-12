using FrankenToilet.Bryan;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace FrankenToilet.prideunique;
public static class Popups
{
    public static GameObject MainPrefab;
    public static List<VideoClip> VideoClips = new List<VideoClip>();

    public static RenderTexture BaseRenderTexture;
    public static Dictionary<GameObject, RenderTexture> RenderTextures = new Dictionary<GameObject, RenderTexture>();

    public static AudioClip VideoCloseSound;

    public static void Init()
    {
        if (!AssetsController.AssetsLoaded)
            return;

        if (!CameraController.Instance || !OptionsMenuToManager.Instance)
            return;

        _ = PopupCloser.Instance;

        MainPrefab = AssetsController.LoadAsset<GameObject>("assets/aizoaizo/popup.prefab");
        GameObject videoPlayer = MainPrefab.GetComponentInChildren<VideoPlayer>().gameObject;
        videoPlayer.AddComponent<NonReplaceableVideo>();

        MainPrefab.SetActive(false);

        BaseRenderTexture = AssetsController.LoadAsset<RenderTexture>("assets/aizoaizo/videotexture.rendertexture");

        VideoCloseSound = AssetsController.LoadAsset<AudioClip>("assets/aizoaizo/pum.ogg");
        for (int i = 1; i <= 19; i++)
        {
            if (i == 14) // had problems with this one
                continue;

            VideoClips.Add(AssetsController.LoadAsset<VideoClip>("assets/aizoaizo/" + i.ToString() + ".mp4"));
        }

        CoroutineRunner.Run(PopupHandler());

        FoxyPopup.Init();
    }

    private static IEnumerator PopupHandler()
    {
        while (true)
        {
            VideoClips.Shuffle();

            if (AssetsController.IsSlopSafe)
            {
                SpawnPopup(VideoClips[0]);
                SpawnPopup(VideoClips[1]);
                SpawnPopup(VideoClips[2]);

                yield return new WaitForSeconds(((float)VideoClips[0].length) * 3);
            }
            else
            {
                SpawnPopup(VideoClips[0]);
                SpawnPopup(VideoClips[1]);

                yield return new WaitForSeconds(((float)VideoClips[0].length) * 7);
            }
        }
    }

    private static GameObject SpawnPopup(VideoClip videoClip)
    {
        GameObject go = UnityEngine.Object.Instantiate(MainPrefab);
        go.SetActive(true);

        RenderTexture renderTexture = new RenderTexture(BaseRenderTexture);
        renderTexture.Create();

        RenderTextures.Add(go, renderTexture);

        VideoPlayer videoPlayer = go.GetComponentInChildren<VideoPlayer>();
        videoPlayer.targetTexture = renderTexture;

        RawImage rawImage = go.GetComponentInChildren<RawImage>();
        rawImage.rectTransform.sizeDelta = new Vector2(videoClip.width, videoClip.height);
        rawImage.texture = renderTexture;

        Popup pu = rawImage.gameObject.AddComponent<Popup>();
        pu.Parent = go;
        pu.CloseSound = VideoCloseSound;

        videoPlayer.source = VideoSource.VideoClip;
        videoPlayer.clip = videoClip;
        videoPlayer.SetDirectAudioVolume(0, PrefsManager.Instance.GetFloat("allVolume", 0f));

        videoPlayer.Prepare();

        videoPlayer.prepareCompleted += (vp) =>
        {
            Vector3 dir = Random.onUnitSphere;
            Vector3 pos = dir.normalized * RandomForMe.Next(384f, 512f);

            Follow f = go.gameObject.AddComponent<Follow>();
            f.target = CameraController.Instance.transform;
            f.mimicPosition = true;

            go.transform.GetChild(0).position = pos;

            var co = go.transform.GetChild(0).gameObject.AddComponent<AlwaysLookAtCamera>();
            co.useXAxis = true;
            co.useYAxis = true;
            co.useZAxis = true;

            vp.Play();
        };

        return go;
    }
}
