using System;
using DarkMachine.UI;
using UnityEngine;
using UnityEngine.UI;
using WKLib.Core;
using WKLib.UI.Settings;
using TMPro;
using WKLib.Utilities;

namespace WKLib.UI.Components;

/// <summary>
/// Slider component
/// </summary>
public class WkSlider : WkComponent
{
    public float DefaultValue { get; private set; }
    public float MinValue { get; private set; }
    public float MaxValue { get; private set; }

    private Action<WkSlider> _onValueChanged;

    /// <summary>
    /// Create a slider with a parent column enum
    /// </summary>
    public WkSlider(UIColumn parentColumn, string settingName, string displayName, 
                   float defaultValue = 0f, float minValue = 0f, float maxValue = 1f)
        : base(parentColumn, settingName, displayName)
    {
        DefaultValue = defaultValue;
        MinValue = minValue;
        MaxValue = maxValue;
    }

    protected override string GetTemplatePath()
    {
        return UIManager.SliderTemplatePath;
    }

    protected override void Configure()
    {
        if (GameObject == null) return;

        WkSettings.RegisterSetting(SettingName, DefaultValue);
        
        RemoveOldBinders<SliderSettingBinder>();
        
        var submitSlider = GameObject.GetComponentInChildren<SubmitSlider>();
        if (submitSlider == null)
        {
            WKLog.Error($"WkSlider: No SubmitSlider found in {GameObject.name}");
            return;
        }

        submitSlider.onValueChanged.RemoveAllListeners();
        submitSlider.minValue = MinValue;
        submitSlider.maxValue = MaxValue;

        float currentValue = GetValue();
        submitSlider.SetValueWithoutNotify(currentValue);
        submitSlider.value = currentValue;

        submitSlider.onValueChanged.AddListener(OnValueChanged);

        SetLabelText();
        
        UpdateValueDisplay(currentValue);

        _onValueChanged?.Invoke(this);

        WKLog.Info($"WkSlider: Configured '{SettingName}' successfully");
    }

    private void OnValueChanged(float value)
    {
        WkSettings.SetSetting(SettingName, value);
        UpdateValueDisplay(value);
        _onValueChanged?.Invoke(this);
    }

    /// <summary>
    /// Update the value display text
    /// </summary>
    private void UpdateValueDisplay(float value)
    {
        if (GameObject == null) return;
        
        var valueObj = GameObject.transform.Find("Value (1)");
        if (valueObj != null)
        {
            var textComponent = valueObj.GetComponent<TMPro.TMP_Text>();
            if (textComponent != null)
            {
                float roundedValue = (float)Math.Round((double)value, 2);
                textComponent.text = roundedValue.ToString();
            }
        }
    }

    /// <summary>
    /// Set a custom listener for value changes - receives the WkSlider instance
    /// </summary>
    public WkSlider SetListener(Action<WkSlider> listener)
    {
        _onValueChanged = listener;
        if (GameObject != null)
        {
            _onValueChanged?.Invoke(this);
        }
        return this;
    }

    /// <summary>
    /// Get current slider value
    /// </summary>
    public float GetValue()
    {
        return WkSettings.GetSetting(SettingName, DefaultValue);
    }

    /// <summary>
    /// Set slider value
    /// </summary>
    public WkSlider SetValue(float value)
    {
        value = Mathf.Clamp(value, MinValue, MaxValue);
        WkSettings.SetSetting(SettingName, value);
        if (GameObject != null)
        {
            var submitSlider = GameObject.GetComponentInChildren<SubmitSlider>();
            if (submitSlider != null)
            {
                submitSlider.SetValueWithoutNotify(value);
                UpdateValueDisplay(value);
            }
        }
        return this;
    }
} 