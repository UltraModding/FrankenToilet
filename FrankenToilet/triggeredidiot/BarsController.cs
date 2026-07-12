using System.Text;
using FrankenToilet.Core;
using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FrankenToilet.triggeredidiot;

public sealed class BarsController : MonoBehaviour
{
    public static BarsController? Instance { get; private set; } = null;

    public GameObject gunPanel;
    public GameObject container;

    public Slider adhd;   // no moving
    public Slider boring; // no style

    public float AdhdAmount = 0.0f;
    public float AdhdRate = 1.0f / 2.0f;

    public bool BoringActive = false;
    public float BoringAmount = 0.0f;
    public float BoringRate = 1.0f / 40.0f;

    public CanvasGroup warningTextCanvas;
    public TextMeshProUGUI warningText;
    public bool ShowWarningText = false;
    private int _warningTextRepeatAmount = 32;
    private string _warningText = "";

    public string WarningText
    {
        get
        {
            int spaceIndex = _warningText.IndexOf(' ');
            return spaceIndex == -1 ? _warningText : _warningText[..spaceIndex];
        }
        set
        {
            if (WarningText == value) return;

            var sb = new StringBuilder();
            for (int i = 0; i < _warningTextRepeatAmount; i++)
                sb.Append(value + " ");

            _warningText = sb.ToString();
        }
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        adhd = container.transform.Find("AdhdPanel").GetComponentInChildren<Slider>(includeInactive:true);
        boring = container.transform.Find("BoringPanel").GetComponentInChildren<Slider>(includeInactive:true);

        AdhdAmount = -1.0f;
        BoringAmount = -0.125f;

        if (AssetsController.IsSlopSafe)
        {
            // make it harder for the content!!11!!
            AdhdRate *= 1.5f;
            BoringRate *= 1.95f;
        }

        warningText.gameObject.AddComponent<TMPInfiniteScroll>();
    }

    private Vector3 lastPos = Vector3.zero;
    private void Update()
    {
        container.SetActive(gunPanel.activeInHierarchy);
        transform.localPosition = new Vector3(-1.06f, -0.53f, 1.1f);

        if(NewMovement.Instance == null) return;

        bool movementLocked = !NewMovement.Instance.enabled;

        if(NewMovement.Instance.dead || !gunPanel.activeInHierarchy || Time.timeSinceLevelLoad < 5.0f) // give the player some time to process whats goin on
        {
            AdhdAmount = -1.0f;
            BoringAmount = -0.125f;
        }

        if (movementLocked)
        {
            lastPos = NewMovement.Instance.transform.position;

            adhd.value = AdhdAmount;
            if (BoringActive)
                boring.value = BoringAmount;

            if (ShowWarningText)
            {
                ShowWarningText = false;
                warningTextCanvas.gameObject.SetActive(false);
            }

            return;
        }

        var deltaPos = (NewMovement.Instance.transform.position - lastPos).magnitude;

        AdhdAmount += Time.deltaTime * AdhdRate;
        if(deltaPos > 0.105f)
            AdhdAmount = 0.0f;
        adhd.value = AdhdAmount;

        if (BoringActive)
        {
            BoringAmount += Time.deltaTime * BoringRate;

            if (StyleHUD.Instance != null)
            {
                int max = StyleHUD.Instance.ranks.Count - 1;
                int rank = StyleHUD.Instance.rankIndex;
                if(rank > Mathf.Min(3, max) && BoringAmount > 0)
                    BoringAmount -= Time.deltaTime * BoringRate * (rank - 1);
            }

            boring.value = BoringAmount;
        }

        ShowWarningText = false;
        if (BoringAmount > 0.67f/2.0f) // im fucked up and evil
        {
            WarningText = "DULL";
            ShowWarningText = true;
        }
        if (AdhdAmount > 0.45f) // moving is more important
        {
            WarningText = "MOVE";
            ShowWarningText = true;
        }

        warningTextCanvas.gameObject.SetActive(ShowWarningText);
        if (ShowWarningText)
            warningText.text = _warningText;

        if(AdhdAmount > 1.0f || BoringAmount > 1.0f)
        {
            DeltaruneExplosion.ExplodePlayer();
            AdhdAmount = -1.0f;
            BoringAmount = -0.125f;
        }

        lastPos = NewMovement.Instance.transform.position;
    }
}

[PatchOnEntry]
[HarmonyPatch(typeof(NewMovement), "Start")]
public static class BarsInjector_Start
{
    public static void Prefix(NewMovement __instance)
    {
        var warningCanvas = AssetsController.LoadAsset("WarningText");

        HudController hc = __instance.GetComponentInChildren<HudController>(includeInactive: true);
        if (hc != null)
        {
            var gunCanvas = hc.transform.Find("GunCanvas");
            if (gunCanvas != null)
            {
                gunCanvas.transform.localPosition = new Vector3(-1.06f, -0.53f, 1.1f);
                var hud = AssetsController.LoadAsset("FUckingPanel");
                if(hud == null) return;
                hud.transform.SetParent(gunCanvas.transform, worldPositionStays:false);
                var bc = gunCanvas.gameObject.AddComponent<BarsController>();
                bc.container = hud;
                bc.gunPanel = gunCanvas.transform.Find("GunPanel").gameObject;
                bc.warningText = warningCanvas!.GetComponentInChildren<TextMeshProUGUI>(includeInactive:true);
                bc.warningTextCanvas = warningCanvas!.GetComponentInChildren<CanvasGroup>(includeInactive:true);
            }
        }
    }
}
[PatchOnEntry]
[HarmonyPatch(typeof(NewMovement), "Respawn")]
public static class BarsInjector_Respawn
{
    public static void Prefix(NewMovement __instance)
    {
        BarsController.Instance!.AdhdAmount = -1.0f;
        BarsController.Instance!.BoringAmount = -0.125f;
    }
}

[PatchOnEntry]
[HarmonyPatch(typeof(LevelNamePopup), "NameAppear")]
public static class LevelNamePopup_NameAppear
{
    public static void Prefix(LevelNamePopup __instance)
    {
        BarsController.Instance!.BoringActive = true;
    }
}

[PatchOnEntry]
[HarmonyPatch(typeof(ScreenZone), "Update")]
public static class ScreenZone_Update
{
    public static void Prefix(ScreenZone __instance)
    {
        if (__instance.inZone)
            BarsController.Instance!.AdhdAmount = -1.0f;
    }
}