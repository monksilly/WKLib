using System.IO;
using BepInEx;
using Newtonsoft.Json;
using WKLib.API.Config;

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
    
    public string BasePath { get; }
    internal ConfigFile DefaultConfigFile { get; private set; } // WKLib default config
    public JsonSerializerSettings JsonSerializerSettings { get; }

    private CoreSettings()
    {
        // Set base path
        BasePath = Path.Combine(Paths.ConfigPath, "WKLib");
        if (!Directory.Exists(BasePath)) 
            Directory.CreateDirectory(BasePath);
        
        // Create default WKLib config
        DefaultConfigFile = new ConfigFile(Path.Combine(BasePath, "DefaultConfig.json"));
    }   
}