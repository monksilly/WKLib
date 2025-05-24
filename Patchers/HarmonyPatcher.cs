using System;
using System.IO;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using WK_Lib.API;

namespace WK_Lib.Patchers;

public static class HarmonyPatcher
{
    private static Harmony _harmony;
    
    private const string GameAssemblyName = "Assembly-CSharp";

    public static void ApplyAll()
    {
        _harmony = new Harmony(MyPluginInfo.PLUGIN_GUID + ".patches");
        
        var baseDir = AppDomain.CurrentDomain.BaseDirectory;
        var pluginsPath = Path.Combine(baseDir, "BepInEx", "plugins");
        
        var assemblies = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => !a.IsDynamic && !string.IsNullOrEmpty(a.Location))
            .Where(a => a.Location.StartsWith(pluginsPath, StringComparison.OrdinalIgnoreCase))
            .Where(a => a.GetReferencedAssemblies()
                .Any(r => r.Name.Equals(GameAssemblyName, StringComparison.OrdinalIgnoreCase))
            );

        int patches = 0;

        foreach (var assembly in assemblies)
        {
            var patcherTypes = assembly.GetTypes()
                .Where(t => typeof(IPatcher).IsAssignableFrom(t) && !t.IsAbstract && !t.IsInterface);

            foreach (var patcherType in patcherTypes)
            {
                var attrs = (PatchTargetAttribute[])patcherType.GetCustomAttributes(typeof(PatchTargetAttribute), false);
                foreach (var attr in attrs)
                {
                    var original = AccessTools.Method(attr.TargetType, attr.MethodName);
                    if (original == null)
                    {
                        WKLog.Warn($"Could not find method {attr.TargetType.FullName}.{attr.MethodName} for patcher {patcherType.FullName}");
                        continue;
                    }
                    
                    var prefixMethod = patcherType.GetMethod("Prefix", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                    var postfixMethod = patcherType.GetMethod("Postfix", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
                    
                    var prefix = prefixMethod != null ? new HarmonyMethod(prefixMethod) : null;
                    var postfix = postfixMethod != null ? new HarmonyMethod(postfixMethod) : null;
                    
                    _harmony.Patch(original, prefix, postfix);
                    patches++;
                    WKLog.Info($"Patched {attr.TargetType.Name}.{attr.MethodName} with {patcherType.Name}");
                }
            }
        }
        
        WKLog.Info($"HarmonyPatcher applied {patches} patches across {assemblies.Count()} assemblies.");
    }
}