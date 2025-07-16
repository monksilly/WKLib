using UnityEngine;
using WKLib.Core;
using WKLib.UI.Settings;
using WKLib.Utilities;
using Object = UnityEngine.Object;

namespace WKLib.UI.Components;

/// <summary>
/// Base class for all WKLib UI components.<br/>
/// </summary>
public abstract class WKComponent : IWKComponent
{
    public GameObject GameObject { get; protected set; }
    public string SettingName { get; protected set; }
    public string LabelText { get; protected set; }
    
    public TMPro.TextMeshProUGUI Label { get; protected set; }
    
    protected UIColumn ParentColumn;
    protected bool IsCreated = false;

    protected WKComponent(UIColumn parentColumn, string settingName, string displayName)
    {
        ParentColumn = parentColumn;
        SettingName = settingName;
        LabelText = displayName;
        
        UIManager.ExecuteWhenReady(CreateGameObject);
    }
    

    /// <summary>
    /// Reset the component state for recreation in a new scene
    /// </summary>
    public void Reset()
    {
        IsCreated = false;
        GameObject = null;
        Label = null;
    }

    /// <summary>
    /// Called by UIManager when it's ready to create GameObjects
    /// </summary>
    public void CreateGameObject()
    {
        if (IsCreated) 
        {
            return;
        }
        
        try
        {
            string resolvedParentPath = UIManager.GetColumnPath(ParentColumn);
            
            var parentTransform = UIManager.FindUIPath(resolvedParentPath);
            if (parentTransform == null)
            {
                WKLog.Error($"WkComponent: Could not find parent path: {resolvedParentPath}");
                return;
            }

            GameObject = CreateFromTemplate(parentTransform);
            if (GameObject != null)
            {
                Configure();
                IsCreated = true;
                WKLog.Info($"WkComponent: Successfully created {GetType().Name} '{SettingName}'");
            }
        }
        catch (System.Exception ex)
        {
            WKLog.Error($"WkComponent: Failed to create {GetType().Name} '{SettingName}': {ex.Message}");
        }
    }

    /// <summary>
    /// Create the UI component from a template
    /// </summary>
    protected GameObject CreateFromTemplate(Transform parent)
    {
        string templatePath = GetTemplatePath();
        GameObject template = UIManager.FindTemplate(templatePath);
        if (template == null)
        {
            WKLog.Error($"{GetType().Name}: Could not find template at path: {templatePath}");
            return null;
        }

        GameObject newObj = Object.Instantiate(template, parent);
        newObj.name = SettingName;
        newObj.SetActive(true);

        return newObj;
    }

    /// <summary>
    /// Set the label text for the component
    /// </summary>
    protected void SetLabelText()
    {
        if (GameObject == null)
        {
            WKLog.Warn($"WkComponent: Cannot set label text - GameObject is null for '{SettingName}'");
            return;
        }

        if (string.IsNullOrEmpty(LabelText))
        {
            WKLog.Warn($"WkComponent: LabelText is null or empty for '{SettingName}'");
            return;
        }
        
        var tmpComponents = GameObject.GetComponentsInChildren<TMPro.TextMeshProUGUI>();
        if (tmpComponents.Length > 0)
        {
            Label = tmpComponents[0];
            Label.text = LabelText;
            WKLog.Info($"WkComponent: Set label to '{LabelText}' for '{SettingName}'");
        }
        else
        {
            WKLog.Warn($"WkComponent: No TextMeshPro components found for '{SettingName}'");
        }
    }

    /// <summary>
    /// Remove the old binder components, these arent needed anymore
    /// </summary>
    protected void RemoveOldBinders<T>() where T : Component
    {
        var binders = GameObject.GetComponentsInChildren<T>();
        foreach (var binder in binders)
        {
            Object.DestroyImmediate(binder);
        }
    }

    protected abstract string GetTemplatePath();
    protected abstract void Configure();
} 