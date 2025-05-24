using System.Collections.Generic;
using WK_Lib.API;

namespace WK_Lib.Loaders;

public static class RegionLoader
{
    private static readonly List<string> RegionOrder = new();
    private static readonly Dictionary<string, ICustomRegion> Regions = new();
    
    public static void Setup()
    {
        WKLog.Info("RegionLoader initialized.");
    }

    public static void Register(ICustomRegion region)
    {
        if (!Regions.TryAdd(region.Key, region))
        {
            Regions[region.Key] = region;
            WKLog.Warn($"Replaced existing region '{region.Key}'");
        }
        else
        {
            RegionOrder.Add(region.Key);
            WKLog.Info($"Registered new region '{region.Key}'");
        }
    }

    public static IEnumerable<ICustomRegion> GetAll()
    {
        foreach (var key in RegionOrder)
            yield return Regions[key];
    }
}