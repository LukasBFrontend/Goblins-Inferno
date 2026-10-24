using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Player))]
public class Weapons : MonoBehaviour
{
    [SerializeField] SO_WeaponsConfig weaponsConfig;
    [SerializeField] Transform weaponParent;

    HashSet<Weapon> _unlockedWeapons;
    HashSet<Weapon> _availableWeapons;
    float _lastAttackTime;
    Player _wielder;

    public HashSet<Weapon> Available => _availableWeapons;
    public HashSet<Weapon> Unlocked => _unlockedWeapons;

    void Awake()
    {
        _wielder = GetComponent<Player>();


        _availableWeapons = new();

        foreach(SO_WeaponData weaponData in weaponsConfig.availableWeapons)
        {
            _availableWeapons.Add(
                weaponData.Spawn(weaponParent, _wielder)
            );
        }

        _unlockedWeapons = new();
        _lastAttackTime = 0;
    }

    void Start()
    {
        Unlock(_availableWeapons.First().Name);
    }

    public void Attack()
    {
        if (Time.time <= Game.Player.Stats.AttackCooldown + _lastAttackTime)
        {
            return;
        }

        Enemy closestEnemy = Utils.ClosestEnemy(Game.Player);

        Vector3 attackDir = closestEnemy != null
            ? Game.Player.DirectionTo(closestEnemy)
            : Utils.RandomDirection()
        ;

        float rotation = Mathf.Atan2(attackDir.x, attackDir.z);
        weaponParent.rotation = Quaternion.Euler(new (0, Mathf.Rad2Deg * rotation + 90 , 0));

        foreach(Weapon weapon in _unlockedWeapons)
        {
            weapon.Attack();
        }
        _lastAttackTime = Time.time;
    }

    public void Unlock(string weaponName)
    {
        Weapon weapon = Available.First(weapon => weapon.Name == weaponName);

        if (weapon == null)
        {
            throw new System.Exception($"Weapon '{weaponName}' is not in the list of available weapons");
        }

        _wielder.Stats.GetWeaponModifier(weaponName).MarkUnlocked();
        _unlockedWeapons.Add(weapon);
    }
}
