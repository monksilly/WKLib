using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WKLib.Gamemodes.Builders;

/// <summary>
/// A fluent builder class for creating and configuring an <see cref="M_Region"/> instance<br/>
/// It simplifies the process of defining a region with its name, subregions, and other properties
/// </summary>
public class RegionBuilder
{
    private string _name = "New Region";
    private List<M_Subregion> _subregions = [];
    private bool _hasStartingLevel = true;

    /// <summary>
    /// Sets the name for the region. This name will be used for both display and internal identification
    /// </summary>
    /// <param name="regionName">The desired name for the region</param>
    /// <returns>The current <see cref="RegionBuilder"/> instance for fluent chaining</returns>
    public RegionBuilder WithName(string regionName)
    {
        _name = regionName;
        return this;
    }

    /// <summary>
    /// Sets the list of subregions that belong to this region
    /// </summary>
    /// <param name="subregions">A <see cref="List{T}"/> of <see cref="M_Subregion"/> instances</param>
    /// <returns>The current <see cref="RegionBuilder"/> instance for fluent chaining</returns>
    public RegionBuilder WithSubregions(List<M_Subregion> subregions)
    {
        _subregions = subregions ?? [];
        return this;
    }

    /// <summary>
    /// Sets whether the region should attempt to include a default starting level
    /// </summary>
    /// <param name="hasStartingLevel">A boolean value. If true, the builder will look for a default starting level</param>
    /// <returns>The current <see cref="RegionBuilder"/> instance for fluent chaining</returns>
    /// <remarks>
    /// This method is marked as obsolete because the game's internal logic often handles the selection of starting levels automatically<br/>
    /// It's generally recommended to let the game decide on the starting level
    /// </remarks>
    [Obsolete("Game Handles this automatically, Let the game decide")]
    public RegionBuilder WithStartingLevel(bool hasStartingLevel)
    {
        _hasStartingLevel = hasStartingLevel;
        return this;
    }

    /// <summary>
    /// Constructs and returns a new <see cref="M_Region"/> instance based on the properties configured in this builder
    /// </summary>
    /// <returns>A new <see cref="M_Region"/> object populated with the specified settings</returns>
    public M_Region Build()
    {
        var region = ScriptableObject.CreateInstance<M_Region>();
        region.regionName = _name;
        region.name = _name;

        List<M_Region.SubregionGroup> subRegGroups = [];
        foreach (var subregion in _subregions)
        {
            subRegGroups.Add(new M_Region.SubregionGroup(){subregions = [subregion]});
        }
        
        // Wrap the subregions in SubregionGroups
        region.subregionGroups = subRegGroups;

        if (_subregions.Count > 1 && _subregions[0].levels.Count > 1)
        {
            region.transitionLevels =
            [
                new M_Region.TransitionLevels() {
                    fromRegion = _name,
                    levels = [_subregions[0].levels[^1]]
                }
            ];
            
            _subregions[0].levels.RemoveAt(_subregions[0].levels.Count - 1);
        }
        
        region.regionHeight = _subregions.Sum(sr => sr.subregionHeight);
        

        // pick a default start level - M1_Intro_01 (if exists)
        var defaultPrefab = CL_AssetManager.instance.levelPrefabs
            .FirstOrDefault(pref => pref.name == "M1_Intro_01");
        var defaultLevelComp = defaultPrefab?.GetComponent<M_Level>();
        if (defaultLevelComp is not null && _hasStartingLevel)
        {
            region.startLevels = [defaultLevelComp];
        }

        region.regionOrder = M_Region.RegionOrder.playlist;
        region.introText = _name;

        // Flatten all sessionEventLists from subregions
        region.sessionEventLists = _subregions
            .SelectMany(sr => sr.sessionEventLists)
            .ToList();

        return region;
    }
}