using System;
using UnityEngine;

namespace WKLib.Core.Config;

internal class ThemeConfig : IEquatable<ThemeConfig>, ICloneable
{
    public bool HighContrast = false;
    public Color AccentColor = new Color(0.05f, 0.45f, 0.75f, 1f);
    
    public ThemeConfig() { }

    public ThemeConfig(bool highContrast, Color accentColor)
    {
        HighContrast = highContrast;
        AccentColor = accentColor;
    }
    
    public object Clone()
    {
        return new ThemeConfig(HighContrast, AccentColor);
    }
    
    public override int GetHashCode()
    {
        return HighContrast.GetHashCode() + AccentColor.GetHashCode();
    }
    
    public bool Equals(ThemeConfig other)
    {
        if (other is null)
            return false;

        return HighContrast == other.HighContrast && AccentColor == other.AccentColor;
    }
    
    public override bool Equals(object obj) => Equals(obj as ThemeConfig);
}