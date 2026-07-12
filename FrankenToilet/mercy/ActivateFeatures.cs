using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using FrankenToilet.Core;
using FrankenToilet.mercy.Features;
using UnityEngine;

namespace FrankenToilet.mercy;

public sealed class ActivateFeatures : MonoBehaviour
{
    public static Stopwatch timer = new();
    public static long time = 0;
    public static List<MethodInfo> features = new();
    public void Awake()
    {
        timer.Start();
        time = Plugin.rand.Next(10, 20);
        Assembly assembly = Assembly.GetExecutingAssembly();
        // getting the list of methods with the [MercyFeature] attribute
        features = assembly.GetTypes().SelectMany(t => t.GetMethods())
                           .Where(m => 
                                m.GetCustomAttributes(typeof(MercyFeatureAttribute), false).Length > 0).ToList();
    }
    public void Update()
    {
        // ty nora
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        if (timer.Elapsed.TotalSeconds > time && features.Count > 0)
        {
            if (!SteamHelper.IsSlopTuber)
            {
                int featureIndex;
                if (features.Count - 1 > 0) featureIndex = Plugin.rand.Next(0, features.Count - 1);
                else featureIndex = 0;
                features[featureIndex].Invoke(null, null);
                features.RemoveAt(featureIndex);
                if (features.Count > 0)
                {
                    time = Plugin.rand.Next(10, 20);
                    timer.Restart();
                }
                else timer.Reset();
            }
            else
            {
                foreach (MethodInfo feature in features) feature.Invoke(null, null);
                timer.Reset();
            }
            Helper.CreateImage<NewFeatureAlert>("New Feature Alert", 400, 40);
            
        }
    }
    private void OnDestroy() => timer.Reset();

}
