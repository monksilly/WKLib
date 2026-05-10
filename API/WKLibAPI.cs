using System;
using System.Collections.Generic;
using WKLib.API.Assets;
using WKLib.API.Config;
using WKLib.API.UI;

namespace WKLib.API;

public class WKLibAPI
{
    internal static List<WKLibAPI> internalAPIs = new List<WKLibAPI>();

    public string DisplayName { get; internal set; } = string.Empty;
    public string GUID { get; internal set; } = string.Empty;

    public List<WKLibWindow> Windows { get; internal set; } = new List<WKLibWindow>();
    public ModTab ModTab { get; internal set; } = null;
    public ConfigFolder ConfigFolder { get; internal set; } = null;
    public ConfigFile DefaultConfigFile { get; internal set; } = null;

    public AssetService AssetService { get; internal set; } = null;
    
    private WKLibAPI(string displayName, string guid, string defaultConfigFileName)
    {
        DisplayName = displayName;
        GUID = guid;
        
        ConfigFolder = new ConfigFolder(displayName);
        DefaultConfigFile = ConfigFolder.GetOrCreateConfigFile(defaultConfigFileName);
        AssetService = new AssetService(this);
    }

    /// <summary>
    /// Creates and registers a new API instance.
    /// </summary>
    /// <param name="displayName">The plugin display name.</param>
    /// <param name="guid">The plugin GUID.</param>
    /// <returns>The created API instance.</returns>
    public static WKLibAPI Create(string displayName, string guid)
    {
        return Create_Internal(displayName, guid);
    }
    
    /// <summary>
    /// Creates and registers a new API instance.
    /// </summary>
    /// <param name="displayName">The plugin display name.</param>
    /// <param name="guid">The plugin GUID.</param>
    /// <param name="defaultConfigFileName">The default config file name.</param>
    /// <returns>The created API instance.</returns>
    public static WKLibAPI Create(string displayName, string guid, string defaultConfigFileName)
    {
        return Create_Internal(displayName, guid, defaultConfigFileName);
    }
    
    private static WKLibAPI Create_Internal(string displayName, string guid, string defaultConfigFileName = "DefaultConfig")
    {
        foreach(WKLibAPI API in internalAPIs)
        {
            if (string.Equals(guid, API.GUID))
                throw new Exception($"{displayName} collides with {API.DisplayName}, they both have the same guid, {guid}");
        }

        WKLibAPI newAPI = new WKLibAPI(displayName, guid, defaultConfigFileName);
        internalAPIs.Add(newAPI);
        internalAPIs.Sort((a, b) => string.Compare(a.DisplayName, b.DisplayName, StringComparison.OrdinalIgnoreCase));
        return newAPI;
    }

    public void AddWindow(WKLibWindow window)
    {
        if (Windows.Contains(window))
            return;

        Windows.Add(window);
    }

    public void AddToModList(ModTab modTab)
    {
        if (ModTab != null)
            throw new Exception($"Mod tab already exists, cant add new one");
        
        ModTab = modTab;
    }
    
    public void Destroy()
    {
        if (internalAPIs.Contains(this))
            internalAPIs.Remove(this);
    }
}