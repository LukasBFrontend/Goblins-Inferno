using System.Collections.Generic;
using System.Linq;

public struct WeaponStats : IStats
{
    public readonly string WeaponName => _weaponName;
    public readonly int WeaponLvl => _damageStat.Lvl + _projectileCountStat.Lvl;
    public readonly AdditiveStat Damage => _damageStat;
    public readonly AdditiveStat ProjectileCount => _projectileCountStat;
    private AdditiveStat _damageStat;
    private AdditiveStat _projectileCountStat;
    private Dictionary<string, Stat> _statLookup;
    private string _weaponName;

    public WeaponStats(string name, WeaponStatsData data)
    {
        _weaponName = name;
        _damageStat = new (data.damage);
        _projectileCountStat = new (data.projectileCount);

        _statLookup = new()
        {
            { data.damage.name, _damageStat },
            { data.projectileCount.name, _projectileCountStat }
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
