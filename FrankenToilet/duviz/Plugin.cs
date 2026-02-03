#pragma warning disable CS8618
using FrankenToilet.Core;
using FrankenToilet.duviz.commands;
using FrankenToilet.duviz.events;
using GameConsole;
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
    }

    public static T Ass<T>(string path) { return Addressables.LoadAssetAsync<T>((object)path).WaitForCompletion(); }
}