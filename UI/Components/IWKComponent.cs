namespace WKLib.UI.Components;

/// <summary>
/// Interface for all WKLib UI components
/// </summary>
public interface IWKComponent
{
    /// <summary>
    /// Reset the component state for recreation in a new scene
    /// </summary>
    void Reset();
    
    /// <summary>
    /// Called by UIManager when it's ready to create GameObjects
    /// </summary>
    void CreateGameObject();
} 