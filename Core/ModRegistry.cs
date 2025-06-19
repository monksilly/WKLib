using System.Collections.Generic;

namespace WKLib.Core;

public static class ModRegistry
{
    private static readonly Dictionary<string, ModContext> Contexts = new();

    public static ModContext Register(string modID, string version = null)
    {
        if (Contexts.TryGetValue(modID, out var existing))
            return existing;
        
        var ctx = new ModContext(modID, version);
        Contexts[modID] = ctx;
        return ctx;
    }
    
    public static ModContext Get(string modID) =>
        Contexts.TryGetValue(modID,
            out var ctx)
            ? ctx
            : throw new KeyNotFoundException($"Mod '{modID}' not registered.");
}