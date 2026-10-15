
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
struct ParticleSystemVariables
{
    public ParticleSystemControls controls;
    [Range(0f, 1f)]
    public float tryRegisterHitsAt;
}

// TODO: Seperate different projectile behaviors into components
public class Projectile : MonoBehaviour
{
    [SerializeField] SO_ProjectileData data;
    [Header("Optional")]
    [SerializeField] ProjectileTargetTracker targetTracker;
    [SerializeField] ParticleSystemVariables particleSystem;
    [Header("Physics")]
    [SerializeField] Rigidbody rigidbody;
    [SerializeField] bool logOnTargetHit;
    public SO_ProjectileData Data => data;
    BaseCharacter _sender;
    int _damage;
    bool _hitEverything;
    List<BaseCharacter> _charactersHit = new();

    /// <summary>
    /// Manually sets the target character of the projectile and initiates the projectile arc.
    /// </summary>
    /// <param name="initialTarget"></param>
    /// <param name="damage"></param>
    /// <exception cref="NotImplementedException"></exception>
    public void Initialize(BaseCharacter sender, BaseCharacter initialTarget, int damage)
    {
        _sender = sender;
        _damage = damage;

        // TODO: Add damage on contact ass a field in SO_Projectile_Data
        switch (data.arcMode)
        {
            case ProjectileArcMode.None:
                _hitEverything = false;
                StartCoroutine(StaticAnimationRoutine(initialTarget));
                break;
            case ProjectileArcMode.HitMiss:
                _hitEverything = true;
                StartCoroutine(HitMissRoutine(initialTarget));
                break;
            case ProjectileArcMode.Boomerang:
                _hitEverything = true;
                StartCoroutine(BoomerangRoutine(initialTarget));
                break;
            case ProjectileArcMode.Physics:
                _hitEverything = false;
                StartCoroutine(BombRoutine(initialTarget));
                break;
            default:
                throw new System.NotImplementedException();
        }
    }

    public static Vector3 RandomMaxRange(Transform transform, float range)
    {
        Vector2 random = Random.insideUnitCircle.normalized;
        return transform.position + new Vector3(random.x, 0f, random.y) * range;
    }

    IEnumerator StaticAnimationRoutine(BaseCharacter target)
    {
        targetTracker.Initialize(target);
        float elapsed = 0f;
        float duration = particleSystem.controls.EffectiveDuration;
        bool hasAttempedHits = false;

        while (elapsed < duration)
        {
            float elapsedRelative = elapsed / duration;

            if (elapsedRelative >= particleSystem.tryRegisterHitsAt && !hasAttempedHits)
            {
                HitTargetIfWithinRange(target);

                hasAttempedHits = true;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        // elapsedRelative is unlikely to land at exactly 1f.
        if (!hasAttempedHits)
        {
            HitTargetIfWithinRange(target);
        }


        Destroy(gameObject);
    }

    void HitTargetsWithinRange()
    {
        foreach (var character in targetTracker.InRange(data.newTargetRange, data.targetsLayerMask))
        {
            RegisterHit(character);
        }
    }

    IEnumerator BombRoutine(BaseCharacter target)
    {
        targetTracker.Initialize();

        Vector3 direction = PointToPosition(target != null
            ? target.ColliderCenter
            : RandomMaxRange(transform, Data.newTargetRange)
        );
        direction.y = .5f; // Throw at an upwards angle.

        rigidbody.AddForce(direction * data.initialVelocity, ForceMode.Impulse);

        float duration = data.lifeTime;
        yield return new WaitForSeconds(duration);

        if (gameObject == null)
        {
            yield break;
        }

        DestroyProjectile();
    }

    void HitTargetIfWithinRange(BaseCharacter target)
    {
        if (targetTracker.InRange(data.newTargetRange, data.targetsLayerMask).Contains(target))
        {
            RegisterHit(target);
        }
    }

    void DestroyProjectile()
    {
        if (data.arcMode == ProjectileArcMode.Physics) // Is bomb.
        {
            HitTargetsWithinRange();
        }

        Instantiate(data.onDestroyVFX, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    IEnumerator HitMissRoutine(BaseCharacter target)
    {
        if (targetTracker)
        {
            targetTracker.gameObject.SetActive(false);
        }

        float elapsed = 0f;

        Vector3 direction = PointToPosition(target != null
            ? target.ColliderCenter
            : RandomMaxRange(transform, Data.newTargetRange)
        );

        while (elapsed < data.lifeTime)
        {
            if (_charactersHit.Count >= 1)
            {
                break;
            }

            float step = data.initialVelocity * Time.deltaTime;
            transform.position += direction * step;

            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }

    IEnumerator BoomerangRoutine(BaseCharacter initialTarget)
    {
        targetTracker.Initialize(initialTarget);
        float threshold = data.targetReachedThreshold;

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

    void MoveTowardsTarget()
    {
        Vector3 targetPosition = targetTracker.TargetPosition;
        PointToPosition(targetPosition);

        float step = data.initialVelocity * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);
    }

    void OnTriggerEnter(Collider other)
    {
        if ((data.targetsLayerMask & (1 << other.gameObject.layer)) == 0 || !other.TryGetComponent<BaseCharacter>(out var character) || character == _sender)
        {
            return;
        }

        RegisterHit(character);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!data.destroyOnContact || (data.targetsLayerMask & (1 << collision.gameObject.layer)) == 0)
        {
            return;
        }

        DestroyProjectile();
    }

    void OnValidate()
    {
        if (data.arcMode == ProjectileArcMode.None && particleSystem.controls == null)
        {
            Debug.LogWarning($"{nameof(Projectile)} <b><color=white>{name}</color></b> with '{nameof(data)} = {nameof(ProjectileArcMode.None)}' is missing field <b><color=white>{nameof(ParticleSystemControls)}</color></b>");
        }

        if (data.arcMode == ProjectileArcMode.Physics && rigidbody == null)
        {
            Debug.LogWarning($"Bomb projectile is missing particleSystemControls or rigidbody");
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
