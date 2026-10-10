
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
struct ParticleSystemVariables
{
    public ParticleSystemControls controls;
    [Range(0f, 1f)]
    public float tryRegisterHitsAt;
}

public class Projectile : MonoBehaviour
{
    [SerializeField] SO_ProjectileData projectileData;
    [Header("Optional")]
    [SerializeField] ProjectileTargetTracker targetTracker;
    [SerializeField] ParticleSystemVariables particleSystem;
    [SerializeField] bool logOnTargetHit;
    public SO_ProjectileData Data => projectileData;
    BaseCharacter _sender;
    int _damage;
    bool _hitEverything;
    List<BaseCharacter> _charactersHit = new();

    /// <summary>
    /// Manually sets the target character and initiates the projectile arc.
    /// </summary>
    /// <param name="initialTarget"></param>
    /// <param name="damage"></param>
    /// <exception cref="NotImplementedException"></exception>
    public void Initialize(BaseCharacter sender, BaseCharacter initialTarget, int damage)
    {
        _sender = sender;
        _damage = damage;

        switch (projectileData.arcMode)
        {
            case ProjectileArcMode.None:
                _hitEverything = false;
                StartCoroutine(WaitForTargetRoutine(sender, initialTarget));
                break;
            case ProjectileArcMode.HitMiss:
                _hitEverything = true;
                StartCoroutine(HitMissRoutine(initialTarget));
                break;
            case ProjectileArcMode.Boomerang:
                _hitEverything = true;
                StartCoroutine(BoomerangRoutine(sender, initialTarget));
                break;
            case ProjectileArcMode.Bounce:
                _hitEverything = false;
                StartCoroutine(BounceRoutine(sender, initialTarget));
                break;
            default:
                throw new System.NotImplementedException();
        }
    }

    IEnumerator WaitForTargetRoutine(BaseCharacter sender, BaseCharacter target)
    {
        targetTracker.Initialize(sender, target);
        float elapsed = 0f;
        float duration = particleSystem.controls.EffectiveDuration;
        bool hasAttempedHits = false;

        while (elapsed < duration)
        {
            float elapsedRelative = elapsed / duration;

            if (elapsedRelative >= particleSystem.tryRegisterHitsAt && !hasAttempedHits)
            {
                HitTargetWithinRange(target);

                hasAttempedHits = true;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        // elapsedRelative is unlikely to land at exactly 1f.
        if (!hasAttempedHits)
        {
            HitTargetWithinRange(target);
        }


        Destroy(gameObject);
    }

    void HitTargetWithinRange(BaseCharacter target)
    {
        if (targetTracker.IsCharacterWithinRange(target))
        {
            RegisterHit(target);
        }
    }

    IEnumerator HitMissRoutine(BaseCharacter target)
    {
        if (targetTracker)
        {
            targetTracker.gameObject.SetActive(false);
        }

        float elapsed = 0f;
        Vector3 direction = PointToPosition(target.ColliderCenter);

        while (elapsed < projectileData.lifeTime)
        {
            if (_charactersHit.Count >= 1)
            {
                break;
            }

            float step = projectileData.initialVelocity * Time.deltaTime;
            transform.position += direction * step;

            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }

    IEnumerator BoomerangRoutine(BaseCharacter sender, BaseCharacter initialTarget)
    {
        targetTracker.Initialize(sender, initialTarget);
        float threshold = projectileData.targetReachedThreshold;

        while (!targetTracker.IsTargetPositionReached(threshold))
        {
            MoveTowardsTarget();
            yield return null;
        }

        // Return to sender.
        Vector3 fallbackPosition = _sender.ColliderCenter;
        targetTracker.SetNewTarget(_sender, fallbackPosition);

        while (!targetTracker.IsTargetPositionReached(threshold))
        {
            MoveTowardsTarget();
            yield return null;
        }

        Destroy(gameObject);
    }

    IEnumerator BounceRoutine(BaseCharacter sender, BaseCharacter initialTarget)
    {
        targetTracker.Initialize(sender, initialTarget);
        throw new System.NotImplementedException();
    }

    void MoveTowardsTarget()
    {
        Vector3 targetPosition = targetTracker.TargetPosition;
        PointToPosition(targetPosition);

        float step = projectileData.initialVelocity * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!_hitEverything || !other.TryGetComponent<BaseCharacter>(out var character) || character == _sender)
        {
            return;
        }

        RegisterHit(character);
    }

    void OnValidate()
    {
        if (projectileData.arcMode == ProjectileArcMode.None && particleSystem.controls == null)
        {
            Debug.LogWarning($"{nameof(Projectile)} <b><color=white>{name}</color></b> with '{nameof(projectileData)} = {nameof(ProjectileArcMode.None)}' is missing field <b><color=white>{nameof(ParticleSystemControls)}</color></b>");
        }
    }

    void RegisterHit(BaseCharacter character)
    {
        character.Health.TakeDamage(_damage);
        _charactersHit.Add(character);

        if (logOnTargetHit)
        {
            Debug.Log($"Projectile <color=white>{name}</color> found its target {character.name}");
        }
    }

    Vector3 PointToPosition(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        direction.y = 0;

        transform.rotation = Quaternion.Euler(0, Mathf.Rad2Deg * Mathf.Atan2(direction.x, direction.z), 0);

        return direction;
    }
}
