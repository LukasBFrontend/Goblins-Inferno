using UnityEngine;

public abstract class Stat
{
    public int Lvl => _lvl;
    public Sprite Sprite => _sprite;
    public string Name => _name;
    public string ShortName => _shortName;
    public float PerLevelIncrease => _perLevelIncrease;
    public bool IncludeAsUpgradeOption => _includeAsUpgradeOption;
    protected Sprite _sprite;
    protected bool _includeAsUpgradeOption;
    protected string _name;
    protected string _shortName;
    protected int _lvl;
    protected float _perLevelIncrease;

    /// <summary>
    /// Advance the stat's lvl by one.
    /// </summary>
    /// <returns>The new lvl.</returns>
    public int LevelUp()
    {
        return _lvl += 1;
    }

    public abstract float Evaluate();

}

/// <summary>
/// A stat which is meant to scale via a flat per-level increase.
/// </summary>
public class AdditiveStat : Stat
{
    /// <summary>
    /// Stat increase, starting at 0 at lvl 1.
    /// </summary>
    public float BaseValue => _baseValue;
    private float _baseValue;

    public AdditiveStat(AdditiveStatData statData)
    {
        _lvl = 0;
        _sprite = statData.UISprite;
        _name = statData.name;
        _shortName = statData.shortName;
        _baseValue = statData.baseValue;
        _perLevelIncrease = statData.flatScaling;
        _includeAsUpgradeOption = statData.includeAsUpgradeOption;
    }

    public override float Evaluate()
    {
        return  _baseValue + (_lvl + 1) * _perLevelIncrease;
    }
}

/// <summary>
/// A stat which is meant to scale via a multiplier.
/// </summary>
public sealed class MultiplicativeStat : Stat
{
    public MultiplicativeStat(MultiplicativeStatData statData)
    {
        _lvl = 0;
        _sprite = statData.UISprite;
        _name = statData.name;
        _shortName = statData.shortName;
        _perLevelIncrease = statData.multiplierScaling;
        _includeAsUpgradeOption = statData.includeAsUpgradeOption;
    }

    /// <summary>
    /// Stat multiplier, starting at 1f at lvl 1.
    /// </summary>
    public override float Evaluate()
    {
        return 1f + (Lvl * _perLevelIncrease / 100f);
    }
}
