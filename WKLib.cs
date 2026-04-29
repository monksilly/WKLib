using System.Reflection;
using BepInEx;
using HarmonyLib;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
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
    public const string VERSION = "0.2.1";

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
        if (scene.name == "Main-Menu")
        {
            var versionText = GameObject.Find("Canvas - Main Menu/Main Menu/Version Text")?.GetComponent<TextMeshProUGUI>();
            if (versionText is not null)
            {
                versionText.text += $" (wklib-{VERSION}) ({BepInEx.Bootstrap.Chainloader.PluginInfos.Count} Mods)";
            }
            
            var updateInfoGO = GameObject.Find("Canvas - Main Menu/Main Menu/Support Menu/Update Info");
            if (updateInfoGO != null)
            {
                GameObject toggleOverlayGO = Instantiate(updateInfoGO, updateInfoGO.transform.parent);
                toggleOverlayGO.transform.SetSiblingIndex(0);
                toggleOverlayGO.name = "Toggle Overlay";
                
                var uiMenuButton = toggleOverlayGO.GetComponent<UI_MenuButton>();
                if (uiMenuButton is not null)
                    DestroyImmediate(uiMenuButton);
                
                var textComponent = toggleOverlayGO.GetComponentInChildren<TextMeshProUGUI>();
                if (textComponent != null)
                {
                    textComponent.text = "Toggle Overlay";
                }

                var buttonComponent = toggleOverlayGO.GetComponent<Button>();
                if (buttonComponent != null)
                {
                    buttonComponent.onClick.RemoveAllListeners();
                    buttonComponent.onClick.AddListener(() =>
                        {
                            RootPanel.Instance.IsOpen = !RootPanel.Instance.IsOpen;
                            EventSystem.current.SetSelectedGameObject(null);
                        }
                    );
                }
            }
        }
        else
        {
            // If pause menu exists, we add our button
            var pauseLayout = GameObject.Find("GameManager/Canvas/Pause/Pause Menu/Pause Buttons/Pause Layout");
            if (pauseLayout != null)
            {
                var uiGap = pauseLayout.transform.Find("Gap.01");
                var settingsButton = pauseLayout.transform.Find("Settings");

                if (uiGap != null && settingsButton != null)
                {
                    // Create gap
                    GameObject uiGapGO = Instantiate(uiGap.gameObject, uiGap.transform.parent);
                    uiGapGO.transform.SetSiblingIndex(uiGap.parent.childCount - 1);
                    uiGapGO.name = "Gap.03";
                    
                    // Create button
                    GameObject toggleOverlayGO = Instantiate(settingsButton.gameObject, settingsButton.transform.parent);
                    toggleOverlayGO.transform.SetSiblingIndex(settingsButton.parent.childCount - 1);
                    toggleOverlayGO.name = "Toggle Overlay";
                
                    var uiMenuButton = toggleOverlayGO.GetComponent<UI_MenuButton>();
                    if (uiMenuButton is not null)
                        DestroyImmediate(uiMenuButton);
                    
                    var textComponent = toggleOverlayGO.GetComponentInChildren<TextMeshProUGUI>();
                    if (textComponent != null)
                    {
                        textComponent.text = "TOGGLE OVERLAY";
                    }

                    var buttonComponent = toggleOverlayGO.GetComponent<Button>();
                    if (buttonComponent != null)
                    {
                        buttonComponent.onClick.RemoveAllListeners();
                        buttonComponent.onClick.AddListener(() =>
                            {
                                RootPanel.Instance.IsOpen = !RootPanel.Instance.IsOpen;
                                EventSystem.current.SetSelectedGameObject(null);
                            }
                        );
                    }
                }
            }
        }
    }

    private void OnDestroy()
    {
        harmony?.UnpatchSelf();
        Destroy(RootPanel.Instance.ImuiPanel.Canvas);
        
        SceneManager.sceneLoaded -= OnSceneLoaded;
        WKLog.Info($"Plugin {NAME} unloaded!");
    }
}