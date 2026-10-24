public class WeaponStatOption : UpgradeOption
{
    string _weaponName;
    int _weaponLevel;
    Stat _stat;

    public string WeaponName => _weaponName;
    public int WeaponLevel => _weaponLevel;
    public Stat Stat => _stat;

    public WeaponStatOption(string weaponName, int weaponLevel, Stat stat)
    {
        _weaponName = weaponName;
        _weaponLevel = weaponLevel;
        _stat = stat;
    }

    public override void AssignTo(RSO_UpgradeCardData data)
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
