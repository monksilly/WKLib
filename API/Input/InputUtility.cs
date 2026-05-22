using System;
using System.Collections.Generic;
using System.Linq;
using Imui.Core;
using Imui.IO.Events;
using UnityEngine;
using UnityEngine.InputSystem;

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
        
        // Handle mouse buttons
        // Ignore Mouse0 (Left click)
        currentKeys[KeyCode.Mouse1] = UnityEngine.Input.GetKey(KeyCode.Mouse1);
        currentKeys[KeyCode.Mouse2] = UnityEngine.Input.GetKey(KeyCode.Mouse2);
        currentKeys[KeyCode.Mouse3] = UnityEngine.Input.GetKey(KeyCode.Mouse3);
        currentKeys[KeyCode.Mouse4] = UnityEngine.Input.GetKey(KeyCode.Mouse4);
        currentKeys[KeyCode.Mouse5] = UnityEngine.Input.GetKey(KeyCode.Mouse5);
        currentKeys[KeyCode.Mouse6] = UnityEngine.Input.GetKey(KeyCode.Mouse6);
        
        // Handle gamepad
        var gamepad = Gamepad.current;
        if (gamepad == null)
            return;
        
        // Not the actual way unity uses these keycodes, but you know..... its fine
        currentKeys[KeyCode.JoystickButton0] = gamepad.buttonSouth.isPressed;
        currentKeys[KeyCode.JoystickButton1] = gamepad.buttonEast.isPressed;
        currentKeys[KeyCode.JoystickButton2] = gamepad.buttonWest.isPressed;
        currentKeys[KeyCode.JoystickButton3] = gamepad.buttonNorth.isPressed;
        
        currentKeys[KeyCode.JoystickButton4] = gamepad.leftShoulder.isPressed;
        currentKeys[KeyCode.JoystickButton5] = gamepad.rightShoulder.isPressed;

        currentKeys[KeyCode.JoystickButton6] = gamepad.selectButton.isPressed;
        currentKeys[KeyCode.JoystickButton7] = gamepad.startButton.isPressed;

        currentKeys[KeyCode.JoystickButton8] = gamepad.leftStickButton.isPressed;
        currentKeys[KeyCode.JoystickButton9] = gamepad.rightStickButton.isPressed;

        currentKeys[KeyCode.JoystickButton10] = gamepad.dpad.up.isPressed;
        currentKeys[KeyCode.JoystickButton11] = gamepad.dpad.down.isPressed;
        currentKeys[KeyCode.JoystickButton12] = gamepad.dpad.left.isPressed;
        currentKeys[KeyCode.JoystickButton13] = gamepad.dpad.right.isPressed;
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

    public static KeyCode? GetFirstActiveKey()
    {
        foreach (var pair in currentKeys)
        {
            if (pair.Value)
                return pair.Key;
        }

        return null;
    }
}
