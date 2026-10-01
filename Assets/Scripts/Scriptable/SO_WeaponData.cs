using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    [SerializeField] GameObject weaponPrefab;
    [SerializeField] string weaponName;
    [SerializeField] WeaponType weaponType;
    [SerializeField] WeaponStatsData stats;
    [Tooltip("A weapontype of 'Area' will damage enemies directly while a weapontype of 'Projectile' will attempt to spawn projectiles")]
    [SerializeField] LayerMask hitMask;
    [Header("Only required for projectile weapons")]
    [SerializeField] GameObject projectilePrefab;
    public string WeaponName => weaponName;
    public WeaponStatsData Stats => stats;
    MonoBehaviour _activeMonoBehavior;
    List<Enemy> _targetsInRange;
    float _lastAttackTime;

    /// <summary>
    /// Instatiates the corresponding prefab and initializes the corresponding WeaponTargetManager component with the WeaponData.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="activeMonoBehavior"></param>
    public void Spawn(Transform parent, MonoBehaviour activeMonoBehavior)
    {
        this._activeMonoBehavior = activeMonoBehavior;
        _lastAttackTime = 0;

        Initialize(parent);
    }

    void Initialize(Transform parent)
    {
        GameObject _weaponModel = Instantiate(weaponPrefab);

        _weaponModel.transform.SetParent(parent);
        _weaponModel.transform.localPosition = Vector2.zero;
        _weaponModel.transform.localRotation = Quaternion.identity;

        WeaponTargetTracker targetTracker = _weaponModel.GetComponentsInChildren<WeaponTargetTracker>().First();
        targetTracker.Initialize(this);
    }

    /// <summary>
    /// Automatically attack all targets inside the weapon range if the weapon is not on cooldown. Meant to be used inside the Update method.
    /// </summary>
    public void Attack(float playerAttackCooldown, Transform transformTarget, float attackAngle)
    {
        if (Time.time <= playerAttackCooldown + _lastAttackTime)
        {
            return;
        }
        transformTarget.localRotation = Quaternion.Euler(new (0, attackAngle , 0));
        _lastAttackTime = Time.time;
        _activeMonoBehavior.StartCoroutine(AttackRoutine(playerAttackCooldown, stats.projectileCount.baseValue));
    }

    public void EnterIntoRange(Enemy enemy)
    {
        _targetsInRange.Add(enemy);
    }

    public void ExitFromRange(Enemy enemy)
    {
        _targetsInRange.Remove(enemy);
    }

    IEnumerator AttackRoutine(float duration, int projectileCount)
    {
        float elapsed = 0f;
        float timestep = duration / projectileCount;

        while (elapsed < duration)
        {
            _targetsInRange.RemoveAll(enemy => enemy == null);
            List<Enemy> currentTargets = new (_targetsInRange);

            if (weaponType == WeaponType.Area)
            {
                Damage(currentTargets);
            }
            else if (weaponType == WeaponType.Projectile)
            {
                Shoot(currentTargets);
            }

            yield return new WaitForSeconds(timestep);
            elapsed += timestep;
        }
    }

    void Shoot(List<Enemy> enemies)
    {
        foreach (Enemy enemy in enemies)
        {
            GameObject projectileObject = Instantiate(projectilePrefab);
            Projectile projectile = projectileObject.GetComponent<Projectile>();

            projectile.Initialize(enemy);
        }
    }

    void Damage(List<Enemy> enemies)
    {
        foreach (Enemy enemy in enemies)
        {
            enemy.Health.TakeDamage(stats.damage.baseValue);
        }
    }
}
