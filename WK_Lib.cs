using BepInEx;
using BepInEx.Logging;
using WKLib.Utilities;

namespace WKLib;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class WkLib : BaseUnityPlugin
{
    private void Awake()
    {
        // Initialize Logger
        WKLog.Initialize(Logger);
        
        WKLog.Info($"Plugin {MyPluginInfo.PLUGIN_NAME} v{MyPluginInfo.PLUGIN_VERSION} is loaded!");
    }
}
