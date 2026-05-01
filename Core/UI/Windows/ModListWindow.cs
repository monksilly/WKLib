using System;
using System.Collections.Generic;
using System.Text;
using Imui.Controls;
using Imui.Core;
using WKLib.API;

namespace WKLib.Core.UI.Windows;

internal static class ModListWindow
{
    public static bool isOpen = true;

    public static void Draw(ImGui gui, bool open)
    {
        if (!open)
            return;

        if (!gui.BeginWindow("Mod list", ref isOpen, new ImSize(250, 500), ImWindowFlag.None))
            return;

        gui.BeginVertical();

        foreach (var API in WKLibAPI.internalAPIs)
        {
            if (API == null)
                continue;

            if (gui.BeginTreeNode(API.ModTab.DisplayName))
            {
                API.ModTab.DrawSubMenu(gui);
                gui.EndTreeNode();
            }
        }

        gui.EndVertical();
        gui.EndWindow();
    }

    public static void HandleInput(ImGui gui, bool open)
    {

    }
}
