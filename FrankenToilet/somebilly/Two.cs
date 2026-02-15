#pragma warning disable CS8618
using UnityEngine;
using UnityEngine.Video;
using UnityObject = UnityEngine.Object;


namespace FrankenToilet.somebilly {
    public class Two : MonoBehaviour {
        public static float twoSpawnDistance = 300f;
        public static float twoSpawnHeight = 500f;

        // THIS IS THE TWO.
        public static void SpawnTwo() {
            float twoSpawnDistance = 300f;
            float twoSpawnHeight = 500f;

            GameObject billboard = GameObject.CreatePrimitive(PrimitiveType.Quad);
            billboard.name = "FrankenTwo";
            billboard.transform.localScale = new Vector3(60, 60, 60);

            RenderTexture videoTexture = new RenderTexture(374, 210, 16);
            videoTexture.Create();

            VideoPlayer video = billboard.AddComponent<VideoPlayer>();
            video.clip = Bib.Assets.LoadAsset<VideoClip>("Assets/Bib/2.mp4");
            video.targetTexture = videoTexture;
            video.isLooping = true;
            video.playOnAwake = true;

            Material mat = new Material(Shader.Find("Unlit/Texture"));
            mat.color = new Color(1, 1, 1, 1);
            mat.mainTexture = videoTexture;
            MeshRenderer renderer = billboard.GetComponent<MeshRenderer>();
            renderer.material = mat;

            AlwaysLookAtCamera looker = billboard.AddComponent<AlwaysLookAtCamera>();
            looker.rotationOffset = new Vector3(180, 0, 180);
            Destroy(billboard.GetComponent<MeshCollider>());
            billboard.transform.position = GetPointOnCircle(NewMovement.Instance.transform.position, Two.twoSpawnDistance, Two.twoSpawnHeight);

            billboard.AddComponent<TwoFaller>();
        }

        public static Vector3 GetPointOnCircle(Vector3 center, float radius, float heightOffset) {
            float angleDeg = Random.Range(0, 360);
            float angleRad = angleDeg * Mathf.Deg2Rad;
            return new Vector3(
                center.x + radius * Mathf.Cos(angleRad),
                center.y + heightOffset,
                center.z + radius * Mathf.Sin(angleRad)
            );
        }
    }

    public class TwoSpawner : MonoBehaviour {
        public float time = 1.25f;
        public float currentTime = 0f;

        void Update() {
            currentTime += Time.deltaTime;
            if (currentTime >= time) {
                currentTime = 0f;
                Two.SpawnTwo();
            }
        }
    }

    public class TwoFaller : MonoBehaviour {
        public float speed;
        public float fallenDistance = 0f;
        public float maxFallenDistance = 1500f;

        void Awake() {
            speed = Random.Range(30, 90);
        }

        void Update() {
            float fallDistance = speed * Time.deltaTime;
            Vector3 pos = this.transform.position;
            this.transform.position = new Vector3(pos.x, pos.y - fallDistance, pos.z);
            fallenDistance += fallDistance;
            if (fallenDistance >= maxFallenDistance) {
                Destroy(this.gameObject);
            }
        }
    }
}