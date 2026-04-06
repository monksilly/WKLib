using Imui.Core;
using JetBrains.Annotations;

namespace WKLib.API.UI;

[PublicAPI]
public abstract class WKLibWindow
{
    public bool isOpen = false;
    public bool isMainConfigWindow = false;

    public abstract void Draw(ImGui gui, bool isRootPanelOpen);
    public abstract void HandleInput(ImGui gui);
}
