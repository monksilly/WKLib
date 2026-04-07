using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Imui.Controls;
using Imui.Core;
using Imui.Rendering;
using WKLib.Core.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.Text;

namespace WKLib.API.UI;

public static class UIUtility
{
    public struct LabeledScope : IDisposable
    {
        private ImGui gui;

        public LabeledScope(ImGui gui, ReadOnlySpan<char> label, float firstRectSize = 0.4f)
        {
            this.gui = gui;

            gui.PushId(label);

            gui.AddSpacingIfLayoutFrameNotEmpty();
            gui.BeginHorizontal();
            var rect = gui.AddLayoutRect(gui.GetLayoutWidth() * firstRectSize, gui.GetRowHeight());
            gui.Text(label, rect, overflow: ImTextOverflow.Ellipsis);
            gui.BeginVertical();
        }

        public void Dispose()
        {
            gui.EndVertical();
            gui.EndHorizontal();
            gui.PopId();
        }
    }

    public static void SimpleKeybind(this ImGui gui, string label, ref KeyBind keyBind)
    {
        var id = gui.GetControlId(label);
        gui.PushId(id);

        gui.AddSpacingIfLayoutFrameNotEmpty();
        gui.BeginHorizontal();
        var rect = gui.AddLayoutRect(gui.GetLayoutWidth() * 0.8f, gui.GetRowHeight());
        gui.Text(label, rect, overflow: ImTextOverflow.Ellipsis);
        gui.BeginVertical();

        if (gui.GetActiveControl() == id)
        {
            gui.Button("...");

            if (keyBind.SetToPressedKey(gui))
            {
                gui.ResetActiveControl();
            }
        }
        else if (gui.Button(keyBind.KeyCode.ToString()))
        {
            gui.SetActiveControl(id);
        }

        gui.EndVertical();
        gui.EndHorizontal();
        gui.PopId();
    }

    public static void ShowPopupForTime(string text, float seconds = 2.5f)
    {
        OverlayState.Popups.Add(new PopupSettings(text, seconds));
    }
}