#pragma warning disable CS8618
using FrankenToilet.Core;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


namespace FrankenToilet.somebilly {
    // THIS IS THE INFO TEXT.
    public class GoodInfoText : MonoBehaviour {
        RectTransform rect;
        GameObject lineWelcome; TextMeshProUGUI textWelcome;
        GameObject lineDate; TextMeshProUGUI textDate;
        GameObject lineRandom; TextMeshProUGUI textRandom;
        GameObject lineScene; TextMeshProUGUI textScene;
        float timerSecond = 0f;

        public void Start() {
            VerticalLayoutGroup layout = this.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.spacing = 20f;
            layout.childAlignment = UnityEngine.TextAnchor.UpperCenter;

            // place at the top
            rect = GetComponent<RectTransform>();
            rect.anchorMax = new Vector2(0.5f, 1f);
            rect.anchorMin = new Vector2(0.5f, 1f);
            rect.anchoredPosition = new Vector2(0f, -10f);
            rect.pivot = new Vector2(0.5f, 1f);

            lineWelcome = new GameObject("LineWelcome");
            lineWelcome.transform.SetParent(layout.transform);
            textWelcome = lineWelcome.AddComponent<TextMeshProUGUI>();
            textWelcome.fontSize = 28;
            textWelcome.enableWordWrapping = false;
            textWelcome.horizontalAlignment = TMPro.HorizontalAlignmentOptions.Center;

            lineDate = new GameObject("LineDate");
            lineDate.transform.SetParent(layout.transform);
            GoodSwayer lineDateSwayer = lineDate.AddComponent<GoodSwayer>();
            lineDateSwayer.speed = 0.5f * UnityEngine.Random.Range(2, 10);
            textDate = lineDate.AddComponent<TextMeshProUGUI>();
            textDate.fontSize = 28;
            textDate.enableWordWrapping = false;
            textDate.horizontalAlignment = TMPro.HorizontalAlignmentOptions.Center;

            lineRandom = new GameObject("LineRandom");
            lineRandom.transform.SetParent(layout.transform);
            GoodScaler lineRandomScaler = lineRandom.AddComponent<GoodScaler>();
            lineRandomScaler.speed = 0.5f * UnityEngine.Random.Range(2, 10);
            lineRandomScaler.progress = 4.5f;
            textRandom = lineRandom.AddComponent<TextMeshProUGUI>();
            textRandom.fontSize = 28;
            textRandom.enableWordWrapping = false;
            textRandom.horizontalAlignment = TMPro.HorizontalAlignmentOptions.Center;

            lineScene = new GameObject("LineScene");
            lineScene.transform.SetParent(layout.transform);
            GoodSwayer lineSceneSwayer = lineScene.AddComponent<GoodSwayer>();
            lineSceneSwayer.speed = 0.5f * UnityEngine.Random.Range(2, 10);
            lineSceneSwayer.progress = 2f;
            textScene = lineScene.AddComponent<TextMeshProUGUI>();
            textScene.fontSize = 28;
            textScene.enableWordWrapping = false;
            textScene.horizontalAlignment = TMPro.HorizontalAlignmentOptions.Center;
            UpdateTexts();
        }

        public void Update() {
            timerSecond += Time.deltaTime;
            if (timerSecond >= 1f) {
                timerSecond = 0f;
                UpdateTexts();
            }
        }

        public void UpdateTexts() {
            textWelcome.text = "<b>WELCOME TO FRANKENTOILET. TODAY IS:</b>";
            textDate.text = $"<b>{System.DateTime.Now.ToString("MMMM dd, yyyy")}</b>";
            textRandom.text = $"<b>Random number: {UnityEngine.Random.Range(0, 100001)}</b>";
            textScene.text = $"<b>Current scene: {SceneHelper.CurrentScene}</b>";
        }
    }

    // THIS IS THE SWAYER.
    public class GoodSwayer : MonoBehaviour {
        public float progress = 0f;
        public float speed = 2f;
        public float minAngle = -5f;
        public float maxAngle = 5f;
        public void Update() {
            progress += Time.deltaTime * speed;
            progress = progress % (2f * Mathf.PI);

            float normalizedProgress = 0.5f * (Mathf.Sin(progress) + 1f);
            float newAngle = Mathf.Lerp(minAngle, maxAngle, normalizedProgress);
            this.transform.eulerAngles = new Vector3(0f, 0f, newAngle);
        }
    }

    // THIS IS THE SCALER.
    public class GoodScaler : MonoBehaviour {
        public float progress = 0f;
        public float speed = 2f;
        public float minScale = 0.9f;
        public float maxScale = 1.1f;
        public void Update() {
            progress += Time.deltaTime * speed;
            progress = progress % (2f * Mathf.PI);

            float normalizedProgress = 0.5f * (Mathf.Sin(progress) + 1f);
            float newScale = Mathf.Lerp(minScale, maxScale, normalizedProgress);
            this.transform.localScale = new Vector3(newScale, newScale, newScale);
        }
    }


    // ---------------
    // ----- HUD -----
    // ---------------
    [PatchOnEntry]
    [HarmonyPatch]
    public static class HUDPatch {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(OptionsMenuToManager), "Start")]
        public static void Postfix(OptionsMenuToManager __instance) {
            if (__instance.name != "Canvas") {
                return;
            }
            GameObject info = new GameObject("FrankenInfo");
            info.transform.SetParent(__instance.transform);
            info.transform.SetSiblingIndex(5);
            info.AddComponent<GoodInfoText>();
        }
    }
}