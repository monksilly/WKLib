using System.Collections.Generic;
using System.IO;
using UnityEngine;
using WK_Lib.API;

namespace WK_Lib.Loaders;

public static class AssetBundleLoader
{
    private static readonly Dictionary<string, string> BundlePaths = new();
    private static readonly Dictionary<string, AssetBundle> Bundles = new();

    public static void Setup()
    {
        WKLog.Info("AssetBundleLoader initialized.");
    }
    
    public static void Register(string key, string filePath)
    {
        if (BundlePaths.ContainsKey(key))
            WKLog.Warn($"AssetBundle '{key}' already registered, overwriting path.");

        BundlePaths[key] = filePath;
        WKLog.Info($"AssetBundle '{key}' -> '{filePath}' registered.");
    }
    
    public static AssetBundle GetBundle(string key)
    {
        if (Bundles.TryGetValue(key, out var bundle))
            return bundle;

        if (!BundlePaths.TryGetValue(key, out var path) || !File.Exists(path))
        {
            WKLog.Error($"AssetBundle path not found for key '{key}'");
            return null;
        }

        bundle = AssetBundle.LoadFromFile(path);
        if (bundle == null)
        {
            WKLog.Error($"Failed to load AssetBundle from '{path}'");
            return null;
        }

        Bundles[key] = bundle;
        WKLog.Info($"AssetBundle '{key}' loaded successfully.");
        return bundle;
    }
    
    public static T LoadAsset<T>(string bundleKey, string assetName) where T : UnityEngine.Object
    {
        var bundle = GetBundle(bundleKey);
        if (!bundle)
        {
            WKLog.Warn($"Bundle '{bundleKey}' not found");
            return null;
        }

        var asset = bundle.LoadAsset<T>(assetName);
        if (!asset)
            WKLog.Warn($"Asset '{assetName}' not found in bundle '{bundleKey}'");
        
        return asset;
    }
}