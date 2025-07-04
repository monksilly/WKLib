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
public class WkDropdown : WkComponent
{
    public int DefaultValue { get; private set; }
    public List<string> Options { get; private set; }

    private Action<WkDropdown> _onValueChanged;

    /// <summary>
    /// Create a dropdown with a parent column enum
    /// </summary>
    public WkDropdown(UIColumn parentColumn, string settingName, string displayName, 
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

        WkSettings.RegisterSetting(SettingName, DefaultValue);
        
        RemoveOldBinders<DropdownBinder>();
        
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
        WkSettings.SetSetting(SettingName, value);
        _onValueChanged?.Invoke(this);
    }

    /// <summary>
    /// Get current dropdown value (index)
    /// </summary>
    public int GetValue()
    {
        int settingValue = WkSettings.GetSetting(SettingName, DefaultValue);
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
    public WkDropdown SetValue(int value)
    {
        value = Mathf.Clamp(value, 0, Options.Count - 1);
        WkSettings.SetSetting(SettingName, value);
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
    public WkDropdown SetValue(string text)
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
    public WkDropdown SetOptions(List<string> newOptions)
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
    public WkDropdown SetListener(Action<WkDropdown> listener)
    {
        _onValueChanged = listener;
        if (GameObject != null)
        {
            _onValueChanged?.Invoke(this);
        }
        return this;
    }
} 