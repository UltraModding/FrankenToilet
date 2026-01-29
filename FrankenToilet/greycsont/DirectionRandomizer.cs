using UnityEngine;

using FrankenToilet.Core;

namespace FrankenToilet.greycsont;


public static class DirectionRandomizer
{
    private static Vector3 cachedDirection;
    private static bool cachedValid;

    public static void Reset() => cachedValid = false;
    
    public static Vector3 Randomize4Dir(Vector3 direction)
    {
        if (cachedValid) return cachedDirection;

        float originalMag = direction.magnitude;

        Vector3 right = Vector3.Cross(Vector3.up, direction).normalized;

        var updatedDirection = (Direction)Random.Range(0, 4);
        Vector3 resultDir;

        switch (updatedDirection)
        {
            case Direction.Upwards:
                resultDir = Quaternion.AngleAxis(-90, right) * direction;
                break;
            case Direction.Backwards:
                resultDir = -direction;
                break;
            case Direction.Right:
                resultDir = Quaternion.AngleAxis(90, Vector3.up) * direction;
                resultDir.y = -resultDir.y;
                break;
            case Direction.Left:
                resultDir = Quaternion.AngleAxis(-90, Vector3.up) * direction;
                resultDir.y = -resultDir.y;
                break;
            default:
                resultDir = direction;
                break;
        }

        LogHelper.LogDebug($"Direction: {updatedDirection}");

        cachedDirection = resultDir.normalized * originalMag;
        cachedValid = true;
        return cachedDirection;
    }
}


public enum Direction
{
    Backwards = 0,
    Upwards = 1,
    Left = 2,
    Right = 3
}