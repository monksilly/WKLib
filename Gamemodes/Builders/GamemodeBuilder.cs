using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WKLib.Utilities;

namespace WKLib.Gamemodes.Builders;

/// <summary>
/// A fluent builder class for creating and configuring an <see cref="M_Gamemode"/> instance.<br/>
/// It allows setting various properties of a gamemode such as regions, name, intro text,
/// and gameplay flags in a chainable manner before finally building the <see cref="M_Gamemode"/> object.
/// </summary>
public class GamemodeBuilder
{
    private List<M_Region> _regions = [];
    private string _modeName = "Custom Gamemode";
    private string _introText = "ASCEND";
    private bool _isEndless;
    private bool _hasPerks;
    private bool _hasRevives;
    private Sprite _capsuleSprite;
    private Sprite _screenArtSprite;
    private List<M_Gamemode.SpawnItem> _spawnItems;
    private M_Gamemode.GameType _gameType;
    
    /// <summary>
    /// Sets the list of regions for this gamemode
    /// </summary>
    /// <param name="regions">A <see cref="List{T}"/> of <see cref="M_Region"/> instances to be used in the gamemode</param>
    /// <returns>The current <see cref="GamemodeBuilder"/> instance for fluent chaining</returns>
    public GamemodeBuilder WithRegions(List<M_Region> regions)
    {
        _regions = regions ?? [];
        return this;
    }
    
    /// <summary>
    /// Sets the displayed name of the gamemode (which can also serves as the capsule name)
    /// </summary>
    /// <param name="modeName">The desired name for the gamemode</param>
    /// <returns>The current <see cref="GamemodeBuilder"/> instance for fluent chaining</returns>
    public GamemodeBuilder WithName(string modeName)
    {
        _modeName = modeName;
        return this;
    }
    
    /// <summary>
    /// Sets the intro text for the gamemode<br/>
    /// This text is shown when loaded into the gamemode
    /// </summary>
    /// <param name="introText">The introductory text</param>
    /// <returns>The current <see cref="GamemodeBuilder"/> instance for fluent chaining</returns>
    public GamemodeBuilder WithIntroText(string introText)
    {
        _introText = introText;
        return this;
    }
    
    /// <summary>
    /// Sets whether the gamemode should be endless
    /// </summary>
    /// <param name="isEndless">A boolean value indicating if the gamemode is endless</param>
    /// <returns>The current <see cref="GamemodeBuilder"/> instance for fluent chaining</returns>
    public GamemodeBuilder IsEndless(bool isEndless)
    {
        _isEndless = isEndless;
        return this;
    }
    
    /// <summary>
    /// Sets whether perks are available in this gamemode
    /// </summary>
    /// <param name="hasPerks">A boolean value indicating if perks are enabled</param>
    /// <returns>The current <see cref="GamemodeBuilder"/> instance for fluent chaining</returns>
    public GamemodeBuilder HasPerks(bool hasPerks)
    {
        _hasPerks = hasPerks;
        return this;
    }
    
    /// <summary>
    /// Sets whether revives are available in this gamemode
    /// </summary>
    /// <param name="hasRevives">A boolean value indicating if revives are enabled</param>
    /// <returns>The current <see cref="GamemodeBuilder"/> instance for fluent chaining</returns>
    public GamemodeBuilder HasRevives(bool hasRevives)
    {
        _hasRevives = hasRevives;
        return this;
    }
    
    /// <summary>
    /// Provides a <see cref="Sprite"/> to use for the gamemode's capsule art
    /// </summary>
    /// <param name="sprite">The <see cref="Sprite"/> to set as the capsule art</param>
    /// <returns>The current <see cref="GamemodeBuilder"/> instance for fluent chaining</returns>
    public GamemodeBuilder WithCapsuleSprite(Sprite sprite)
    {
        _capsuleSprite = sprite;
        return this;
    }

    /// <summary>
    /// Provides a <see cref="Sprite"/> to use for the gamemode's screen art
    /// </summary>
    /// <param name="sprite">The <see cref="Sprite"/> to set as the screen art</param>
    /// <returns>The current <see cref="GamemodeBuilder"/> instance for fluent chaining</returns>
    public GamemodeBuilder WithScreenArt(Sprite sprite)
    {
        _screenArtSprite = sprite;
        return this;
    }

    /// <summary>
    /// Sets the list of items players will start with when entering this gamemode
    /// </summary>
    /// <param name="spawnItems">A <see cref="List{T}"/> of <see cref="M_Gamemode.SpawnItem"/> defining the initial inventory</param>
    /// <returns>The current <see cref="GamemodeBuilder"/> instance for fluent chaining</returns>
    public GamemodeBuilder WithStartItems(List<M_Gamemode.SpawnItem> spawnItems)
    {
        _spawnItems = spawnItems;
        return this;
    }

    /// <summary>
    /// Sets the game type for the gamemode based on a string input
    /// </summary>
    /// <param name="gameType">A string representing the desired game type (e.g., "endless", "standard", "playlist", "playlist-shuffle", "single")</param>
    /// <returns>The current <see cref="GamemodeBuilder"/> instance for fluent chaining</returns>
    /// <remarks>
    /// The string input is case-insensitive and mapped to the corresponding <see cref="M_Gamemode.GameType"/> enum value<br/>
    /// If an unknown string is provided, the existing <see cref="_gameType"/> value is retained (auto-chosen)
    /// </remarks>
    public GamemodeBuilder WithGameType(string gameType)
    {
        _gameType = gameType.ToLower() switch
        {
            "endless" => M_Gamemode.GameType.endlessPlaylist,
            "standard" => M_Gamemode.GameType.standard,
            "playlist" => M_Gamemode.GameType.playlist,
            "playlist-shuffle" => M_Gamemode.GameType.shuffledPlaylist,
            "single" => M_Gamemode.GameType.single,
            _ => _gameType
        };

        return this;
    }
    
    /// <summary>
    /// Constructs and returns a new <see cref="M_Gamemode"/> instance based on the properties configured in this builder
    /// </summary>
    /// <returns>A new <see cref="M_Gamemode"/> object populated with the specified settings</returns>
    public M_Gamemode Build()
    {
        var gm = ScriptableObject.CreateInstance<M_Gamemode>();

        // Core flags and settings
        gm.allowAchievements = false;
        gm.allowCheatedScores = false;
        gm.allowCheats = true;
        gm.allowLeaderboardScoring = true;
        gm.steamLeaderboardName = "";
        gm.allowHeightAchievements = false;
        gm.baseGamemode = true;
        gm.modeType = _isEndless
            ? M_Gamemode.GameType.endlessPlaylist
            : M_Gamemode.GameType.playlist;

        gm.capsuleName = _modeName;
        gm.gamemodeName = _modeName;
        gm.introText = _introText;
        gm.isEndless = _isEndless;
        gm.hasPerks = _hasPerks;
        gm.hasRevives = _hasRevives;
        gm.gamemodeScene = "Game-Main";
        gm.roachBankID = $"custom-{_modeName}";

        var numLevelsToLoad = 0;
        
        _regions.ForEach(reg => reg.subregionGroups.ForEach(subRegGroup =>
            subRegGroup.subregions.ForEach(subReg => numLevelsToLoad += subReg.levels.Count)));
        
        WKLog.Debug($"[Gamemode Builder] Will load {numLevelsToLoad} levels for {_modeName}");

        switch (numLevelsToLoad)
        {
            case 1:
                gm.modeType = M_Gamemode.GameType.single;
                gm.playlistLevels = [_regions[0].subregionGroups[0].subregions[0].levels[0]];
                WKLog.Debug($"[Gamemode Builder] Loading one singular level");
                break;
            case > 1 when _gameType == M_Gamemode.GameType.single:
                gm.modeType = M_Gamemode.GameType.playlist;
                _regions.ForEach(reg => reg.subregionGroups.ForEach(subRegGroup =>
                    subRegGroup.subregions.ForEach(subReg => gm.playlistLevels.AddRange(subReg.levels))));
                break;
            case > 1:
                gm.modeType = _gameType;
                List<M_Level> levels = [];
                var loadedLevels = 0;
                
                foreach (var level in from region in _regions
                         from subRegionGroup in region.subregionGroups
                         from subRegion in subRegionGroup.subregions
                         from level in subRegion.levels
                         select level)
                {
                    try
                    {
                        levels.Add(level);
                        loadedLevels++;
                    }
                    catch (Exception e)
                    {
                        WKLog.Error($"[Gamemode Builder] Failed to load: {e.Message}");
                    }
                }
                WKLog.Debug($"[Gamemode Builder] Loaded {loadedLevels}/{numLevelsToLoad} levels");
                
                //_regions.ForEach(reg => reg.subregionGroups.ForEach(subRegGroup =>
                //    subRegGroup.subregions.ForEach(subReg => gm.playlistLevels.AddRange(subReg.levels))));
                gm.playlistLevels = levels;
                break;
        }
        
        gm.levelsToGenerate = numLevelsToLoad;
        gm.name = _modeName;

        // If a sprites were provided, assign them:
        if (_capsuleSprite is not null)
            gm.capsuleArt = _capsuleSprite;

        if (_screenArtSprite is not null)
            gm.screenArt = _screenArtSprite;

        // Assign regions:
        gm.regions = _regions;

        // GameObjects: find “World_Root” in the currently loaded objects
        var worldRoot = Resources.FindObjectsOfTypeAll<GameObject>()
            .FirstOrDefault(go => go.name == "World_Root");
        gm.gamemodeObjects = worldRoot is not null
            ? [worldRoot]
            : [];

        // start items (hard‐coded hammer if no items specified)
        var hammerItem = new M_Gamemode.SpawnItem { itemid = "Item_Hammer" };
        gm.startItems = _spawnItems ?? [hammerItem];

        return gm;
    }
}