using System;
using Imui.Core;
using Imui.IO.Events;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;

namespace WKLib.API.UI;

[Serializable]
public class KeyBind : IEquatable<KeyBind>, ICloneable
{
    [JsonProperty]
    [JsonConverter(typeof(StringEnumConverter))]
    public KeyCode KeyCode { get; set; }

    [JsonIgnore]
    private bool _isHeld;

    public KeyBind() { }
    public KeyBind(KeyCode KeyCode)
    {
        this.KeyCode = KeyCode;
    }
    
    public bool SetToPressedKey(ImGui gui)
    {
        for (int i = 0; i < gui.Input.KeyboardEventsCount; ++i)
        {
            var keyboardEvent = gui.Input.GetKeyboardEvent(i);

            if (keyboardEvent.Type != ImKeyboardEventType.Down)
                continue;

            if (keyboardEvent.Key == KeyCode.Escape)
            {
                KeyCode = KeyCode.None;
                return true;
            }

            KeyCode = keyboardEvent.Key;
            return true;
        }

        return false;
    }

    public bool IsPressed(ImGui gui)
    {
        if (!IsSet())
            return false;

        bool pressedThisFrame = false;

        for (int i = 0; i < gui.Input.KeyboardEventsCount; ++i)
        {
            var keyboardEvent = gui.Input.GetKeyboardEvent(i);

            if (keyboardEvent.Key != KeyCode)
                continue;

            if (keyboardEvent.Type == ImKeyboardEventType.Down)
            {
                if (!_isHeld)
                {
                    pressedThisFrame = true;
                    _isHeld = true;
                }
            }
            else if (keyboardEvent.Type == ImKeyboardEventType.Up)
            {
                _isHeld = false;
            }
        }

        return pressedThisFrame;
    }

    public bool IsDown(ImGui gui)
    {
        if (!IsSet())
            return false;

        for (int i = 0; i < gui.Input.KeyboardEventsCount; ++i)
        {
            var keyboardEvent = gui.Input.GetKeyboardEvent(i);

            if (keyboardEvent.Type != ImKeyboardEventType.Down)
                continue;

            if (keyboardEvent.Key == KeyCode)
                return true;
        }

        return false;
    }

    public bool IsSet()
    { 
        return KeyCode != KeyCode.None; 
    }
    
    public object Clone()
    {
        return new KeyBind(KeyCode);
    }
    
    public override int GetHashCode()
    {
        return KeyCode.GetHashCode();
    }
    
    public bool Equals(KeyBind other)
    {
        if (other is null)
            return false;
        return KeyCode == other.KeyCode;
    }
    
    public override bool Equals(object obj) => Equals(obj as KeyBind);
}
