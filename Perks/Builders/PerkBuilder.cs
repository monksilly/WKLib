using System;
using System.Collections.Generic;
using UnityEngine;
using WKLib.Utilities;

namespace WKLib.Perks.Builders;

/// <summary>
/// A fluent builder class for creating and configuring a <see cref="Perk"/> instance.<br/>
/// It allows setting various properties of a perk such as title, description, buffs,
/// modules, and other attributes in a chainable manner before finally building the <see cref="Perk"/> object.
/// </summary>
public class PerkBuilder
{
    private string _title = "New Perk";
    private string _id = "new_perk";
    private string _description = "A new perk";
    private string _flavorText = "";
    private Perk.PerkType _perkType = Perk.PerkType.standard;
    private bool _competitive = true;
    private int _cost = 9;
    private Perk.PerkPool _spawnPool = Perk.PerkPool.standard;
    private bool _spawnInEndless = true;
    private bool _canStack = false;
    private int _stackMax = 1;
    private AnimationCurve _multiplierCurve = AnimationCurve.Linear(0, 1, 1, 1);
    private bool _useBuff = false;
    private BuffContainer _buff;
    private bool _useBaseBuff = false;
    private BuffContainer _baseBuff;
    private float _buffMultiplier = 1f;
    private List<string> _flags = new();
    private List<PerkModule> _modules = new();
    private Sprite _icon;
    private Sprite _perkCard;
    private Sprite _perkFrame;
    private string _unlockProgressionID = "";
    private int _unlockXP = 0;

    /// <summary>
    /// Sets the title of the perk
    /// </summary>
    /// <param name="title">The display title for the perk</param>
    /// <returns>The current <see cref="PerkBuilder"/> instance for fluent chaining</returns>
    public PerkBuilder WithTitle(string title)
    {
        _title = title;
        return this;
    }

    /// <summary>
    /// Sets the unique identifier of the perk
    /// </summary>
    /// <param name="id">The unique ID for the perk</param>
    /// <returns>The current <see cref="PerkBuilder"/> instance for fluent chaining</returns>
    public PerkBuilder WithID(string id)
    {
        _id = id;
        return this;
    }

    /// <summary>
    /// Sets the description of the perk
    /// </summary>
    /// <param name="description">The description text for the perk</param>
    /// <returns>The current <see cref="PerkBuilder"/> instance for fluent chaining</returns>
    public PerkBuilder WithDescription(string description)
    {
        _description = description;
        return this;
    }

    /// <summary>
    /// Sets the flavor text of the perk
    /// </summary>
    /// <param name="flavorText">The flavor text for the perk</param>
    /// <returns>The current <see cref="PerkBuilder"/> instance for fluent chaining</returns>
    public PerkBuilder WithFlavorText(string flavorText)
    {
        _flavorText = flavorText;
        return this;
    }

    /// <summary>
    /// Sets the perk type
    /// </summary>
    /// <param name="perkType">The type of perk (standard, orange, red, unstable, peripheral, delta, rho)</param>
    /// <returns>The current <see cref="PerkBuilder"/> instance for fluent chaining</returns>
    public PerkBuilder WithPerkType(Perk.PerkType perkType)
    {
        _perkType = perkType;
        return this;
    }

    /// <summary>
    /// Sets whether the perk is available in competitive modes
    /// </summary>
    /// <param name="competitive">True if the perk is available in competitive mode(No stored roaches)</param>
    /// <returns>The current <see cref="PerkBuilder"/> instance for fluent chaining</returns>
    public PerkBuilder IsCompetitive(bool competitive)
    {
        _competitive = competitive;
        return this;
    }

    /// <summary>
    /// Sets the cost of the perk
    /// </summary>
    /// <param name="cost">The cost in roaches</param>
    /// <returns>The current <see cref="PerkBuilder"/> instance for fluent chaining</returns>
    public PerkBuilder WithCost(int cost)
    {
        _cost = cost;
        return this;
    }

    /// <summary>
    /// Sets the spawn pool for the perk
    /// </summary>
    /// <param name="spawnPool">The spawn pool (standard, unstable, never)</param>
    /// <returns>The current <see cref="PerkBuilder"/> instance for fluent chaining</returns>
    public PerkBuilder WithSpawnPool(Perk.PerkPool spawnPool)
    {
        _spawnPool = spawnPool;
        return this;
    }

    /// <summary>
    /// Sets whether the perk spawns in endless mode
    /// </summary>
    /// <param name="spawnInEndless">True if the perk spawns in endless mode</param>
    /// <returns>The current <see cref="PerkBuilder"/> instance for fluent chaining</returns>
    public PerkBuilder SpawnInEndless(bool spawnInEndless)
    {
        _spawnInEndless = spawnInEndless;
        return this;
    }

    /// <summary>
    /// Sets whether the perk can stack
    /// </summary>
    /// <param name="canStack">True if the perk can stack</param>
    /// <returns>The current <see cref="PerkBuilder"/> instance for fluent chaining</returns>
    public PerkBuilder CanStack(bool canStack)
    {
        _canStack = canStack;
        return this;
    }

    /// <summary>
    /// Sets the maximum stack count for the perk
    /// </summary>
    /// <param name="stackMax">The maximum number of stacks</param>
    /// <returns>The current <see cref="PerkBuilder"/> instance for fluent chaining</returns>
    public PerkBuilder WithStackMax(int stackMax)
    {
        _stackMax = stackMax;
        return this;
    }

    /// <summary>
    /// Sets the multiplier curve for stacking
    /// </summary>
    /// <param name="curve">The animation curve for stack multipliers</param>
    /// <returns>The current <see cref="PerkBuilder"/> instance for fluent chaining</returns>
    public PerkBuilder WithMultiplierCurve(AnimationCurve curve)
    {
        _multiplierCurve = curve;
        return this;
    }

    /// <summary>
    /// Adds a buff to the perk
    /// </summary>
    /// <param name="buff">The buff container to add</param>
    /// <returns>The current <see cref="PerkBuilder"/> instance for fluent chaining</returns>
    public PerkBuilder WithBuff(BuffContainer buff)
    {
        _useBuff = true;
        _buff = buff;
        return this;
    }

    /// <summary>
    /// Adds a base buff to the perk
    /// </summary>
    /// <param name="baseBuff">The base buff container to add</param>
    /// <returns>The current <see cref="PerkBuilder"/> instance for fluent chaining</returns>
    public PerkBuilder WithBaseBuff(BuffContainer baseBuff)
    {
        _useBaseBuff = true;
        _baseBuff = baseBuff;
        return this;
    }

    /// <summary>
    /// Sets the buff multiplier
    /// </summary>
    /// <param name="multiplier">The buff multiplier value</param>
    /// <returns>The current <see cref="PerkBuilder"/> instance for fluent chaining</returns>
    public PerkBuilder WithBuffMultiplier(float multiplier)
    {
        _buffMultiplier = multiplier;
        return this;
    }

    /// <summary>
    /// Adds game flags that are set when the perk is active
    /// </summary>
    /// <param name="flags">List of flag names to set</param>
    /// <returns>The current <see cref="PerkBuilder"/> instance for fluent chaining</returns>
    public PerkBuilder WithFlags(List<string> flags)
    {
        _flags = flags ?? new();
        return this;
    }

    /// <summary>
    /// Adds perk modules to the perk
    /// </summary>
    /// <param name="modules">List of perk modules</param>
    /// <returns>The current <see cref="PerkBuilder"/> instance for fluent chaining</returns>
    public PerkBuilder WithModules(List<PerkModule> modules)
    {
        _modules = modules ?? new();
        return this;
    }

    /// <summary>
    /// Sets the icon sprite for the perk
    /// </summary>
    /// <param name="icon">The sprite to use as the icon for the perk</param>
    /// <returns>The current <see cref="PerkBuilder"/> instance for fluent chaining</returns>
    public PerkBuilder WithIcon(Sprite icon)
    {
        _icon = icon;
        return this;
    }

    /// <summary>
    /// Sets the perk card sprite
    /// </summary>
    /// <param name="perkCard">The sprite to use as the perk card</param>
    /// <returns>The current <see cref="PerkBuilder"/> instance for fluent chaining</returns>
    public PerkBuilder WithPerkCard(Sprite perkCard)
    {
        _perkCard = perkCard;
        return this;
    }

    /// <summary>
    /// Sets the perk frame sprite
    /// </summary>
    /// <param name="perkFrame">The sprite to use as the perk frame</param>
    /// <returns>The current <see cref="PerkBuilder"/> instance for fluent chaining</returns>
    public PerkBuilder WithPerkFrame(Sprite perkFrame)
    {
        _perkFrame = perkFrame;
        return this;
    }

    /// <summary>
    /// Sets the unlock progression ID requirement
    /// </summary>
    /// <param name="unlockProgressionID">The progression ID required to unlock</param>
    /// <returns>The current <see cref="PerkBuilder"/> instance for fluent chaining</returns>
    public PerkBuilder WithUnlockProgressionID(string unlockProgressionID)
    {
        _unlockProgressionID = unlockProgressionID;
        return this;
    }

    /// <summary>
    /// Sets the XP requirement to unlock the perk
    /// </summary>
    /// <param name="unlockXP">The XP amount required</param>
    /// <returns>The current <see cref="PerkBuilder"/> instance for fluent chaining</returns>
    public PerkBuilder WithUnlockXP(int unlockXP)
    {
        _unlockXP = unlockXP;
        return this;
    }

    /// <summary>
    /// Constructs and returns a new <see cref="Perk"/> instance based on the properties configured in this builder
    /// </summary>
    /// <returns>A new <see cref="Perk"/> object populated with the specified settings</returns>
    public Perk Build()
    {
        var perk = ScriptableObject.CreateInstance<Perk>();

        perk.title = _title;
        perk.id = _id;
        perk.description = _description;
        perk.flavorText = _flavorText;
        perk.perkType = _perkType;
        perk.competitive = _competitive;
        perk.cost = _cost;
        perk.spawnPool = _spawnPool;
        perk.spawnInEndless = _spawnInEndless;
        perk.canStack = _canStack;
        perk.stackMax = _stackMax;
        perk.multiplierCurve = _multiplierCurve;
        perk.useBuff = _useBuff;
        perk.buff = _buff;
        perk.useBaseBuff = _useBaseBuff;
        perk.baseBuff = _baseBuff;
        perk.buffMultiplier = _buffMultiplier;
        perk.flags = _flags;
        perk.modules = _modules;
        perk.icon = _icon;
        perk.perkCard = _perkCard;
        perk.perkFrame = _perkFrame;
        perk.unlockProgressionID = _unlockProgressionID;
        perk.unlockXP = _unlockXP;
        perk.name = _id;

        WKLog.Debug($"[Perk Builder] Built perk: {_title} (ID: {_id})");

        return perk;
    }
}

