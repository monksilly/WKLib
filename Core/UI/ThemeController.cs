using Imui.Controls;
using Imui.Core;
using Imui.IO.UGUI;
using Imui.Style;
using UnityEngine;
using WKLib.Core.Config;
using static WKLib.Core.Config.ConfigManager;
using static WKLib.API.UI.UIUtility;

namespace WKLib.Core.UI;

internal class ThemeController : MonoBehaviour
{
    public ImTheme BaseTheme = SetBaseTheme(ImThemeBuiltin.Dark());
    
    private Vector2 lastScreenSize = new Vector2(0f, 0f);

    public static ImTheme SetBaseTheme(ImTheme theme)
    {
        // Colors
        theme.Background = new Color(0f, 0f, 0f, 1f);
        theme.Foreground = new Color(1f, 1f, 1f, 1f);
        theme.Accent = ThemeSettings.Value.AccentColor;
        theme.Control = new Color(0.15f, 0.15f, 0.15f, 1f);

        theme.Contrast = ThemeSettings.Value.HighContrast ? 1f : 0f;
        theme.BorderContrast = 1f;
        
        // Values
        theme.TextSize = 16f;
        theme.BorderRadius = 1f;
        theme.ReadOnlyColorMultiplier = 0.25f;

        return theme;
    }
    
    public void DetectChanges(ImGui gui)
    {
        if (Screen.width == lastScreenSize.x && Screen.height == lastScreenSize.y)
            return;
        
        lastScreenSize = new Vector2(Screen.width, Screen.height);
    }

    public void DrawAppearanceEditor(ImGui gui)
    {
        if (gui.Checkbox(ref ThemeSettings.RefValue.HighContrast, "High contrast"))
        {
            BaseTheme.Contrast = ThemeSettings.Value.HighContrast ? 1f : 0f;
            SetTheme(gui);
            
            CoreSettings.Instance.DefaultConfigFile.SaveAsync();
        }

        if (gui.ColorEdit(ref ThemeSettings.RefValue.AccentColor))
        {
            BaseTheme.Accent = ThemeSettings.Value.AccentColor;
            SetTheme(gui);
            
            CoreSettings.Instance.DefaultConfigFile.SaveAsync();
        }
    }
    
    public static bool DrawThemeEditor(ImGui gui, ref ImTheme theme)
    {
        var changed = false;
        
        gui.Separator("Colors");

        using (new LabeledScope(gui, nameof(theme.Foreground))) changed |= gui.ColorEdit(ref theme.Foreground);
        using (new LabeledScope(gui, nameof(theme.Background))) changed |= gui.ColorEdit(ref theme.Background);
        using (new LabeledScope(gui, nameof(theme.Accent))) changed |= gui.ColorEdit(ref theme.Accent);
        using (new LabeledScope(gui, nameof(theme.Control))) changed |= gui.ColorEdit(ref theme.Control);
        using (new LabeledScope(gui, nameof(theme.Contrast))) changed |= gui.NumericEdit(ref theme.Contrast, min: -1.0f, max: +1.0f, flags: ImNumericEditFlag.Slider);
        using (new LabeledScope(gui, nameof(theme.BorderContrast))) changed |= gui.NumericEdit(ref theme.BorderContrast, min: -1.0f, max: +2.0f, flags: ImNumericEditFlag.Slider);

        gui.Separator("Values");

        using (new LabeledScope(gui, nameof(theme.TextSize))) changed |= gui.NumericEdit(ref theme.TextSize, min: 4.0f, max: 128.0f, flags: ImNumericEditFlag.Slider);
        using (new LabeledScope(gui, nameof(theme.Spacing))) changed |= gui.NumericEdit(ref theme.Spacing, min: 0.0f, max: 32.0f, flags: ImNumericEditFlag.Slider);
        using (new LabeledScope(gui, nameof(theme.InnerSpacing))) changed |= gui.NumericEdit(ref theme.InnerSpacing, min: 0.0f, max: 32.0f, flags: ImNumericEditFlag.Slider);
        using (new LabeledScope(gui, nameof(theme.Indent))) changed |= gui.NumericEdit(ref theme.Indent, min: 0.0f, max: 128.0f, flags: ImNumericEditFlag.Slider);
        using (new LabeledScope(gui, nameof(theme.ExtraRowHeight))) changed |= gui.NumericEdit(ref theme.ExtraRowHeight, min: 0.0f, max: 128.0f, flags: ImNumericEditFlag.Slider);
        using (new LabeledScope(gui, nameof(theme.ScrollBarSize))) changed |= gui.NumericEdit(ref theme.ScrollBarSize, min: 2.0f, max: 128.0f, flags: ImNumericEditFlag.Slider);
        using (new LabeledScope(gui, nameof(theme.WindowBorderRadius))) changed |= gui.NumericEdit(ref theme.WindowBorderRadius, min: 0.0f, max: 32.0f, flags: ImNumericEditFlag.Slider);
        using (new LabeledScope(gui, nameof(theme.WindowBorderThickness))) changed |= gui.NumericEdit(ref theme.WindowBorderThickness, min: 0.0f, max: 8.0f, step: 0.5f, flags: ImNumericEditFlag.Slider);
        using (new LabeledScope(gui, nameof(theme.BorderRadius))) changed |= gui.NumericEdit(ref theme.BorderRadius, min: 0.0f, max: 16.0f, flags: ImNumericEditFlag.Slider);
        using (new LabeledScope(gui, nameof(theme.BorderThickness))) changed |= gui.NumericEdit(ref theme.BorderThickness, min: 0.0f, max: 8.0f, step: 0.5f, flags: ImNumericEditFlag.Slider);
        using (new LabeledScope(gui, nameof(theme.ReadOnlyColorMultiplier))) changed |= gui.NumericEdit(ref theme.ReadOnlyColorMultiplier, min: 0.0f, max: 8.0f, flags: ImNumericEditFlag.Slider);
        
        return changed;
    }

    public void SetTheme(ImGui gui)
    {
        gui.SetTheme(BaseTheme);
    }
}