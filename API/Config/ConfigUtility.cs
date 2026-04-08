using System;

namespace WKLib.API.Config;

internal static class ConfigUtility
{
    public static T CloneIfPossible<T> (T obj)
    {
        if (obj is ICloneable cloneable)
            return (T)cloneable.Clone();

        return obj;
    }   
}