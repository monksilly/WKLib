using HarmonyLib;
using ImuiBepInEx.API;
using WKLib.Core.Attributes;
using WKLib.Core.UI;

namespace WKLib.Core.Patches;

[PatchOnEntry]
[HarmonyPatch]
internal static class InputManagerPatch
{
    [HarmonyPatch(typeof(InputManager), nameof(InputManager.Start)), HarmonyPostfix]
    private static void InputManager_Start(InputManager __instance)
    {
        if (RootPanel.Instance != null)
            return;

        var imuiPanel = ImuiBepInExAPI.CreateImuiPanel<RootPanel>();
        RootPanel.Instance.ImuiPanel = imuiPanel;
    }
}
