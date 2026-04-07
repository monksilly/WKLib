using System.Reflection;
using BepInEx;
using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using WKLib.Core.Attributes;
using WKLib.Core.UI;
using WKLib.Utilities;

namespace WKLib;

/// <summary>
/// Main Entry point of the whole Library
/// </summary>
[BepInDependency(ImuiBepInEx.Plugin.GUID, BepInDependency.DependencyFlags.HardDependency)]
[BepInPlugin(GUID, NAME, VERSION)]
public class WKLibPlugin : BaseUnityPlugin
{
    public const string GUID = "com.monksilly.WKLib";
    public const string NAME = "WKLib";
    public const string VERSION = "0.1.0";

    private static Harmony harmony = null;
    
    private void Awake()
    {
        // Initialize Logger
        WKLog.Initialize(Logger);
        
        harmony = new Harmony(GUID);
        
        foreach (var type in typeof(WKLibPlugin).Assembly.GetTypes())
        {
            if (type.GetCustomAttribute<PatchOnEntryAttribute>() != null)
                harmony.PatchAll(type);
        }
        
        WKLog.Info($"Plugin {NAME} v{VERSION} is loaded!");
        
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "Main-Menu") return;

        var versionText = GameObject.Find("Canvas - Main Menu/Main Menu/Version Text").GetComponent<TextMeshProUGUI>();
        if (versionText is null) return;

        versionText.text += $" (wklib-{VERSION}) ({BepInEx.Bootstrap.Chainloader.PluginInfos.Count} Mods)";
    }

    private void OnDestroy()
    {
        harmony?.UnpatchSelf();
        Destroy(RootPanel.Instance.ImuiPanel.Canvas);
        
        SceneManager.sceneLoaded -= OnSceneLoaded;
        WKLog.Info($"Plugin {NAME} unloaded!");
    }
}