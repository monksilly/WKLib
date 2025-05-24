namespace WK_Lib.API;

public interface ICustomRegion
{
    string Key { get; }
    // List of subregion keys in this region
    string[] SubregionKeys { get; }
}