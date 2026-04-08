using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using Imui.Controls;
using Imui.Core;
using WKLib.API.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using static WKLib.Core.Config.ConfigManager;

namespace WKLib.Core.UI.Windows;

internal static class ConfigWindow
{
    public static bool isOpen = true;
    
    public static void Draw(ImGui gui, bool isRootPanelOpen)
    {
        if (!isRootPanelOpen)
            return;

        if (!gui.BeginWindow("WKLib config", ref isOpen, new ImSize(300, 400), ImWindowFlag.None))
            return;

        gui.Separator("General");
        gui.SimpleKeybind("Overlay key", ref OverlayKey.RefValue);

        if (CommandConsole.instance)
        {
            bool hasCheatedNotSet = !CommandConsole.hasCheated && CommandConsole.cheatsEnabled;
            if (gui.Checkbox(ref CommandConsole.cheatsEnabled, "Enabled cheats")
                || hasCheatedNotSet)
            {
                if (CommandConsole.cheatsEnabled)
                    CommandConsole.instance.EnableCheatsCommand(new string[] { "true" });
                else
                    CommandConsole.instance.EnableCheatsCommand(new string[] { "false" });
            }
        }
        
        gui.Separator("Appearance");

        RootPanel.Instance.ThemeController.DrawApperanceEditor(gui);
        
        gui.Separator("Debug");
        
        gui.Checkbox(ref AdvancedSettings.RefValue, "Advanced settings");
        if (AdvancedSettings)
        {
            gui.Checkbox(ref AutoCloseOverlay.RefValue, "Automatically close overlay");
            gui.Checkbox(ref EnableDemoWindow.RefValue, "Enable demo window");
        }

        gui.EndWindow();
    }
}
