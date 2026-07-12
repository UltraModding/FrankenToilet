using FrankenToilet.Core;
using FrankenToilet.duviz.commands;
using GameConsole;
using HarmonyLib;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace FrankenToilet.duviz;

public class Meow : MonoBehaviour
{
    public static Meow meow;

    float damageTimer = 0;
    float damageCooldown = 0.05f;

    public void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoad;
        meow = this;
    }

    public void Start()
    {
        Console.instance.RegisterCommand(new NotFoundReplace());

        AudioConfiguration config = AudioSettings.GetConfiguration();
        config.numRealVoices = 128 * 4;
        config.numVirtualVoices = 128 * 4;
        AudioSettings.Reset(config);
    }

    public void OnSceneLoad(Scene s, LoadSceneMode _)
    {
        if (SceneHelper.CurrentScene == "Level 5-4")
            StartCoroutine(WaitForLoad("5-4"));
        if (SceneHelper.CurrentScene == "Level 5-3")
            StartCoroutine(WaitForLoad("5-3"));
        if (SceneHelper.CurrentScene == "Level 5-2")
            StartCoroutine(WaitForLoad("5-2"));
        if (SceneHelper.CurrentScene == "Level 5-1")
            StartCoroutine(WaitForLoad("5-1"));
        if (SceneHelper.CurrentScene == "Level 4-4")
            StartCoroutine(WaitForLoad("4-4"));
        if (SceneHelper.CurrentScene == "Level 1-4")
            StartCoroutine(WaitForLoad("1-4"));
        if (SceneHelper.CurrentScene == "Level 2-1")
            StartCoroutine(WaitForLoad("2-1"));
        if (SceneHelper.CurrentScene == "Level 1-S")
            StartCoroutine(WaitForLoad("1-S"));
    }

    public IEnumerator WaitForLoad(string s)
    {
        while (SceneHelper.PendingScene != null) yield return null;
        yield return null;
        if (s == "5-4")
        {
            Destroy(GameObject.Find("Underwater").transform.Find("UnderwaterWater").gameObject);
            var objs = GameObject.FindObjectsOfType<GameObject>(true);
            foreach (var obj in objs)
            {
                if (obj.name == "Surface")
                {
                    GameObject NonStuff = obj.transform.Find("Nonstuff").gameObject;
                    GameObject Stuff = obj.transform.Find("Stuff").gameObject;
                    GameObject StuffC = obj.transform.Find("Stuff(Clone)").gameObject;

                    ReplaceMaterials(NonStuff.transform.Find("SeaSurface"));
                    ReplaceMaterials(Stuff.transform.Find("Watersurface"));
                    ReplaceMaterials(Stuff.transform.Find("Watersurface (Sunken)"));
                    ReplaceMaterials(StuffC.transform.Find("Watersurface"));
                    ReplaceMaterials(StuffC.transform.Find("Watersurface (Sunken)"));

                    Destroy(Stuff.transform.Find("Boss/Leviathan/SplashBig"));
                    Destroy(StuffC.transform.Find("Boss/Leviathan/SplashBig"));
                    Stuff.transform.Find("Boss/Leviathan").GetComponent<LeviathanController>().bigSplash = null;
                    StuffC.transform.Find("Boss/Leviathan").GetComponent<LeviathanController>().bigSplash = null;
                }
            }
        }
        else if (s == "5-3")
        {
            NewMovement.instance.gameObject.AddComponent<Lois>();

            var objs = GameObject.FindObjectsOfType<GameObject>(true);
            foreach (var obj in objs)
            {
                if (obj.name == "Sea" && obj.transform.parent == null)
                    ReplaceMaterials(obj.transform);
            }
        }
        else if (s == "5-2")
        {
            var objs = GameObject.FindObjectsOfType<GameObject>(true);
            foreach (var obj in objs)
            {
                if (obj.name == "Sea" && obj.transform.parent == null)
                    ReplaceMaterials(obj.transform, "Environment/Layer 5/CaveRock1");

                if (obj.name == "DeathZone (Enemies)" || obj.name == "HurtZone (Player)")
                    Destroy(obj);

                if (obj.name == "Sea Itself")
                    Destroy(obj.GetComponent<SeaBodies>());

                if (obj.name == "WaterTrigger")
                {
                    Destroy(obj.GetComponent<Water>());
                    obj.GetComponent<Collider>().isTrigger = false;
                    obj.layer = LayerMask.NameToLayer("Outdoors");
                }
            }
        }
        else if (s == "5-1")
        {
            var objs = GameObject.FindObjectsOfType<GameObject>(true);
            foreach (var obj in objs)
            {
                if (obj.name == "WatersDisabler" && obj.transform.parent == null)
                {
                    obj.transform.Find("1").gameObject.SetActive(true);
                    obj.transform.Find("1").Find("2").gameObject.SetActive(true);
                    obj.transform.Find("1").Find("2").Find("3").gameObject.SetActive(true);
                    obj.transform.Find("1").Find("2").Find("3").Find("GameObject").gameObject.SetActive(true);
                }
            }
        }
        else if (s == "4-4")
        {
            StockMapInfo.Instance.levelName = "404: NOT FOUND";
            LevelNamePopup.Instance.Invoke("Start", 0);
            ReplaceEverything();
        }
        else if (s == "1-4")
        {
            var root = SceneManager.GetActiveScene().GetRootGameObjects();
            foreach (var obj in root)
            {
                if (obj.name == "V2 - Arena")
                {
                    GameObject Stuff = obj.transform.Find("V2 Stuff").gameObject;
                    GameObject StuffC = obj.transform.Find("V2 Stuff(Clone)").gameObject;
                    Stuff.transform.Find("V2").Find("v2_combined").localScale += new Vector3(30, 0, 0);
                    StuffC.transform.Find("V2").Find("v2_combined").localScale += new Vector3(10, 0, 0);
                }
                if (obj.name == "Music - Versus")
                {
                    obj.GetComponent<AudioSource>().clip = Bundle.bundle.LoadAsset<AudioClip>("wide");
                }
            }
        }
        else if (s == "2-1")
        {
            RenderSettings.skybox.mainTexture = Bundle.bundle.LoadAsset<Texture>("TransLustSky");
        }
        else if (s == "1-S")
        {
            var objs = GameObject.FindObjectsOfType<GameObject>(true);
            var puz = GameObject.FindObjectsOfType<PuzzleController>(true);
            foreach (var obj in objs)
            {
                if (obj.name == "GlobalLights")
                    foreach (Transform tr in obj.transform)
                        tr.gameObject.SetActive(false);
                if (obj.name == "Point Light (5)")
                    obj.GetComponent<Light>().intensity /= 5;
            }
            foreach (var c in puz)
                foreach (Transform cc in c.transform)
                    if (!cc.gameObject.activeSelf)
                        cc.gameObject.SetActive(UnityEngine.Random.Range(0, 2) == 0);
            ReplaceEverything();
        }
    }

    public static void ReplaceEverything(bool includeActive = true)
    {
        var root = SceneManager.GetActiveScene().GetRootGameObjects();
        foreach (var obj in root)
        {
            if (!obj.activeSelf && !includeActive) continue;
            if (obj.name == "Player" || obj.name == "Canvas") continue;
            ReplaceMaterials(obj.transform, Bundle.bundle.LoadAsset<Material>("404"));
        }
    }

    static void ReplaceMaterials(Transform tr, string path = "Environment/Layer 4/SandLarge")
    {
        foreach (Transform c in tr)
        {
            if (c.childCount > 0)
                ReplaceMaterials(c, path);
            if (c.name == "Lightning (1)" || c.GetComponent<ParticleSystem>() != null) continue;
            c.GetComponent<Renderer>()?.material = new Material(Plugin.Ass<Material>($"Assets/Materials/{path}.mat"));
            if (c.GetComponent<ScrollingTexture>() != null)
                Destroy(c.GetComponent<ScrollingTexture>());
        }
    }

    static void ReplaceMaterials(Transform tr, Material mat)
    {
        foreach (Transform c in tr)
        {
            if (c.childCount > 0)
                ReplaceMaterials(c, mat);
            if (c.name == "Lightning (1)" || c.GetComponent<ParticleSystem>() != null) continue;
            c.GetComponent<Renderer>()?.material = new Material(mat);
            if (c.GetComponent<ScrollingTexture>() != null)
                Destroy(c.GetComponent<ScrollingTexture>());
        }
    }

    public void Update()
    {
        if (NewMovement.instance == null) return;

        if (NewMovement.instance.touchingWaters.Count > 0)
        {
            damageTimer += Time.deltaTime;
            if (damageTimer >= damageCooldown)
            {
                damageTimer -= damageCooldown;
                NewMovement.instance.GetHurt(1, false);
            }
        }
    }
}

[PatchOnEntry]
[HarmonyPatch(typeof(LeviathanController))]
public static class LeviathanPatch
{
    [HarmonyPrefix]
    [HarmonyPatch("Awake")]
    public static void Prefix(LeviathanController __instance)
    {
        __instance.gameObject.GetComponent<EnemyIdentifier>().sandified = true;
        if (SteamHelper.IsSlopTuber)
            __instance.gameObject.GetComponent<EnemyIdentifier>().radianceTier = float.PositiveInfinity;
    }
}

[PatchOnEntry]
[HarmonyPatch(typeof(HookPoint))]
public static class HookPointPatch
{
    [HarmonyPrefix]
    [HarmonyPatch("Start")]
    public static void Prefix(HookPoint __instance)
    {
        if (__instance.type != hookPointType.Switch
            && __instance.onHook.toDisActivateObjects.Length == 0
            && __instance.onHook.toActivateObjects.Length == 0
            && __instance.onReach.toActivateObjects.Length == 0
            && __instance.onReach.toDisActivateObjects.Length == 0
            && __instance.onUnhook.toActivateObjects.Length == 0
            && __instance.onUnhook.toDisActivateObjects.Length == 0
            )
            GameObject.Destroy(__instance.gameObject);
    }
}

[PatchOnEntry]
[HarmonyPatch(typeof(SisyphusPrime))]
public static class SisyphusPrimePatch
{
    [HarmonyPrefix]
    [HarmonyPatch("Awake")]
    public static void Prefix(SisyphusPrime __instance)
    {
        GunControl.instance.NoWeapon();
    }
    [HarmonyPrefix]
    [HarmonyPatch("Update")]
    public static void PrefixUpdate(SisyphusPrime __instance)
    {
        GunControl.instance.NoWeapon();
    }
}

[PatchOnEntry]
[HarmonyPatch(typeof(SisyphusPrimeIntro))]
public static class SisyphusPrimeIntroPatch
{
    [HarmonyPrefix]
    [HarmonyPatch("Start")]
    public static void Prefix(SisyphusPrimeIntro __instance)
    {
        GunControl.instance.NoWeapon();
        HudMessageReceiver.instance.SendHudMessage("Fuck you no weapons :3");
    }
}

[PatchOnEntry]
[HarmonyPatch(typeof(LevelSelectPanel))]
public static class LevelSelectPanelPatch
{
    [HarmonyPostfix]
    [HarmonyPatch("Setup")]
    public static void Prefix(LevelSelectPanel __instance)
    {
        if (__instance.name == "4-4 Panel")
        {
            __instance.transform.Find("Image").GetComponent<Image>().sprite = Bundle.bundle.LoadAsset<Sprite>("404 thumbnail"); ;
        }
    }
}

[PatchOnEntry]
[HarmonyPatch(typeof(TextMeshProUGUI))]
public static class TMP_TextPatch
{
    [HarmonyPrefix]
    [HarmonyPatch("Awake")]
    public static void Prefix(TextMeshProUGUI __instance)
    {
        if (__instance.text != null)
            if (__instance.text.ToUpper() == "4-4: CLAIR DE SOLEIL")
                __instance.text = "404: NOT FOUND";
    }
}

[PatchOnEntry]
[HarmonyPatch(typeof(Revolver))]
[HarmonyPatch("Shoot")]
public class LookThisPatchIsNeeded
{
    public static void Postfix(int shotType = 1)
    {
        CameraController.Instance.GetComponent<Camera>().fieldOfView = 10;
        Meow.meow.StartCoroutine(CameraFov());
    }

    public static IEnumerator CameraFov()
    {
        yield return null;
        CameraController.Instance.GetComponent<Camera>().fieldOfView = 10;
    }
}
