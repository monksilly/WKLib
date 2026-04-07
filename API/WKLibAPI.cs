using System;
using System.Collections.Generic;
using WKLib.API.Assets;
using WKLib.API.Config;
using WKLib.API.UI;

namespace WKLib.API;

public class WKLibAPI
{
    internal static List<WKLibAPI> internalAPIs = new List<WKLibAPI>();

    public string DisplayName = string.Empty;
    public string GUID = string.Empty;

    public List<WKLibWindow> Windows = new List<WKLibWindow>();
    public ConfigFolder ConfigFolder = null;
    public ConfigFile DefaultConfigFile = null;

    public AssetService AssetService = null;
    
    public WKLibAPI(string displayName, string guid)
    {
        DisplayName = displayName;
        GUID = guid;
        
        //ConfigFolder = new ConfigFolder(displayName);
        //DefaultConfigFile = Config.ConfigFolder.GetOrCreateConfigFile(guid);
        AssetService = new AssetService(this);
    }

    /// <summary>
    /// Create a new configurator. Use one instance throughout the session
    /// </summary>
    /// <param name="displayName">Name of the plugin, displayName will be set to this</param>
    /// <param name="guid">ID of the plugin, guid will be set to this</param>
    public static WKLibAPI Create(string displayName, string guid)
    {
        foreach(WKLibAPI API in internalAPIs)
        {
            if (string.Equals(guid, API.GUID))
                throw new Exception($"{displayName} collides with {API.DisplayName}, they both have the same guid, {guid}");
        }

        WKLibAPI newAPI = new WKLibAPI(displayName, guid);
        internalAPIs.Add(newAPI);
        return newAPI;
    }

    public void AddWindow(WKLibWindow window)
    {
        if (Windows.Contains(window))
            return;

        Windows.Add(window);
    }
    
    public void Destroy()
    {
        if (internalAPIs.Contains(this))
            internalAPIs.Remove(this);
    }
}