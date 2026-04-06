using UnityEngine;

namespace WKLib.Core.Classes;

[DefaultExecutionOrder(-200)]
internal abstract class MonoSingleton : MonoBehaviour { }

internal abstract class MonoSingleton<T> : MonoSingleton where T : MonoSingleton<T>
{
    private static T instance = null;

    public static T Instance
    {
        get => instance;
        set => instance = value;
    }

    public virtual void Awake()
    {
        if (Instance && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = (T)this;
        }
    }

    public virtual void OnEnable()
    {
        Instance = (T)this;
    }
}
