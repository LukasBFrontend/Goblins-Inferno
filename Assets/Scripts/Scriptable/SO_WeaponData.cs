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
    [SerializeField] GameObject weaponModelPrefab;
    [SerializeField] string weaponName;
    [SerializeField] WeaponType weaponType;
    [SerializeField] WeaponStatsData stats;
    [Tooltip("A weapontype of 'Area' will damage enemies directly while a weapontype of 'Projectile' will attempt to spawn projectiles")]
    [SerializeField] LayerMask hitMask;
    [Header("Only required for projectile weapons")]
    [SerializeField] GameObject projectilePrefab;
    public string WeaponName => weaponName;
    public WeaponStatsData Stats => stats;
    List<Enemy> _targetsInRange;
    WeaponModel _weaponModel;


    /// <summary>
    /// Instatiates and initializes the weapon prefab.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="activeMonoBehavior"></param>
    public GameObject Spawn(Transform parent)
    {
        GameObject weaponObject = Instantiate(weaponModelPrefab);
        weaponObject.transform.SetParent(parent);
        weaponObject.transform.SetLocalPositionAndRotation(Vector2.zero, Quaternion.identity);

        _weaponModel = weaponObject.GetComponentsInChildren<WeaponModel>().First();
        _weaponModel.Initialize(this);

        return weaponObject;
    }


    /// <summary>
    /// Automatically attack all targets inside the weapon range if the weapon is not on cooldown. Meant to be used inside the Update method.
    /// </summary>
    public void Attack()
    {
        _weaponModel.StartCoroutine(AttackRoutine(Game.Player.Stats.AttackCooldown * .95f, (int)stats.projectileCount.baseValue));
    }

    IEnumerator AttackRoutine(float duration, int projectileCount)
    {
        float elapsed = 0f;
        float timestep = duration / projectileCount;
        float animationDuration = timestep / 3f;

        while (elapsed < duration)
        {
            _targetsInRange.RemoveAll(enemy => enemy == null);
            List<Enemy> currentTargets = new(_targetsInRange);

            if (weaponType == WeaponType.Area)
            {
                _weaponModel.StartAnimation(animationDuration);

                yield return new WaitForSeconds(animationDuration / 2f);

                Damage(currentTargets);

                yield return new WaitForSeconds(timestep - animationDuration / 2f);
            }
            else if (weaponType == WeaponType.Projectile)
            {
                Shoot(currentTargets);

                yield return new WaitForSeconds(timestep);
            }

            elapsed += timestep;
        }
    }

    public void EnterIntoRange(Enemy enemy)
    {
        _targetsInRange.Add(enemy);
    }

    public void ExitFromRange(Enemy enemy)
    {
        _targetsInRange.Remove(enemy);
    }

    void Shoot(List<Enemy> enemies)
    {
        var characterStats = Game.Player.Stats.Character;
        float damageMultiplier = characterStats.DamageMultiplier.Evaluate();

        foreach (Enemy enemy in enemies)
        {
            GameObject projectileObject = Instantiate(projectilePrefab);
            Projectile projectile = projectileObject.GetComponent<Projectile>();

            int damage = (int)(stats.damage.baseValue * damageMultiplier);

            projectile.SetTarget(enemy, damage);
        }
    }

    void Damage(List<Enemy> enemies)
    {
        var characterStats = Game.Player.Stats.Character;
        float damageMultiplier = characterStats.DamageMultiplier.Evaluate();

        foreach (Enemy enemy in enemies)
        {
            enemy.Health.TakeDamage((int)(stats.damage.baseValue * damageMultiplier));
        }
    }
}
