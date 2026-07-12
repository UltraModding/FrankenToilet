using System.IO;
using System.Reflection;
using UnityEngine;

namespace FrankenToilet.alma;
internal class Functions
{
    public static AssetBundle bundle;

    public static AssetBundle GetBundle(string bundleToLoad)
    {
        Assembly assembly = Assembly.GetExecutingAssembly();
        using (Stream stream = assembly.GetManifestResourceStream(bundleToLoad))
        bundle = AssetBundle.LoadFromStream(stream);
        return bundle;

    }
}