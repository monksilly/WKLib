using HarmonyLib;
using WKLib.Core.Attributes;
using WKLib.Core.UI;
using static WKLib.Core.Config.ConfigManager;

namespace WKLib.Core.Patches;

[PatchOnEntry]
[HarmonyPatch]
internal static class CL_GameManagerPatch
{
    [HarmonyPatch(typeof(CL_GameManager), nameof(CL_GameManager.UnPause)), HarmonyPostfix]
    private static void CL_GameManager_UnPause(CL_GameManager __instance)
    {
        if (RootPanel.Instance == null)
            return;

        if (!AutoCloseOverlay
            || !RootPanel.Instance.IsOpen
            || __instance.isPaused)
            return;

        RootPanel.Instance.IsOpen = false;
    }
}
