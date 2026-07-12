using System;
using System.Reflection;

namespace FrankenToilet.flazhik.Utils;

public static class ReflectionUtils
{
    private const BindingFlags PrivateFields = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

    public static T GetPrivate<T>(object instance, Type classType, string field)
    {
        var privateField = classType.GetField(field, PrivateFields);
        return (T)(privateField != null ? privateField.GetValue(instance) : null)!;
    }
}