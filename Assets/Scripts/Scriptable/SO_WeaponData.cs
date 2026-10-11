using UnityEngine;

[System.Serializable]
public enum WeaponType
{
    Area,
    Projectile
}

[System.Serializable]
public struct WeaponStatsData
{
    public AdditiveStatData damage;
    public AdditiveStatData projectileCount;
}

[CreateAssetMenu(fileName = "WeaponData", menuName = "Weapons/WeaponData")]
public class SO_WeaponData : ScriptableObject
{
    public GameObject weaponPrefab;
    public string weaponName;
    public WeaponType weaponType;
    public WeaponStatsData stats;
    public GameObject hitVFX;
    [Tooltip("A weapontype of 'Area' will damage enemies directly while a weapontype of 'Projectile' will attempt to spawn projectiles")]
    public LayerMask hitMask;
    [Header("Only required for projectile weapons")]
    public GameObject projectilePrefab;
    public string WeaponName => weaponName;
    public WeaponStatsData Stats => stats;

    public Weapon Spawn(Transform weaponParent, Player wielder)
    {
        Weapon weapon = Instantiate(weaponPrefab).GetComponent<Weapon>();
        weapon.transform.SetParent(weaponParent);
        weapon.transform.SetLocalPositionAndRotation(Vector2.zero, Quaternion.identity);
        weapon.Initialize(wielder);

        return weapon;
    }
}
