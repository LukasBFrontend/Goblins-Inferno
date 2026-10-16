using System.Collections.Generic;
using UnityEngine;

public class ProjectileTargetTracker : MonoBehaviour
{
    [SerializeField] Projectile projectile;
    [Space()]
    [Tooltip("A cheesy way to trigger validation in order to set collider radius to projectileData.range.")]
    [SerializeField] bool triggerValidate;
    BaseCharacter _targetCurrent;
    Vector3 _fallbackPosition;
    public Vector3 TargetPosition => _targetCurrent != null ? _targetCurrent.ColliderCenter : _fallbackPosition;

    public void Initialize(BaseCharacter initialTarget)
    {
        _targetCurrent = initialTarget;
        _fallbackPosition = Projectile.RandomMaxRange(transform, projectile.Data.newTargetRange);
    }

    public void Initialize()
    {
        _fallbackPosition = Projectile.RandomMaxRange(transform, projectile.Data.newTargetRange);
    }

    public BaseCharacter[] InRange(float range, LayerMask layerMask)
    {
        Collider[] colliders = Physics.OverlapSphere(
            transform.position,
            range,
            layerMask
        );

        List<BaseCharacter> results = new();

        foreach (var collider in colliders)
        {
            if (!collider.TryGetComponent(out BaseCharacter character))
            {
                continue;
            }
            results.Add(character);
        }

        return results.ToArray();
    }

    public void SetNewTarget(BaseCharacter target, Vector3 fallbackPosition)
    {
        _targetCurrent = target;
        _fallbackPosition = fallbackPosition;
    }

    public bool IsTargetPositionReached(float threshold)
    {
        return Vector3.Distance(TargetPosition, transform.position) < threshold;
    }

    BaseCharacter ClosestInRange(float range, LayerMask layerMask)
    {
        BaseCharacter closest = null;
        float closestDistance = float.MaxValue;

        foreach (var character in InRange(range, layerMask))
        {
            if (character == null || !character.gameObject.activeSelf)
            {
                continue;
            }

            float distance = Vector2.Distance(
                transform.position,
                character.Rigidbody.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = character;
            }
        }

        return closest;
    }

    void OnDrawGizmosSelected()
    {
        if (!projectile || !projectile.Data)
        {
            Debug.LogWarning($"<color=white>{name}</color> is missing reference <color=white>{nameof(projectile)}</color> or the <color=white>{nameof(Projectile)}</color> instance does not have a valid data reference.");
            return;
        }
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, projectile.Data.newTargetRange);
    }
}
