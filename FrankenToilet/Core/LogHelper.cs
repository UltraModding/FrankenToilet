using System.Diagnostics;
using BepInEx.Logging;
using JetBrains.Annotations;

namespace FrankenToilet.Core;

/// <summary>
///    Helper class for logging with automatic namespace prefixes.
/// </summary>
[PublicAPI]
public static class LogHelper
{
    public static void LogInfo(object message) => Plugin.Logger.LogInfo(TryAddPrefix(message));

    public static void LogWarning(object message) => Plugin.Logger.LogWarning(TryAddPrefix(message));
    public static void LogError(object message) => Plugin.Logger.LogError(TryAddPrefix(message));
    public static void LogDebug(object message) => Plugin.Logger.LogDebug(TryAddPrefix(message));
    public static void LogFatal(object message) => Plugin.Logger.LogFatal(TryAddPrefix(message));
    public static void LogMessage(object message) => Plugin.Logger.LogMessage(TryAddPrefix(message));
    public static void Log(LogLevel logLevel, object data) => Plugin.Logger.Log(logLevel, TryAddPrefix(data));

    private static object TryAddPrefix(object message)
    {
        if (message is not string str) return message;
        var stackFrame = new StackTrace().GetFrame(2); // 0 is this method, 1 is caller (Log), 2 is the original caller
        var name = stackFrame.HasMethod()
                       ? stackFrame.GetMethod()
                                   .DeclaringType
                                  ?.Namespace
                                  ?.Split('.')[1]
                       : null;
        return string.IsNullOrEmpty(name)
                   ? message
                   : $"[{name}] {str}";
    }
}