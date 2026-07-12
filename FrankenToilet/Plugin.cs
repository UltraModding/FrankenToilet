#pragma warning disable CS0162 // Unreachable code detected
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using FrankenToilet.Core;
using HarmonyLib;
using HarmonyLib.PatchExtensions;
using UnityEngine;
using UnityEngine.InputSystem;
using static FrankenToilet.Core.LogHelper;

namespace FrankenToilet;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public sealed class Plugin : BaseUnityPlugin
{
    internal new static ManualLogSource Logger { get; private set; } = null!;

    private void Awake()
    {
        
        Logger = base.Logger;
        InputSystem.RegisterProcessor<ScaleVector2DeltaTimeProcessor>();
        LogInfo("Welcome to Frankenstein's Toilet...");
        gameObject.hideFlags = HideFlags.DontSaveInEditor;
        var harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
        var entryPointMethods = new List<MethodInfo>();
        var mixinClasses = new List<Type>();
        TogglePatchHelper TPH = new();
        //TPH.Init();
        foreach (var type in typeof(Plugin).Assembly
                                           .GetTypes()
                                           .Where(static type => DevModeInfo.CURRENT_DEV_NAMESPACE == null
                                                              || (type.Namespace
                                                                     ?.Split('.', StringSplitOptions.RemoveEmptyEntries)
                                                                     ?.Any(static n => n.Equals(DevModeInfo.CURRENT_DEV_NAMESPACE,
                                                                               StringComparison.OrdinalIgnoreCase))
                                                               ?? false)))
        {
            if (type.GetCustomAttribute<EntryPointAttribute>() != null)
            {
                var entries = type
                             .GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                             .Where(static mi => mi.GetCustomAttribute<EntryPointAttribute>() != null).ToArray();
                switch (entries.Length)
                {
                    case > 1:
                        LogError($"Type {type.FullName} has multiple entry points defined. Only one is allowed.");
                        break;
                    case 0:
                        LogError($"Type {type.FullName} is marked as an entry point but has no methods marked with EntryPointAttribute.");
                        break;
                    case 1 when entries[0].GetParameters().Length != 0:
                        LogError($"Entry point method {type.FullName}.{entries[0].Name} must have no parameters.");
                        break;
                    case 1:
                        entryPointMethods.Add(entries[0]); // idc abt return values
                        break;
                }
            }

            if (type.GetCustomAttribute<PatchOnEntryAttribute>() != null)
            {
                harmony.PatchAll(type);
                mixinClasses.Add(type);
            }
        }
        MixinLoader.ApplyPatches(harmony, typeof(Plugin).Assembly, mixinClasses.ToArray());

        LogInfo("Patches applied");
        foreach (var method in entryPointMethods)
        {
            LogInfo($"Invoking entry point: {method.DeclaringType?.FullName}.{method.Name}");
            try
            {
                method.Invoke(null, null);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}