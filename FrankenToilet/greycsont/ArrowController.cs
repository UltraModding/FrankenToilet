using UnityEngine;
using UnityEngine.UI;
using FrankenToilet.Core;

namespace FrankenToilet.greycsont;

public static class ArrowController
{
    public static Canvas canvas
    {
        get
        {
            if (field == null)
            {
                field = UnityPathHelper.FindCanvas();
            }
            return field;
        }
    } 
    public static GameObject imgObj
    {
        get
        {
            if (field == null)
            {
                field = new GameObject("HammerArrowIndicator");
                imgObj.transform.SetParent(canvas.transform, false);
                imgObj.transform.SetAsLastSibling();
            }
            return field;
        }
        set;
    }

    public static AudioSource source
    {
        get
        {
            if (field == null)
            {
                field = new GameObject("HammerArrowAudioSource").AddComponent<AudioSource>();
            }
            return field;
        }
    }

    public static void GenerateImage(float timeInSeconds)
    {
        var hammer = ShotgunHammerPatch.lastActiveHammer;
        if (hammer == null) return;
        if (hammer.target == null) return;
        if (hammer.hitEnemy == null) return;
        if (canvas == null) return;

        var clip = AssetBundleController.audioCaches["sam_" + DirectionRandomizer.randomDirection];

        if (clip != null)
        {
            source.SetSpatialBlend(0f);
            source.PlayOneShot(clip, 1f, true);
        }

        var img = imgObj.AddComponent<Image>();
        img.sprite = AssetBundleController.arrowSprites[Random.Range(0, AssetBundleController.arrowSprites.Length)];
        img.SetNativeSize();

        var color = img.color;
        color.a = 0.85f;
        img.color = color;

        var rect = imgObj.GetComponent<RectTransform>();

        rect.localEulerAngles = new Vector3(0, 0, -90f * DirectionRandomizer.randomDirection);
        rect.localScale = new Vector3(1.3f, 1.3f, 1.3f);

        imgObj.AddComponent<DestoryTimer>().lifetime = timeInSeconds;
    }
}