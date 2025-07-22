using UnityEngine;
using TMPro;
using WKLib.Core;
using WKLib.Utilities;

namespace WKLib.UI.Components;

/// <summary>
/// Separator component for UI sections - just a simple divider, can be used if your text phases into an existing component or 
/// </summary>
public class WKSeparator : WKComponent
{
    private bool _isInvisible;
    
    /// <summary>
    /// Create a separator with a parent column enum
    /// </summary>
    /// <param name="isInvisible">If true, clears the separator text to make it invisible</param>
    public WKSeparator(UIColumn parentColumn, string settingName, string displayName, bool isInvisible = false)
        : base(parentColumn, settingName, displayName)
    {
        _isInvisible = isInvisible;
    }

    protected override string GetTemplatePath()
    {
        return UIManager.SeparatorTemplatePath;
    }

    protected override void Configure()
    {
        if (_isInvisible && GameObject != null)
        {
            // Clear the text if invisible mode is enabled
            var textComponent = GameObject.GetComponent<TextMeshProUGUI>();
            if (textComponent != null)
            {
                textComponent.text = "";
            }
        }
        
        WKLog.Info($"WKSeparator: Added separator '{SettingName}' (Invisible: {_isInvisible})");
    }
}
