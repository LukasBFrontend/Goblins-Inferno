using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[DisallowMultipleComponent]

[RequireComponent(typeof(Player))]
public class Weapons : MonoBehaviour
{
    [SerializeField] SO_WeaponsConfig weaponsConfig;
    [SerializeField] Transform weaponParent;
    public List<SO_WeaponData> Available => weaponsConfig.availableWeapons;
    public HashSet<Weapon> Unlocked => _unlockedWeapons;
    HashSet<Weapon> _unlockedWeapons;
    float _lastAttackTime;
    Player _wielder;

    void Awake()
    {
        _wielder = GetComponent<Player>();

        _unlockedWeapons = new (){
            Available.First().Spawn(weaponParent, _wielder)
        };

        _lastAttackTime = 0;
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
        SO_WeaponData weaponData = Available.First(weapon => weapon.WeaponName == weaponName);

        if (weaponData == null)
        {
            throw new System.Exception($"Weapon '{weaponName}' is not in the list of available weapons");
        }

        _unlockedWeapons.Add(
            weaponData.Spawn(weaponParent, _wielder)
        );
    }
}
