using BepInEx.Configuration;
using UnityEngine;
using WKLib.Core.UI;

namespace WKLib.Core.Config;

internal static class ConfigManager
{
    public static ConfigEntry<KeyCode> OverlayKey;

    public static ConfigEntry<bool> AutoCloseOverlay;
    public static ConfigEntry<bool> EnableDemoWindow;

    // Theme settings
    public static ConfigEntry<bool> HighContrast;
    public static ConfigEntry<Color> AccentColor;
    
    public static void CreateEntries(ConfigFile Config)
    {
        OverlayKey = Config.Bind(
            "General",
            "Overlay Key",
            KeyCode.F6,
            "Keybind for opening and closing the overlay menu"
        );
        
        AutoCloseOverlay = Config.Bind(
            "General",
            "Auto Close Overlay",
            true,
            "Automatically close overlay on scene change");
        
        EnableDemoWindow = Config.Bind(
            "General",
            "Enable Demo Window",
            false,
            "Enable demo window, used to showcase the WKLib UI");
        
        // Theme
        HighContrast = Config.Bind(
            "Theme",
            "High Contrast",
            false,
            "Apply high contrast to the UI");
        
        AccentColor = Config.Bind(
            "Theme",
            "Accent color",
            new Color(0.05f, 0.45f, 0.75f, 1f),
            "Change accent color of the UI");

        HighContrast.SettingChanged += (sender, args) =>
        {
            RootPanel.Instance?.ThemeController?.RegisterChanges();
        };
        
        AccentColor.SettingChanged += (sender, args) =>
        {
            RootPanel.Instance?.ThemeController?.RegisterChanges();
        };
    }
}