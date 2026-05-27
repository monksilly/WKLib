using System;
using System.Collections.Generic;
using System.Reflection;

namespace WKLib.Core.Reflection;

// https://github.com/yukieiji/UniverseLib/blob/main/src/Reflection/ReflectionUtility.cs

internal class ReflectionUtility
{
    /// <summary>Key: Type.FullName, Value: Type</summary>
    private static readonly SortedDictionary<string, Type> AllTypes = new(StringComparer.OrdinalIgnoreCase);

    internal static void Initialize()
    {
        SetupTypeCache();
    }
    
    internal static Type GetTypeByName(string fullName)
    {
        AllTypes.TryGetValue(fullName, out Type type);

        if (type == null)
            type = Type.GetType(fullName);

        return type;
    }
    
    private static void SetupTypeCache()
    {
        foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
        {
            CacheTypes(asm);
        }
    }
    
    private static void CacheTypes(Assembly asm)
    {
        Type[] asmTypes = null;
        try
        {
            asmTypes = asm.GetTypes();
        }
        catch
        {
            return;
        }
        if (asmTypes == null)
            return;
        
        foreach (Type type in asmTypes)
        {
            // Cache the type. Overwrite type if one exists with the full name
            AllTypes[type.FullName] = type;
        }
    }
}