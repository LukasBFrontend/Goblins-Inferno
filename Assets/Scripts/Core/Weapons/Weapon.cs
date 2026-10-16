using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon :  MonoBehaviour
{
    [SerializeField] SO_WeaponData data;
    [SerializeField] WeaponTargetTracker targetTracker;
    [Header("Optional")]
    [SerializeField] ParticleSystemControls particleSystem;
    [SerializeField] float projectileOriginY;

    public string Name => data.weaponName;
    public SO_WeaponData Data => data;

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
        WeaponStats currentStats = _wielder.Stats.GetWeaponModifier(data.weaponName);

        float weaponSize = currentStats.BaseSize.Evaluate() * currentStats.SizeMultiplier.Evaluate();
        float projectileCount = currentStats.ProjectileCount.Evaluate();

        targetTracker.SetAttackSize(weaponSize);

        StartCoroutine(AttackRoutine(
            Game.Player.Stats.AttackCooldown * .95f,
            (int)projectileCount
        ));
    }

    void OnValidate()
    {
        if (data == null)
        {
            return;
        }
        float baseWeaponSize = data.stats.baseSize.baseValue;
        targetTracker.SetAttackSize(baseWeaponSize);
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
                targetTracker.EnemiesInRange.RemoveAll(enemy => enemy == null || !enemy.gameObject.activeSelf);
                Shoot();

                yield return new WaitForSeconds(timeBetweenAttacks);
            }
        }
    }

    void Shoot()
    {
        var characterStats = Game.Player.Stats.Character;
        float damageMultiplier = characterStats.DamageMultiplier.Evaluate();

        Vector3 transformOrigin = new(_wielder.transform.position.x, projectileOriginY, _wielder.transform.position.z);

        Projectile projectile = Game.ObjectPool.Spawn<Projectile>(data.projectile, transformOrigin, Quaternion.identity);

        var enemiesInRange = targetTracker.EnemiesInRange;

        BaseCharacter random = enemiesInRange.Count >= 1
            ? enemiesInRange[Random.Range(0, enemiesInRange.Count)]
            : null
        ;

        int damage = (int)(data.stats.damage.baseValue * damageMultiplier);

        projectile.Initialize(_wielder, random, damage);
    }

    void RegisterHit(Enemy enemy)
    {
        if (data.hitVFX)
        {
            Instantiate(data.hitVFX, enemy.transform.position + new Vector3(0, 1, -1f), Quaternion.identity);
        }

        var characterStats = Game.Player.Stats.Character;
        int damage = (int)(data.stats.damage.baseValue * characterStats.DamageMultiplier.Evaluate());

        int enemyHp = enemy.Health.TakeDamage(damage);
        if (enemyHp == 0)
        {
            targetTracker.MarkDirty();
        }
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
