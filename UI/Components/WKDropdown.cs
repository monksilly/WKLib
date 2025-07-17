using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using WKLib.Core;
using WKLib.UI.Settings;
using WKLib.Utilities;

namespace WKLib.UI.Components;

/// <summary>
/// Dropdown component
/// </summary>
public class WKDropdown : WKComponent
{
    public int DefaultValue { get; private set; }
    public List<string> Options { get; private set; }

    private Action<WKDropdown> _onValueChanged;

    /// <summary>
    /// Create a dropdown with a parent column enum
    /// </summary>
    public WKDropdown(UIColumn parentColumn, string settingName, string displayName, 
                     List<string> options, int defaultValue = 0)
        : base(parentColumn, settingName, displayName)
    {
        Options = options ?? new List<string>();
        DefaultValue = Mathf.Clamp(defaultValue, 0, Options.Count - 1);
    }

    protected override string GetTemplatePath()
    {
        return UIManager.DropdownTemplatePath;
    }

    protected override void Configure()
    {
        if (GameObject == null) return;

        WKSettings.RegisterSetting(SettingName, DefaultValue);
        
        RemoveOldBinders<DropdownBinder>();
        RemoveOldBinders<Settings_Resolution>();
        
        var dropdown = GameObject.GetComponentInChildren<TMP_Dropdown>();
        if (dropdown == null)
        {
            WKLog.Error($"WkDropdown: No TMP_Dropdown found in {GameObject.name}");
            return;
        }

        dropdown.onValueChanged.RemoveAllListeners();
        
        dropdown.ClearOptions();
        dropdown.AddOptions(Options);
        
        int currentValue = GetValue();
        dropdown.SetValueWithoutNotify(currentValue);
        
        dropdown.onValueChanged.AddListener(OnValueChanged);

        SetLabelText();

        _onValueChanged?.Invoke(this);

        WKLog.Info($"WkDropdown: Configured '{SettingName}' successfully");
    }

    private void OnValueChanged(int value)
    {
        WKSettings.SetSetting(SettingName, value);
        _onValueChanged?.Invoke(this);
    }

    /// <summary>
    /// Get current dropdown value (index)
    /// </summary>
    public int GetValue()
    {
        int settingValue = WKSettings.GetSetting(SettingName, DefaultValue);
        return Mathf.Clamp(settingValue, 0, Options.Count - 1);
    }

    /// <summary>
    /// Get current dropdown text
    /// </summary>
    public string GetText()
    {
        int index = GetValue();
        return (index >= 0 && index < Options.Count) ? Options[index] : "";
    }

    /// <summary>
    /// Set dropdown value by index
    /// </summary>
    public WKDropdown SetValue(int value)
    {
        value = Mathf.Clamp(value, 0, Options.Count - 1);
        WKSettings.SetSetting(SettingName, value);
        if (GameObject != null)
        {
            var dropdown = GameObject.GetComponentInChildren<TMP_Dropdown>();
            dropdown?.SetValueWithoutNotify(value);
        }
        return this;
    }

    /// <summary>
    /// Set dropdown value by text
    /// </summary>
    public WKDropdown SetValue(string text)
    {
        int index = Options.IndexOf(text);
        if (index >= 0)
        {
            SetValue(index);
        }
        return this;
    }

    /// <summary>
    /// Update the dropdown options
    /// </summary>
    public WKDropdown SetOptions(List<string> newOptions)
    {
        Options = newOptions ?? new List<string>();
        if (GameObject != null)
        {
            var dropdown = GameObject.GetComponentInChildren<TMP_Dropdown>();
            if (dropdown != null)
            {
                dropdown.ClearOptions();
                dropdown.AddOptions(Options);
                
                int currentValue = GetValue();
                dropdown.SetValueWithoutNotify(currentValue);
            }
        }
        return this;
    }

    /// <summary>
    /// Set a custom listener for value changes - receives the WkDropdown instance
    /// </summary>
    public WKDropdown SetListener(Action<WKDropdown> listener)
    {
        _onValueChanged = listener;
        if (GameObject != null)
        {
            _onValueChanged?.Invoke(this);
        }
        return this;
    }

    /// <summary>
    /// Override SetLabelText to look for the label on the parent GameObject (SettingsResolution)
    /// instead of searching in children
    /// </summary>
    protected new void SetLabelText()
    {
        if (GameObject == null)
        {
            WKLog.Warn($"WkDropdown: Cannot set label text - GameObject is null for '{SettingName}'");
            return;
        }

        if (string.IsNullOrEmpty(LabelText))
        {
            WKLog.Warn($"WkDropdown: LabelText is null or empty for '{SettingName}'");
            return;
        }
        
        // Look for TextMeshPro component on the parent GameObject (SettingsResolution)
        var tmpComponent = GameObject.GetComponent<TMPro.TextMeshProUGUI>();
        if (tmpComponent != null)
        {
            Label = tmpComponent;
            Label.text = LabelText;
            WKLog.Info($"WkDropdown: Set label to '{LabelText}' for '{SettingName}' on parent GameObject");
        }
        else
        {
            WKLog.Warn($"WkDropdown: No TextMeshPro component found on parent GameObject for '{SettingName}'");
        }
    }
} 