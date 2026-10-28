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
    public int Lvl => _wielder.Stats.GetWeaponModifier(data.weaponName).Lvl;
    public Sprite Sprite => _wielder.Stats.GetWeaponModifier(data.weaponName).Damage.Sprite;
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
                    targetTracker.EnemiesInRange.RemoveAll(
                        enemy => enemy == null || !enemy.gameObject.activeSelf
                    );

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
                    float elapsed = 0f;

                    while (elapsed < remainingTime)
                    {
                        yield return null;
                        elapsed += Time.deltaTime;
                    }
                }

                continue;
            }
            else if (data.weaponType == WeaponType.Projectile)
            {
                targetTracker.EnemiesInRange.RemoveAll(
                    enemy => enemy == null || !enemy.gameObject.activeSelf
                );

                Shoot();

                float elapsed = 0f;

                while (elapsed < timeBetweenAttacks)
                {
                    yield return null;
                    elapsed += Time.deltaTime;
                }
            }
        }
    }

    void Shoot()
    {
        CharacterStats characterStats = Game.Player.Stats.Character;
        WeaponStats weaponStats = Game.Player.Stats.GetWeaponModifier(data.weaponName);
        float damageMultiplier = characterStats.DamageMultiplier.Evaluate();
        float baseDamage = weaponStats.Damage.Evaluate();

        Vector3 transformOrigin = new(_wielder.transform.position.x, projectileOriginY, _wielder.transform.position.z);

        Projectile projectile = Game.ObjectPooler.Spawn<Projectile>(data.projectile, transformOrigin, Quaternion.identity);

        List<Enemy> enemiesInRange = targetTracker.EnemiesInRange;

        BaseCharacter random = enemiesInRange.Count >= 1
            ? enemiesInRange[Random.Range(0, enemiesInRange.Count)]
            : null
        ;

        int damage = CalculateHitDamage();

        projectile.Initialize(_wielder, random, damage, (int)weaponStats.CritChance.Evaluate());
    }

    void RegisterHit(Enemy enemy)
    {
        if (data.hitVFX)
        {
            Instantiate(data.hitVFX, enemy.transform.position + new Vector3(0, 1, -1f), Quaternion.identity);
        }

        int damage = CalculateHitDamage(out int critLevel);
        int enemyHp = enemy.Health.TakeDamage(damage, critLevel); // REEE

        if (enemyHp == 0)
        {
            targetTracker.MarkDirty();
        }
    }

    int CalculateHitDamage()
    {
        CharacterStats characterStats = Game.Player.Stats.Character;
        WeaponStats weaponStats = Game.Player.Stats.GetWeaponModifier(data.weaponName);
        float damageMultiplier = characterStats.DamageMultiplier.Evaluate();
        float baseDamage = weaponStats.Damage.Evaluate();

        int damage = (int)(baseDamage * damageMultiplier);

        return damage;        
    }

    int CalculateHitDamage(out int critLevel)
    {
        CharacterStats characterStats = Game.Player.Stats.Character;
        WeaponStats weaponStats = Game.Player.Stats.GetWeaponModifier(data.weaponName);
        float damageMultiplier = characterStats.DamageMultiplier.Evaluate();
        float baseDamage = weaponStats.Damage.Evaluate();

        int damage = (int)(baseDamage * damageMultiplier);

        int critModifier = WeaponsUtils.RollCritModifier((int)weaponStats.CritChance.Evaluate(), out int level);

        critLevel = level;
        damage *= critModifier;

        return damage; 
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
