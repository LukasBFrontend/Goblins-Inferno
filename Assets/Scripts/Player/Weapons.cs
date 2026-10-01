using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[DisallowMultipleComponent]
public class Weapons : MonoBehaviour
{
    [SerializeField] SO_WeaponsConfig weaponsConfig;
    [SerializeField] Transform weaponParent;
    public HashSet<SO_WeaponData> AvailableWeapons => _availableWeapons;
    HashSet<SO_WeaponData> _availableWeapons;
    HashSet<SO_WeaponData> _unlockedWeapons;
    Dictionary<string, GameObject> _weaponObjectLookup;

    void Awake()
    {
        _availableWeapons =  weaponsConfig.availableWeapons.ToHashSet();
        _unlockedWeapons = new (){ _availableWeapons.First() };
    }

    void Start()
    {
        foreach(SO_WeaponData weapon in _unlockedWeapons)
        {
            weapon.Spawn(weaponParent, this);
        }
    }

    public void Unlock(string weaponName)
    {
        SO_WeaponData weapon = _availableWeapons.First(weapon => weapon.WeaponName == weaponName);

        if (weapon == null)
        {
            throw new System.Exception($"Weapon '{weaponName}' is not in the list of available weapons");
        }
        _unlockedWeapons.Add(weapon);
    }

    public void Get(string weaponName)
    {
        throw new System.NotImplementedException();
    }
}
