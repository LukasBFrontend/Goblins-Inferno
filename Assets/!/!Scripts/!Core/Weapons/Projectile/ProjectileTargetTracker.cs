using System.Collections.Generic;
using UnityEngine;

public class ProjectileTargetTracker : MonoBehaviour
{
    [SerializeField] BaseProjectile projectile;

    BaseCharacter _targetCurrent;
    Vector3 _fallbackPosition;

    public Vector3 TargetPosition => _targetCurrent != null ? _targetCurrent.ColliderCenter : _fallbackPosition;

    public void SetTarget(BaseCharacter initialTarget)
    {
        _targetCurrent = initialTarget;

        _fallbackPosition = initialTarget != null 
            ? initialTarget.ColliderCenter 
            : BaseProjectile.RandomMaxRange(transform, projectile.Data.newTargetRange)
        ;
    }

    public void SetTarget(Vector3 position)
    {
        _fallbackPosition = position;
    }

    public void Reset()
    {
        _targetCurrent = null;
        _fallbackPosition = Vector3.zero;
    }

    public BaseCharacter[] InRange(float range, LayerMask layerMask)
    {
        Collider[] hitColliders = new Collider[6];
        int numColliders = Physics.OverlapSphereNonAlloc(transform.position, range, hitColliders, layerMask);

        List<BaseCharacter> results = new();

        for (int i = 0; i < numColliders; i++)
        {
            if (!hitColliders[i].TryGetComponent(out BaseCharacter character))
            {
                continue;
            }
            results.Add(character);
        }

        return results.ToArray();
    }

    public bool IsTargetPositionReached(float threshold)
    {
        return Vector3.Distance(TargetPosition, transform.position) < threshold;
    }

    BaseCharacter ClosestInRange(float range, LayerMask layerMask)
    {
        BaseCharacter closest = null;
        float closestDistance = float.MaxValue;

        foreach (BaseCharacter character in InRange(range, layerMask))
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
            Debug.LogWarning($"<color=white>{name}</color> is missing reference <color=white>{nameof(projectile)}</color> or the <color=white>{nameof(BaseProjectile)}</color> instance does not have a valid data reference.");
            return;
        }
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, projectile.Data.newTargetRange);
    }
}
