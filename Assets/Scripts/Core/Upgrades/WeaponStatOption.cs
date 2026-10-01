using UnityEngine;

public class WeaponStatOption : UpgradeOption
{
    public string WeaponName => _weaponName;
    public Stat Stat => _stat;
    string _weaponName;
    Stat _stat;

    public WeaponStatOption(string weaponName, Stat stat)
    {
        _weaponName = weaponName;
        _stat = stat;
    }

    public override void AssignTo(SO_UpgradeOptionData data)
    {
        data.SetValues(this);
    }

    public override void Select()
    {
        Player player = Player.Instance;
        WeaponStats weaponStats = player.Stats.GetWeaponModifier(_weaponName);
        Stat stat = weaponStats.GetStat(_stat.Name);
        Debug.Log($"Stat '{stat.Name}' leveled up to {stat.LevelUp()}. New value: {stat.Evaluate()}");
    }
}
