using UnityEngine;

public class WeaponUnlockOption : UpgradeOption
{
    string _weaponName;
    Sprite _sprite;
    
    public string WeaponName => _weaponName;
    public Sprite Sprite => _sprite;

    public WeaponUnlockOption(string weaponName, Sprite sprite)
    {
        _weaponName = weaponName;
        _sprite = sprite;
    }

    public override void AssignTo(SO_UpgradeOptionData data)
    {
        data.SetValues(this);
    }

    public override void Select()
    {
        Game.Player.Weapons.Unlock(WeaponName);
    }
}
