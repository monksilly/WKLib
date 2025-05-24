using System.Collections.Generic;
using WK_Lib.API;

namespace WK_Lib.Loaders;

public static class SubregionLoader
{
    private static readonly List<string> SubregionOrder = new();
    private static readonly Dictionary<string, ICustomSubregion> Subregions = new();
    
    public static void Setup()
    {
        WKLog.Info("SubregionLoader initialized.");
    }
    
    public static void Register(ICustomSubregion subregion)
    {
        if (!Subregions.TryAdd(subregion.Key, subregion))
        {
            Subregions[subregion.Key] = subregion;
            WKLog.Warn($"Replaced existing subregion '{subregion.Key}'");
        }
        else
        {
            SubregionOrder.Add(subregion.Key);
            WKLog.Info($"Registered new subregion '{subregion.Key}'");
        }
    }
    
    public static IEnumerable<ICustomSubregion> GetAll()
    {
        foreach (var key in SubregionOrder)
            yield return Subregions[key];
    }
}