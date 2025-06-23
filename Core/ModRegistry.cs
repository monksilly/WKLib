using System.Collections.Generic;

namespace WKLib.Core;

/// <summary>
/// Provides a static registry for managing and accessing <see cref="ModContext"/> instances.<br/>
/// This ensures that each mod is registered only once and its context can be retrieved globally.
/// </summary>
public static class ModRegistry
{
    private static readonly Dictionary<string, ModContext> Contexts = new();

    /// <summary>
    /// Registers a new mod with the registry or returns an existing context if the mod is already registered.
    /// </summary>
    /// <param name="modID">The unique identifier for the mod to register.</param>
    /// <param name="version">The version of the mod. This can be null if no version is specified.</param>
    /// <returns>
    /// A <see cref="ModContext"/> instance for the given modID.<br/>
    /// If the mod was not previously registered, a new context is created and returned.<br/>
    /// If the mod was already registered, the existing context is returned.
    /// </returns>
    public static ModContext Register(string modID, string version = null)
    {
        if (Contexts.TryGetValue(modID, out var existing))
            return existing;
        
        var ctx = new ModContext(modID, version);
        Contexts[modID] = ctx;
        return ctx;
    }
    
    /// <summary>
    /// Retrieves the <see cref="ModContext"/> for a previously registered mod.
    /// </summary>
    /// <param name="modID">The unique identifier of the mod to retrieve.</param>
    /// <returns>The <see cref="ModContext"/> associated with the specified modID.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the specified modID has not been registered.</exception>
    public static ModContext Get(string modID) =>
        Contexts.TryGetValue(modID,
            out var ctx)
            ? ctx
            : throw new KeyNotFoundException($"Mod '{modID}' not registered.");
}