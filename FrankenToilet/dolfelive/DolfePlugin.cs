﻿using FrankenToilet.Core;
using HarmonyLib.PatchExtensions;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
// ReSharper disable UnnecessaryWhitespace

namespace FrankenToilet.dolfelive;

[EntryPoint]
// ReSharper disable once ClassNeverInstantiated.Global
public class DolfePlugin
{
    public static DolfeCountdown? countdown = null!;
    public static int? sRankTime = 9999;
    public static GameObject sinGO = null!;
    public static Sin sin = null!;
    // public static GameObject? shaderApplier = null;
    // public static Material? overlayMaterial = null;
    public static GameObject containerInstance = null!;
    public static GameObject textPrefab = null!;
    public static GameObject realSin = null!;
    public static AssetBundle bundle => BundleLoader.bundle;
    
    
    [EntryPoint]
    public static void Awake()
    {
        LogHelper.LogDebug("DolfePlugin Awake");
        Application.runInBackground = true;
        
        if (bundle == null)
        {
            LogHelper.LogError("BundleLoader.bundle is null");
            return;
        }
        
        void PrepareSin()
        {
            sinGO = bundle.LoadAsset<GameObject>("Assets/Sin/Sin.prefab");
            sinGO.transform.localScale = Vector3.one * 5f;
            sin = sinGO.AddComponent<Sin>();
            sin.image = sinGO.GetComponentInChildren<Canvas>().GetComponentInChildren<Image>();
            sin.frames =
            [
                bundle.LoadAsset<Sprite>("Assets/Sin/Frames/sin_frame_0001.png"),
                bundle.LoadAsset<Sprite>("Assets/Sin/Frames/sin_frame_0002.png"),
                bundle.LoadAsset<Sprite>("Assets/Sin/Frames/sin_frame_0003.png"),
                bundle.LoadAsset<Sprite>("Assets/Sin/Frames/sin_frame_0004.png"),
                bundle.LoadAsset<Sprite>("Assets/Sin/Frames/sin_frame_0005.png"),
                bundle.LoadAsset<Sprite>("Assets/Sin/Frames/sin_frame_0006.png"),
                bundle.LoadAsset<Sprite>("Assets/Sin/Frames/sin_frame_0007.png")
            ];
            sin.trailPrefab = bundle.LoadAsset<GameObject>("Assets/Sin/TrailBall.prefab");
            sin.zenRelease = bundle.LoadAsset<AudioClip>("Assets/Sin/ZenRelease.wav");
            sin.actionsNBanger = bundle.LoadAsset<AudioClip>("Assets/Sin/ACTION_&_CONSEQUENCE_-_Grace_OST.mp3");
            
            // overlayMaterial = bundle.LoadAsset<Material>("Assets/Sin/SinAngy/SurfaceBroken.mat");
        }
        PrepareSin();
        
        SceneManager.sceneLoaded += (_, _) =>
        {
            PatchClass.graceTime = 40f;
            Canvas? canvas = GameObject.Find("/Canvas")?.GetComponent<Canvas>();
            if (canvas != null)
            {
                textPrefab = bundle.LoadAsset<GameObject>("Assets/Text (TMP).prefab");
                textPrefab.GetComponent<TextMeshProUGUI>().fontSize = 24;
                
                GameObject container =
                    bundle.LoadAsset<GameObject>("Assets/CountdownContainer.prefab");
                containerInstance = GameObject.Instantiate(container, canvas.transform, false);
                
                RectTransform rt = containerInstance.GetComponent<RectTransform>();
                
                rt.anchorMin = new Vector2(0.5f, 1f);
                rt.anchorMax = new Vector2(0.5f, 1f);
                rt.pivot = new Vector2(0.5f, 0.5f);
                rt.anchoredPosition = new Vector2(0, -40);
                
                RectTransform textRt = textPrefab.GetComponent<RectTransform>();
                textRt.anchorMin = new Vector2(0.5f, 0.5f);
                textRt.anchorMax = new Vector2(0.5f, 0.5f);
                textRt.pivot = new Vector2(0.5f, 0.5f);
                
                countdown = containerInstance.AddComponent<DolfeCountdown>();
                countdown.textPrefab = textPrefab.GetComponent<TextMeshProUGUI>();
                countdown.container = containerInstance.transform;
                countdown.timeRunOutClip = BundleLoader.bundle.LoadAsset<AudioClip>("Assets/timer_runout.wav");
            }
            
            StatsManager sm = StatsManager.Instance;
            if (sm != null)
            {
                sRankTime = sm.timeRanks[3];
                countdown?.countingDown = sm.timer;
            }
            else
            {
                LogHelper.LogError("StatsManager.Instance is null");
            }
        };
    }
    
    public static void SpawnSin()
    {
        if (sinGO == null)
        {
            LogHelper.LogError("sinGO is null, cannot spawn Sin");
            return;
        }
        realSin = GameObject.Instantiate(sinGO);
        realSin.transform.position = (NewMovement.instance?.transform.position ?? Vector3.zero) + new Vector3(0, 350f, 0);
        realSin.GetComponent<Sin>().countdown = countdown;
        
        // isn't working on all objects, so canceled
        // shaderApplier = new GameObject("ShaderApplier");
        // ApplyShader applyShader = shaderApplier.AddComponent<ApplyShader>();
        // applyShader.overlayMaterial = overlayMaterial; 
    }
}

[PatchOnEntry]
public static class PatchClass
{
    public static float graceTime = 40f;
    private const float lessTime = 0f; //115f 
    
    #if DEBUG
    // [Patch(typeof(Bootstrap), "Start", AT.REDIRECT, "SceneHelper.LoadScene", occurrence: 3)]
    public static void SceneRedirect(string sceneName, bool noblocker = false)
    {
        SceneHelper.LoadScene("Level 6-2", noblocker);
    }
    
    // [Patch(typeof(Bootstrap), "Start", AT.REDIRECT, "ILogger::set_filterLogType", occurrence: 0)]
    public static void StopBlockingLogz(ILogger logger, LogType logType)
    {
        LogHelper.LogDebug($"Prevented log suppression: {logType}");
    }
    #endif
    
    [Patch(typeof(StatsManager), "Update", AT.RETURN)]
    public static void SecondsInc(float ___seconds)
    {
        if (DolfePlugin.countdown != null && !DolfePlugin.countdown._sinSpawned)
        {
            // ReSharper disable once RedundantCast
            DolfePlugin.countdown.timeLeft = ((float)(DolfePlugin.sRankTime - lessTime ?? 9999f) - ___seconds) + graceTime;
        }
    }
    
    [Patch(typeof(StatsManager), "StartTimer", AT.RETURN)]
    public static void StartTimerPatch(ref bool ___timer)
    {
        if (SceneHelper.CurrentScene != "uk_construct" && SceneHelper.CurrentScene != "Endless")
        {
            DolfePlugin.countdown?.StartTimer();
            DolfePlugin.countdown?.countingDown = ___timer;
        }
    }
    
    [Patch(typeof(StatsManager), "StopTimer", AT.RETURN)]
    public static void StopTimerPatch(ref bool ___timer)
    {
        if (SceneHelper.CurrentScene != "uk_construct" && SceneHelper.CurrentScene != "Endless")
            DolfePlugin.countdown?.StopTimer();
    }
    
    [Patch(typeof(NewMovement), "Respawn", AT.RETURN)]
    public static void RespawnPatch()
    {
        DolfePlugin.countdown?.StopTimer();
        GameObject.Destroy(DolfePlugin.countdown);
        GameObject.Destroy(DolfePlugin.containerInstance);
        
        Canvas? canvas = GameObject.Find("/Canvas")?.GetComponent<Canvas>();
        GameObject container =
            DolfePlugin.bundle.LoadAsset<GameObject>("Assets/CountdownContainer.prefab");
        DolfePlugin.containerInstance = GameObject.Instantiate(container, canvas?.transform, false);
        
        DolfePlugin.countdown = null;
        DolfePlugin.countdown = DolfePlugin.containerInstance.AddComponent<DolfeCountdown>();
        DolfePlugin.countdown.textPrefab = DolfePlugin.textPrefab.GetComponent<TextMeshProUGUI>();
        DolfePlugin.countdown.container = DolfePlugin.containerInstance.transform;
        DolfePlugin.countdown.timeRunOutClip = BundleLoader.bundle.LoadAsset<AudioClip>("Assets/timer_runout.wav");
        DolfePlugin.countdown.countingDown = true;
        
        GameObject.Destroy(DolfePlugin.realSin);
        DolfePlugin.realSin = null!;
        
        graceTime += 20f;
    }
    
}
