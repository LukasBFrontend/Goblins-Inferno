
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

[System.Obsolete("Should be refactored")]
public class Projectile : MonoBehaviour, IPoolable
{
    [SerializeField] SO_ProjectileData data;
    [SerializeField] GameObject render;
    [Header("Optional")]
    [SerializeField] ProjectileTargetTracker targetTracker;
    [SerializeField] ParticleSystemVariables particleSystem;
    [Header("Physics")]
    [SerializeField] Rigidbody rigidbody;
    [SerializeField] bool logOnTargetHit;

    BaseCharacter _sender;
    int _damage;
    bool _hitEverything;
    List<BaseCharacter> _charactersHit = new();
    WaitForSeconds _bombWaitForSeconds;

    public SO_ProjectileData Data => data;
    public string PoolableId { get; set; }

    /// <summary>
    /// Manually sets the target character of the projectile and initiates the projectile arc.
    /// </summary>
    /// <param name="initialTarget"></param>
    /// <param name="damage"></param>
    public void Initialize(BaseCharacter sender, BaseCharacter initialTarget, int damage)
    {
        _sender = sender;
        _damage = damage;

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


        Game.ObjectPooler.Return(this);
    }

    void HitTargetsWithinRange()
    {
        foreach (BaseCharacter character in targetTracker.InRange(data.newTargetRange, data.targetsLayerMask))
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

        yield return _bombWaitForSeconds;

        StartCoroutine(BombExplosionRoutine());
    }

    void HitTargetIfWithinRange(BaseCharacter target)
    {
        if (targetTracker.InRange(data.newTargetRange, data.targetsLayerMask).Contains(target))
        {
            RegisterHit(target);
        }
    }

    IEnumerator BombExplosionRoutine()
    {
        if (gameObject == null)
        {
            yield break;
        }

        HitTargetsWithinRange();
        Instantiate(data.onDestroyVFX, transform.position, Quaternion.identity);
        Game.ObjectPooler.Return(this);
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

        Game.ObjectPooler.Return(this);
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

        Game.ObjectPooler.Return(this);
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
        if (
            !_hitEverything
            || !other.TryGetComponent(out BaseCharacter character)
            || character == _sender
            || (data.targetsLayerMask & (1 << other.gameObject.layer)) == 0
            || !gameObject.activeSelf
        )
        {
            return;
        }

        RegisterHit(character);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (
            !data.destroyOnContact
            || (data.targetsLayerMask & (1 << collision.gameObject.layer)) == 0
            || !gameObject.activeSelf
        )
        {
            return;
        }

        StartCoroutine(BombExplosionRoutine());
    }

    void OnValidate()
    {
        if (
            data.arcMode == ProjectileArcMode.None && particleSystem.controls == null
            || !gameObject.activeSelf
        )
        {
            Debug.LogWarning($"{nameof(Projectile)} <b><color=white>{name}</color></b> with '{nameof(data)} = {nameof(ProjectileArcMode.None)}' is missing field <b><color=white>{nameof(ParticleSystemControls)}</color></b>");
        }

        if (data.arcMode == ProjectileArcMode.Physics && rigidbody == null)
        {
            Debug.LogWarning($"Bomb projectile is missing particleSystemControls or rigidbody");
        }
    }

    void OnEnable()
    {
        _bombWaitForSeconds = new(data.lifeTime);
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

    public void Reset()
    {
        transform.rotation = Quaternion.identity;

        _sender = null;
        _damage = 0;
        _hitEverything = false;
        _charactersHit.Clear();

        if (rigidbody)
        {
            rigidbody.linearVelocity = Vector3.zero;
            rigidbody.angularVelocity = Vector3.zero;
        }

        if (particleSystem.controls)
        {
            particleSystem.controls.Stop();
        }

        if (render)
        {
            render.SetActive(true);
        }
    }
}
