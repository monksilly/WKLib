using System;
using System.Collections.Generic;
using Imui.Core;
using Imui.IO.Events;
using UnityEngine;

namespace WKLib.API.Input;

public static class InputUtility
{
    private static Dictionary<KeyCode, bool> currentKeys = new();
    private static Dictionary<KeyCode, bool> previousKeys = new();
    
    internal static void HandleInput(ImGui gui)
    {
        previousKeys.Clear();

        // Copy current state into previous state
        foreach (var pair in currentKeys)
        {
            previousKeys[pair.Key] = pair.Value;
        }
        
        for (int i = 0; i < gui.Input.KeyboardEventsCount; ++i)
        {
            var keyboardEvent = gui.Input.GetKeyboardEvent(i);
            var key = keyboardEvent.Key;

            switch (keyboardEvent.Type)
            {
                case ImKeyboardEventType.Down:
                    currentKeys[key] = true;
                    break;

                case ImKeyboardEventType.Up:
                    currentKeys[key] = false;
                    break;
            }
        }
    }
    
    public static bool GetKey(KeyCode key)
    {
        return currentKeys.GetValueOrDefault(key, false);
    }

    public static bool GetKeyDown(KeyCode key)
    {
        bool current = currentKeys.GetValueOrDefault(key, false);
        bool previous = previousKeys.GetValueOrDefault(key, false);

        return current && !previous;
    }

    public static bool GetKeyUp(KeyCode key)
    {
        bool current = currentKeys.GetValueOrDefault(key, false);
        bool previous = previousKeys.GetValueOrDefault(key, false);

        return !current && previous;
    }
    
}