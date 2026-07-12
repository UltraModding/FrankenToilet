namespace FrankenToilet.Bryan;

using UnityEngine;

/// <summary> Dupes projectiles after 0.5 seconds </summary>
public class ProjectileFucker : MonoBehaviour
{
    /// <summary> The fellow projectile component next to this component :3 </summary>
    public Projectile proj;

    /// <summary> Time when created so we know when to dupe. </summary>
    public float time = Time.time;

    /// <summary> Grab the projectile component. </summary>
    public void Awake() =>
        proj = GetComponent<Projectile>();

    /// <summary> Check if we should dupe. </summary>
    public void Update()
    {
        if (Time.time - time > 0.5f)
        {
            enabled = false; // disable this component so we dont keep duping over and over

            Vector3 rotate = new(0f, 5f, 0f);
            Quaternion newRot = Quaternion.Euler(transform.localEulerAngles - rotate);
            Instantiate(gameObject, transform.position, newRot);

            transform.localEulerAngles += rotate;
            proj.targetRotation.eulerAngles += rotate;
        }
    }
}