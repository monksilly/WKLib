namespace WKLib.Core;

/// <summary>
/// Represents the context for a registered mod, containing its unique identifier and version.
/// </summary>
public class ModContext
{
    /// <summary>
    /// Unique identifier of the mod.
    /// </summary>
    public string ModID { get; }
    
    /// <summary>
    /// Version of the mod.<br/> This can be null if no version is specified.
    /// </summary>
    public string Version { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ModContext"/> class.<br/>
    /// This constructor is internal, meaning only types within the same assembly can create instances.
    /// </summary>
    /// <param name="modID">The unique identifier of the mod.</param>
    /// <param name="version">The version of the mod. Can be null.</param>
    internal ModContext(string modID, string version = null)
    {
        ModID = modID;
        Version = version;
    }
}