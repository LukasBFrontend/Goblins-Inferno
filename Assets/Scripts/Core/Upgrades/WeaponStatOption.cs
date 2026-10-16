public class WeaponStatOption : UpgradeOption
{
    string _weaponName;
    Stat _stat;
    
    public string WeaponName => _weaponName;
    public Stat Stat => _stat;

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
        stat.LevelUp();
    }
}
