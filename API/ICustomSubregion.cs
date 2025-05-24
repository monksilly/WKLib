namespace WK_Lib.API;

public interface ICustomSubregion
{
    string Key { get; }
    // List of level keys in this subregion
    string[] LevelKeys { get; }
}