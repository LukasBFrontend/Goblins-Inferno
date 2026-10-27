using System.Collections.Generic;
using System.Linq;

public struct WeaponStats : IStats
{
    private AdditiveStat _baseSizeStat;
    private AdditiveStat _damageStat;
    private AdditiveStat _projectileCountStat;
    private AdditiveStat _critChanceStat;
    private MultiplicativeStat _sizeMultiplierStat;
    private Dictionary<string, Stat> _statLookup;
    private string _weaponName;
    private bool _unlocked;

    public  readonly MultiplicativeStat SizeMultiplier => _sizeMultiplierStat;
    public readonly string WeaponName => _weaponName;
    public readonly int Lvl => _baseSizeStat.Lvl + _damageStat.Lvl + _projectileCountStat.Lvl + (_unlocked ? 0 : 1);

    public readonly AdditiveStat BaseSize => _baseSizeStat;
    public readonly AdditiveStat Damage => _damageStat;
    public readonly AdditiveStat ProjectileCount => _projectileCountStat;
    public readonly AdditiveStat CritChance => _critChanceStat;

    public WeaponStats(string name, WeaponStatsData data)
    {
        _weaponName = name;
        _unlocked = false;

        _baseSizeStat = new (data.baseSize);
        _damageStat = new (data.damage);
        _projectileCountStat = new (data.projectileCount);
        _sizeMultiplierStat = new(data.size);
        _critChanceStat = new(data.critChance);

        _statLookup = new()
        {
            { data.baseSize.name, _baseSizeStat },
            { data.damage.name, _damageStat },
            { data.projectileCount.name, _projectileCountStat },
            { data.critChance.name, _critChanceStat},
            { data.size.name, _sizeMultiplierStat },
        };
    }

    public readonly Stat GetStat(string statName)
    {
        return _statLookup[statName];
    }

    public readonly Stat[] All()
    {
        return _statLookup.Values.ToArray();
    }

    public void MarkUnlocked()
    {
        _unlocked = true;
    }
}
