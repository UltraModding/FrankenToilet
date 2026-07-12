using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using static FrankenToilet.Core.LogHelper;

namespace FrankenToilet.prideunique;

public static class AudioClipsAddressableLoader
{
    private static List<IResourceLocation> _resourcesToLoad = new List<IResourceLocation>();
    private static Dictionary<string, string> _internalIds = new Dictionary<string, string>();

    // I ask for forgiveness
    public static void LoadSoundAndMusicAddressables()
    {
        string[] catalogs = FindCatalogs(Application.streamingAssetsPath);
        if (catalogs == null)
            return;

        foreach (string catalog in catalogs)
        {
            string jsonString = File.ReadAllText(catalog);

            JObject jsonObject = JObject.Parse(jsonString);

            JArray internalIds = (JArray)jsonObject["m_InternalIds"];

            if (internalIds != null)
            {
                foreach (var id in internalIds)
                {
                    ProcessInternalId(id.ToString(), internalIds.IndexOf(id), internalIds.Count);
                }
            }
            else
            {
                LogInfo("m_InternalIds not found in the JSON file.");
            }
        }
    }

    private static string[] FindCatalogs(string directoryPath)
    {
        if (Directory.Exists(directoryPath))
        {
            string[] files = Directory.GetFiles(directoryPath, "catalog.json", SearchOption.AllDirectories);

            if (files.Length > 0)
            {
                LogInfo($"Found {files.Length} 'catalog.json' file(s):");
                return files;
            }
            else
            {
                LogInfo("No 'catalog.json' files found in the specified directory and its subdirectories.");
            }
        }
        else
        {
            LogInfo("The specified directory does not exist.");
        }

        return null;
    }

    private static void ProcessInternalId(string internalId, int index, int count)
    {
        if (internalId.EndsWith(".bundle"))
            return;

        GetAssetPath(internalId, index, count);
    }

    private static void GetAssetPath(string address, int index, int count)
    {
        // Load resource locations for the given address
        Addressables.LoadResourceLocationsAsync(address).Completed += handle =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                IList<IResourceLocation> locations = handle.Result;
                foreach (var location in locations)
                {
                    if (location.PrimaryKey == address)
                        continue;

                    _internalIds[location.PrimaryKey.ToString()] = location.InternalId.ToString();

                    string bundle = GetBundleFromLocation(location);
                    if (bundle.Equals("sounds.bundle")
                        || bundle.Equals("music_assets_.bundle")
                        || bundle.Equals("music_asets_layer0.bundle")
                        || bundle.Equals("music_asets_layer1.bundle")
                        || bundle.Equals("music_asets_layer2.bundle")
                        || bundle.Equals("music_asets_layer3.bundle")
                        || bundle.Equals("music_asets_layer4.bundle")
                        || bundle.Equals("music_asets_layer5.bundle")
                        || bundle.Equals("music_asets_layer6.bundle")
                        || bundle.Equals("music_asets_layere.bundle")
                        || bundle.Equals("music_asets_layerp.bundle")
                        || location.ResourceType == typeof(AudioClip))
                    {
                        _resourcesToLoad.Add(location);
                    }
                }
            }
            else
            {
                LogError("Failed to load resource locations.");
            }

            if (index == count - 1)
            {
                foreach (var location in _resourcesToLoad)
                {
                    var handle1 = Addressables.LoadAssetAsync<UnityEngine.Object>(location);
                    handle1.Completed += h =>
                    {
                        if (h.Status == AsyncOperationStatus.Succeeded)
                        {
                            UnityEngine.Object asset = h.Result;
                            if (asset != null &&
                                location.ResourceType == typeof(AudioClip))
                            {
                                SoundRandomizer.AddressableAudioClips.Add(asset as AudioClip);
                            }

                            //LogInfo($"Loaded {asset.name} ({location.ResourceType})");
                        }
                        else
                        {
                            //LogError($"Failed to load {location.PrimaryKey}");
                        }
                    };
                }
            }
        };
    }
    public static string GetBundleFromLocation(IResourceLocation location)
    {
        if (location.Dependencies == null || location.Dependencies.Count == 0)
            return null;

        var bundleLoc = location.Dependencies[0];

        // InternalId usually contains the bundle path or url
        return System.IO.Path.GetFileName(bundleLoc.InternalId);
    }
}