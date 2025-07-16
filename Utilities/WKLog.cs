using BepInEx.Logging;

namespace WKLib.Utilities;

/// <summary>
/// A static class used for All logging purposes of the library
/// </summary>
public static class  WKLog
{
    private static ManualLogSource _log;
    internal static void Initialize(ManualLogSource logSource)
        => _log = logSource;

    public static void Info(object msg)  => _log?.LogInfo($"[WKLib] {msg}");
    public static void Warn(object msg)  => _log?.LogWarning($"[WKLib] {msg}");
    public static void Error(object msg) => _log?.LogError($"[WKLib] {msg}");
    public static void Debug(object msg) => _log?.LogDebug($"[WKLib] {msg}");
}