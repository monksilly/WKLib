using System.Collections.Generic;
using UnityEngine;
using WK_Lib.API;

namespace WK_Lib.Loaders;

/// <summary>
/// Provides a simple registration API for custom levels.
/// </summary>
public static class LevelLoader
{
    private static readonly List<string> LevelOrder = new();
    private static readonly Dictionary<string, ICustomLevel> Levels = new();
    
    /// <summary>
    /// Called by <see cref="WkLib.Awake"/> to auto‐wire up if needed.
    /// </summary>
    public static void Setup()
    {
        WKLog.Info("LevelLoader initialized.");
    }

    
    /// <summary>
    /// Register or replace a level at its key
    /// </summary>
    /// <param name="level"><see cref="ICustomLevel"/> to load</param>
    public static void Register(ICustomLevel level)
    {
        if (!Levels.TryAdd(level.Key, level))
        {
            Levels[level.Key] = level;
            WKLog.Warn($"Replaced existing level '{level.Key}'");
        }
        else
        {
            LevelOrder.Add(level.Key);
            WKLog.Info($"Registered new level '{level.Key}'");
        }
    }
    
    /// <summary>
    /// Insert custom level after an existing key
    /// </summary>
    /// <param name="existingKey">Existing level name</param>
    /// <param name="level"><see cref="ICustomLevel"/> level to be inserted after <see cref="existingKey"/></param>
    public static void InsertAfter(string existingKey, ICustomLevel level)
    {
        if (!LevelOrder.Contains(existingKey))
        {
            WKLog.Error($"Cannot insert after unknown level '{existingKey}'");
            return;
        }
        Register(level);
        var index = LevelOrder.IndexOf(existingKey) + 1;
        LevelOrder.Remove(level.Key);
        LevelOrder.Insert(index, level.Key);
        WKLog.Info($"Inserted level '{level.Key}' after '{existingKey}'");
    }
    
    /// <summary>
    /// Insert custom level before an existing key
    /// </summary>
    /// <param name="existingKey">Existing level name</param>
    /// <param name="level"><see cref="ICustomLevel"/> level to be inserted before <see cref="existingKey"/></param>
    public static void InsertBefore(string existingKey, ICustomLevel level)
    {
        if (!LevelOrder.Contains(existingKey))
        {
            WKLog.Error($"Cannot insert before unknown level '{existingKey}'");
            return;
        }
        Register(level);
        var index = LevelOrder.IndexOf(existingKey);
        LevelOrder.Remove(level.Key);
        LevelOrder.Insert(index, level.Key);
        WKLog.Info($"Inserted level '{level.Key}' before '{existingKey}'");
    }
    
    // Internal: called by WorldLoader to load levels in order
    public static IEnumerable<GameObject> LoadAll()
    {
        foreach (var key in LevelOrder)
        {
            if (Levels.TryGetValue(key, out var lvl))
                yield return lvl.LoadLevel();
            else
                WKLog.Error($"Level '{key}' not found during load.");
        }
    }
}