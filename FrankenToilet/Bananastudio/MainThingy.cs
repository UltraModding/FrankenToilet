using FrankenToilet.Bryan;
using FrankenToilet.Core;
using HarmonyLib;
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

        if (SceneHelper.CurrentScene == "Main Menu")
        {
            new GameObject("FallerManager").AddComponent<PlushyFaller>();
            if (gameAlreadyOpened) return;
            int timesGameOpened = PlayerPrefs.GetInt("TimesOpened", 0);
            PlayerPrefs.SetInt("TimesOpened", timesGameOpened + 1);
            timesGameOpened += 1;
            if (timesGameOpened == 1)
            {
                AchievementManager.ExecuteAchievement("Biggest Mistake", "Play the game with Frankentoilet");
                if (SteamHelper.IsSlopTuber)
                {
                    AchievementManager.ExecuteAchievement("Hi youtube!", "Be a slop tuber");
                }
            } else if(timesGameOpened == 5)
            {
                AchievementManager.ExecuteAchievement("Stop playing", "Open the game 5 times");
            } else if(timesGameOpened == 20)
            {
                AchievementManager.ExecuteAchievement("STOPAH", ":(");
            } else if(timesGameOpened == 1000)
            {
                AchievementManager.ExecuteAchievement("Why?", "Why did you play this 1000 times");
            }
            
            gameAlreadyOpened = true;
        }
    }
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

        [HarmonyPostfix]
        public static void SpawnEvilV1()
        {
            recordedPositions.Clear();
            if(Random.value <= 0.35f) // 35% chance to spawn evil V1
            {
                HudMessageReceiver.Instance.SendHudMessage("<color=red>[WARNING]</color> Evil V1 is coming to your level in 5 seconds");
                NewMovement.Instance.StartCoroutine(recordPositions());
                NewMovement.Instance.StartCoroutine(spawnEvilV1());
            }
        }

        static IEnumerator recordPositions()
        {
            while (true)
            {
                recordedPositions.Add(NewMovement.Instance.transform.position);
                yield return new WaitForSeconds(1f); // Get position every second
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
                t += Time.deltaTime;

                transform.position = Vector3.Lerp(prevPos, recordedPositions[0], t);

                if(t >= 1)
                {
                    prevPos = transform.position;
                    t = 0;
                    recordedPositions.RemoveAt(0);
                }

                if(Vector3.Distance(transform.position, NewMovement.Instance.transform.position) <= 0.1f)
                {
                    NewMovement.Instance.GetHurt(9999, false, ignoreInvincibility: true);
                }
            }
        }
    }

    [PatchOnEntry]
    [HarmonyPatch(typeof(EnemyIdentifier), nameof(EnemyIdentifier.DeliverDamage))]
    public static class DamageAchievements
    {
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

        public static void Postfix(EnemyIdentifier __instance, float multiplier)
        {
            if (__instance.dead) return;
            if (firstDamage)
            {
                firstDamage = false;
                AchievementManager.ExecuteAchievement(
                    "First damage",
                    "Deal damage for the first time."
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
                    "Deal 100 damage to a single enemy within 0.25 seconds."
                );
            }

            if (!ultraUnlocked && window.accumulatedDamage >= 1000f)
            {
                ultraUnlocked = true;
                AchievementManager.ExecuteAchievement(
                    "ULTRA damage",
                    "Deal 1000 damage to a single enemy within 0.25 seconds."
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

        public static void Postfix(NewMovement __instance)
        {
            if (__instance.inman.InputSource.Move.ReadValue<Vector2>().magnitude > 0 && !hasMoved)
            {
                AchievementManager.ExecuteAchievement("Baby steps", "Press a move key");
                hasMoved = true;
            }

            if (__instance.inman.InputSource.Jump.WasPerformedThisFrame && !hasJumped)
            {
                AchievementManager.ExecuteAchievement("Up up and away!", "Press jump");
                hasJumped = true;
            }

            if (__instance.inman.InputSource.Slide.WasPerformedThisFrame && !hasSlid)
            {
                AchievementManager.ExecuteAchievement("Slippery?", "Slide");
                hasSlid = true;
            }

            if (__instance.inman.InputSource.Dodge.WasPerformedThisFrame && !hasDashed)
            {
                AchievementManager.ExecuteAchievement("Dark Souls ahh movement", "Dash");
                hasDashed = true;
            }

            if (__instance.inman.InputSource.Dodge.WasPerformedThisFrame && !hasFire2)
            {
                AchievementManager.ExecuteAchievement("Cooler pew pew", "Alt fire your gun");
                hasFire2 = true;
            }

            if (__instance.inman.InputSource.Dodge.WasPerformedThisFrame && !hasFire1)
            {
                AchievementManager.ExecuteAchievement("Pew pew", "Fire your gun");
                hasFire1 = true;
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
                AchievementManager.ExecuteAchievement("Noob", "Die for the first time.");
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
        public static void Prefix(EnemyIdentifier __instance)
        {
            if (__instance.dead) return;
            DamageAchievements.damageWindows.Remove(__instance);
            if (!hasKilledEnemy)
            {
                hasKilledEnemy = true;
                AchievementManager.ExecuteAchievement("First blood", "Kill your first enemy");
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
                AchievementManager.ExecuteAchievement("Is that Minos Prime", "See Minos Prime");
                
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
                if (ads.Contains(__instance.clip)) return;


                NewMovement.Instance.StartCoroutine(smallDelay(__instance));
            }
        }

        static IEnumerator smallDelay(VideoPlayer __instance)
        {
            yield return new WaitForEndOfFrame();
            VideoClip randomClip = ads[Random.Range(0, ads.Count)];
            __instance.clip = randomClip;
            __instance.isLooping = false;
        }
    }

} 