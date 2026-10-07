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
    List<Enemy> _enemiesInRange;
    WeaponModel _weaponModel;
    Player _wielder;


    /// <summary>
    /// Instatiates and initializes the weapon prefab.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="activeMonoBehavior"></param>
    public GameObject Spawn(Player wielder, Transform parent)
    {
        GameObject weaponObject = Instantiate(weaponModelPrefab);
        weaponObject.transform.SetParent(parent);
        weaponObject.transform.SetLocalPositionAndRotation(Vector2.zero, Quaternion.identity);

        _wielder = wielder;
        _weaponModel = weaponObject.GetComponentsInChildren<WeaponModel>().First();
        _weaponModel.Initialize(this);

        return weaponObject;
    }


    /// <summary>
    /// Automatically attack all targets inside the weapon range if the weapon is not on cooldown. Meant to be used inside the Update method.
    /// </summary>
    public void Attack()
    {
        _weaponModel.StartCoroutine(AttackRoutine(Game.Player.Stats.AttackCooldown * .95f, (int)_wielder.Stats.GetWeaponModifier(weaponName).ProjectileCount.Evaluate()));
    }

    IEnumerator AttackRoutine(float duration, int projectileCount)
    {
        float timeBetweenAttacks = duration / projectileCount;
        float attackAnimationDuration = timeBetweenAttacks / 3f;

        for (int attack = 0; attack < projectileCount; attack++)
        {
            HashSet<Enemy> damagedThisAttack = new();

            if (weaponType == WeaponType.Area)
            {
                _weaponModel.StartAnimation(attackAnimationDuration);

                float animationElapsed = 0f;

                while (animationElapsed < attackAnimationDuration)
                {
                    _enemiesInRange.RemoveAll(enemy => enemy == null);

                    foreach (Enemy enemy in _enemiesInRange)
                    {
                        if (damagedThisAttack.Add(enemy))
                            Damage(new List<Enemy> { enemy });
                    }

                    yield return null;
                    animationElapsed += Time.deltaTime;
                }

                float remainingTime = timeBetweenAttacks - attackAnimationDuration;

                if (remainingTime > 0f)
                {
                    yield return new WaitForSeconds(remainingTime);
                }

                continue;
            }
            else if(weaponType == WeaponType.Projectile)
            {
                _enemiesInRange.RemoveAll(enemy => enemy == null);

                Shoot();

                yield return new WaitForSeconds(timeBetweenAttacks);
            }

        }
    }

    public void EnterIntoMeleeRange(Enemy enemy)
    {
        _enemiesInRange.Add(enemy);
    }

    public void ExitFromMeleeRange(Enemy enemy)
    {
        _enemiesInRange.Remove(enemy);
    }

    void Shoot()
    {
        var characterStats = Game.Player.Stats.Character;
        float damageMultiplier = characterStats.DamageMultiplier.Evaluate();


        GameObject projectileObject = Instantiate(projectilePrefab);
        Projectile projectile = projectileObject.GetComponent<Projectile>();

        Collider[] hits = Physics.OverlapSphere(_wielder.transform.position, projectile.Data.range);
        List<Enemy> enemiesInRange = new();

        foreach(var hit in hits)
        {
            if (!hit.TryGetComponent<Enemy>(out var enemy))
            {
                continue;
            }

            enemiesInRange.Add(enemy);
        }

        Enemy random = enemiesInRange.Count >= 1
            ? enemiesInRange[Random.Range(0, enemiesInRange.Count)]
            : null
        ;

        int damage = (int)(stats.damage.baseValue * damageMultiplier);

        projectile.Initialize(_wielder, random, damage);
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
