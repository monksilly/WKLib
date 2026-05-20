using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using BepInEx;
using BepInEx.Configuration;
using Imui.Controls;
using Imui.Core;
using UnityEngine;
using WKLib.API.UI;
using WKLib.Core.Config;

namespace WKLib.Core.UI.Windows;

internal static class ModListWindow
{
    public static bool isOpen = true;<

    private static PluginContainer[] pluginContainers = [];
    
    private static string searchString = "";
    
    public static void Initialize()
    {
        var pContainers = PluginConfigSearcher.GetPluginSettings();
        
        foreach (var pluginContainer in pContainers)
        {
            pluginContainer.PluginName =
                PrettifyName(pluginContainer.PluginInfo.Metadata.Name);
        }

        // Sort by name
        pluginContainers = pContainers
            .OrderBy(x => x.PluginName, StringComparer.OrdinalIgnoreCase)
            .ToArray();
        
        // https://github.com/Ikeiwa/WKModMenu/blob/main/ModMenu.cs#L262
        string PrettifyName(string input)
        {
            input = Regex.Replace(input, "([a-z])([A-Z])", "$1 $2");
            input = Regex.Replace(input, "([A-Z])([A-Z][a-z])", "$1 $2");
            input = Regex.Replace(input, @"\s+", " ");
            input = Regex.Replace(input, @"([A-Z]\.)\s([A-Z]\.)", "$1$2");

            return input.Trim();
        }
    }

    public static void Draw(ImGui gui, bool open)
    {
        if (!open)
            return;

        if (!gui.BeginWindow("Mod list", ref isOpen, new ImSize(250, 500), ImWindowFlag.None))
            return;

        gui.BeginVertical();
        gui.Separator("Mods");
        gui.TextEdit(ref searchString, hint: "Search for mod");
        
        gui.AddSpacing();

        for (int i = 0; i < pluginContainers.Length; i++)
        {
            ref var pluginContainer = ref pluginContainers[i];
            if (pluginContainer == null || pluginContainer.PluginInfo == null)
                continue;
            
            var API = pluginContainer.APIReference;
            if (API == null)
            {
                var pluginName = pluginContainer.PluginInfo.Metadata.Name;
                if (pluginName.IsNullOrWhiteSpace())
                    continue;

                if (searchString.Trim() != string.Empty)
                {
                    if (!pluginName.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                        continue;
                }
                
                gui.PushId(pluginContainer.PluginInfo.Metadata.GUID);

                if (gui.Button(pluginName))
                {
                    pluginContainer.IsWindowOpen = !pluginContainer.IsWindowOpen;
                }

                gui.PopId();
            }
            else
            {
                if (API.ModTab == null)
                    continue;

                if (searchString.Trim() != string.Empty)
                {
                    if (!API.ModTab.DisplayName.Contains(searchString, StringComparison.OrdinalIgnoreCase))
                        continue;
                }
                
                if (gui.BeginTreeNode(API.ModTab.DisplayName))
                {
                    API.ModTab.DrawSubMenu(gui);
                    gui.EndTreeNode();
                }
            }

        }

        gui.EndVertical();
        gui.EndWindow();

        DrawConfigWindows(gui);
    }

    public static void CloseConfigWindows()
    {
        for (int i = 0; i < pluginContainers.Length; i++)
        {
            ref var pluginContainer = ref pluginContainers[i];
            if (pluginContainer == null)
                continue;

            pluginContainer.IsWindowOpen = false;
        }
    }

    private static void DrawConfigWindows(ImGui gui)
    {
        for (int i = 0; i < pluginContainers.Length; i++)
        {
            ref var pluginContainer = ref pluginContainers[i];
            if (pluginContainer == null || pluginContainer.PluginInfo == null)
                continue;
            
            if (!pluginContainer.IsWindowOpen)
                continue;

            var pluginName = pluginContainer.PluginInfo.Metadata.Name;
            if (pluginName.IsNullOrWhiteSpace())
                continue;
            
            if (!gui.BeginWindow(pluginName + " " + pluginContainer.PluginInfo.Metadata.Version, ref pluginContainer.IsWindowOpen, new ImSize(500, 500), ImWindowFlag.None))
                continue;

            foreach (var configSection in pluginContainer.ConfigSection)
            {
                if (!configSection.Section.IsNullOrWhiteSpace())
                {
                    gui.AddSpacing();
                    gui.Separator(configSection.Section);       
                }

                foreach (var configEntry in configSection.ConfigEntries)
                {
                    DrawConfigEntry(gui, configEntry);
                }
            }
            
            gui.EndWindow();
        }
    }

    private static void DrawConfigEntry(ImGui gui, ConfigEntryBase configEntry)
    {
        string label = configEntry.Definition.Key;
        string description = configEntry.Description.Description;
        description = InsertLineBreaks(description, 40);
        
        object value = configEntry.BoxedValue;
        Type type = configEntry.SettingType;

        if (type == typeof(bool))
        {
            bool v = (bool)value;

            if (gui.Checkbox(ref v, label))
                configEntry.BoxedValue = v;
        }
        else if (type == typeof(int))
        {
            int v = (int)value;

            using (new UIUtility.LabeledScope(gui, label))
            {
                if (gui.NumericEdit(ref v, flags: ImNumericEditFlag.Slider))
                {
                    configEntry.BoxedValue = v;
                }                      
            }
        }
        else if (type == typeof(float))
        {
            float v = (float)value;

            using (new UIUtility.LabeledScope(gui, label))
            { 
                if (gui.NumericEdit(ref v, flags: ImNumericEditFlag.Slider))
                {
                    configEntry.BoxedValue = v;
                }                     
            }
        }
        else if (type == typeof(double))
        {
            double v = (double)value;

            using (new UIUtility.LabeledScope(gui, label))
            {
                if (gui.NumericEdit(ref v, flags: ImNumericEditFlag.Slider))
                {
                    configEntry.BoxedValue = v;
                }                        
            }
        }
        else if (type == typeof(string))
        {
            string v = (string)value ?? "";

            using (new UIUtility.LabeledScope(gui, label))
            {
                if (gui.TextEdit(ref v))
                {
                    configEntry.BoxedValue = v;
                }                        
            }
        }
        else if (type == typeof(Color))
        {
            using (new UIUtility.LabeledScope(gui, label))
            {
                var tempValue = (Color)configEntry.BoxedValue;
                if (gui.ColorEdit(ref tempValue))
                    configEntry.BoxedValue = tempValue;
            }
        }
        else if (type == typeof(KeyCode))
        {
            var tempValue = (KeyCode)configEntry.BoxedValue;
            if (gui.Keybind(label, ref tempValue))
                configEntry.BoxedValue = tempValue;
        }
        else if (type.IsEnum)
        {
            string[] names = Enum.GetNames(type);

            int currentIndex = Array.IndexOf(names, value.ToString());
            
            using (new UIUtility.LabeledScope(gui, label))
            {
                if (gui.Dropdown(ref currentIndex, names))
                {
                    object enumValue = Enum.Parse(type, names[currentIndex]);
                    configEntry.BoxedValue = enumValue;
                }
            }
        }
        // UNSUPPORTED
        else
        {
            gui.Text($"{label}: Unsupported type ({type.Name})");
        }

        gui.TooltipAtLastControl(description);
        
        string InsertLineBreaks(string text, int maxLineLength)
        {
            if (string.IsNullOrWhiteSpace(text) || maxLineLength <= 0)
                return text;

            var words = text.Split(' ');
            var sb = new StringBuilder();

            int currentLineLength = 0;

            foreach (var word in words)
            {
                if (currentLineLength + word.Length + 1 > maxLineLength)
                {
                    sb.AppendLine();
                    currentLineLength = 0;
                }
                else if (currentLineLength > 0)
                {
                    sb.Append(' ');
                    currentLineLength++;
                }

                sb.Append(word);
                currentLineLength += word.Length;
            }

            return sb.ToString();
        }
    }
    
    public static void HandleInput(ImGui gui, bool open) { }
}
