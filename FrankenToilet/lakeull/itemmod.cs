using FrankenToilet.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

/*
 *   --- Public Apology ---
 * Dear all future coders of this project.
 * I deeply regret to inform you that the code in this project is fucking dogshit
 * Please find it in your heart to forgive me.
*/

namespace FrankenToilet.lakeull;

[EntryPoint]
public class ItemModMain
{
    private static GameObject[] packedObjects = [];
    private static AssetBundle? bundle; // change name of "bundled" to the file name of the bundle
    private static string bundlePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "lakeullsfunnybundle");
    public static GameObject itemCanvas;
    public static GameObject itemBox;
    public static GameObject refillSfx;
    public static GameObject useSfx;
    private static GameObject explosionIcon;
    private static GameObject coinIcon;
    private static GameObject iglooIcon;
    private static GameObject airheadsIcon;
    private static GameObject gatoradeIcon;
    private static GameObject orbitIcon;
    private static GameObject donutIcon;
    public static bool canUseItem = false;
    private static List<GameObject> abilityIcons = new List<GameObject>();
    private static int abilityIndex;
    private static int oldRandomGen = 2;
    public static GameObject iglooObject;
    public static GameObject orbitObject;
    public static GameObject donutObject;
    public static GameObject osakaSignObject;

    [EntryPoint]
    public static void Awake()
    {
        // scene
        SceneManager.sceneLoaded += new UnityAction<Scene, LoadSceneMode>(OnSceneLoaded);
        // Plugin startup logic
        LogHelper.LogInfo($"Lakeull's Plugin is loaded!");
        //LogHelper.LogInfo(bundlePath);

        // loads bundle
        LogHelper.LogInfo("[lakeul] Loading assets");
        byte[] data;
        try
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string resourceName = $"FrankenToilet.lakeull.lakeullsfunnybundle";
            var s = assembly.GetManifestResourceStream(resourceName);
            s = s ?? throw new FileNotFoundException($"Could not find embedded resource '{resourceName}'.");
            using var ms = new MemoryStream();
            s.CopyTo(ms);
            data = ms.ToArray();
        }
        catch (Exception ex)
        {
            LogHelper.LogError($"[lakeull] Error loading assets: " + ex.Message);
            return;
        }
        bundle = AssetBundle.LoadFromMemory(data);
    }

    public static void OnSceneLoaded(Scene scene, LoadSceneMode lsm)
    {
        //LogHelper.LogInfo("loading bundle (itemMod) " + bundlePath);
        LoadBundle();
    }

    public static void LoadBundle()
    {
        if(SceneHelper.CurrentScene != "Bootstrap" && SceneHelper.CurrentScene != "Main Menu" && SceneHelper.CurrentScene != "Intro")
        {
            canUseItem = false;
            // i stole this from earthling
            // it loads embed bundles i think

            // load bundle, find the item canvas
            packedObjects = bundle.LoadAllAssets<GameObject>();
            foreach (GameObject gameObject in packedObjects)
            {
                // grabs the name of the specific item I WANT IT!!!!!!!!!!!
                //LogHelper.LogInfo($"{gameObject.name}");
                if ($"{gameObject.name}" == "Item Canvas")
                {
                    itemCanvas = GameObject.Instantiate(gameObject, new Vector3(0, 0, 0), Quaternion.identity);
                }
                else if ($"{gameObject.name}" == "igloo")
                {
                    iglooObject = gameObject;
                }
                else if ($"{gameObject.name}" == "orbit")
                {
                    orbitObject = gameObject;
                }
                else if ($"{gameObject.name}" == "donut")
                {
                    donutObject = gameObject;
                }
                else if ($"{gameObject.name}" == "osakasign")
                {
                    osakaSignObject = gameObject;
                }
                LogHelper.LogInfo($"{gameObject.name}");
            }
            // gets the item box sprite
            itemBox = GameObject.Find(itemCanvas.name + "/Item Box");
            refillSfx = GameObject.Find(itemCanvas.name + "/refill sfx");
            useSfx = GameObject.Find(itemCanvas.name + "/use sfx");

            // determine whether to reposition the item box (broken in frankentoilet, thanks guys)
            if (PrefsManager.Instance.GetInt("weaponHoldPosition") == 2)
            {
                //LogHelper.LogInfo("2");
                itemBox.transform.localPosition = new Vector3(-800, -380);
            }
            else
            {
                itemBox.transform.localPosition = new Vector3(800, -380);
            }
            itemBox.AddComponent<ItemModUpdates>();

            createOsakaSign();
            InitialAssignPower();
            RandomizePower();
        }
    }

    public static void createOsakaSign()
    {
        if (GameObject.FindAnyObjectByType<FirstRoomPrefab>() != null)
        {
            GameObject osakaSignInstance = GameObject.Instantiate(osakaSignObject);
            Vector3 targetLocation = GameObject.FindAnyObjectByType<FirstRoomPrefab>().transform.position;
            Quaternion targetRotation = GameObject.FindAnyObjectByType<FirstRoomPrefab>().transform.rotation;
            //LogHelper.LogInfo("target location: " + targetLocation.ToString());
            osakaSignInstance.transform.position = targetLocation;
            osakaSignInstance.transform.rotation = targetRotation;
            osakaSignInstance.transform.Rotate(0, 90, 0);
            osakaSignInstance.transform.Translate(45, 2, -14f);
        }
    }

    public static void InitialAssignPower()
    {
        // clear the list to not fuck anything up (it took me 3 hours to realize this)
        abilityIcons.Clear();

        // first ability, explode: index 0
        explosionIcon = GameObject.Find(itemCanvas.name + "/" + itemBox.name + "/placeholder 1");
        explosionIcon.SetActive(false);
        abilityIcons.Add(explosionIcon);

        // second ability, big fucking coin: index 1
        coinIcon = GameObject.Find(itemCanvas.name + "/" + itemBox.name + "/placeholder 2");
        coinIcon.SetActive(false);
        abilityIcons.Add(coinIcon);

        // third ability, igloo: index 2
        iglooIcon = GameObject.Find(itemCanvas.name + "/" + itemBox.name + "/iglooicon");
        iglooIcon.SetActive(false);
        abilityIcons.Add(iglooIcon);

        // foruth ability, random buffs: index 3
        airheadsIcon = GameObject.Find(itemCanvas.name + "/" + itemBox.name + "/airheadsicon");
        airheadsIcon.SetActive(false);
        abilityIcons.Add(airheadsIcon);

        // fifth ability, gatorade: index 4
        gatoradeIcon = GameObject.Find(itemCanvas.name + "/" + itemBox.name + "/gatoradeicon");
        gatoradeIcon.SetActive(false);
        abilityIcons.Add(gatoradeIcon);

        // sixth ability, orbicular: index 5
        orbitIcon = GameObject.Find(itemCanvas.name + "/" + itemBox.name + "/orbiticon");
        orbitIcon.SetActive(false);
        abilityIcons.Add(orbitIcon);

        // seventh ability, orbicular: index 5
        donutIcon = GameObject.Find(itemCanvas.name + "/" + itemBox.name + "/donuticon");
        donutIcon.SetActive(false);
        abilityIcons.Add(donutIcon);

        foreach (GameObject ability in abilityIcons)
        {
            ability.AddComponent<HudOpenEffect>();
        }
    }

    public static void RandomizePower()
    {
        // define random
        int randomGen = UnityEngine.Random.Range(0, abilityIcons.Count);
        if (oldRandomGen == randomGen) // checks to see if the ability was the same as last time
        {
            randomGen = UnityEngine.Random.Range(0, abilityIcons.Count);
        }
        oldRandomGen = randomGen;

        // do ability index stuff
        abilityIndex = randomGen;
        abilityIcons[randomGen].SetActive(true);

        // reset canUseItem
        canUseItem = true;
    }

    public static void disableAllIcons()
    {
        foreach (GameObject item in abilityIcons)
        {
            item.SetActive(false);
        }
    }
    public static void usePower()
    {
        // ability 0, kill yourself (self destruct)
        if (abilityIndex == 0)
        {
            // loads explosion, makes it big
            GameObject exploderRef = Addressables.LoadAssetAsync<GameObject>("Assets/Prefabs/Attacks and Projectiles/Explosions/Explosion Malicious Railcannon.prefab").WaitForCompletion();
            GameObject exploder = GameObject.Instantiate(exploderRef);
            exploder.transform.SetPositionAndRotation(NewMovement.Instance.transform.position, new Quaternion(0f, 0f, 0f, 0f));
            exploder.transform.Find("Sphere_8").GetComponent<Explosion>().maxSize = 100;
            exploder.transform.Find("Sphere_8").GetComponent<Explosion>().speed = 15;
            exploder.transform.Find("Sphere_8").GetComponent<Explosion>().playerDamageOverride = 1;
            exploder.transform.localScale = new Vector3(3f, 3f, 3f);
            GameObject.Instantiate(exploder);
        }
        // ability 1, big fucking coin
        else if (abilityIndex == 1)
        {
            // loads coin, makes it big, makes it do crazy fucking wicked damage
            GameObject bigcoinaddress = Addressables.LoadAssetAsync<GameObject>("Assets/Prefabs/Attacks and Projectiles/Coin.prefab").WaitForCompletion();
            for(var i = 0; i < 4; i++)
            {
                GameObject bigcoin = GameObject.Instantiate(bigcoinaddress);
                bigcoin.transform.SetPositionAndRotation(NewMovement.Instance.transform.position, new Quaternion(0f, 0f, 0f, 0f));
                bigcoin.transform.GetComponent<Rigidbody>().useGravity = false;
                bigcoin.transform.GetComponent<Rigidbody>().velocity = new Vector3(0f, 5f, 0f);
                bigcoin.transform.localScale = new Vector3(30f, 30f, 30f);
                bigcoin.transform.Translate(new Vector3(0f, 3f, 0f));
                bigcoin.AddComponent<AlwaysLookAtCamera>();
                bigcoin.GetComponent<Coin>().power = 30;
            }
        }
        // ability 2, igloo
        else if (abilityIndex == 2)
        {
            if (GameObject.Find("igloo(Clone)") == null)
            {
                // make new igloo
                GameObject iglooObjectInstance = GameObject.Instantiate(iglooObject);
                iglooObjectInstance.transform.position = NewMovement.Instance.transform.position;
            }
            else
            {
                // set position of existing igloo to player location if its alr present
                GameObject.Find("igloo(Clone)").transform.position = NewMovement.Instance.transform.position;
            }
        }
        // ability 3, random buff
        else if (abilityIndex == 3)
        {
            // grabs a random status effect, 33% chance
            int buffRandomIndex = UnityEngine.Random.Range(0, 3);
            //LogHelper.LogInfo("airhead index : " + buffRandomIndex);
            if (buffRandomIndex == 0) // extra speed
            {
                NewMovement.Instance.transform.GetComponent<NewMovement>().walkSpeed *= 2;
            }
            if (buffRandomIndex == 1) // extra jump
            {
                NewMovement.Instance.transform.GetComponent<NewMovement>().jumpPower *= 2;
            }
            if (buffRandomIndex == 2) // extra hp
            {
                NewMovement.Instance.transform.GetComponent<NewMovement>().hp *= 2;
            }
        }
        // ability 4, gatorade
        else if (abilityIndex == 4)
        {
            float bigOrSmall = UnityEngine.Random.Range(0, 3);
            float sizeRandomIndex;
            if(bigOrSmall == 0)
            {
                sizeRandomIndex = UnityEngine.Random.Range(.01f, .75f);
            } else if(bigOrSmall == 1)
            {
                sizeRandomIndex = UnityEngine.Random.Range(1f, 2.5f);
            } else
            {
                sizeRandomIndex = UnityEngine.Random.Range(.5f, 1.5f);
            }
            NewMovement.instance.transform.localScale = new Vector3(sizeRandomIndex, sizeRandomIndex, sizeRandomIndex);
            NewMovement.instance.transform.Find("Main Camera").localScale = new Vector3(1 / sizeRandomIndex, 1 / sizeRandomIndex, 1 / sizeRandomIndex); // inverse of the size
            NewMovement.instance.transform.Find("SlopeCheck").localScale = new Vector3(1 / sizeRandomIndex, 1 / sizeRandomIndex, 1 / sizeRandomIndex); // inverse of the size
            NewMovement.instance.transform.Find("GroundCheck").localScale = new Vector3(.8f / sizeRandomIndex, .8f / sizeRandomIndex, .85f / sizeRandomIndex); // inverse of the size
            NewMovement.instance.transform.Find("Main Camera").GetComponent<Camera>().nearClipPlane = 0.0001f;
            NewMovement.instance.transform.Find("Main Camera").GetComponent<Camera>().farClipPlane = 10000f;
            if (sizeRandomIndex > 1)
            {
                // makes player bigger if the player will get bigger to make sure the player can touch the ground
                NewMovement.instance.transform.Find("SlopeCheck").localScale = new Vector3(1 * sizeRandomIndex, 1 * sizeRandomIndex, 1 * sizeRandomIndex);
                NewMovement.instance.transform.Find("GroundCheck").localScale = new Vector3(.8f * sizeRandomIndex, .8f * sizeRandomIndex, .85f * sizeRandomIndex);
            }
        }
        // ability 5, orbitular dog ( black hole)
        else if (abilityIndex == 5)
        {
            GameObject orbitObjectInstance = GameObject.Instantiate(orbitObject);
            orbitObjectInstance.AddComponent<SphereForce>().strength = 4000;
            orbitObjectInstance.AddComponent<AlwaysLookAtCamera>();
            orbitObjectInstance.transform.position = NewMovement.Instance.transform.position;
        }
        // ability 6, mmmmm... donut
        else if (abilityIndex == 6)
        {
            GameObject donutInstance = GameObject.Instantiate(donutObject);
            GameObject donutInstanceCollider = donutInstance.transform.Find("donutcollider").gameObject;
            donutInstanceCollider.AddComponent<JumpPad>();
            donutInstanceCollider.GetComponent<JumpPad>().force = 80;
            AudioSource audioSourceDonut = donutInstance.transform.Find("donutsound").GetComponent<AudioSource>();
            donutInstanceCollider.GetComponent<JumpPad>().aud = audioSourceDonut;
            donutInstanceCollider.GetComponent<JumpPad>().launchSound = audioSourceDonut.clip;
            donutInstanceCollider.GetComponent<JumpPad>().lightLaunchSound = audioSourceDonut.clip;
            donutInstanceCollider.GetComponent<JumpPad>().origPitch = 1;
            donutInstance.transform.position = new Vector3(NewMovement.Instance.transform.position.x, NewMovement.Instance.transform.position.y - 2, NewMovement.Instance.transform.position.z);
        }

        // plays the use sound effect
        useSfx.GetComponent<AudioSource>().Play();
    }
}

[EntryPoint]
public class ItemModUpdates : MonoBehaviour
{
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.C) && ItemModMain.canUseItem == true && SceneHelper.CurrentScene != "Bootstrap" && SceneHelper.CurrentScene != "Main Menu" && SceneHelper.CurrentScene != "Intro")
        {
            //LogHelper.LogInfo("using power.");
            ItemModMain.usePower();
            ItemModMain.disableAllIcons();
            StartCoroutine(Cooldown());// counts for 30 seconds
        }
    }
    public static IEnumerator Cooldown()
    {
        ItemModMain.canUseItem = false;
        yield return new WaitForSeconds(10);
        ItemModMain.RandomizePower();
        // play the refill sound effect
        ItemModMain.refillSfx.GetComponent<AudioSource>().Play();
    }
}