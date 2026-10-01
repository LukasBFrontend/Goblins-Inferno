using System.Collections.Generic;
using System.Linq;

public struct CharacterStats: IStats
{
    public readonly AdditiveStat MaxHealth => _maxHealthStat;
    public readonly MultiplicativeStat DamageMultiplier => _damageMultiplierStat;
    public readonly MultiplicativeStat HealthMultiplier => _healthMultiplierStat;
    public readonly MultiplicativeStat MoveSpeedMultiplier => _movementMultiplierStat;
    public readonly MultiplicativeStat AttackSpeedMultiplier => _attackSpeedMultiplierStat;
    private AdditiveStat _maxHealthStat;
    private MultiplicativeStat _damageMultiplierStat;
    private MultiplicativeStat _healthMultiplierStat;
    private MultiplicativeStat _movementMultiplierStat;
    private MultiplicativeStat _attackSpeedMultiplierStat;
    private Dictionary<string, Stat> _statLookup;

    public CharacterStats(SO_CharacterStatsConfig data)
    {
        _maxHealthStat = new(data.maxHealth);
        _damageMultiplierStat = new (data.damageMultiplier);
        _healthMultiplierStat = new (data.healthMultiplier);
        _movementMultiplierStat = new (data.movementMultiplier);
        _attackSpeedMultiplierStat = new (data.attackSpeedMultiplier);

        _statLookup = new()
        {
            { data.damageMultiplier.name, _damageMultiplierStat },
            { data.healthMultiplier.name, _healthMultiplierStat },
            { data.movementMultiplier.name, _movementMultiplierStat },
            { data.attackSpeedMultiplier.name, _attackSpeedMultiplierStat}
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
