using FrankenToilet.Core;
using Steamworks;
using System;

namespace FrankenToilet.prideunique;

public static class RandomForMe
{
    private static System.Random rng = new System.Random((int)DateTime.Now.Ticks);

    public static int Next(int maxValue) //0 to range-1
    {
        int value = rng.Next(maxValue);

        try
        {
            if (SteamClient.IsLoggedOn)
                value = UserRandom.Shared.Next(maxValue);
        }
        catch { }

        return value;
    }

    public static float Next(float minValue, float maxValue)
    {
        if (minValue >= maxValue)
            return minValue;

        double t = rng.NextDouble();

        try
        {
            if (SteamClient.IsLoggedOn)
                t = UserRandom.Shared.NextDouble();
        }
        catch { }

        return (float)(minValue + (t * (maxValue - minValue)));
    }

    public static int Next(int minValue, int maxValue)
    {
        int value = rng.Next(minValue, maxValue);

        try
        {
            if (SteamClient.IsLoggedOn)
                value = UserRandom.Shared.Next(minValue, maxValue);
        }
        catch { }

        return value;
    }
}
