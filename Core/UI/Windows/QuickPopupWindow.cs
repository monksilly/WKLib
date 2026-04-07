using System;
using System.Collections.Generic;
using System.Text;
using Imui.Controls;
using Imui.Core;

namespace WKLib.Core.UI.Windows;

internal static class QuickPopupWindow
{
    public static void Draw(ImGui gui, string text)
    {
        if (text.Trim() == string.Empty)
            return;

        var textSize = gui.MeasureTextSize(text);
        gui.BeginWindow("Popup", new ImSize(textSize.x * 1.5f, gui.GetRowHeight() * 5f), ImWindowFlag.None);

        gui.Text(text);

        gui.EndWindow();
    }
}