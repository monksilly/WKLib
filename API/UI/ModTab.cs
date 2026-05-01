using Imui.Core;

namespace WKLib.API.UI;

public abstract class ModTab
{
    public abstract string DisplayName { get; }

    public abstract void DrawSubMenu(ImGui gui);
}