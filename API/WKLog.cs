using BepInEx.Logging;

namespace WK_Lib.API;

public static class WKLog
{
    private static ManualLogSource _log;
    internal static void Initialize(ManualLogSource logSource)
        => _log = logSource;

    public static void Info(string msg)  => _log?.LogInfo($"[WK] {msg}");
    public static void Warn(string msg)  => _log?.LogWarning($"[WK] {msg}");
    public static void Error(string msg) => _log?.LogError($"[WK] {msg}");
    public static void Debug(string msg) => _log?.LogDebug($"[WK] {msg}");
}