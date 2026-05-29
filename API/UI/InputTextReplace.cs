using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using WKLib.API.Input;

namespace WKLib.API.UI;

/// <summary>
/// Once placed on a TextMeshPro text it will be able to replace special tags into proper input names/symbols
/// for Input Actions : {a:ActionName}
/// for KeyCodes : {k:KeyCode}
/// for InputControl : {c:ControlPath}
/// up to two elements can be put in a tag separated by a comma, the first element is for keyboard/mouse while the second is for gamepad
/// eg: {k:KeyCode,c:ControlPath}
/// </summary>
[RequireComponent(typeof(TMP_Text))]
public class InputTextReplace : MonoBehaviour
{
    public string forceSchemeName = string.Empty;

    private TMP_Text _text;
    private PlayerInput _input;
    private string _originalText;
    private bool _lastIsGamepad;

    private void Awake()
    {
        _input = InputManager.GetPlayerInput();
    }

    private void OnEnable()
    {
        _lastIsGamepad = InputManager.IsGamepad();
        
        if (_text == null)
        {
            _text = GetComponent<TMP_Text>();
            if (_text == null)
                return;
            _originalText = _text.text;
        }

        ReloadText();
    }

    private void Update()
    {
        if (_lastIsGamepad == InputManager.IsGamepad()) return;
        
        ReloadText();
        _lastIsGamepad = InputManager.IsGamepad();
    }

    public void ReloadText()
    {
        _text.text = _originalText;
        
        int inputIndex = _text.text.IndexOf("{", StringComparison.Ordinal);
        while (inputIndex != -1)
        {
            string keyTag = _text.text.Substring(inputIndex,
                _text.text.IndexOf("}", inputIndex, StringComparison.Ordinal)-inputIndex+1);
            string tagContent = keyTag.Replace("{", "").Replace("}", "");

            if (tagContent.StartsWith("k:", StringComparison.Ordinal) ||
                tagContent.StartsWith("c:", StringComparison.Ordinal) ||
                tagContent.StartsWith("a:", StringComparison.Ordinal))
            {
                string[] keycodes = tagContent.Split(',');

                string keycodeId = tagContent;
                if (keycodes.Length == 2)
                    keycodeId = InputManager.IsGamepad() ? keycodes[1] : keycodes[0];
                
                _text.text = _text.text.Replace(keyTag, GetInputName(keycodeId));
            }
            
            inputIndex = _text.text.IndexOf("{",inputIndex+1, StringComparison.Ordinal);
        }
    }

    private string GetInputName(string tag)
    {
        if (string.IsNullOrEmpty(tag))
            return "Unknown Key";
        
        char prefix = tag[0];
        tag = tag.Remove(0, 2);
        
        switch (prefix)
        {
            case 'c':
                var control = InputSystem.FindControl(tag);
                if(control != null)
                    return control.displayName;
                break;
            
            case 'a':
                InputUser user = _input.user;
                if (!user.valid)
                    return "Unknown Key";
                
                string scheme = forceSchemeName == string.Empty ? _input.currentControlScheme : forceSchemeName;
                
                var action = user.actions.FirstOrDefault(a => a.name == tag);
                if (action != null)
                    return InputManager.QuickGetBindingDisplayString(scheme, action);
                break;
            
            case 'k':
                if (Enum.TryParse(tag, out KeyCode keycode))
                {
                    if (InputUtility.KeyCodeToControl(keycode) is InputControl keycodeControl)
                        return keycodeControl.displayName;
                }
                break;
        }

        return "Unknown Key";
    }
}