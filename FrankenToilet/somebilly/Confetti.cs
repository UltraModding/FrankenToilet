#pragma warning disable CS8618
using FrankenToilet.Core;
using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using UnityObject = UnityEngine.Object;
using UnityEngine.SceneManagement;
using System.Linq;


namespace FrankenToilet.somebilly {
    // --------------------
    // ----- CONFETTI -----
    // --------------------
    public class Confetti {
        public static Transform GetCanvas() {
            Scene activeScene = SceneManager.GetActiveScene();
            Transform canvas = (from obj in activeScene.GetRootGameObjects()
                where obj.name == "Canvas"
                select obj).First().transform;
            return canvas;
        }

        public static GameObject DoConfetti() {
            Transform canvas = Confetti.GetCanvas();

            if (canvas == null) {
                return null;
            }


            GameObject confettiObject = new GameObject("Confetti");
            confettiObject.transform.SetParent(canvas);
            // confettiObject.transform.localPosition = new Vector3(0, 0, 0);
            confettiObject.transform.localScale = new Vector3(1, 1, 1);
            Vector3 pos = confettiObject.transform.position;

            Image image = confettiObject.AddComponent<Image>();
            image.sprite = Sprite.Create(Bib.ConfettiTexture, new Rect(0, 0, Bib.ConfettiTexture.width, Bib.ConfettiTexture.height), new Vector2(0.5f, 0.5f));

            RectTransform rect = confettiObject.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            RectTransform canvasRect = canvas.GetComponent<RectTransform>();
            rect.anchoredPosition = new Vector2(0, canvasRect.sizeDelta.y);

            confettiObject.AddComponent<RemoveAfter>();
            ConfettiFall fall = confettiObject.AddComponent<ConfettiFall>();
            fall.speed = UnityEngine.Random.Range(0.75f, 1.25f) * Screen.height;

            return confettiObject;
        }
    }

    public class RemoveAfter : MonoBehaviour {
        public float time = 4f;
        public float currentTime = 0f;
        void Update() {
            currentTime += Time.deltaTime;
            if (currentTime >= time) {
                UnityObject.Destroy(this.gameObject);
            }
        }
    }

    public class ConfettiFall : MonoBehaviour {
        public float speed = 100f;
        void Update() {
            Vector3 pos = this.transform.position;
            this.transform.position = new Vector3(pos.x, pos.y - speed * Time.deltaTime, pos.z);
        }
    }

    [PatchOnEntry]
    [HarmonyPatch]
    public static class ConfettiPatch {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(StyleHUD), "rankIndex", MethodType.Setter)]
        public static void Postfix(int value, StyleHUD __instance) {
            if (value == 7) {
                GameObject confettiObject = Confetti.DoConfetti();
                Bib.Instance.AddAndPlayVoice(confettiObject, Bib.Voices["CONFETTI_SOUND"]);
            }
        }
    }
}