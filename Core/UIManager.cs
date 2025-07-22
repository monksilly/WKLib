using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;
using System.Collections.Generic;
using System;
using WKLib.UI.Components;
using WKLib.Utilities;
using WKLib.UI.Settings;

namespace WKLib.Core;

/// <summary>
/// UI Column types for easy reference
/// </summary>
public enum UIColumn
{
    AccessibilityInterface,
    AccessibilityVisuals,
    AccessibilityOther,
    VideoScreenInfo,
    VideoGraphics,
    VideoAudio,
    ControlsCamera,
    ControlsToggles
}

/// <summary>
/// Core UI management system for WKLib.<br/>
/// Dynamically finds UI paths at runtime to be resilient to hierarchy changes.<br/>
/// Handles scene transitions and persistent component recreation.
/// </summary>
public static class UIManager
{
    #region Template Paths
    public static string SliderTemplatePath { get; private set; }
    public static string ToggleTemplatePath { get; private set; }
    public static string DropdownTemplatePath { get; private set; }
    public static string SeparatorTemplatePath { get; private set; }    
    #endregion
    #region Column Paths
    public static string AccessibilityInterfaceColumn { get; private set; }
    public static string AccessibilityVisualsColumn { get; private set; }
    public static string AccessibilityOtherColumn { get; private set; }
    public static string VideoScreenInfoColumn { get; private set; }
    public static string VideoGraphicsColumn { get; private set; }
    public static string VideoAudioColumn { get; private set; }
    public static string ControlsCameraColumn { get; private set; }
    public static string ControlsTogglesColumn { get; private set; }
    #endregion

    private static bool _initialized = false;
    private static string _currentSceneName = "";
    
    private static readonly List<IWKComponent> RegisteredComponents = new();
    private static readonly List<Action> RegisteredActions = new();
    
    private static readonly List<IWKComponent> PendingComponents = new();
    private static readonly List<Action> PendingActions = new();

    static UIManager()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    /// <summary>
    /// Handle scene changes - reinitialize if in a compatible scene
    /// </summary>
    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        string sceneName = scene.name;
        _currentSceneName = sceneName;
        
        WKLog.Info($"UIManager: Scene loaded - {sceneName}");
        
        bool isCompatibleScene = sceneName == "Main-Menu" || sceneName == "Game-Main";
        
        if (isCompatibleScene)
        {
            WKLog.Info($"UIManager: Compatible scene detected, attempting initialization...");
            
            _initialized = false;
            
            foreach (var component in RegisteredComponents)
            {
                component.Reset();
            }
            
            PendingComponents.AddRange(RegisteredComponents);
            PendingActions.AddRange(RegisteredActions);
            
            TryInitialize();
        }
        else
        {
            WKLog.Info($"UIManager: Non-compatible scene, UI components will wait for next compatible scene");
            _initialized = false;
        }
    }

    /// <summary>
    /// Registers a UI component with the UIManager for persistence across scene transitions and ensures its associated GameObject is created when the UI is ready.
    /// </summary>
    /// <param name="component">The component to be registered, implementing the <see cref="IWKComponent"/> interface.</param>
    public static void RegisterComponent(IWKComponent component)
    {
        if (!RegisteredComponents.Contains(component))
        {
            RegisteredComponents.Add(component);
        }
        
        ExecuteWhenReady(component.CreateGameObject);
    }

    /// <summary>
    /// Execute an action when the UI is ready
    /// </summary>
    public static void ExecuteWhenReady(Action action)
    {
        if (!RegisteredActions.Contains(action))
        {
            RegisteredActions.Add(action);
        }
        
        if (_initialized)
        {
            action?.Invoke();
        }
        else
        {
            if (!PendingActions.Contains(action))
            {
                PendingActions.Add(action);
            }
        }
    }

    /// <summary>
    /// Try to initialize - called from scene loading and can be called manually
    /// </summary>
    public static bool TryInitialize()
    {
        if (_initialized) return true;

        _currentSceneName = SceneManager.GetActiveScene().name;
        
        if (!IsValidUIScene(_currentSceneName))
        {
            return false;
        }

        Initialize();
        return _initialized;
    }

    private static bool IsValidUIScene(string sceneName)
    {
        return sceneName == "Main-Menu" || sceneName == "Game-Main";
    }

    /// <summary>
    /// Initialize the UIManager by finding all UI template paths
    /// </summary>
    public static void Initialize()
    {
        if (_initialized) return;

        // Initialize the settings system
        WKSettings.Initialize();

        var accessibilitySettingsPath = FindObjectPath("Accessibility Settings");
        var videoSettingsPath = FindObjectPath("Video Settings");
        var controlsPagePath = FindObjectPath("Controls Page");

        if (string.IsNullOrEmpty(accessibilitySettingsPath) || 
            string.IsNullOrEmpty(videoSettingsPath) || 
            string.IsNullOrEmpty(controlsPagePath))
        {
            WKLog.Warn($"UIManager: Could not find UI template paths in scene '{_currentSceneName}'. Will retry when entering compatible scene.");
            return;
        }

        BuildPaths(accessibilitySettingsPath, videoSettingsPath, controlsPagePath);

        _initialized = true;
        WKLog.Info($"UIManager: Successfully initialized in scene '{_currentSceneName}'");

        foreach (var component in PendingComponents)
        {
            try
            {
                component.CreateGameObject();
            }
            catch (Exception ex)
            {
                WKLog.Error($"UIManager: Error creating pending component: {ex.Message}");
            }
        }
        PendingComponents.Clear();

        foreach (var action in PendingActions)
        {
            try
            {
                action?.Invoke();
            }
            catch (Exception ex)
            {
                WKLog.Error($"UIManager: Error executing pending action: {ex.Message}");
            }
        }
        PendingActions.Clear();
        
        WKLog.Info($"UIManager: Processed all pending components and actions");
    }

    /// <summary>
    /// Build all UI paths using the discovered template paths
    /// </summary>
    private static void BuildPaths(string accessibilityPath, string videoPath, string controlsPath)
    {
        AccessibilityInterfaceColumn = $"{accessibilityPath}/Options Tab/Interface Column";
        AccessibilityVisualsColumn = $"{accessibilityPath}/Options Tab/Accessibility";
        AccessibilityOtherColumn = $"{accessibilityPath}/Options Tab/Other Column";

        VideoScreenInfoColumn = $"{videoPath}/Options Tab/Video";
        VideoGraphicsColumn = $"{videoPath}/Options Tab/Other";
        VideoAudioColumn = $"{videoPath}/Options Tab/Audio";

        ControlsCameraColumn = $"{controlsPath}/Options Tab/Column 01";
        ControlsTogglesColumn = $"{controlsPath}/Options Tab/Toggles";

        SliderTemplatePath = $"{AccessibilityInterfaceColumn}/SliderAsset - UI Scale";
        ToggleTemplatePath = $"{AccessibilityVisualsColumn}/Item High Visibility";
        DropdownTemplatePath = $"{VideoScreenInfoColumn}/Screen Resolution";
        SeparatorTemplatePath = $"{VideoAudioColumn}/--- (1).01";
    }

    /// <summary>
    /// Find a unique object by name in the scene and return its full hierarchy path
    /// </summary>
    private static string FindObjectPath(string uniqueName)
    {
        foreach (var root in SceneManager.GetActiveScene().GetRootGameObjects())
        {
            var target = root.GetComponentsInChildren<Transform>(true).FirstOrDefault(t => t.gameObject.name == uniqueName);
            if (target != null)
            {
                var path = new LinkedList<string>();
                var current = target;
                while (current != null)
                {
                    path.AddFirst(current.name);
                    current = current.parent;
                }
                return string.Join("/", path);
            }
        }
        return null;
    }
    
    /// <summary>
    /// Check if the UI Manager has been initialized and paths are ready
    /// </summary>
    public static bool IsUIReady() => _initialized;

    /// <summary>
    /// Resolve a UIColumn enum to its actual path
    /// </summary>
    public static string GetColumnPath(UIColumn column)
    {
        if (!_initialized)
        {
            WKLog.Warn($"UIManager: Attempted to resolve {column} before initialization");
            return null;
        }

        return column switch
        {
            UIColumn.AccessibilityInterface => AccessibilityInterfaceColumn,
            UIColumn.AccessibilityVisuals => AccessibilityVisualsColumn,
            UIColumn.AccessibilityOther => AccessibilityOtherColumn,
            UIColumn.VideoScreenInfo => VideoScreenInfoColumn,
            UIColumn.VideoGraphics => VideoGraphicsColumn,
            UIColumn.VideoAudio => VideoAudioColumn,
            UIColumn.ControlsCamera => ControlsCameraColumn,
            UIColumn.ControlsToggles => ControlsTogglesColumn,
            _ => throw new ArgumentException($"Unknown UIColumn: {column}")
        };
    }

    /// <summary>
    /// Find a UI element's Transform using its full hierarchy path
    /// </summary>
    public static Transform FindUIPath(string path)
    {
        if (!IsUIReady())
        {
            WKLog.Warn($"UIManager.FindUIPath called before UI was ready. Path: {path}");
            return null;
        }
        if (string.IsNullOrEmpty(path)) return null;
        
        var go = GameObject.Find(path);
        if (go == null)
        {
            WKLog.Warn($"UIManager: Could not find object at path: {path}");
        }
        return go?.transform;
    }

    /// <summary>
    /// Find a template GameObject using its full hierarchy path
    /// </summary>
    public static GameObject FindTemplate(string templatePath)
    {
        if (!IsUIReady())
        {
            WKLog.Warn($"UIManager.FindTemplate called before UI was ready. Path: {templatePath}");
            return null;
        }
        if (string.IsNullOrEmpty(templatePath)) return null;

        return GameObject.Find(templatePath);
    }
} 