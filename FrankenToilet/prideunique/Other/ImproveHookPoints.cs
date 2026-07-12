using System;
using UnityEngine;

namespace FrankenToilet.prideunique;
public static class ImproveHookPoints
{
    public static void Init()
    {
        try
        {
            var allHookPoints = UnityEngine.Object.FindObjectsByType<HookPoint>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (var hk in allHookPoints)
            {
                if (hk.type == hookPointType.Normal)
                    hk.type = hookPointType.Slingshot;
            }
        }
        catch (Exception)
        {
            // ignore
        }
    }
}
