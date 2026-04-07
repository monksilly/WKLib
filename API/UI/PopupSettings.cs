using UnityEngine;

namespace WKLib.API.UI;

public class PopupSettings
{
    public PopupSettings() { }
    public PopupSettings(string text, float seconds = 2.5f)
    {
        PopupText = text;
        PopupTime = seconds;
        
        TimeTillClose = Time.realtimeSinceStartup + PopupTime;
    }
    
    public string PopupText = "";
    public float PopupTime = 2.5f;

    public float TimeTillClose = -1f;
}