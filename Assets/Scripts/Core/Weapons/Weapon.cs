using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon :  MonoBehaviour
{
    [SerializeField] SO_WeaponData data;
    [SerializeField] WeaponTargetTracker targetTracker;
    [Header("Optional")]
    [SerializeField] ParticleSystemControls particleSystem;
    Player _wielder;

    public void Initialize(Player wielder)
    {
        _wielder = wielder;
    }

    /// <summary>
    /// Automatically attack all targets inside the weapon range if the weapon is not on cooldown. Meant to be used inside the Update method.
    /// </summary>
    public void Attack()
    {
        StartCoroutine(AttackRoutine(
            Game.Player.Stats.AttackCooldown * .95f,
            (int)_wielder.Stats.GetWeaponModifier(data.weaponName).ProjectileCount.Evaluate()
        ));
    }

    IEnumerator AttackRoutine(float duration, int projectileCount)
    {
        float timeBetweenAttacks = duration / projectileCount;
        float attackAnimationDuration = timeBetweenAttacks / 3f;

        for (int attack = 0; attack < projectileCount; attack++)
        {
            HashSet<BaseCharacter> damagedThisAttack = new();

            if (data.weaponType == WeaponType.Area)
            {
                PlayWeaponVFX(attackAnimationDuration);
                float animationElapsed = 0f;

                while (animationElapsed < attackAnimationDuration)
                {
                    targetTracker.EnemiesInRange.RemoveAll(enemy => enemy == null);

                    foreach (Enemy enemy in targetTracker.EnemiesInRange)
                    {
                        if (damagedThisAttack.Add(enemy))
                        {
                            RegisterHit(enemy);
                        }
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
            else if(data.weaponType == WeaponType.Projectile)
            {
                targetTracker.EnemiesInRange.RemoveAll(enemy => enemy == null);

                Shoot();

                yield return new WaitForSeconds(timeBetweenAttacks);
            }
        }
    }

    void Shoot()
    {
        var characterStats = Game.Player.Stats.Character;
        float damageMultiplier = characterStats.DamageMultiplier.Evaluate();

        GameObject projectileObject = Instantiate(data.projectilePrefab, _wielder.transform.position, Quaternion.identity);
        Projectile projectile = projectileObject.GetComponent<Projectile>();

        var enemiesInRange = targetTracker.EnemiesInRange;
        /*
        Collider[] hits = Physics.OverlapSphere(_wielder.transform.position, projectile.Data.range);

        foreach(var hit in hits)
        {
            if (!hit.TryGetComponent<BaseCharacter>(out var enemy))
            {
                continue;
            }

            enemiesInRange.Add(enemy);
        }
        */
        BaseCharacter random = enemiesInRange.Count >= 1
            ? enemiesInRange[Random.Range(0, enemiesInRange.Count)]
            : null
        ;

        int damage = (int)(data.stats.damage.baseValue * damageMultiplier);

        projectile.Initialize(_wielder, random, damage);
    }

    void RegisterHit(Enemy enemy)
    {
        var characterStats = Game.Player.Stats.Character;
        float damageMultiplier = characterStats.DamageMultiplier.Evaluate();

        if (data.hitVFX)
        {
            Instantiate(data.hitVFX, enemy.transform.position + new Vector3(0, 1, -1f), Quaternion.identity);
        }

        enemy.Health.TakeDamage((int)(data.stats.damage.baseValue * damageMultiplier));
    }

    void PlayWeaponVFX(float animationDuration)
    {
        if (particleSystem.EffectiveDuration != animationDuration)
        {
            particleSystem.SetEffectiveDuration(animationDuration);
        }

        particleSystem.Play();
    }
}
