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
    public static WKSlider CreateSlider(UIColumn parentColumn, string settingName, string displayName, 
                                       float defaultValue = 0f, float minValue = 0f, float maxValue = 1f)
    {
        return new WKSlider(parentColumn, settingName, displayName, defaultValue, minValue, maxValue);
    }

    /// <summary>
    /// Create a new toggle component
    /// </summary>
    public static WKToggle CreateToggle(UIColumn parentColumn, string settingName, string displayName, 
                                       bool defaultValue = false)
    {
        return new WKToggle(parentColumn, settingName, displayName, defaultValue);
    }

    /// <summary>
    /// Create a new dropdown component
    /// </summary>
    public static WKDropdown CreateDropdown(UIColumn parentColumn, string settingName, string displayName, 
                                           List<string> options, int defaultValue = 0)
    {
        return new WKDropdown(parentColumn, settingName, displayName, options, defaultValue);
    }

    /// <summary>
    /// Create a new separator component
    /// </summary>
    /// <param name="isInvisible">If true, clears the separator text to make it invisible</param>
    public static WKSeparator CreateSeparator(UIColumn parentColumn, string settingName, string displayName, bool isInvisible = false)
    {
        return new WKSeparator(parentColumn, settingName, displayName, isInvisible);
    }
} 