using UnityEngine;
using WKLib.API.Config;
using WKLib.API.UI;

namespace WKLib.Core.Config;

internal static class ConfigManager
{
    public static ConfigValue<KeyBind> OverlayKey = new ConfigValue<KeyBind>(nameof(OverlayKey), new KeyBind(KeyCode.F6));

    public static ConfigValue<bool> AdvancedSettings = new ConfigValue<bool>(nameof(AdvancedSettings), false);
    public static ConfigValue<bool> AutoCloseOverlay = new ConfigValue<bool>(nameof(AutoCloseOverlay), true);
    public static ConfigValue<bool> EnableDemoWindow = new ConfigValue<bool>(nameof(EnableDemoWindow), false);   
}