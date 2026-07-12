using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System.Collections.Generic;

namespace FrankenToilet.earthling;

public static class FishProvider
{
    private static Dictionary<string, FishObject> fishes = new Dictionary<string, FishObject>();
    private static System.Random rand = new System.Random();

    public static void LoadFishes()
    {
        string[] fishPaths = {
            "Assets/Data/Fishing/Fishes/Funny Stupid Fish.asset", // Funny Stupid Fish (Friend)
            "Assets/Data/Fishing/Fishes/pitr fish.asset", // PITR Fish
            "Assets/Data/Fishing/Fishes/Trout.asset", // Trout
            "Assets/Data/Fishing/Fishes/Amid Evil Fish.asset", // Metal Fish
            "Assets/Data/Fishing/Fishes/Chomper.asset", // Chomper
            "Assets/Data/Fishing/Fishes/Bomb Fish.asset", // Bomb Fish
            "Assets/Data/Fishing/Fishes/Gib Eye.asset", // Eyeball
            "Assets/Data/Fishing/Fishes/Iron Lung Fish.asset", // Frog (?)
            "Assets/Data/Fishing/Fishes/Dope Fish.asset", // Dope Fish
            "Assets/Data/Fishing/Fishes/Stickfish.asset", // Stickfish
            "Assets/Data/Fishing/Fishes/Cooked Fish.asset", // Cooked Fish
            "Assets/Data/Fishing/Fishes/Shark.asset", // Shark
        };

        foreach (string fishPath in fishPaths)
        {
            FishObject fish = Addressables.LoadAssetAsync<FishObject>(fishPath).WaitForCompletion();
            fishes.Add(fish.fishName, fish);
        }

        FishObject? chenFish = AssetBundleHelper.LoadAsset<FishObject>("Assets/Bundles/toiletonfire/chen fish.asset");
        if (chenFish != null)
        {
            fishes.Add(chenFish.fishName, chenFish);
        }
    }

    public static FishObject GetFish(string fishName)
    {
        return fishes[fishName];
    }

    public static FishObject GetRandomFish()
    {
        return fishes.ElementAt(rand.Next(fishes.Count)).Value;
    }

    public static FishObject[] GetFishes()
    {
        return fishes.Values.ToArray();
    }

    public static ItemIdentifier CreateFishPickup(FishObject fish)
    {
        ItemIdentifier itemIdentifier;

        if (fish.customPickup != null)
        {
            itemIdentifier = GameObject.Instantiate(fish.customPickup);
            if (!itemIdentifier.GetComponent<FishObjectReference>())
            {
                itemIdentifier.gameObject.AddComponent<FishObjectReference>().fishObject = fish;
            }
        }
        else
        {
            GameObject defaultPickup = AssetHelper.LoadPrefab("Assets/Prefabs/Fishing/Fish Pickup Template.prefab");
            itemIdentifier = GameObject.Instantiate(defaultPickup).GetComponent<ItemIdentifier>();
            itemIdentifier.gameObject.AddComponent<FishObjectReference>().fishObject = fish;

            Transform obj = itemIdentifier.transform.GetChild(0).transform;
            Vector3 localPosition = obj.localPosition;
            Quaternion localRotation = obj.localRotation;
            Vector3 localScale = obj.localScale;
            GameObject.Destroy(obj.gameObject);

            GameObject obj2 = fish.InstantiateDumb();
            obj2.transform.SetParent(itemIdentifier.transform);
            obj2.transform.localPosition = localPosition;
            obj2.transform.localRotation = localRotation;
            obj2.transform.localScale = localScale;
        }

        return itemIdentifier;
    }
}