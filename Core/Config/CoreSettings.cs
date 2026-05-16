using System.IO;
using BepInEx;
using Newtonsoft.Json;
using WKLib.API.Config;
using WKLib.Core.Config.Converters;

namespace WKLib.Core.Config;

internal class CoreSettings
{
    private static CoreSettings instance;
    
    public static CoreSettings Instance
    {
        get
        {
            if (instance == null)
                instance = new CoreSettings();
            
            return instance;
        }
    }

    internal ConfigFolder DefaultConfigFolder = null;
    internal ConfigFile DefaultConfigFile { get; private set; } // WKLib default config
    public JsonSerializerSettings JsonSerializerSettings { get; } = new JsonSerializerSettings();
    
    private CoreSettings()
    {
        // Add converters
        JsonSerializerSettings.Converters.Add(new ColorJsonConverter());
        
        // Create default WKLib folder and config
        DefaultConfigFolder = new ConfigFolder("WKLib");
        DefaultConfigFile = DefaultConfigFolder.GetOrCreateConfigFile("DefaultConfig.json");
    }   
}