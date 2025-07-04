using System;
using System.Collections.Generic;
using WKLib.Core;
using WKLib.UI.Components;

namespace WKLib.UI;

/// <summary>
/// Convenient builder class for creating UI components
/// </summary>
public static class UIBuilder
{
    /// <summary>
    /// Create a new slider component
    /// </summary>
    public static WkSlider CreateSlider(UIColumn parentColumn, string settingName, string displayName, 
                                       float defaultValue = 0f, float minValue = 0f, float maxValue = 1f)
    {
        return new WkSlider(parentColumn, settingName, displayName, defaultValue, minValue, maxValue);
    }

    /// <summary>
    /// Create a new toggle component
    /// </summary>
    public static WkToggle CreateToggle(UIColumn parentColumn, string settingName, string displayName, 
                                       bool defaultValue = false)
    {
        return new WkToggle(parentColumn, settingName, displayName, defaultValue);
    }

    /// <summary>
    /// Create a new dropdown component
    /// </summary>
    public static WkDropdown CreateDropdown(UIColumn parentColumn, string settingName, string displayName, 
                                           List<string> options, int defaultValue = 0)
    {
        return new WkDropdown(parentColumn, settingName, displayName, options, defaultValue);
    }
} 