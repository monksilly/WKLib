using System.Collections;
using System.Collections.Generic;
using WK_Lib.API;
using WK_Lib.Loaders;

namespace WK_Lib.Patchers;

public class WorldLoaderPatcher
{
    [PatchTarget(typeof(WorldLoader), "Initialize")]
    public class WorldLoaderInitPatcher : IPatcher
    {
        // After WorldLoader.Initialize, set up our loaders
        public static void Postfix(WorldLoader __instance)
        {
            LevelLoader.Setup();
            SubregionLoader.Setup();
            RegionLoader.Setup();
            WKLog.Info("WK_Lib Loaders initialized in WorldLoader.");
        }
    }

    [PatchTarget(typeof(WorldLoader), "GenerateLevels")]
    public class WorldLoaderGeneratePatcher : IPatcher
    {
        // Intercept generation coroutine to inject a custom sequence
        public static bool Prefix(WorldLoader __instance, ref IEnumerator __result, WorldLoader.GenerationParameters genParams)
        {
            var customObjs = new List<UnityEngine.GameObject>(LevelLoader.LoadAll());
            
            if (customObjs.Count > 0)
            {
                // Convert GameObjects to M_Level
                var customLevels = new List<M_Level>();
                foreach (var obj in customObjs)
                {
                    var lvl = obj.GetComponent<M_Level>();
                    if (lvl != null)
                        customLevels.Add(lvl);
                    else
                        WKLog.Warn($"Custom object '{obj.name}' missing M_Level component.");
                }

                // Replace the generation list
                genParams.preloadLevels = customLevels;
                genParams.generationType = WorldLoader.GenerationParameters.GenerationType.replace;

                WKLog.Info($"Injected {customLevels.Count} custom levels into generation pipeline.");
            }
            // Let the original method run with our modified genParams
            return true;
        }
    }
}