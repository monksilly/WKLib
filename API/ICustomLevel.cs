namespace WK_Lib.API;

public interface ICustomLevel
{
    string Key { get; }
    // Called to instantiate or fetch the level object
    UnityEngine.GameObject LoadLevel();
}