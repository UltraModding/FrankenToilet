using System.IO;
using UnityEngine;
using FrankenToilet.Core;

namespace FrankenToilet.doomahreal;

[EntryPoint]
public static class IMLOADINGITSOHARDDDD
{
    const string skibidifile = "FrankenToilet.doomahreal.veryuniqueassetsnametrust.bundle";
    public static AssetBundle? thegrundle;

    [EntryPoint]
    public static void Initialize()
    {
        if (thegrundle != null) return;

        var asm = typeof(IMLOADINGITSOHARDDDD).Assembly;
        using var stream = asm.GetManifestResourceStream(skibidifile);
        if (stream == null)
        {
            LogHelper.LogError("dude you LOADED THE WRONG FUCKING THING YOU STUPID IDIOT");
            return;
        }

        using var ms = new MemoryStream();
        stream.CopyTo(ms);
        thegrundle = AssetBundle.LoadFromMemory(ms.ToArray());

        if (thegrundle == null)
            LogHelper.LogError("nice one tons of hussle fun, next time try loading it correctly");
        else
            LogHelper.LogInfo("yayyyyyy grungle woaded");

    }
}