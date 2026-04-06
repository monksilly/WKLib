using BepInEx;
using BepInEx.Logging;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using WKLib.Utilities;
using WKLib.Core;

namespace WKLib;

/// <summary>
/// Main Entry point of the whole Library
/// </summary>
[BepInPlugin(GUID, NAME, VERSION)]
public class WkLib : BaseUnityPlugin
{
    public const string GUID = "com.monksilly.WKLib";
    public const string NAME = "WKLib";
    public const string VERSION = "0.0.2";
    
    private void Awake()
    {
        // Initialize Logger
        WKLog.Initialize(Logger);
        
        WKLog.Info($"Plugin {NAME} v{VERSION} is loaded!");
        
        // Initialize UI System
        UIManager.TryInitialize();

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "Main-Menu") return;

        var versionText = GameObject.Find("Canvas - Main Menu/Main Menu/Version Text").GetComponent<TextMeshProUGUI>();
        if (versionText is null) return;

        versionText.text += $" (wklib-{VERSION}) ({BepInEx.Bootstrap.Chainloader.PluginInfos.Count} Mods)";
    }
    
}
