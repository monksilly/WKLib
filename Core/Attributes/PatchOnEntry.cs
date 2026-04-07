using System;

namespace WKLib.Core.Attributes;

/// <summary>
///     Marker attribute to indicate that a patch class will be applied in the Plugin.Awake method.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false)]
internal sealed class PatchOnEntryAttribute : Attribute;