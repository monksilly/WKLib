using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using WKLib.Core.Reflection;

namespace WKLib.API.Input;

// https://github.com/yukieiji/UniverseLib/blob/main/src/Input/InputSystem.cs

public static class InputUtility
{
    #region Reflection cache
    // typeof(InputSystem.Keyboard)
    static Type TKeyboard => t_Keyboard ??= ReflectionUtility.GetTypeByName("UnityEngine.InputSystem.Keyboard");
    static Type? t_Keyboard;

    // typeof(InputSystem.Mouse)
    static Type TMouse => t_Mouse ??= ReflectionUtility.GetTypeByName("UnityEngine.InputSystem.Mouse");
    static Type? t_Mouse;
    
    // typeof(InputSystem.Gamepad)
    static Type TGamepad => t_Gamepad ??= ReflectionUtility.GetTypeByName("UnityEngine.InputSystem.Gamepad");
    static Type? t_Gamepad;
    
    // typeof (InputSystem.Key)
    static Type TKey => t_Key ??= ReflectionUtility.GetTypeByName("UnityEngine.InputSystem.Key");
    static Type? t_Key;

    // InputSystem.Controls.ButtonControl.isPressed
    static PropertyInfo? p_btnIsPressed;
    // InputSystem.Controls.ButtonControl.wasPressedThisFrame
    static PropertyInfo? p_btnWasPressed;
    // InputSystem.Controls.ButtonControl.wasReleasedThisFrame
    static PropertyInfo? p_btnWasReleased;

    // Keyboard.current
    static object? CurrentKeyboard => p_kbCurrent?.GetValue(null, null);
    static PropertyInfo? p_kbCurrent;
    // Keyboard.this[Key]
    static PropertyInfo? p_kbIndexer;

    // Mouse.current
    static object? CurrentMouse => p_mouseCurrent?.GetValue(null, null);
    static PropertyInfo? p_mouseCurrent;

    // Mouse.current.leftButton
    static object? LeftMouseButton => p_leftButton?.GetValue(CurrentMouse, null);
    static PropertyInfo? p_leftButton;

    // Mouse.current.rightButton
    static object? RightMouseButton => p_rightButton?.GetValue(CurrentMouse, null);
    static PropertyInfo? p_rightButton;

    // Mouse.current.middleButton
    static object? MiddleMouseButton => p_middleButton?.GetValue(CurrentMouse, null);
    static PropertyInfo? p_middleButton;

    // Mouse.current.forwardButton
    static object? ForwardMouseButton => p_forwardButton?.GetValue(CurrentMouse, null);
    static PropertyInfo? p_forwardButton;

    // Mouse.current.backButton
    static object? BackMouseButton => p_backButton?.GetValue(CurrentMouse, null);
    static PropertyInfo? p_backButton;
    
    // Gamepad.current
    static object? CurrentGamepad => p_gamepadCurrent?.GetValue(null, null);
    static PropertyInfo? p_gamepadCurrent;
    
    // Gamepad.current.buttonWest
    static object? ButtonGamepadWest => p_buttonWest?.GetValue(CurrentGamepad, null);
    static PropertyInfo? p_buttonWest;

    // Gamepad.current.buttonNorth
    static object? ButtonGamepadNorth => p_buttonNorth?.GetValue(CurrentGamepad, null);
    static PropertyInfo? p_buttonNorth;

    // Gamepad.current.buttonSouth
    static object? ButtonGamepadSouth => p_buttonSouth?.GetValue(CurrentGamepad, null);
    static PropertyInfo? p_buttonSouth;

    // Gamepad.current.buttonEast
    static object? ButtonGamepadEast => p_buttonEast?.GetValue(CurrentGamepad, null);
    static PropertyInfo? p_buttonEast;

    // Gamepad.current.startButton
    static object? StartButton => p_startButton?.GetValue(CurrentGamepad, null);
    static PropertyInfo? p_startButton;

    // Gamepad.current.selectButton
    static object? SelectButton => p_selectButton?.GetValue(CurrentGamepad, null);
    static PropertyInfo? p_selectButton;

    // Gamepad.current.leftStickButton
    static object? LeftStickButton => p_leftStickButton?.GetValue(CurrentGamepad, null);
    static PropertyInfo? p_leftStickButton;

    // Gamepad.current.rightStickButton
    static object? RightStickButton => p_rightStickButton?.GetValue(CurrentGamepad, null);
    static PropertyInfo? p_rightStickButton;

    // Gamepad.current.dpad
    static object? Dpad => p_dpad?.GetValue(CurrentGamepad, null);
    static PropertyInfo? p_dpad;

    // Gamepad.current.dpad.up
    static object? DpadUp => p_dpadUp?.GetValue(Dpad, null);
    static PropertyInfo? p_dpadUp;

    // Gamepad.current.dpad.down
    static object? DpadDown => p_dpadDown?.GetValue(Dpad, null);
    static PropertyInfo? p_dpadDown;

    // Gamepad.current.dpad.left
    static object? DpadLeft => p_dpadLeft?.GetValue(Dpad, null);
    static PropertyInfo? p_dpadLeft;

    // Gamepad.current.dpad.right
    static object? DpadRight => p_dpadRight?.GetValue(Dpad, null);
    static PropertyInfo? p_dpadRight;

    // Gamepad.current.leftShoulder
    static object? LeftShoulder => p_leftShoulder?.GetValue(CurrentGamepad, null);
    static PropertyInfo? p_leftShoulder;

    // Gamepad.current.rightShoulder
    static object? RightShoulder => p_rightShoulder?.GetValue(CurrentGamepad, null);
    static PropertyInfo? p_rightShoulder;

    // Gamepad.current.leftStick
    static object? LeftStick => p_leftStick?.GetValue(CurrentGamepad, null);
    static PropertyInfo? p_leftStick;

    // Gamepad.current.rightStick
    static object? RightStick => p_rightStick?.GetValue(CurrentGamepad, null);
    static PropertyInfo? p_rightStick;

    // Gamepad.current.leftTrigger
    static object? LeftTrigger => p_leftTrigger?.GetValue(CurrentGamepad, null);
    static PropertyInfo? p_leftTrigger;

    // Gamepad.current.rightTrigger
    static object? RightTrigger => p_rightTrigger?.GetValue(CurrentGamepad, null);
    static PropertyInfo? p_rightTrigger;

    // InputSystem.InputControl<Vector2>.ReadValue()
    static MethodInfo? m_ReadV2Control;

    // Mouse.current.position
    static object? MousePositionInfo => p_position?.GetValue(CurrentMouse, null);
    static PropertyInfo? p_position;

    // Mouse.current.scroll
    static object? MouseScrollInfo => p_scrollDelta?.GetValue(CurrentMouse, null);
    static PropertyInfo? p_scrollDelta;
    #endregion

    internal static bool Initialized = false;
    
    internal static void Initialize()
    {
        if (Initialized)
            return;

        p_kbCurrent = TKeyboard.GetProperty("current");
        p_kbIndexer = TKeyboard.GetProperty("Item",  new[] {TKey} );

        Type t_btnControl = ReflectionUtility.GetTypeByName("UnityEngine.InputSystem.Controls.ButtonControl");
        p_btnIsPressed = t_btnControl.GetProperty("isPressed");
        p_btnWasPressed = t_btnControl.GetProperty("wasPressedThisFrame");
        p_btnWasReleased = t_btnControl.GetProperty("wasReleasedThisFrame");

        p_mouseCurrent = TMouse.GetProperty("current");
        p_leftButton = TMouse.GetProperty("leftButton");
        p_rightButton = TMouse.GetProperty("rightButton");
        p_middleButton = TMouse.GetProperty("middleButton");
        p_backButton = TMouse.GetProperty("backButton");
        p_forwardButton = TMouse.GetProperty("forwardButton");
        p_scrollDelta = TMouse.GetProperty("scroll");

        p_position = ReflectionUtility.GetTypeByName("UnityEngine.InputSystem.Pointer")
                       .GetProperty("position");
        
        p_gamepadCurrent = TGamepad.GetProperty("current");
        p_buttonWest = TGamepad.GetProperty("buttonWest");
        p_buttonNorth = TGamepad.GetProperty("buttonNorth");
        p_buttonSouth = TGamepad.GetProperty("buttonSouth");
        p_buttonEast = TGamepad.GetProperty("buttonEast");
        p_startButton = TGamepad.GetProperty("startButton");
        p_selectButton = TGamepad.GetProperty("selectButton");
        p_leftStickButton = TGamepad.GetProperty("leftStickButton");
        p_rightStickButton = TGamepad.GetProperty("rightStickButton");
        
        p_dpad = TGamepad.GetProperty("dpad");

        Type t_dpad = ReflectionUtility.GetTypeByName("UnityEngine.InputSystem.Controls.DpadControl");

        p_dpadUp = t_dpad.GetProperty("up");
        p_dpadDown = t_dpad.GetProperty("down");
        p_dpadLeft = t_dpad.GetProperty("left");
        p_dpadRight = t_dpad.GetProperty("right");
        
        p_leftShoulder = TGamepad.GetProperty("leftShoulder");
        p_rightShoulder = TGamepad.GetProperty("rightShoulder");
        p_leftStick = TGamepad.GetProperty("leftStick");
        p_rightStick = TGamepad.GetProperty("rightStick");
        p_leftTrigger = TGamepad.GetProperty("leftTrigger");
        p_rightTrigger = TGamepad.GetProperty("rightTrigger");

        m_ReadV2Control = ReflectionUtility.GetTypeByName("UnityEngine.InputSystem.InputControl`1")
                                  .MakeGenericType(typeof(Vector2))
                                  .GetMethod("ReadValue");

        Initialized = true;
    }
    
    #region KeyCode -> control helpers
    private static Dictionary<KeyCode, object?> KeyCodeToKeyboardKeyCache = new();

    private static readonly Dictionary<string, string> keycodeToKeyFixes = new()
    {
        { "Control", "Ctrl" },
        { "Return", "Enter" },
        { "Alpha", "Digit" },
        { "Keypad", "Numpad" },
        { "Numlock", "NumLock" },
        { "Print", "PrintScreen" },
        { "BackQuote", "Backquote" }
    };

    private enum KeyCodeDeviceType
    {
        None,
        Keyboard,
        Mouse,
        Gamepad
    }

    private static bool IsJoystickKeyCode(KeyCode key)
    {
        return key >= KeyCode.JoystickButton0 && key <= KeyCode.JoystickButton19;
    }

    private static bool IsMouseKeyCode(KeyCode key)
    {
        return key >= KeyCode.Mouse0 && key <= KeyCode.Mouse6;
    }

    private static object? KeyCodeToKeyEnum(KeyCode key)
    {
        string s = key.ToString();

        foreach (KeyValuePair<string, string> fix in keycodeToKeyFixes)
        {
            if (s.Contains(fix.Key))
            {
                s = s.Replace(fix.Key, fix.Value);
                break;
            }
        }

        try
        {
            return Enum.Parse(TKey, s);
        }
        catch
        {
            return null;
        }
    }
    
    private static object? KeyCodeToKeyboardControl(KeyCode key)
    {
        if (CurrentKeyboard == null)
            return null;
        
        if (KeyCodeToKeyboardKeyCache.TryGetValue(key, out object? cached))
            return cached;

        object? result = null;

        if (p_kbIndexer is not null && CurrentKeyboard is not null)
        {
            object? parsed = KeyCodeToKeyEnum(key);

            if (parsed is not null)
            {
                try
                {
                    result = p_kbIndexer.GetValue(CurrentKeyboard, [parsed]);
                }
                catch
                {
                    result = null;
                }
            }
        }

        KeyCodeToKeyboardKeyCache[key] = result;
        return result;
    }
    
    private static object? KeyCodeToMouseControl(KeyCode key)
    {
        if (CurrentMouse == null)
            return null;
        
        return key switch
        {
            KeyCode.Mouse0 => LeftMouseButton,
            KeyCode.Mouse1 => RightMouseButton,
            KeyCode.Mouse2 => MiddleMouseButton,
            KeyCode.Mouse3 => BackMouseButton,
            KeyCode.Mouse4 => ForwardMouseButton,
            _ => null
        };
    }
    
    private static object? KeyCodeToGamepadControl(KeyCode key)
    {
        if (CurrentGamepad == null)
            return null;
        
        return key switch
        {
            KeyCode.JoystickButton0 => ButtonGamepadSouth,
            KeyCode.JoystickButton1 => ButtonGamepadEast,
            KeyCode.JoystickButton2 => ButtonGamepadWest,
            KeyCode.JoystickButton3 => ButtonGamepadNorth,
            KeyCode.JoystickButton4 => LeftShoulder,
            KeyCode.JoystickButton5 => RightShoulder,
            KeyCode.JoystickButton6 => LeftTrigger,
            KeyCode.JoystickButton7 => RightTrigger,
            KeyCode.JoystickButton8 => SelectButton,
            KeyCode.JoystickButton9 => StartButton,
            KeyCode.JoystickButton10 => LeftStickButton,
            KeyCode.JoystickButton11 => RightStickButton,
            KeyCode.JoystickButton12 => DpadUp,
            KeyCode.JoystickButton13 => DpadDown,
            KeyCode.JoystickButton14 => DpadLeft,
            KeyCode.JoystickButton15 => DpadRight,
            _ => null
        };
    }
    
    private static KeyCodeDeviceType GetDeviceType(KeyCode key)
    {
        if (IsMouseKeyCode(key))
            return KeyCodeDeviceType.Mouse;

        if (IsJoystickKeyCode(key))
            return KeyCodeDeviceType.Gamepad;

        return KeyCodeDeviceType.Keyboard;
    }

    private static object? KeyCodeToControl(KeyCode key)
    {
        object? control = null;

        switch (GetDeviceType(key))
        {
            case KeyCodeDeviceType.Mouse:
                control = KeyCodeToMouseControl(key);
                break;

            case KeyCodeDeviceType.Gamepad:
                control = KeyCodeToGamepadControl(key);
                break;

            case KeyCodeDeviceType.Keyboard:
                control = KeyCodeToKeyboardControl(key);
                break;
        }
        
        return control;
    }

    private static bool TryGetControlState(object? control, PropertyInfo? stateProperty, out bool state)
    {
        state = false;

        if (control is null || stateProperty is null)
            return false;

        try
        {
            object? value = stateProperty.GetValue(control, null);
            if (value is bool b)
            {
                state = b;
                return true;
            }
        }
        catch
        {
            return false;
        }

        return false;
    }
    #endregion
    
    public static bool GetKeyDown(KeyCode key)
    {
        if (!Initialized || p_btnWasPressed is null)
            return false;

        object? control = KeyCodeToControl(key);
        return TryGetControlState(control, p_btnWasPressed, out bool state) && state;
    }

    public static bool GetKey(KeyCode key)
    {
        if (!Initialized || p_btnIsPressed is null)
            return false;

        object? control = KeyCodeToControl(key);
        return TryGetControlState(control, p_btnIsPressed, out bool state) && state;
    }

    public static bool GetKeyUp(KeyCode key)
    {
        if (!Initialized || p_btnWasReleased is null)
            return false;

        object? control = KeyCodeToControl(key);
        return TryGetControlState(control, p_btnWasReleased, out bool state) && state;
    }

    public static KeyCode? GetFirstActiveKey()
    {
        if (!Initialized)
            return null;

        foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
        {
            object? control = KeyCodeToControl(key);
            if (TryGetControlState(control, p_btnIsPressed, out bool state) && state)
                return key;
        }

        return null;
    }
}