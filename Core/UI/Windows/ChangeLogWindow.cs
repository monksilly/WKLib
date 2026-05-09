using Imui.Controls;
using Imui.Core;
using WKLib.Core.Config;

namespace WKLib.Core.UI.Windows;

internal class ChangeLogWindow
{
    public static bool isOpen = false;
    
    public static void Draw(ImGui gui, bool isRootPanelOpen)
    {
        if (!isRootPanelOpen)
            return;

        if (!gui.BeginWindow("WKLib changelog", ref isOpen, new ImSize(500, 400), ImWindowFlag.None))
            return;
        
        gui.Separator("Versions");

        if (gui.BeginTreeNode("Version 0.2.3"))
        {
            gui.Text("+ Change config system default saving folder location, dont use WKLib as the default");
            gui.Text("+ Fix configs not saving as jsons");
            gui.EndTreeNode();
        }

        if (gui.BeginTreeNode("Version 0.2.2"))
        {
            gui.Text("+ Fix plugin object being deleted");
            gui.Text("+ Fix error on scene loaded");
            gui.EndTreeNode();
        }        
        
        if (gui.BeginTreeNode("Version 0.2.1"))
        {
            gui.Text("+ Change config system saving and loading");
            gui.EndTreeNode();
        }
        
        if (gui.BeginTreeNode("Version 0.2.0"))
        {
            gui.Text("+ Added ChangeLog window");
            gui.Text("+ Updated AssetService");
            gui.Text("+ Added Overlay button on main menu and pause menu");
            gui.EndTreeNode();
        }
        
        if (gui.BeginTreeNode("Version 0.1.0"))
        {
            gui.Text("+ Reworked UI and Config API");
            gui.EndTreeNode();
        }
        
        gui.EndWindow();
    }   
}