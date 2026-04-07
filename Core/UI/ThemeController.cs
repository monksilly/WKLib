using Imui.Core;
using Imui.IO.UGUI;
using Imui.Style;
using UnityEngine;

namespace WKLib.Core.UI;

internal class ThemeController : MonoBehaviour
{
    public ImTheme BaseTheme = ImThemeBuiltin.Dark();

    private Vector2 lastScreenSize;

    public void DetectChanges(ImGui gui)
    {
        if (Screen.width == lastScreenSize.x && Screen.height == lastScreenSize.y)
            return;
        
        lastScreenSize = new Vector2(Screen.width, Screen.height);
        
        SetTheme(gui);
    }
    
    public void SetTheme(ImGui gui)
    {
        gui.SetTheme(BaseTheme);
        
        SetTextSize(gui);
    }

    private void SetTextSize(ImGui gui)
    {
        gui.Style.Layout.TextSize *= Screen.height / 1080f;
    }
}