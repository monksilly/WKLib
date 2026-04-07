using System;
using System.Collections.Generic;
using Imui.Core;
using Imui.IO.Events;
using UnityEngine;

namespace WKLib.API.Input;

public static class InputUtility
{
    private static Dictionary<KeyCode, bool> keysDictionary = new Dictionary<KeyCode, bool>();
        
    public static void HandleInput(ImGui gui)
    {
        if (keysDictionary.Keys.Count <= 0)
        {
            foreach (KeyCode keyCode in Enum.GetValues(typeof(KeyCode)))
            {
                keysDictionary[keyCode] = false;
            }
        }
        
        for (int i = 0; i < gui.Input.KeyboardEventsCount; ++i)
        {
            var keyboardEvent = gui.Input.GetKeyboardEvent(i);
            var key = keyboardEvent.Key;

            bool isDown = keyboardEvent.Type == ImKeyboardEventType.Down;
            bool isUp = keyboardEvent.Type == ImKeyboardEventType.Up;

            if (isDown)
            {
                keysDictionary[key] = true;
            }
            else if (isUp)
            {
                keysDictionary[key] = false;
            }
        }
    }

    public static bool GetKeyDown(KeyCode key)
    {
        if (keysDictionary.TryGetValue(key, out var keyState))
            return keyState;
        throw new Exception($"No key ({key.ToString()}) could be found in {nameof(InputUtility)}.{nameof(keysDictionary)}.");
    }
}