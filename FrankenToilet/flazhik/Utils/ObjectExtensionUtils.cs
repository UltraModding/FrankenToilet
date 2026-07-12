using System.Linq;

namespace FrankenToilet.flazhik.Utils;

public static class ObjectExtensionUtils
{
    public static bool IsOneOf<T>(this T o, params T[] parameters)
        => parameters.Any(p => p.Equals(o));
}