namespace WKLib.Core;

public class ModContext
{
    public string ModID { get; }
    public string Version { get; }

    internal ModContext(string modID, string version = null)
    {
        ModID = modID;
        Version = version;
    }
}