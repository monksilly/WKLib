using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WKLib.Gamemodes.Builders;

/// <summary>
/// Builds an <see cref="M_Subregion"/> from a name plus a list of <see cref="M_Level"/> instances<br/>
/// This builder simplifies the creation of subregion ScriptableObjects
/// </summary>
public class SubRegionBuilder
{
    private string _name = "New Subregion";
    private List<M_Level> _levels = [];

    /// <summary>
    /// Sets the name for the subregion. This name will be used for both display and internal identification
    /// </summary>
    /// <param name="subregionName">The desired name for the subregion</param>
    /// <returns>The current <see cref="SubRegionBuilder"/> instance for fluent chaining</returns>
    public SubRegionBuilder WithName(string subregionName)
    {
        _name = subregionName;
        return this;
    }

    /// <summary>
    /// Sets the list of <see cref="M_Level"/> instances that comprise this subregion
    /// </summary>
    /// <param name="levels">A <see cref="List{T}"/> of <see cref="M_Level"/> instances to be included in the subregion</param>
    /// <returns>The current <see cref="SubRegionBuilder"/> instance for fluent chaining</returns>
    public SubRegionBuilder WithLevels(List<M_Level> levels)
    {
        _levels = levels ?? [];
        return this;
    }

    /// <summary>
    /// Constructs and returns a new <see cref="M_Subregion"/> instance based on the properties configured in this builder
    /// </summary>
    /// <returns>A new <see cref="M_Subregion"/> object populated with the specified settings</returns>
    public M_Subregion Build()
    {
        var subregion = ScriptableObject.CreateInstance<M_Subregion>();
        subregion.subregionName = _name;
        subregion.name = _name;
        subregion.levels = _levels;
        subregion.subregionHeight = _levels.Sum(lv => lv.GetHeight());
        subregion.sessionEventLists = _levels.SelectMany(lv => lv.sessionEventLists).ToList();
        return subregion;
    }
}