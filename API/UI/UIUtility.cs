using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Imui.Controls;
using Imui.Core;
using Imui.IO.Events;
using Imui.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.Text;
using WKLib.Core.UI;

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

    
    public static bool Keybind(this ImGui gui, string label, ref KeyCode keyCode)
    {
        var changed = false;
        
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

            if (SetToPressedKey(gui, ref keyCode))
            {
                changed = true;
                gui.ResetActiveControl();
            }
        }
        else if (gui.Button(keyCode.ToString()))
        {
            gui.SetActiveControl(id);
        }

        gui.EndVertical();
        gui.EndHorizontal();
        gui.PopId();
        
        return changed;
        
        bool SetToPressedKey(ImGui gui, ref KeyCode keyCode)
        {
            for (int i = 0; i < gui.Input.KeyboardEventsCount; ++i)
            {
                var keyboardEvent = gui.Input.GetKeyboardEvent(i);

                if (keyboardEvent.Type != ImKeyboardEventType.Down)
                    continue;

                if (keyboardEvent.Key == KeyCode.Escape)
                {
                    keyCode = KeyCode.None;
                    return true;
                }

                keyCode = keyboardEvent.Key;
                return true;
            }

            return false;
        }
    }

    public static void ShowPopupForTime(string text, float seconds = 2.5f)
    {
        OverlayState.Popups.Add(new PopupSettings(text, seconds));
    }
}