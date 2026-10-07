using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] SO_ProjectileData projectileData;
    [SerializeField] ProjectileTargetTracker targetTracker;
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
                _hitEverything = true;
                StartCoroutine(MoveStraightRoutine(initialTarget));
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
                throw new NotImplementedException();
        }
    }

    IEnumerator MoveStraightRoutine(BaseCharacter target)
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

        while (!targetTracker.HasReachedTarget(threshold))
        {
            MoveTowardsTarget();
            yield return null;
        }

        // Return to sender.
        Vector3 fallbackPosition = _sender.ColliderCenter;
        targetTracker.SetNewTarget(_sender, fallbackPosition);

        while (!targetTracker.HasReachedTarget(threshold))
        {
            MoveTowardsTarget();
            yield return null;
        }

        Destroy(gameObject);
    }

    IEnumerator BounceRoutine(BaseCharacter sender, BaseCharacter initialTarget)
    {
        targetTracker.Initialize(sender, initialTarget);
        throw new NotImplementedException();
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
