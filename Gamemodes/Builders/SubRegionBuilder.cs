using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WKLib.Gamemodes.Builders;

/// <summary>
/// Builds an <see cref="M_Subregion"/> from a name plus a list of <see cref="M_Level"/> instances.
/// </summary>
public class SubRegionBuilder
{
    private string _name = "New Subregion";
    private List<M_Level> _levels = [];

    public SubRegionBuilder WithName(string subregionName)
    {
        _name = subregionName;
        return this;
    }

    public SubRegionBuilder WithLevels(List<M_Level> levels)
    {
        _levels = levels ?? [];
        return this;
    }

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