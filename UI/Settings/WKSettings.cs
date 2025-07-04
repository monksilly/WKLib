using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using HarmonyLib;
using UnityEngine;
using System.Linq;
using Newtonsoft.Json;
using WKLib.Utilities;

namespace WKLib.UI.Settings;

/// <summary>
/// Unified settings management system for WKLib
/// Handles custom settings registration and persistence via Newtonsoft(I love this now!!!)
/// </summary>
public static class WKSettings
{
    private static Dictionary<string, SettingInfo> _customSettings = new Dictionary<string, SettingInfo>();
    private static Dictionary<string, object> _runtimeValues = new Dictionary<string, object>();
    private static bool _isInitialized = false;

    public class SettingInfo
    {
        public string Name;
        public Type Type;
        public object DefaultValue;
        public object CurrentValue;
    }

    /// <summary>
    /// Initialize the settings system
    /// </summary>
    public static void Initialize()
    {
        if (_isInitialized) return;

        try
        {
            var harmony = new Harmony("wklib.settings");
            ApplyHarmonyPatches(harmony);
            LoadCustomSettings(); // Load existing settings on initialization
            _isInitialized = true;
            WKLog.Info("WKSettings: Initialized successfully");
        }
        catch (Exception ex)
        {
            WKLog.Error($"WKSettings: Failed to initialize: {ex.Message}");
        }
    }

    /// <summary>
    /// Register a new custom setting
    /// </summary>
    public static void RegisterSetting<T>(string settingName, T defaultValue)
    {
        if (_customSettings.ContainsKey(settingName))
        {
            WKLog.Warn($"WKSettings: Setting '{settingName}' already registered");
            return;
        }

        var settingInfo = new SettingInfo
        {
            Name = settingName,
            Type = typeof(T),
            DefaultValue = defaultValue,
            CurrentValue = defaultValue
        };

        _customSettings[settingName] = settingInfo;
        
        // Check if there is already a saved value for this setting
        if (_runtimeValues.ContainsKey(settingName))
        {
            try
            {
                // Convert the loaded value to the correct type
                object loadedValue = _runtimeValues[settingName];
                T convertedValue;
                
                if (loadedValue is T directValue)
                {
                    convertedValue = directValue;
                }
                else
                {
                    // Try to convert from string or other types
                    convertedValue = (T)ConvertValueToType(loadedValue.ToString(), typeof(T));
                }
                
                settingInfo.CurrentValue = convertedValue;
                _runtimeValues[settingName] = convertedValue;
                WKLog.Info($"WKSettings: Loaded saved value '{convertedValue}' for setting '{settingName}'");
            }
            catch (Exception ex)
            {
                WKLog.Warn($"WKSettings: Failed to load saved value for '{settingName}': {ex.Message}, using default");
                _runtimeValues[settingName] = defaultValue;
                settingInfo.CurrentValue = defaultValue;
            }
        }
        else
        {
            _runtimeValues[settingName] = defaultValue;
        }

        WKLog.Info($"WKSettings: Registered setting '{settingName}' with value '{settingInfo.CurrentValue}'");
    }

    /// <summary>
    /// Get a custom setting value
    /// </summary>
    public static T GetSetting<T>(string settingName, T fallback = default(T))
    {
        if (_runtimeValues.TryGetValue(settingName, out object value))
        {
            try
            {
                return (T)Convert.ChangeType(value, typeof(T));
            }
            catch
            {
                return fallback;
            }
        }
        return fallback;
    }

    /// <summary>
    /// Set a custom setting value
    /// </summary>
    public static void SetSetting<T>(string settingName, T value)
    {
        if (_customSettings.ContainsKey(settingName))
        {
            _customSettings[settingName].CurrentValue = value;
            _runtimeValues[settingName] = value;
            
            // Auto-save when settings change
            SaveCustomSettings();
        }
        else
        {
            WKLog.Warn($"WKSettings: Attempted to set unregistered setting '{settingName}'");
        }
    }

    /// <summary>
    /// Check if a setting is a custom setting
    /// </summary>
    public static bool IsCustomSetting(string settingName)
    {
        return _customSettings.ContainsKey(settingName);
    }

    /// <summary>
    /// Get all registered custom settings (for debugging)
    /// </summary>
    public static Dictionary<string, SettingInfo> GetAllCustomSettings()
    {
        return new Dictionary<string, SettingInfo>(_customSettings);
    }

    #region Harmony Patches
    private static void ApplyHarmonyPatches(Harmony harmony)
    {
        try
        {
            var originalGetSetting = typeof(global::SettingsManager).GetMethod("GetSetting", new Type[] { typeof(string) });
            var originalSetSetting = typeof(global::SettingsManager).GetMethod("SetSetting", new Type[] { typeof(string[]) });
            var originalSaveSettings = typeof(global::SettingsManager).GetMethod("SaveSettings");
            var originalLoadSettings = typeof(global::SettingsManager).GetMethod("LoadSettings");

            harmony.Patch(originalGetSetting, prefix: new HarmonyMethod(typeof(WKSettings), nameof(GetSetting_Patch)));
            harmony.Patch(originalSetSetting, prefix: new HarmonyMethod(typeof(WKSettings), nameof(SetSetting_Patch)));
            harmony.Patch(originalSaveSettings, postfix: new HarmonyMethod(typeof(WKSettings), nameof(SaveSettings_Patch)));
            harmony.Patch(originalLoadSettings, postfix: new HarmonyMethod(typeof(WKSettings), nameof(LoadSettings_Patch)));
        }
        catch (Exception ex)
        {
            WKLog.Error($"WKSettings: Failed to apply Harmony patches: {ex.Message}");
        }
    }

    private static bool GetSetting_Patch(string variableName, ref string result)
    {
        if (IsCustomSetting(variableName))
        {
            result = _runtimeValues[variableName]?.ToString() ?? _customSettings[variableName].DefaultValue?.ToString();
            return false;
        }
        return true;
    }

    private static bool SetSetting_Patch(string[] args)
    {
        if (args.Length >= 2 && IsCustomSetting(args[0]))
        {
            try
            {
                var settingInfo = _customSettings[args[0]];
                object convertedValue = ConvertValueToType(args[1], settingInfo.Type);
                settingInfo.CurrentValue = convertedValue;
                _runtimeValues[args[0]] = convertedValue;
                SaveCustomSettings();
            }
            catch (Exception ex)
            {
                WKLog.Error($"WKSettings: Failed to set '{args[0]}': {ex.Message}");
            }
            return false;
        }
        return true;
    }

    private static void SaveSettings_Patch()
    {
        SaveCustomSettings();
    }

    private static void LoadSettings_Patch()
    {
        LoadCustomSettings();
    }
    #endregion

    #region Persistence
    private static string GetCustomSettingsPath()
    {
        return Path.Combine(Application.persistentDataPath, "WKLib_Settings.json");
    }

    private static void SaveCustomSettings()
    {
        try
        {
            var saveData = new SaveData();
            foreach (var kvp in _customSettings)
            {
                var entry = new SettingEntry
                {
                    key = kvp.Key,
                    value = kvp.Value.CurrentValue?.ToString() ?? "",
                    type = kvp.Value.Type.FullName
                };
                saveData.settings.Add(entry);
            }

            string json = JsonConvert.SerializeObject(saveData, Formatting.Indented);
            File.WriteAllText(GetCustomSettingsPath(), json);
            WKLog.Info($"WKSettings: Saved {_customSettings.Count} custom settings");
        }
        catch (Exception ex)
        {
            WKLog.Error($"WKSettings: Failed to save custom settings: {ex.Message}");
        }
    }

    private static void LoadCustomSettings()
    {
        try
        {
            string path = GetCustomSettingsPath();
            if (!File.Exists(path))
            {
                WKLog.Info("WKSettings: No custom settings file found, using defaults");
                return;
            }

            string json = File.ReadAllText(path);
            var saveData = JsonConvert.DeserializeObject<SaveData>(json);

            if (saveData?.settings != null)
            {
                foreach (var entry in saveData.settings)
                {
                    try
                    {
                        Type type = Type.GetType(entry.type);
                        if (type != null)
                        {
                            object value = ConvertValueToType(entry.value, type);
                            _runtimeValues[entry.key] = value;
                        }
                    }
                    catch (Exception ex)
                    {
                        WKLog.Warn($"WKSettings: Failed to load setting '{entry.key}': {ex.Message}");
                    }
                }
                WKLog.Info($"WKSettings: Loaded {saveData.settings.Count} custom settings from file");
            }
        }
        catch (Exception ex)
        {
            WKLog.Error($"WKSettings: Failed to load custom settings: {ex.Message}");
        }
    }

    private static object ConvertValueToType(string valueString, Type targetType)
    {
        if (targetType == typeof(bool))
            return bool.Parse(valueString);
        else if (targetType == typeof(int))
            return int.Parse(valueString);
        else if (targetType == typeof(float))
            return float.Parse(valueString);
        else if (targetType == typeof(string))
            return valueString;
        else
            return Convert.ChangeType(valueString, targetType);
    }

    [Serializable]
    private class SaveData
    {
        public List<SettingEntry> settings = new List<SettingEntry>();
    }

    [Serializable]
    private class SettingEntry
    {
        public string key = "";
        public string value = "";
        public string type = "";
    }
    #endregion
} 