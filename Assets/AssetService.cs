using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using WKLib.Core;
using UnityEngine;
using WKLib.Utilities;
using Object = UnityEngine.Object;

namespace WKLib.Assets;

/// <summary>
/// Loads and caches AssetBundles and assets, scoped by ModContext to avoid collisions.
/// </summary>
public class AssetService
{
    private readonly ModContext _modContext;
    private readonly string _assemblyFolder;
    private readonly Dictionary<string, AssetBundle> _bundleCache = new();
    private readonly Dictionary<string, Dictionary<string, M_Level>> _loadedLevelsCache = new();
    private readonly Dictionary<string, M_Gamemode> _gamemodeCache = new();
    
    private readonly CancellationTokenSource _sQuitCts = new();
    
    public AssetService(ModContext modContext)
    {
        _modContext = modContext ?? throw new ArgumentNullException(nameof(modContext));
        var assemblyPath = Assembly.GetExecutingAssembly().Location;
        _assemblyFolder = Path.GetDirectoryName(assemblyPath) ?? string.Empty;

        Application.quitting += () =>
        {
            WKLog.Debug("[AssetService] Quitting...");
            _sQuitCts.Cancel();
        };
    }
    
    #region Loading Asset Bundles
    
    /// <summary>
    /// Loads an <see cref="AssetBundle"/> given a path relative to the mod assembly folder.<br/>
    /// Caches under a key combining ModID and bundle name.
    /// <returns><see cref="AssetBundle"/> | null</returns>
    /// </summary>
    public async Task<AssetBundle> LoadBundleRelativeAsync(
        string relativePath,
        IProgress<float> progress = null)
    {
        progress?.Report(0f);
        if (string.IsNullOrEmpty(relativePath))
        {
            WKLog.Error("[AssetService] Path is null or empty.");
            progress?.Report(1f);
            return null;
        }

        var cacheKey = $"{_modContext.ModID}:{Path.GetFileNameWithoutExtension(relativePath)}";
        if (_bundleCache.TryGetValue(cacheKey, out var cached) && cached is not null)
        {
            WKLog.Debug($"[AssetService] Returning cached bundle: {cacheKey}");
            progress?.Report(1f);
            return cached;
        }

        var fullPath = Path.Combine(_assemblyFolder, relativePath);
        if (!File.Exists(fullPath))
        {
            WKLog.Error($"[AssetService] Bundle not found at {fullPath}");
            progress?.Report(1f);
            return null;
        }

        var request = AssetBundle.LoadFromFileAsync(fullPath);
        if (request is null)
        {
            WKLog.Error($"[AssetService] Failed to load bundle: {fullPath} (request)");
            progress?.Report(1f);
            return null;
        }

        try
        {
            while (!request.isDone)
            {
                _sQuitCts.Token.ThrowIfCancellationRequested();

                progress?.Report(request.progress * 1.1f);
                await Task.Yield();
            }
        }
        catch (OperationCanceledException)
        {
            WKLog.Debug($"[AssetService] Asset Bundle Load cancelled");
            return null;
        }
        
        var bundle = request.assetBundle;
        if (bundle is null)
        {
            WKLog.Error($"[AssetService] Failed to load bundle at {fullPath} (bundle)");
            progress?.Report(1f);
            return null;
        }

        _bundleCache[cacheKey] = bundle;
        WKLog.Debug($"[AssetService] Loaded & cached bundle: {cacheKey}");
        progress?.Report(1f);
        return bundle;
    }
    
    #endregion
    
    #region Unloading Asset Bundles
    
    /// <summary>
    /// Unloads a specific bundle for this mod.
    /// </summary>
    public void UnloadBundleRelative(string relativePath, bool unloadAllLoadedObjects = false)
    {
        var cacheKey = $"{_modContext.ModID}:{relativePath}";
        if (_bundleCache.TryGetValue(cacheKey, out var bundle) && bundle is not null)
        {
            try
            {
                bundle.Unload(unloadAllLoadedObjects);
            }
            catch
            {
                WKLog.Error($"[AssetService] Failed to unload bundle: {cacheKey}");
                return;
            }
            WKLog.Debug($"[AssetService] Unloaded bundle: {cacheKey}");
            _bundleCache.Remove(cacheKey);
        }
    }
    
    /// <summary>
    /// Unloads all bundles loaded by this mod.
    /// </summary>
    public void UnloadAllBundles(bool unloadAllLoadedObjects = false)
    {
        var keys = _bundleCache.Keys.Where(k => k.StartsWith(_modContext.ModID + ":")).ToList();
        foreach (var key in keys)
        {
            if (_bundleCache[key] is not null)
            {
                try
                {
                    _bundleCache[key].Unload(unloadAllLoadedObjects);
                }
                catch
                {
                    WKLog.Error($"[AssetService] Failed to unload bundle: {key}");
                    continue;
                }
                WKLog.Debug($"[AssetService] Unloaded bundle: {key}");
            }
            _bundleCache.Remove(key);
        }
    }
    
    #endregion
    
    #region Loading Levels
    
    /// <summary>
    /// Given a loaded AssetBundle, iterate over every ".prefab" asset name,
    /// Load it as a GameObject, check for an M_Level component; if found, add to the returned list.<br/>
    /// Also logs each M_Level found via WKLog.Debug("LevelName").<br/>
    /// If you want to register each prefab in CL_AssetManager.instance.assetDatabase.levelPrefabs,
    /// this method will do it automatically.
    /// </summary>
    public async Task<Dictionary<string, M_Level>> LoadAllLevelsFromBundle(
        AssetBundle bundle,
        IProgress<float> progress = null)
    {
        var foundLevels = new Dictionary<string, M_Level>();
        
        if (bundle is null)
        {
            WKLog.Error("[AssetService] LoadAllLevelsFromBundle called with null bundle.");
            progress?.Report(1f);
            return foundLevels;
        }

        var cacheKey = $"{_modContext.ModID}:{bundle.name}";

        if (_loadedLevelsCache.ContainsKey(cacheKey))
        {
            WKLog.Debug($"[AssetService] Returning Cached loaded levels for bundle: {cacheKey}");
            progress?.Report(1f);
            return _loadedLevelsCache.GetValueOrDefault(cacheKey);
        }

        var request = bundle.LoadAllAssetsAsync<GameObject>();
        try
        {
            while (!request.isDone)
            {
                _sQuitCts.Token.ThrowIfCancellationRequested();

                progress?.Report(request.progress * 0.5f);
                await Task.Yield();
            }
        }
        catch (OperationCanceledException)
        {
            WKLog.Debug($"[AssetService] LoadAllLevelsFromBundle canceled.");
            return null;
        }
        
        var allGOs = request.allAssets.Cast<GameObject>().ToList();
        var total = allGOs.Count;
        const int batchSize = 20;

        for (var i = 0; i < total; i += batchSize)
        {
            for (var j = i; j < i + batchSize && j < total; j++)
            {
                var go = allGOs[j];
                if (go.TryGetComponent<M_Level>(out var level))
                {
                    if (!CL_AssetManager.instance.assetDatabase.levelPrefabs.Contains(go))
                        CL_AssetManager.instance.assetDatabase.levelPrefabs.Add(go);
                
                    // Log its name for debugging
                    WKLog.Debug($"[AssetService] Found level: {level.name}");

                    if (!foundLevels.TryAdd(level.name, level))
                        WKLog.Debug($"[AssetService] Duplicate prefab.name '{level.name}' found; skipping the duplicate.");
                }
            }
            progress?.Report(0.5f + (i / (float)total) * 0.5f);
            await Task.Yield();
        }
        
        _loadedLevelsCache[cacheKey] = foundLevels;
        progress?.Report(1f);
        return foundLevels;
    }
    
    /// <summary>
    /// Returns all M_Level instances whose prefab names contain the given substring.
    /// (Uses CL_AssetManager.instance.assetDatabase.levelPrefabs)
    /// </summary>
    public List<M_Level> FindLevelsByName(string nameContains)
    {
        return CL_AssetManager.instance.assetDatabase.levelPrefabs
            .Where(prefab => prefab.name.Contains(nameContains))
            .Select(prefab => prefab.GetComponent<M_Level>())
            .Where(level => level is not null)
            .ToList();
    }
    
    #endregion
    
    #region Loading Gamemodes
    
    /// <summary>
    /// Loads a gamemode from an asset bundle
    /// </summary>
    /// <returns><see cref="M_Gamemode"/> | null</returns>
    public async Task<M_Gamemode> LoadGameModeFromBundle(
        AssetBundle bundle,
        string assetName,
        IProgress<float> progress = null)
    {
        if (bundle is null)
        {
            WKLog.Error("[AssetService] Bundle is null; cannot load gamemode.");
            progress?.Report(1f);
            return null;
        }

        var cacheKey = $"{_modContext.ModID}:{assetName}";
        
        if (_gamemodeCache.TryGetValue(cacheKey, out var cachedGamemode))
        {
            WKLog.Debug($"[AssetService] Returning cached gamemode: '{cacheKey}'");
            progress?.Report(1f);
            return cachedGamemode;
        }

        var request = bundle.LoadAssetAsync<ScriptableObject>(assetName);
        while (!request.isDone)
        {
            progress?.Report(request.progress);
            await Task.Yield();
        }
        
        var gamemode = request.asset as M_Gamemode;

        if (gamemode is null)
        {
            WKLog.Error($"[AssetService] Gamemode '{assetName}' not found in bundle {bundle.name}");
            progress?.Report(1f);
            return null;
        }
        
        if (string.IsNullOrEmpty(gamemode.unlockAchievement))
            gamemode.unlockAchievement = "ACH_TUTORIAL";
        
        gamemode.gamemodePanel = Resources.FindObjectsOfTypeAll<UI_GamemodeScreen_Panel>()
            .FirstOrDefault(x => x.name == "Gamemode_Panel_Base");
        gamemode.loseScreen = Resources.FindObjectsOfTypeAll<UI_ScoreScreen>()
            .FirstOrDefault(x => x.name == "ScorePanel_Standard_Death");
        gamemode.winScreen = Resources.FindObjectsOfTypeAll<UI_ScoreScreen>()
            .FirstOrDefault(x => x.name == "ScorePanel_Standard_Win");
        
        _gamemodeCache[cacheKey] = gamemode;
        progress?.Report(1f);
        WKLog.Debug($"[AssetService] Loaded and cached gamemode: '{assetName}'");
        return gamemode;
    }
    
    #endregion
    
    #region Helper Methods
    
    /// <summary>
    /// Loads a PNG as a Sprite relative to the mod folder.
    /// <returns><see cref="Sprite"/> | null </returns>
    /// </summary>
    public Sprite LoadPngAsSpriteRelative(string relativePngPath)
    {
        var fullPath = Path.Combine(_assemblyFolder, relativePngPath);
        if (!File.Exists(fullPath))
        {
            WKLog.Error($"[AssetService] PNG not found at: {fullPath}");
            return null;
        }
        var data = File.ReadAllBytes(fullPath);
        var tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        if (!tex.LoadImage(data))
        {
            WKLog.Error($"[AssetService] Failed to load image: {relativePngPath}");
            return null;
        }
        var sprite = Sprite.Create(tex, new Rect(0,0,tex.width,tex.height), new Vector2(0.5f,0.5f));
        sprite.name = Path.GetFileNameWithoutExtension(relativePngPath);
        return sprite;
    }
    
    /// <summary>
    /// Convenience: Create a Sprite from a PNG filename.
    /// </summary>
    public Sprite LoadPngAsSprite(string pngFileName)
    {
        var tex = LoadPngTexture(pngFileName);
        if (tex is null) return null;

        var sprite = Sprite.Create(
            tex,
            new Rect(0, 0, tex.width, tex.height),
            new Vector2(0.5f, 0.5f)
        );
        sprite.name = pngFileName.Split(".png")[0];
        return sprite;
    }
    
    /// <summary>
    /// Loads a PNG, returns as <see cref="Texture2D"/>.
    /// <returns><see cref="Texture2D"/> | null</returns>
    /// </summary>
    private Texture2D LoadPngTexture(string pngFileName)
    {
        var pngPath = Path.Combine(_assemblyFolder, "Assets", pngFileName);
        if (!File.Exists(pngPath))
        {
            WKLog.Error($"[AssetService] PNG not found at: {pngPath}");
            return null;
        }

        var data = File.ReadAllBytes(pngPath);
        var tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        
        if (tex.LoadImage(data)) return tex;
        
        WKLog.Error($"[AssetService] Failed to decode PNG: {pngPath}");
        return null;
    }
    
    #endregion
}