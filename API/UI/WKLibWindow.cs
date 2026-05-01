using Imui.Core;

namespace WKLib.API.UI;

public abstract class WKLibWindow
{
    public bool isOpen = false;

    public abstract void Draw(ImGui gui, bool isRootPanelOpen);
    public abstract void HandleInput(ImGui gui);
}
