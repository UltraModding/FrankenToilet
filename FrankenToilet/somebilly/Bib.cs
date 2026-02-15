#pragma warning disable CS8618
using FrankenToilet.Core;
using BepInEx;
using UnityEngine;
using UnityEngine.Networking;
using System.Reflection;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace FrankenToilet.somebilly {
    // THIS IS THE BIB.
    public class Bib : MonoBehaviour {
        public static Bib Instance = null;

        public static AssetBundle Assets;
        public static Texture2D ConfettiTexture;
        public static Dictionary<string, AudioClip> Voices = new Dictionary<string, AudioClip>();
        
        public static AssetBundle LoadBundle(string resourceName) {
            Assembly assembly = Assembly.GetExecutingAssembly();
            using Stream stream = assembly.GetManifestResourceStream(resourceName);
            if (stream == null) {
                LogHelper.LogError($"Embedded resource '{resourceName}' not found");
                return null;
            }
            
            var resourceStream = assembly.GetManifestResourceStream(resourceName);
            return AssetBundle.LoadFromStream(resourceStream);
        }

        public void ObtainVoicesFromAssetsFolder(string folderPath)
        {
            foreach (var path in Assets.GetAllAssetNames().Where(static p => p.StartsWith("Assets/Bib/Voices")))
                RegisterVoice(path);
        }

        public static void RegisterVoice(string path) {
            int idx = path.LastIndexOf('/');

            if (idx == -1)
                return;
            string clipName = path[(idx + 1)..].Split(".")[0];
            AudioClip clip = Assets.LoadAsset<AudioClip>(path);
            clip.name = clipName;
            Voices[clipName] = clip;
        }

        public void Start() {
            MegaAss();
        }

        public void MegaAss()
        {
            Assets = LoadBundle("FrankenToilet.somebilly.assets.bundle");
            ConfettiTexture = Assets.LoadAsset<Texture2D>("Assets/Bib/CONFETTI.png");
            ObtainVoicesFromAssetsFolder("Assets/Bib/Voices");
        }

        // minotaur is added twice because
        public void AddAndPlayVoice(GameObject obj, AudioClip voice, float volume = 0.65f) {
            if (voice == null) {
                return;
            }

            if (Bib.CheckForVoiceObject(obj, voice.name)) {
                return;
            }

            GameObject voiceObject = new GameObject("FrankenVoiceObject");
            voiceObject.transform.SetParent(obj.transform);
            AudioSource audio = voiceObject.AddComponent<AudioSource>();
            audio.clip = voice;
            audio.volume = volume;
            audio.PlayDelayed(UnityEngine.Random.Range(0.0f, 0.2f));

            StartCoroutine(WaitAndMute(audio));
        }

        public static bool CheckForVoiceObject(GameObject obj, string voiceName) {
            Transform tryFind = obj.transform.Find("FrankenVoiceObject");
            if (tryFind == null) {
                return false;
            }
            AudioSource tryAudio = tryFind.GetComponent<AudioSource>();
            if (tryAudio == null) {
                return false;
            }
            if (tryAudio.clip.name == voiceName) {
                return true;
            }
            return false;
        }

        public static IEnumerator WaitAndMute(AudioSource audio) {
            while (audio && audio.isPlaying) {
                yield return null;
            }
            if (audio) {
                audio.mute = true;
            }
        }
    }
}