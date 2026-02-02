namespace FrankenToilet.Bryan;

using FrankenToilet.Core;

/// <summary> Class for setting up my part of this mod (oh god) </summary>
[EntryPoint]
public class Plugin
{
    /// <summary> why did i sign up for this </summary>
    [EntryPoint]
    public static void Awake()
    {
        BundleLoader.Load();
    }
}
