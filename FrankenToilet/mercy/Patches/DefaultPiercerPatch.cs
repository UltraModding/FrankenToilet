using FrankenToilet.Core;
using FrankenToilet.mercy.Features;
using HarmonyLib;
using UnityEngine;

namespace FrankenToilet.mercy.Patches;

[PatchOnEntry]
[HarmonyPatch(typeof(RevolverBeam))]
public class DefaultPiercerPatch
{
    [HarmonyPatch("ExecuteHits")]
    [HarmonyPostfix]
    public static void DefaultPiercerBuff(RevolverBeam __instance, ref PhysicsCastResult currentHit)
    {
        EnemyIdentifier? eid = currentHit.transform.GetComponentInParent<EnemyIdentifier>();
        if (eid != null && __instance.sourceWeapon.name == "Revolver Pierce(Clone)" && Plugin.canvas.GetComponentInChildren<DefaultPiercer>())
        {
            Vector3 force = (currentHit.transform.position - __instance.transform.position).normalized * __instance.bulletForce;
            eid.DeliverDamage(currentHit.transform.gameObject, force, currentHit.point, 
                (float) DefaultPiercer.mult-1, false);
        }
    }

}
