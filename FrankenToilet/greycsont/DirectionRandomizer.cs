using UnityEngine;

using FrankenToilet.Core;

namespace FrankenToilet.greycsont;


public static class DirectionRandomizer
{
    public static int randomDirection;

    public static void GenerateRandomDirection() => randomDirection = Random.Range(0, 4);

    public static Vector3 Randomize4Dir(Vector3 direction)
    {
        Vector3 resultDir;

        var camT = MonoSingleton<CameraController>.Instance?.transform;

        var gravityDir = MonoSingleton<NewMovement>.Instance.rb.GetGravityDirection();

        switch ((Direction)randomDirection)
        {
            case Direction.Upwards:
                resultDir = camT.up;
                break;
            case Direction.Backwards:
                resultDir = -direction;
                break;
            case Direction.Right:
                if (Mathf.Abs(Vector3.Dot(direction.normalized, Vector3.up)) > 0.94f)
                    resultDir = Quaternion.AngleAxis(90, camT.up) * direction;
                else
                    resultDir = RotateAndInvertHeight(90f, -gravityDir, direction);
                break;
            case Direction.Left:
                if (Mathf.Abs(Vector3.Dot(direction.normalized, Vector3.up)) > 0.94f)
                    resultDir = -camT.right;
                else
                    resultDir = RotateAndInvertHeight(-90f, -gravityDir, direction);
                break;
            default:
                resultDir = direction;
                LogHelper.LogDebug("[greycsont] FUCK IENUMERATOR");
                break;
        }
        return resultDir;;
    }

    private static Vector3 RotateAndInvertHeight(float angle, Vector3 axis, Vector3 dir)
    {
        Vector3 rotated = Quaternion.AngleAxis(angle, axis) * dir;
        
        Vector3 vertical = Vector3.Project(rotated, axis);
        
        return rotated - 2 * vertical;
    }
}

