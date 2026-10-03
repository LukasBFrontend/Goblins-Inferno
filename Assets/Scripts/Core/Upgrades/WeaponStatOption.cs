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
        Player player = Game.Player;
        WeaponStats weaponStats = player.Stats.GetWeaponModifier(_weaponName);
        Stat stat = weaponStats.GetStat(_stat.Name);
        Debug.Log($"Stat <color=magenta>{stat.Name}</color> leveled up to <color=white>{stat.LevelUp()}</color>. New evaluated value: <color=white>{stat.Evaluate()}</color>");
    }
}
