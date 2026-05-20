using System;
using BepInEx;
using BepInEx.Configuration;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using UnityEngine;
using WKLib.Utilities;

namespace WKLib.Core.Config;

// https://github.com/BepInEx/BepInEx.ConfigurationManager/blob/master/ConfigurationManager/SettingSearcher.cs

internal static class PluginConfigSearcher
{
    /// <summary>
    /// Search for all instances of BaseUnityPlugin loaded by chainloader or other means.
    /// </summary>
    public static BaseUnityPlugin[] FindPlugins()
    {
        // Search for instances of BaseUnityPlugin to also find dynamically loaded plugins.
        // Have to use FindObjectsOfType(Type) instead of FindObjectsOfType<T> because the latter is not available in some older unity versions.
        // Still look inside Chainloader.PluginInfos in case the BepInEx_Manager GameObject uses HideFlags.HideAndDontSave, which hides it from Object.Find methods.
        return Chainloader.PluginInfos.Values.Select(x => x.Instance)
                          .Where(plugin => plugin != null)
                          .Union(UnityEngine.Object.FindObjectsOfType(typeof(BaseUnityPlugin)).Cast<BaseUnityPlugin>())
                          .ToArray();
    }

    // Key = Plugin Info
    // Values = Config entries
    public static Dictionary<PluginInfo, ConfigEntryBase[]> GetPluginSettings()
    {
        var pluginSettings = new Dictionary<PluginInfo, ConfigEntryBase[]>();
        
        foreach (var plugin in FindPlugins())
        {
            var type = plugin.GetType();
            if (type.GetCustomAttributes(typeof(BrowsableAttribute), false).Cast<BrowsableAttribute>()
                .Any(x => !x.Browsable))
                continue;
            
            var configEntries = new List<ConfigEntryBase>();
            
            foreach (var configEntryBase in plugin.Config.Select(configEntry => configEntry.Value))
            {
                var tags = configEntryBase.Description?.Tags;
                if (tags != null && tags.Contains("Hidden"))
                    continue;

                configEntries.Add(configEntryBase);
            }

            if (configEntries.Count <= 0)
                continue;
            
            pluginSettings.TryAdd(plugin.Info, configEntries.ToArray());
        }
        
        return pluginSettings;
    }
}