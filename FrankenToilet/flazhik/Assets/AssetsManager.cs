using System;
using System.Linq;
using System.Reflection;
using FrankenToilet.flazhik.Components;
using TMPro;
using UnityEngine;
using Object = UnityEngine.Object;

namespace FrankenToilet.flazhik.Assets;

[ConfigureSingleton(SingletonFlags.PersistAutoInstance)]
public sealed class AssetsManager : MonoSingleton<AssetsManager>
{
    private const BindingFlags Flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static;
    private AssetBundle _bundle;

    public void LoadAssets(string resourcePath)
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceStream = assembly.GetManifestResourceStream(resourcePath);
        _bundle = AssetBundle.LoadFromStream(resourceStream);
    }

    public void RegisterPrefabs()
    {
        foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
            CheckType(type);
    }

    private void CheckType(IReflect type)
    {
        type.GetFields(Flags)
            .ToList()
            .ForEach(ProcessField);
    }

    private void ProcessField(FieldInfo field)
    {
        if (field.FieldType.IsArray
         || !typeof(Object).IsAssignableFrom(field.FieldType)
         || !field.IsStatic)
            return;

        var externalAsset = field.GetCustomAttribute<ExternalAsset>();

        if (field.GetCustomAttribute<ExternalAsset>() != null)
            field.SetValue(null, LoadAsset(externalAsset.Path, externalAsset.Type));
    }

    private Object LoadAsset(string path, Type type) => UnFuckGameObject(_bundle.LoadAsset(path, type));

    private static Object UnFuckGameObject(Object o)
    {
        if (o is not GameObject go)
            return o;

        foreach (var txt in go.GetComponentsInChildren<TextMeshProUGUI>(true))
        {
            var fp = txt.gameObject.AddComponent<FontProtector>();
            fp.originalFont = txt.font;
            fp.originalColor = txt.color;
        }

        return o;
    }
}