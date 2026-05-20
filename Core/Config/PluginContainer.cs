using BepInEx;
using BepInEx.Configuration;
using WKLib.API;

namespace WKLib.Core.Config;

public class PluginContainer
{
    public class ConfigEntrySection
    {
        public string Section = "";
        public ConfigEntryBase[] ConfigEntries = [];
    }

    public string PluginName = ""; // This string is modified to look pretty
    public PluginInfo PluginInfo = null;
    public ConfigEntrySection[] ConfigSection = [];

    public bool IsWindowOpen = false;
    public WKLibAPI APIReference = null;
}