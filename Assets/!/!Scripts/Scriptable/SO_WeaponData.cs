using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Weapons/WeaponData")]
public class SO_WeaponData : ScriptableObject
{
    public GameObject weaponPrefab;
    public string weaponName;
    public WeaponType weaponType;
    public Sprite sprite;
    public WeaponStatsData stats;
    public GameObject hitVFX;
    [Tooltip("A weapontype of 'Area' will damage enemies directly while a weapontype of 'Projectile' will attempt to spawn projectiles")]
    public LayerMask hitMask;
    [Header("Only required for projectile weapons")]
    public PoolableData projectile;
    public bool parentProjectiles;

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

[System.Serializable]
public enum WeaponType
{
    Area,
    Projectile
}

[System.Serializable]
public struct WeaponStatsData
{
    [Tooltip("Equivalent to the base range of the weapon. Can be the same as or different to the projectile range.")]
    public AdditiveStatData baseSize;
    public AdditiveStatData damage;
    public AdditiveStatData projectileCount;
    public AdditiveStatData critChance;
    public MultiplicativeStatData size;
}
