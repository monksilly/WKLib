using System;
using UnityEngine;
using UnityEngine.UI;
using WKLib.Core;
using WKLib.UI.Settings;
using WKLib.Utilities;

namespace WKLib.UI.Components;

/// <summary>
/// Toggle component 
/// </summary>
public class WKToggle : WKComponent
{
    public bool DefaultValue { get; private set; }

    private Action<WKToggle> _onValueChanged;
    private Toggle _toggleComponent;

    /// <summary>
    /// Create a toggle with a parent column enum
    /// </summary>
    public WKToggle(UIColumn parentColumn, string settingName, string displayName, bool defaultValue = false)
        : base(parentColumn, settingName, displayName)
    {
        DefaultValue = defaultValue;
    }

    protected override string GetTemplatePath()
    {
        return UIManager.ToggleTemplatePath;
    }

    protected override void Configure()
    {
        if (GameObject == null) return;

        WKSettings.RegisterSetting(SettingName, DefaultValue);
        
        RemoveOldBinders<ToggleSettingsBinder>();
        
        _toggleComponent = GameObject.GetComponentInChildren<Toggle>();
        if (_toggleComponent == null)
        {
            WKLog.Error($"WkToggle: No Toggle found in {GameObject.name}");
            return;
        }

        _toggleComponent.onValueChanged.RemoveAllListeners();
        
        bool currentValue = GetValue();
        _toggleComponent.SetIsOnWithoutNotify(currentValue);

        _toggleComponent.onValueChanged.AddListener(OnValueChanged);

        SetLabelText();

        _onValueChanged?.Invoke(this);

        WKLog.Info($"WkToggle: Configured '{SettingName}' successfully");
    }

    private void OnValueChanged(bool value)
    {
        WKSettings.SetSetting(SettingName, value);
        _onValueChanged?.Invoke(this);
    }

    /// <summary>
    /// Set a custom listener for value changes - receives the WkToggle instance
    /// </summary>
    public WKToggle SetListener(Action<WKToggle> listener)
    {
        _onValueChanged = listener;
        if (GameObject != null)
        {
            _onValueChanged?.Invoke(this);
        }
        return this;
    }

    /// <summary>
    /// Get current toggle value
    /// </summary>
    public bool GetValue()
    {
        return WKSettings.GetSetting(SettingName, DefaultValue);
    }

    /// <summary>
    /// Set toggle value
    /// </summary>
    public WKToggle SetValue(bool value)
    {
        WKSettings.SetSetting(SettingName, value);
        if (GameObject != null)
        {
            _toggleComponent.SetIsOnWithoutNotify(value);
        }
        return this;
    }
} 