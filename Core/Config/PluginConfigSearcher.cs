using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using UnityEngine;
using WKLib.API;
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
    public static List<PluginContainer> GetPluginSettings()
    {
        var pluginContainers = new List<PluginContainer>();
        
        foreach (var plugin in FindPlugins())
        {
            var GUID = plugin.Info.Metadata.GUID;
            var WKLibAPIRef = WKLibAPI.internalAPIs.Find(api => string.Equals(api.GUID, GUID, StringComparison.Ordinal));
            
            var type = plugin.GetType();
            if (type.GetCustomAttributes(typeof(BrowsableAttribute), false).Cast<BrowsableAttribute>()
                .Any(x => !x.Browsable)
                && WKLibAPIRef == null)
                continue;
            
            PluginContainer pluginContainer = new()
            {
                PluginName = plugin.Info.Metadata.Name,
                PluginInfo = plugin.Info,
                APIReference = WKLibAPIRef
            };
            
            if (WKLibAPIRef == null || (WKLibAPIRef != null && WKLibAPIRef.ModTab == null))
            {
                var sections = new Dictionary<string, List<ConfigEntryBase>>();
                
                foreach (var configEntryBase in plugin.Config.Select(configEntry => configEntry.Value))
                {
                    var tags = configEntryBase.Description?.Tags;
                    if (tags != null && tags.Contains("Hidden"))
                        continue;

                    var sectionName = configEntryBase.Definition.Section;

                    if (!sections.TryGetValue(sectionName, out var list))
                    {
                        list = [];
                        sections[sectionName] = list;
                    }

                    list.Add(configEntryBase);
                }
                
                pluginContainer.ConfigSection = sections
                    .Select(section => new PluginContainer.ConfigEntrySection
                    {
                        Section = section.Key,
                        ConfigEntries = section.Value.ToArray()
                    })
                    .ToArray();
                
                if (pluginContainer.ConfigSection.Length <= 0)
                    continue;
            }
            
            pluginContainers.Add(pluginContainer);
        }
        
        return pluginContainers;
    }
}