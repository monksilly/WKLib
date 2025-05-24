using BepInEx;
using BepInEx.Logging;
using WK_Lib.API;

namespace WK_Lib;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class WkLib : BaseUnityPlugin
{
    private void Awake()
    {
        // Initialize Logger
        WKLog.Initialize(Logger);
        
        // Setup Loaders & Patches
        
        Loaders.LevelLoader.Setup();
        Loaders.AssetBundleLoader.Setup();
        Patchers.HarmonyPatcher.ApplyAll();
        
        
        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_NAME} v{MyPluginInfo.PLUGIN_VERSION} is loaded!");
    }
}
