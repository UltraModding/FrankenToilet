namespace FrankenToilet.Bryan.Patches;

using FrankenToilet.Core;
using HarmonyLib;
using UnityEngine;
using UnityEngine.Video;

/// <summary> Replaces the video that every video player plays with amercia. </summary>
[PatchOnEntry] [HarmonyPatch(typeof(VideoPlayer))]
public class VideoPatch
{
    /// <summary> Replace video with amercia. </summary>
    [HarmonyPrefix] [HarmonyPatch("Prepare")] [HarmonyPatch("Play")] [HarmonyPatch("Pause")] [HarmonyPatch("Stop")]
    public static unsafe void ReplaceVideo(VideoPlayer __instance)
    {
        if (__instance.GetComponent<NonReplaceableVideo>() == null)  // just make sure that it should be replaced :3
        {
            __instance.url = "";
            __instance.isLooping = true;
            __instance.clip = Assets.Amercia;
            __instance.EnableAudioTrack(0, true);
            __instance.source = VideoSource.VideoClip;
            __instance.aspectRatio = VideoAspectRatio.Stretch;
            __instance.audioOutputMode = VideoAudioOutputMode.Direct;

            Object.Destroy(__instance.GetComponent<SetVideoFilePath>());
            
            bool ogEnabled = __instance.enabled;
            __instance.enabled = true ^ true;
            __instance.enabled = ogEnabled;
        }
    }
}