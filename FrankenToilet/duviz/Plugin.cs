#pragma warning disable CS8618
using FrankenToilet.Core;
using FrankenToilet.duviz.events;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace FrankenToilet.duviz;

[EntryPoint]
public static class Plugin
{
    [EntryPoint]
    public static void Initialize()
    {
        LogHelper.LogInfo("[MEOW] I loaded :3");

        GameObject m = new GameObject("NewFolder");
        m.hideFlags = HideFlags.HideAndDontSave;

        m.AddComponent<Bundle>();
        m.AddComponent<Jesus>();
        m.AddComponent<Meow>();
        m.AddComponent<EventsManager>();
        m.AddComponent<EventsCreator>();
        m.AddComponent<HealthRemover>();
        m.AddComponent<SpinEvent>();
    }

    public static T Ass<T>(string path) { return Addressables.LoadAssetAsync<T>((object)path).WaitForCompletion(); }
}