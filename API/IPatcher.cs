using System;

namespace WK_Lib.API;

public interface IPatcher
{
    
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class PatchTargetAttribute : Attribute
{
    public Type TargetType { get; }
    public string MethodName { get; }

    public PatchTargetAttribute(Type targetType, string methodName)
    {
        TargetType = targetType;
        MethodName = methodName;
    }
}