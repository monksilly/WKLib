using BepInEx;
using BepInEx.Logging;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using WKLib.Utilities;

namespace WKLib;

/// <summary>
/// Main Entry point of the whole Library
/// </summary>
[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class WkLib : BaseUnityPlugin
{
    private void Awake()
    {
        // Initialize Logger
        WKLog.Initialize(Logger);
        
        WKLog.Info($"Plugin {MyPluginInfo.PLUGIN_NAME} v{MyPluginInfo.PLUGIN_VERSION} is loaded!");

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "Main-Menu") return;

        var versionText = GameObject.Find("Canvas - Main Menu/Main Menu/Version Text").GetComponent<TextMeshProUGUI>();
        if (versionText is null) return;

        versionText.text += $" (wklib-{MyPluginInfo.PLUGIN_VERSION}) ({BepInEx.Bootstrap.Chainloader.PluginInfos.Count} Mods)";
    }
    
}
