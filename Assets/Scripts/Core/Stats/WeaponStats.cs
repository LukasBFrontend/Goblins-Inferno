using System.Collections.Generic;
using System.Linq;

public struct WeaponStats : IStats
{
    private AdditiveStat _baseSizeStat;
    private AdditiveStat _damageStat;
    private AdditiveStat _projectileCountStat;
    private MultiplicativeStat _sizeMultiplierStat;
    private Dictionary<string, Stat> _statLookup;
    private string _weaponName;

    public  readonly MultiplicativeStat SizeMultiplier => _sizeMultiplierStat;
    public readonly string WeaponName => _weaponName;
    public readonly int WeaponLvl => _damageStat.Lvl + _projectileCountStat.Lvl;

    public readonly AdditiveStat BaseSize => _baseSizeStat;
    public readonly AdditiveStat Damage => _damageStat;
    public readonly AdditiveStat ProjectileCount => _projectileCountStat;

    public WeaponStats(string name, WeaponStatsData data)
    {
        _weaponName = name;

        _baseSizeStat = new (data.baseSize);
        _damageStat = new (data.damage);
        _projectileCountStat = new (data.projectileCount);
        _sizeMultiplierStat = new(data.size);

        _statLookup = new()
        {
            { data.baseSize.name, _baseSizeStat },
            { data.damage.name, _damageStat },
            { data.projectileCount.name, _projectileCountStat },
            { data.size.name, _sizeMultiplierStat }
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
}
