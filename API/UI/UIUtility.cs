using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using BepInEx;
using BepInEx.Configuration;
using Imui.Controls;
using Imui.Core;
using Imui.IO.Events;
using Imui.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.Text;
using WKLib.API.Input;
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
            var key = InputUtility.GetFirstActiveKey();
            if (key == null)
                return false;

            keyCode = key.Value;
            return true;
        }
    }
    
    public static void DrawConfigEntry(ImGui gui, ConfigEntryBase configEntry)
    {
        string label = configEntry.Definition.Key;
        string description = configEntry.Description.Description;
        description = InsertLineBreaks(description, 40);
        
        object value = configEntry.BoxedValue;
        Type type = configEntry.SettingType;

        if (type == typeof(bool))
        {
            bool v = (bool)value;

            if (gui.Checkbox(ref v, label))
                configEntry.BoxedValue = v;
        }
        else if (type == typeof(byte))
        {
            byte v = (byte)value;

            using (new UIUtility.LabeledScope(gui, label))
            {
                if (gui.NumericEdit(ref v, flags: ImNumericEditFlag.Slider))
                    configEntry.BoxedValue = (byte)v;
            }
        }
        else if (type == typeof(sbyte))
        {
            int v = (sbyte)value;

            using (new UIUtility.LabeledScope(gui, label))
            {
                if (gui.NumericEdit(ref v, flags: ImNumericEditFlag.Slider, min: sbyte.MinValue, max: sbyte.MaxValue))
                    configEntry.BoxedValue = (sbyte)v;
            }
        }
        else if (type == typeof(short))
        {
            short v = (short)value;

            using (new UIUtility.LabeledScope(gui, label))
            {
                if (gui.NumericEdit(ref v, flags: ImNumericEditFlag.Slider))
                    configEntry.BoxedValue = (short)v;
            }
        }
        else if (type == typeof(ushort))
        {
            int v = (ushort)value;

            using (new UIUtility.LabeledScope(gui, label))
            {
                if (gui.NumericEdit(ref v, flags: ImNumericEditFlag.Slider, min: ushort.MinValue, max: ushort.MaxValue))
                    configEntry.BoxedValue = (ushort)v;
            }
        }
        else if (type == typeof(int))
        {
            int v = (int)value;

            using (new UIUtility.LabeledScope(gui, label))
            {
                if (gui.NumericEdit(ref v, flags: ImNumericEditFlag.Slider))
                    configEntry.BoxedValue = (int)v;
            }
        }
        else if (type == typeof(uint))
        {
            long v = (uint)value;

            using (new UIUtility.LabeledScope(gui, label))
            {
                if (gui.NumericEdit(ref v, flags: ImNumericEditFlag.Slider, min: uint.MinValue, max: uint.MaxValue))
                    configEntry.BoxedValue = (uint)v;
            }
        }
        else if (type == typeof(long))
        {
            long v = (long)value;

            using (new UIUtility.LabeledScope(gui, label))
            {
                if (gui.NumericEdit(ref v, flags: ImNumericEditFlag.Slider))
                    configEntry.BoxedValue = v;
            }
        }
        else if (type == typeof(ulong))
        {
            long v = (long)value;

            using (new UIUtility.LabeledScope(gui, label))
            {
                if (gui.NumericEdit(ref v, flags: ImNumericEditFlag.Slider, min: 0)) // Compromise: Use Long limits on ULong since Imui doesnt handle ULong
                    configEntry.BoxedValue = (ulong)v;
            }
        }
        else if (type == typeof(float))
        {
            float v = (float)value;

            using (new UIUtility.LabeledScope(gui, label))
            { 
                if (gui.NumericEdit(ref v, flags: ImNumericEditFlag.Slider))
                    configEntry.BoxedValue = v;
            }
        }
        else if (type == typeof(double))
        {
            double v = (double)value;

            using (new UIUtility.LabeledScope(gui, label))
            {
                if (gui.NumericEdit(ref v, flags: ImNumericEditFlag.Slider))
                    configEntry.BoxedValue = v;
            }
        }
        else if (type == typeof(decimal))
        {
            double v = Convert.ToDouble((decimal)value);

            using (new UIUtility.LabeledScope(gui, label))
            {
                if (gui.NumericEdit(ref v, flags: ImNumericEditFlag.Slider))
                    configEntry.BoxedValue = (decimal)v;
            }
        }
        else if (type == typeof(string))
        {
            string v = (string)value ?? "";

            using (new UIUtility.LabeledScope(gui, label))
            {
                if (gui.TextEdit(ref v))
                    configEntry.BoxedValue = v;
            }
        }
        else if (type == typeof(Vector2))
        {
            Vector2 v = (Vector2)value;

            using (new UIUtility.LabeledScope(gui, label))
            {
                if (gui.Vector(ref v))
                    configEntry.BoxedValue = v;
            }
        }
        else if (type == typeof(Vector3))
        {
            Vector3 v = (Vector3)value;

            using (new UIUtility.LabeledScope(gui, label))
            {
                if (gui.Vector(ref v))
                    configEntry.BoxedValue = v;
            }
        }
        else if (type == typeof(Vector4))
        {
            Vector4 v = (Vector4)value;

            using (new UIUtility.LabeledScope(gui, label))
            {
                if (gui.Vector(ref v))
                    configEntry.BoxedValue = v;
            }
        }
        else if (type == typeof(Quaternion))
        {
            var quat = (Quaternion)value;
            Vector4 v = new Vector4(quat.x, quat.y, quat.z, quat.w);

            using (new UIUtility.LabeledScope(gui, label))
            {
                if (gui.Vector(ref v))
                    configEntry.BoxedValue = new Quaternion(v.x, v.y, v.z, v.w);
            }
        }
        else if (type == typeof(UnityEngine.Color))
        {
            using (new UIUtility.LabeledScope(gui, label))
            {
                var tempValue = (UnityEngine.Color)configEntry.BoxedValue;
                if (gui.ColorEdit(ref tempValue))
                    configEntry.BoxedValue = tempValue;
            }
        }
        else if (type == typeof(KeyboardShortcut))
        {
            // Ignore
            return;
        }
        else if (type == typeof(KeyCode))
        {
            var tempValue = (KeyCode)configEntry.BoxedValue;
            if (gui.Keybind(label, ref tempValue))
                configEntry.BoxedValue = tempValue;
        }
        else if (type.IsEnum)
        {
            string[] names = Enum.GetNames(type);

            int currentIndex = Array.IndexOf(names, value.ToString());
            
            using (new UIUtility.LabeledScope(gui, label))
            {
                if (gui.Dropdown(ref currentIndex, names))
                {
                    object enumValue = Enum.Parse(type, names[currentIndex]);
                    configEntry.BoxedValue = enumValue;
                }
            }
        }
        // UNSUPPORTED
        else
        {
            gui.Text($"{label}: Unsupported type ({type.Name})");
        }

        if (!description.IsNullOrWhiteSpace())
            gui.TooltipAtLastControl(description);
        
        string InsertLineBreaks(string text, int maxLineLength)
        {
            if (string.IsNullOrWhiteSpace(text) || maxLineLength <= 0)
                return text;

            var words = text.Split(' ');
            var sb = new StringBuilder();

            int currentLineLength = 0;

            foreach (var word in words)
            {
                if (currentLineLength + word.Length + 1 > maxLineLength)
                {
                    sb.AppendLine();
                    currentLineLength = 0;
                }
                else if (currentLineLength > 0)
                {
                    sb.Append(' ');
                    currentLineLength++;
                }

                sb.Append(word);
                currentLineLength += word.Length;
            }

            return sb.ToString();
        }
    }
    
    public static void ShowPopupForTime(string text, float seconds = 2.5f)
    {
        OverlayState.Popups.Add(new PopupSettings(text, seconds));
    }
}