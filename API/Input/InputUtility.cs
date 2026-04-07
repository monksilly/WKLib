using System;
using System.Collections.Generic;
using Imui.Core;
using Imui.IO.Events;
using UnityEngine;

namespace WKLib.API.Input;

// TODO: Add more keys (basically all of them)
public static class InputUtility
{
    private static Dictionary<KeyCode, bool> keys = new Dictionary<KeyCode, bool>()
    {
        { KeyCode.RightShift, false },
        { KeyCode.LeftShift, false },
        { KeyCode.RightControl, false },
        { KeyCode.LeftControl,false },
        { KeyCode.RightAlt, false },
        { KeyCode.LeftAlt, false },
    };
    
    public static void HandleInput(ImGui gui)
    {
        for (int i = 0; i < gui.Input.KeyboardEventsCount; ++i)
        {
            var keyboardEvent = gui.Input.GetKeyboardEvent(i);
            var key = keyboardEvent.Key;

            if (keys.ContainsKey(key))
            {
                bool isDown = keyboardEvent.Type == ImKeyboardEventType.Down;
                bool isUp = keyboardEvent.Type == ImKeyboardEventType.Up;

                if (isDown)
                {
                    keys[key] = true;
                }
                else if (isUp)
                {
                    keys[key] = false;
                }
            }
        }
    }

    public static bool GetKeyDown(KeyCode key)
    {
        if (keys.TryGetValue(key, out var keyState))
            return keyState;
        throw new Exception($"No key ({key.ToString()}) could be found in {nameof(InputUtility)}.{nameof(keys)}.");
    }
}