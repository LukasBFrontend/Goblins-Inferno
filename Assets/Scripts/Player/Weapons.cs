using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[DisallowMultipleComponent]

[RequireComponent(typeof(Player))]
public class Weapons : MonoBehaviour
{
    [SerializeField] SO_WeaponsConfig weaponsConfig;
    [SerializeField] Transform weaponParent;
    public HashSet<SO_WeaponData> Available => _availableWeapons;
    public HashSet<SO_WeaponData> Unlocked => _unlockedWeapons;
    HashSet<SO_WeaponData> _availableWeapons;
    HashSet<SO_WeaponData> _unlockedWeapons;
    Dictionary<string, GameObject> _weaponObjectLookup = new();
    float _lastAttackTime;
    Player _wielder;

    void Awake()
    {
        _wielder = GetComponent<Player>();
        _availableWeapons =  weaponsConfig.availableWeapons.ToHashSet();
        _unlockedWeapons = new (){ _availableWeapons.First() };
        _lastAttackTime = 0;
    }

    void Start()
    {
        foreach(SO_WeaponData weapon in _unlockedWeapons)
        {
           _weaponObjectLookup.Add(weapon.name, weapon.Spawn(_wielder, weaponParent));
        }
    }

    public void Attack()
    {
        if (Time.time <= Game.Player.Stats.AttackCooldown + _lastAttackTime)
        {
            return;
        }

        var closestEnemy = Utils.ClosestEnemy(Game.Player);

        Vector3 attackDir = closestEnemy != null
            ? Game.Player.DirectionTo(closestEnemy)
            : Utils.RandomDirection()
        ;

        float rotation = Mathf.Atan2(attackDir.x, attackDir.z);
        weaponParent.rotation = Quaternion.Euler(new (0, Mathf.Rad2Deg * rotation + 90 , 0));

        foreach(var weapon in _unlockedWeapons)
        {
            weapon.Attack();
        }
        _lastAttackTime = Time.time;
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
