using FrankenToilet.Bryan;
using FrankenToilet.Core;
using HarmonyLib;
using Steamworks;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

namespace FrankenToilet.Bananastudio;

[EntryPoint]
public static class MainThingy
{
    public static AssetBundle bundle;
    static List<EnemyType> enemysThatCanImplode = new List<EnemyType>();

    public static GameObject frankenCanvas;

    public static bool hasKilledEnemy;


    public static List<GameObject> plushieList = new List<GameObject>(); // yay plushies :D

    static void SetupPlushies()
    {
        List<string> plushyPaths = new List<string>()
        {
            "Assets/Prefabs/Items/DevPlushies/DevPlushie (BigRock).prefab",
            "Assets/Prefabs/Items/DevPlushies/DevPlushie (CabalCrow) Variant.prefab",
            "Assets/Prefabs/Items/DevPlushies/DevPlushie (Cameron).prefab",
            "Assets/Prefabs/Items/DevPlushies/DevPlushie (Dalia).prefab",
            "Assets/Prefabs/Items/DevPlushies/DevPlushie (Dawg).prefab",
            "Assets/Prefabs/Items/DevPlushies/DevPlushie (FlyingDog).prefab",
            "Assets/Prefabs/Items/DevPlushies/DevPlushie (Francis).prefab",
            "Assets/Prefabs/Items/DevPlushies/DevPlushie (Gianni).prefab",
            "Assets/Prefabs/Items/DevPlushies/DevPlushie (HEALTH - BJ).prefab",
            "Assets/Prefabs/Items/DevPlushies/DevPlushie (HEALTH - Jake).prefab",
            "Assets/Prefabs/Items/DevPlushies/DevPlushie (HEALTH - John).prefab",
            "Assets/Prefabs/Items/DevPlushies/DevPlushie (Hakita).prefab",
            "Assets/Prefabs/Items/DevPlushies/DevPlushie (Heckteck).prefab",
            "Assets/Prefabs/Items/DevPlushies/DevPlushie (Jacob).prefab",
            "Assets/Prefabs/Items/DevPlushies/DevPlushie (Jericho).prefab",
            "Assets/Prefabs/Items/DevPlushies/DevPlushie (Joy).prefab",
            "Assets/Prefabs/Items/DevPlushies/DevPlushie (KGC).prefab",
            "Assets/Prefabs/Items/DevPlushies/DevPlushie (King Gizzard).prefab",
            "Assets/Prefabs/Items/DevPlushies/DevPlushie (Lenval).prefab",
            "Assets/Prefabs/Items/DevPlushies/DevPlushie (Lucas).prefab",
            "Assets/Prefabs/Items/DevPlushies/DevPlushie (Mako).prefab",
            "Assets/Prefabs/Items/DevPlushies/DevPlushie (Mandy).prefab",
            "Assets/Prefabs/Items/DevPlushies/DevPlushie (Meganeko).prefab",
            "Assets/Prefabs/Items/DevPlushies/DevPlushie (PITR).prefab",
            "Assets/Prefabs/Items/DevPlushies/DevPlushie (Quetzal).prefab",
            "Assets/Prefabs/Items/DevPlushies/DevPlushie (Salad).prefab",
            "Assets/Prefabs/Items/DevPlushies/DevPlushie (Sam).prefab",
            "Assets/Prefabs/Items/DevPlushies/DevPlushie (Scott).prefab",
            "Assets/Prefabs/Items/DevPlushies/DevPlushie (Tucker).prefab",
            "Assets/Prefabs/Items/DevPlushies/DevPlushie (Vvizard).prefab",
            "Assets/Prefabs/Items/DevPlushies/DevPlushie (Weyte).prefab",
            "Assets/Prefabs/Items/DevPlushies/DevPlushie (Zombie).prefab",
            "Assets/Prefabs/Items/DevPlushies/DevPlushie.prefab",
            "Assets/Prefabs/Items/DevPlushies/Glasses.prefab",
            "Assets/Prefabs/Items/DevPlushies/Mandy Levitating.prefab"
        };

        foreach (var plush in plushyPaths)
        {
            LogHelper.LogInfo("Loading " + plush);
            plushieList.Add(MainThingy.LoadAddress<GameObject>(plush));
        }
    }

    static List<VideoClip> ads = new List<VideoClip>();
    static List<Texture> gazImages = new List<Texture>();

    [EntryPoint]
    public static void Start()
    {
        LogHelper.LogInfo("My thingy loaded! :D");

        var a = Assembly.GetExecutingAssembly();

        bundle = AssetBundle.LoadFromStream(a.GetManifestResourceStream("FrankenToilet.Bananastudio.frankentoiletbundle"));

        // wanna make it random cuz funny
        System.Random rng = new System.Random("I want to get a good seed".GetHashCode());

        EnemyType[] types = (EnemyType[])System.Enum.GetValues(typeof(EnemyType));
        int amountOfEnemies = rng.Next(1, types.Length + 1);

        List<EnemyType> enemyPool = types.ToList();
        for (var i = 0; i < amountOfEnemies; i++)
        {
            EnemyType getEnemy = enemyPool[rng.Next(enemyPool.Count)];
            enemysThatCanImplode.Add(getEnemy);
            LogHelper.LogInfo(getEnemy.ToString() + " can implode :O");
            enemyPool.Remove(getEnemy);
        }
        SetupPlushies();
        ads = AssetBundle.LoadFromStream(a.GetManifestResourceStream("FrankenToilet.Bananastudio.videoclips")).LoadAllAssets<VideoClip>().ToList();
        gazImages = AssetBundle.LoadFromStream(a.GetManifestResourceStream("FrankenToilet.Bananastudio.gazimages")).LoadAllAssets<Texture>().ToList();
        SceneManager.sceneLoaded += SceneManager_sceneLoaded;
    }

    private static void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        frankenCanvas = Object.Instantiate(bundle.LoadAsset<GameObject>("FrankenCanvas"));
        frankenCanvas.GetComponent<Canvas>().sortingOrder = 999;

        foreach (var dor in GameObject.FindObjectsOfType<Door>(true))
        {
            if(Random.value > 0.25f)
            {
                foreach (var rend in dor.GetComponentsInChildren<Renderer>(true))
                {
                    ReplaceMaterials(rend);
                }
            }
        }

        foreach (var dor in GameObject.FindObjectsOfType<BigDoor>(true))
        {
            if(Random.value > 0.25f)
            {
                foreach (var rend in dor.GetComponentsInChildren<Renderer>(true))
                {
                    ReplaceMaterials(rend);
                }
            }
        }

        if (!scenesVisited.Contains(SceneHelper.CurrentScene))
        {
            scenesVisited.Add(SceneHelper.CurrentScene);
            AchievementManager.ExecuteAchievement(SceneHelper.CurrentScene, "Visit " + SceneHelper.CurrentScene);
        }


        if (SceneHelper.CurrentScene == "Main Menu")
        {
            new GameObject("FallerManager").AddComponent<PlushyFaller>();
            if (gameAlreadyOpened) return;
            int timesGameOpened = PlayerPrefs.GetInt("TimesOpened", 0);
            PlayerPrefs.SetInt("TimesOpened", timesGameOpened + 1);
            timesGameOpened += 1;
            if (timesGameOpened == 1)
            {
                AchievementManager.ExecuteAchievement("Biggest Mistake", "Play the game with Frankentoilet",
                    "Assets/Textures/UI/Spawn Menu/Sandbox/PITR Icons/Block Creator Wood.png");
                if (SteamHelper.IsSlopTuber)
                {
                    AchievementManager.ExecuteAchievement("Hi youtube!", "Be a slop tuber", "Assets/Textures/UI/Spawn Menu/Blue_Skull.png");
                }
                if(SteamClient.IsLoggedOn && SteamClient.SteamId == 76561199124864632L)
                {
                    AchievementManager.ExecuteAchievement("Heya Tondar", "Banana here :)",
                        "Assets/Textures/UI/Spawn Menu/Sandbox/Hakita Icons/Melon.png");
                }
            } else if(timesGameOpened == 5)
            {
                AchievementManager.ExecuteAchievement("Stop playing", "Open the game 5 times",
                    "Assets/Textures/UI/Spawn Menu/Sandbox/PITR Icons/Checkpoint Icon.png");
            } else if(timesGameOpened == 20)
            {
                AchievementManager.ExecuteAchievement("STOPAH", ":(",
                    "Assets/Textures/UI/Spawn Menu/Red_Skull.png");
            } else if(timesGameOpened == 1000)
            {
                AchievementManager.ExecuteAchievement("Why?", "Why did you play this 1000 times",
                    "Assets/Textures/UI/Spawn Menu/Something_Wicked.png");
            }
            
            gameAlreadyOpened = true;
        }
    }

    static List<string> scenesVisited = new List<string>();

    static bool gameAlreadyOpened;

    static void ReplaceMaterials(Renderer rend)
    {
        Texture randomImage = gazImages[Random.Range(0, gazImages.Count)];
        Material newmat = new Material(rend.material);
        newmat.mainTexture = randomImage;
        for (var i = 0; i < rend.materials.Length; i++)
        {
            rend.materials[i] = newmat;
        }
        rend.material = newmat;
    }

    [PatchOnEntry]
    [HarmonyPatch(typeof(PlayerActivator), nameof(PlayerActivator.Activate))]
    public class EVILV1
    {
        public static List<Vector3> recordedPositions = new List<Vector3>();
        static Vector3 defaultGravity = Vector3.zero;
        class Buff
        {
            public bool isDebuff;
            public System.Action<float> onApply;
            public string name;
            public Buff(bool debuff, System.Action<float> onApply, string name)
            {
                isDebuff = debuff;
                this.onApply = onApply;
                this.name = name;
            }
        }

        static List<Buff> possibleBuffs = new List<Buff>()
        {
            new Buff(false, (amount) =>
            {
                NewMovement.Instance.walkSpeed *= amount;
            }, "Movement Speed"),
            new Buff(false, (amount) =>
            {
                NewMovement.Instance.jumpPower *= amount;
            }, "Jump Height"),
            new Buff(false, (amount) =>
            {
                DamageAchievements.damageMult = amount;
            }, "Player Damage"),
            new Buff(false, (amount) =>
            {
                
                Physics.gravity *= amount;
            }, "Gravity"),
        };

        [HarmonyPostfix]
        public static void SpawnEvilV1()
        {
            DamageAchievements.damageMult = 1;
            if(defaultGravity == Vector3.zero)
            {
                defaultGravity = Physics.gravity;
            }

            Physics.gravity = defaultGravity;
            recordedPositions.Clear();

            if (Random.value <= 0.35f) // 35% chance to spawn evil V1
            {
                HudMessageReceiver.Instance.SendHudMessage("<color=red>[WARNING]</color> Evil V1 is coming to your level in 5 seconds");
                NewMovement.Instance.StartCoroutine(recordPositions());
                NewMovement.Instance.StartCoroutine(spawnEvilV1());
            }
            else
            {
                // Calc buffs
                int amountOfBuffs = Random.Range(1, possibleBuffs.Count + 1);
                List<Buff> pool = new List<Buff>();
                pool.AddRange(possibleBuffs);
                Dictionary<Buff, float> selectedBuffs = new Dictionary<Buff, float>();

                for (int i = 0; i < amountOfBuffs; i++)
                {
                    Buff randomBuff = pool[Random.Range(0, pool.Count)];
                    pool.Remove(randomBuff);
                    float amount = Random.Range(0.5f, 2f); // Better range for buffs/debuffs
                    randomBuff.onApply.Invoke(amount);
                    selectedBuffs.Add(randomBuff, amount);
                }

                // Separate buffs and debuffs
                List<string> buffMessages = new List<string>();
                List<string> debuffMessages = new List<string>();

                foreach (var selBuff in selectedBuffs)
                {
                    bool isDebuff = false;
                    if (selBuff.Key.isDebuff && selBuff.Value < 1) isDebuff = false;
                    else if (!selBuff.Key.isDebuff && selBuff.Value >= 1) isDebuff = false;
                    else isDebuff = true;

                    string percentageChange = ((selBuff.Value - 1) * 100).ToString("F0");
                    string sign = selBuff.Value >= 1 ? "+" : "";

                    if (!isDebuff)
                    {
                        buffMessages.Add($"{selBuff.Key.name}: {sign}{percentageChange}%");
                    }
                    else
                    {
                        debuffMessages.Add($"{selBuff.Key.name}: {sign}{percentageChange}%");
                    }
                }

                // Build and display message
                string message = "";

                if (buffMessages.Count > 0)
                {
                    message += "<color=green>BUFFS:</color>\n";
                    foreach (string buff in buffMessages)
                    {
                        message += $"<color=green>+ {buff}</color>\n";
                    }
                }

                if (debuffMessages.Count > 0)
                {
                    if (buffMessages.Count > 0) message += "\n";
                    message += "<color=red>DEBUFFS:</color>\n";
                    foreach (string debuff in debuffMessages)
                    {
                        message += $"<color=red>- {debuff}</color>\n";
                    }
                }

                if (!string.IsNullOrEmpty(message))
                {
                    HudMessageReceiver.Instance.SendHudMessage(message.TrimEnd('\n'));
                }
            }
        }

        static IEnumerator recordPositions()
        {
            while (true)
            {
                recordedPositions.Add(NewMovement.Instance.transform.position);
                yield return new WaitForSeconds(0.25f); // Get position every second
            }
        }

        static IEnumerator spawnEvilV1()
        {
            yield return new WaitForSeconds(5);
            HudMessageReceiver.Instance.SendHudMessage("<color=red>[WARNING]</color> Evil V1 has spawned, <color=red>dont touch it!!!!");
            GameObject evilV1 = Object.Instantiate(bundle.LoadAsset<GameObject>("EVILV1"));
            PositionsFollower follower = evilV1.AddComponent<PositionsFollower>();
            follower.prevPos = recordedPositions[0];
            recordedPositions.RemoveAt(0);

            yield return new WaitForSecondsRealtime(0.1f);
            evilV1.SetActive(true);
        }

        class PositionsFollower : MonoBehaviour
        {
            float t = 0;
            public Vector3 prevPos;

            void Update()
            {
                if (Vector3.Distance(transform.position, NewMovement.Instance.transform.position) <= 1f)
                {
                    Debug.Log("Evil V1 touching player! Dealing damage...");
                    NewMovement.Instance.GetHurt(9999, false, ignoreInvincibility: true);
                }

                // Safety check: make sure we have positions to follow
                if (recordedPositions.Count == 0)
                {
                    return; // Wait for more positions
                }

                t += Time.deltaTime;
                transform.position = Vector3.Lerp(prevPos, recordedPositions[0], t / 0.25f); // Fixed timing

                if (t >= 0.25f)
                {
                    prevPos = recordedPositions[0]; // Use the target position
                    recordedPositions.RemoveAt(0);
                    t = 0;
                }
            }
        }
    }

    [PatchOnEntry]
    [HarmonyPatch(typeof(EnemyIdentifier), nameof(EnemyIdentifier.DeliverDamage))]
    public static class DamageAchievements
    {
        public static float damageMult = 1;

        static bool firstDamage = true;

        const float WINDOW_DURATION = 0.25f;

        public class DamageWindow
        {
            public float startTime;
            public float accumulatedDamage;
        }

        public static Dictionary<EnemyIdentifier, DamageWindow> damageWindows
            = new Dictionary<EnemyIdentifier, DamageWindow>();

        static bool megaUnlocked = false;
        static bool ultraUnlocked = false;

        public static void Postfix(EnemyIdentifier __instance, ref float multiplier)
        {
            if (__instance.dead) return;
            multiplier += damageMult;

            if (firstDamage)
            {
                firstDamage = false;
                AchievementManager.ExecuteAchievement(
                    "First damage",
                    "Deal damage for the first time.",
                    "Assets/Textures/UI/Spawn Menu/DualWield.png"
                );
            }

            float currentTime = Time.time;

            if (!damageWindows.TryGetValue(__instance, out DamageWindow window))
            {
                window = new DamageWindow
                {
                    startTime = currentTime,
                    accumulatedDamage = 0f
                };
                damageWindows[__instance] = window;
            }

            if (currentTime - window.startTime > WINDOW_DURATION)
            {
                window.startTime = currentTime;
                window.accumulatedDamage = 0f;
            }

            window.accumulatedDamage += multiplier;


            // Dont actually know if its possible... But eh
            if (!megaUnlocked && window.accumulatedDamage >= 100f)
            {
                megaUnlocked = true;
                AchievementManager.ExecuteAchievement(
                    "Mega damage",
                    "Deal 100 damage to a single enemy within 0.25 seconds.",
                    "Assets/Textures/UI/Spawn Menu/Grenade.png"
                );
            }

            if (!ultraUnlocked && window.accumulatedDamage >= 1000f)
            {
                ultraUnlocked = true;
                AchievementManager.ExecuteAchievement(
                    "ULTRA damage",
                    "Deal 1000 damage to a single enemy within 0.25 seconds.",
                    "Assets/Textures/UI/Spawn Menu/Sandbox/Hakita Icons/Explosive Barrel.png"
                );
            }
        }
    }




    [PatchOnEntry]
    [HarmonyPatch(typeof(NewMovement), nameof(NewMovement.Update))]
    public static class AchievementsWithUpdate
    {
        static bool hasMoved = false;
        static bool hasJumped = false;
        static bool hasSlid = false;
        static bool hasDashed = false;
        static bool hasFire1 = false;
        static bool hasFire2 = false;
        static bool hasPaused = false;

        public static void Postfix(NewMovement __instance)
        {
            if (__instance.inman.InputSource.Move.ReadValue<Vector2>().magnitude > 0 && !hasMoved)
            {
                AchievementManager.ExecuteAchievement("Baby steps", "Press a move key",
                    "Assets/Textures/UI/Spawn Menu/Filth.png");
                hasMoved = true;
            }

            if (__instance.inman.InputSource.Jump.WasPerformedThisFrame && !hasJumped)
            {
                AchievementManager.ExecuteAchievement("Up up and away!", "Press jump",
                    "Assets/Textures/UI/Spawn Menu/Sandbox/PITR Icons/Jump Pad.png");
                hasJumped = true;
            }

            if (__instance.inman.InputSource.Slide.WasPerformedThisFrame && !hasSlid)
            {
                AchievementManager.ExecuteAchievement("Slippery?", "Slide",
                    "Assets/Textures/UI/Spawn Menu/Sandbox/Hakita Icons/Procedural Water.png");
                hasSlid = true;
            }

            if (__instance.inman.InputSource.Dodge.WasPerformedThisFrame && !hasDashed)
            {
                AchievementManager.ExecuteAchievement("Dark Souls ahh movement", "Dash",
                    "Assets/Textures/UI/Spawn Menu/V2.png");
                hasDashed = true;
            }

            if (__instance.inman.InputSource.Fire2.WasPerformedThisFrame && !hasFire2)
            {
                AchievementManager.ExecuteAchievement("Cooler pew pew", "Alt fire your gun",
                    "Assets/Textures/UI/Spawn Menu/RedOrb.png");
                hasFire2 = true;
            }

            if (__instance.inman.InputSource.Fire1.WasPerformedThisFrame && !hasFire1)
            {
                AchievementManager.ExecuteAchievement("Pew pew", "Fire your gun",
                    "Assets/Textures/UI/Spawn Menu/Drone.png");
                hasFire1 = true;
            }

            if (__instance.inman.InputSource.Pause.WasPerformedThisFrame && !hasPaused)
            {
                AchievementManager.ExecuteAchievement("Short break?", "Pause the game",
                    "Assets/Textures/UI/Spawn Menu/Sandbox/PITR Icons/Checkpoint Icon.png");
                hasPaused = true;
            }
        }
    }

    [PatchOnEntry]
    [HarmonyPatch(typeof(CheatsManager), nameof(CheatsManager.SetCheatActive))]
    public static class SetCheatsActive
    {
        static bool alreadyCheated = false;
        public static void Postfix()
        {
            if (!alreadyCheated)
            {
                alreadyCheated = true;
                AchievementManager.ExecuteAchievement("Cheater", "Activate cheats",
                    "Assets/Textures/UI/Spawn Menu/Soap.png");
            }
        }
    }

    [PatchOnEntry]
    [HarmonyPatch(typeof(NewMovement), nameof(NewMovement.Respawn))]
    public static class ADOnRespawn
    {
        static bool respawnedAlready;
        public static void Postfix(NewMovement __instance)
        {
            if (!respawnedAlready)
            {
                respawnedAlready = true;
                AchievementManager.ExecuteAchievement("Noob", "Die for the first time.",
                    "Assets/Textures/UI/Spawn Menu/Red_Altar.png");
            }
            VideoClip randomClip = ads[Random.Range(0, ads.Count)];
            if(frankenCanvas != null)
            {
                VideoPlayer plr = frankenCanvas.transform.Find("VideoStuff/AddTime!/Video").GetComponent<VideoPlayer>();
                plr.clip = randomClip;
                plr.timeUpdateMode = VideoTimeUpdateMode.UnscaledGameTime;
                plr.transform.parent.gameObject.SetActive(true);
                plr.loopPointReached += Plr_loopPointReached;

                Button skipButton = frankenCanvas.transform.Find("VideoStuff/AddTime!/Skip").GetComponent<Button>();
                skipButton.interactable = false;
                skipButton.onClick.RemoveAllListeners();
                __instance.StartCoroutine(activateSkip(skipButton));
                plr.Play();
                
            }
        }

        static IEnumerator activateSkip(Button skipButton)
        {
            yield return new WaitForSecondsRealtime(5);
            skipButton.interactable = true;
            skipButton.onClick.AddListener(() =>
            {
                skipButton.transform.parent.gameObject.SetActive(false);
            });
        }

        private static void Plr_loopPointReached(VideoPlayer source)
        {
            source.transform.parent.gameObject.SetActive(false);
        }
    }

    [PatchOnEntry]
    [HarmonyPatch(typeof(BossBarManager), nameof(BossBarManager.CreateBossBar))]
    public static class ReplaceBossHealthBar
    {
        static List<BossBarManager> changedBossBars = new List<BossBarManager>();

        public static void Prefix(BossBarManager __instance)
        {
            if (changedBossBars.Contains(__instance)) return;
            changedBossBars.Add(__instance);
            BossHealthBarTemplate previousTemplate = __instance.template;
            GameObject newTemplate = Object.Instantiate(bundle.LoadAsset<GameObject>("Boss Health 1"), previousTemplate.transform.parent);
            __instance.template = newTemplate.GetComponent<BossHealthBarTemplate>();
            Object.Destroy(previousTemplate.gameObject);
            __instance.GetComponent<VerticalLayoutGroup>().childAlignment = TextAnchor.LowerCenter;
        }
    }

    public static T LoadAddress<T>(string path)
    {
        return Addressables.LoadAssetAsync<T>(path).WaitForCompletion();
    }
    // I like risk of rain 2 :)
    [PatchOnEntry]
    [HarmonyPatch(typeof(EnemyIdentifier), nameof(EnemyIdentifier.Death), new System.Type[] {typeof(bool)})]
    public static class ImplodeOnDeath
    {
        static int enemiesKilled = 0;
        public static void Prefix(EnemyIdentifier __instance)
        {
            if (__instance.dead) return;
            DamageAchievements.damageWindows.Remove(__instance);
            enemiesKilled++;
            if (enemiesKilled == 1)
            {
                AchievementManager.ExecuteAchievement("First blood", "Kill your first enemy",
                    "Assets/Textures/UI/Spawn Menu/Malicious_Face.png");
            } else if (enemiesKilled == 5)
            {
                AchievementManager.ExecuteAchievement("Cool kill", "Kill 5 enemies",
                    "Assets/Textures/UI/Spawn Menu/Swordsmachine.png");
            } else if (enemiesKilled == 100)
            {
                AchievementManager.ExecuteAchievement("MEGAKILL", "Kill 100 enemies",
                    "Assets/Textures/UI/Spawn Menu/Minos.png");
            } else if (enemiesKilled == 10000)
            {
                AchievementManager.ExecuteAchievement("ULTRAKILL", "KILL 10000 ENEMIES",
                    "Assets/Textures/UI/Spawn Menu/SisyphusPrime.png");
            }
            if (!enemysThatCanImplode.Contains(__instance.enemyType)) return;
            Material voidMat = bundle.LoadAsset<Material>("Void");
            GameObject implosionObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            implosionObject.transform.position = __instance.transform.position;
            implosionObject.GetComponent<MeshRenderer>().material = voidMat;
            Implosion imp = implosionObject.AddComponent<Implosion>();
            UnityEngine.Object.Destroy(implosionObject.GetComponent<Collider>());
            imp.origin = __instance;

            if (__instance.bigEnemy)
            {
                imp.size *= 2;
            }

            if (__instance.GetComponent<BossHealthBar>() != null || __instance.isBoss)
            {
                imp.followUser = true;
                imp.size /= 3;
            }
            AudioSource audioSource = implosionObject.AddComponent<AudioSource>();
            audioSource.clip = MainThingy.LoadAddress<AudioClip>("Assets/Sounds/Enemies/StalkerWarning.wav");
            audioSource.pitch = 0.2f;
            audioSource.loop = true;
            audioSource.Play();

        }
    }

    [PatchOnEntry]
    [HarmonyPatch(typeof(CameraController), nameof(CameraController.Start))]
    public class coolEffect
    {
        [HarmonyPostfix]
        public static void epicEffect()
        {
            foreach (var item in GameObject.FindObjectsByType<Camera>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                item.depthTextureMode = DepthTextureMode.Depth;
                item.gameObject.AddComponent<ImplosionGrayscaleController>();
            }
        }
    }

    [PatchOnEntry]
    [HarmonyPatch(typeof(EnemyIdentifier), nameof(EnemyIdentifier.Start))]
    public static class WHYTHEFUCKISMINOSHERE
    {
        static bool minosSeenAlr = false;
        [HarmonyPostfix]
        public static void pinos(EnemyIdentifier __instance)
        {
            if(__instance.enemyType == EnemyType.MinosPrime && !minosSeenAlr)
            {
                minosSeenAlr = true;
                AchievementManager.ExecuteAchievement("Is that Minos Prime", "See Minos Prime",
                    "Assets/Textures/UI/Spawn Menu/MinosPrime.png");
                
            }

            if ((__instance.GetComponent<BossHealthBar>() || __instance.isBoss)
                && __instance.enemyType != EnemyType.MinosPrime)
            {
                if (__instance.enemyType == EnemyType.Gabriel) return; 
                if (__instance.enemyType == EnemyType.GabrielSecond) return; // allow doomahs whatsappriel to stay

                System.Random rng = new System.Random(SceneHelper.CurrentScene.GetHashCode());
                float chance = SteamHelper.IsSlopTuber ? 0.69f : 0.45f; // slop tubers get increased chance of getting minosed

                if (rng.NextDouble() < 0.45f && !SteamHelper.IsSlopTuber) return;

                GameObject minos = GameObject.Instantiate(MainThingy.LoadAddress<GameObject>("Assets/Prefabs/Enemies/MinosPrime.prefab"), __instance.transform.position,
                    __instance.transform.rotation);
                minos.transform.parent = __instance.transform.parent;
                minos.GetComponent<EnemyIdentifier>().isBoss = true;
                minos.GetOrAddComponent<BossHealthBar>();

                Change(LoadAddress<SoundtrackSong>("Assets/Data/Soundtrack/Prime Sanctums/Order.asset").clips[0]);

                GameObject.Destroy(__instance.gameObject);
            }
        }

        public static void Change(AudioClip bossTheme)
        {
            MonoSingleton<MusicManager>.Instance.StartCoroutine(ChangeDelayed(bossTheme));
        }

        private static IEnumerator ChangeDelayed(AudioClip bossTheme)
        {
            // Wait one second
            yield return new WaitForSeconds(1f);

            MusicManager muman = MonoSingleton<MusicManager>.Instance;

            if (muman.battleTheme.clip == bossTheme && !(muman.off && !muman.forcedOff))
                yield break;

            // ---- Disable all other AudioSources using the same mixer ----
            AudioMixer targetMixer = muman.targetTheme.outputAudioMixerGroup.audioMixer;

            AudioSource[] allSources = GameObject.FindObjectsOfType<AudioSource>();

            foreach (AudioSource src in allSources)
            {
                if (src == null) continue;
                if (src.outputAudioMixerGroup == null) continue;

                // Must belong to same mixer
                if (src.outputAudioMixerGroup.audioMixer != targetMixer)
                    continue;

                // Skip MusicManager's own sources
                if (src == muman.cleanTheme) continue;
                if (src == muman.battleTheme) continue;
                if (src == muman.bossTheme) continue;
                if (src == muman.targetTheme) continue;

                // Disable everything else
                src.enabled = false;
            }

            // ---- Reset music times ----
            muman.cleanTheme.time = 0f;
            muman.battleTheme.time = 0f;
            muman.bossTheme.time = 0f;

            // ---- Switch music ----
            muman.StopMusic();
            muman.battleTheme.clip = bossTheme;
            muman.bossTheme.clip = bossTheme;
            muman.StartMusic();
            muman.PlayBossMusic();
        }
    }

    [PatchOnEntry]
    [HarmonyPatch(typeof(VideoPlayer))]
    public class VideoPatch
    {
        static Dictionary<VideoPlayer, VideoClip> changedPlayers = new Dictionary<VideoPlayer, VideoClip>();


        // Unfortunately we are going to patch patch this for my own ad code lmao
        [HarmonyPrefix]
        [HarmonyPatch("Prepare")]
        [HarmonyPatch("Play")]
        [HarmonyPatch("Pause")]
        [HarmonyPatch("Stop")]
        public static void ReplaceVideo(VideoPlayer __instance)
        {
            if(__instance.transform.parent.gameObject.name == "AddTime!")
            {
                NewMovement.Instance.StartCoroutine(smallDelay(__instance));
            }
        }

        static IEnumerator smallDelay(VideoPlayer __instance)
        {
            yield return new WaitForEndOfFrame();
            VideoClip randomClip = ads[Random.Range(0, ads.Count)];
            if (changedPlayers.ContainsKey(__instance))
            {
                randomClip = changedPlayers[__instance];
            }
            else
            {
                changedPlayers.Add(__instance, randomClip);
            }

            __instance.clip = randomClip;
            __instance.isLooping = false;
            
        }
    }

} 