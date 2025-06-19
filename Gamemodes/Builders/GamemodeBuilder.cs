using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using WKLib.Utilities;

namespace WKLib.Gamemodes.Builders;

/// <summary>
/// Builds an M_Gamemode from regions, name, intro text, and flags.
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
    /// Set the list of regions for this gamemode.
    /// </summary>
    public GamemodeBuilder WithRegions(List<M_Region> regions)
    {
        _regions = regions ?? [];
        return this;
    }
    
    /// <summary>
    /// Set the displayed name of the gamemode (and capsule name).
    /// </summary>
    public GamemodeBuilder WithName(string modeName)
    {
        _modeName = modeName;
        return this;
    }
    
    /// <summary>
    /// Set the intro text for the gamemode.<br/>
    /// Shown when starting the gamemode
    /// </summary>
    public GamemodeBuilder WithIntroText(string introText)
    {
        _introText = introText;
        return this;
    }
    
    public GamemodeBuilder IsEndless(bool isEndless)
    {
        _isEndless = isEndless;
        return this;
    }
    
    public GamemodeBuilder HasPerks(bool hasPerks)
    {
        _hasPerks = hasPerks;
        return this;
    }
    
    public GamemodeBuilder HasRevives(bool hasRevives)
    {
        _hasRevives = hasRevives;
        return this;
    }
    
    /// <summary>
    /// Provide a Sprite to use for capsuleArt.
    /// </summary>
    public GamemodeBuilder WithCapsuleSprite(Sprite sprite)
    {
        _capsuleSprite = sprite;
        return this;
    }

    /// <summary>
    /// Provide a Sprite to use for screenArt.
    /// </summary>
    public GamemodeBuilder WithScreenArt(Sprite sprite)
    {
        _screenArtSprite = sprite;
        return this;
    }

    public GamemodeBuilder WithStartItems(List<M_Gamemode.SpawnItem> spawnItems)
    {
        _spawnItems = spawnItems;
        return this;
    }

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