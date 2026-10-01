public class WeaponUnlockOption : UpgradeOption
{
    public string WeaponName => _weaponName;
    string _weaponName;

    public WeaponUnlockOption(string weaponName)
    {
        _weaponName = weaponName;
    }

    public override void AssignTo(SO_UpgradeOptionData data)
    {
        data.SetValues(this);
    }

    public override void Select()
    {
        Player.Instance.Weapons.Unlock(WeaponName);
    }
}
