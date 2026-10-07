using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class ProjectileTargetTracker : MonoBehaviour
{
    [Tooltip("Enemies inside the collider are considered within range of the tracker.")]
    [SerializeField] SphereCollider collider;
    [SerializeField] Projectile projectile;
    [Header("Runtime filled")]
    [SerializeField] List<BaseCharacter> charactersInRange = new();
    BaseCharacter _sender;
    BaseCharacter _targetCurrent;
    Vector3 _fallbackPosition;
    public Vector3 TargetPosition => _targetCurrent != null ? _targetCurrent.ColliderCenter : _fallbackPosition;

    public void Initialize(BaseCharacter sender, BaseCharacter initialTarget)
    {
        _sender = sender;
        _targetCurrent = initialTarget;
        _fallbackPosition = RandomMaxRange(projectile.Data.range);
    }

    public void Initialize(BaseCharacter sender)
    {
        _sender = sender;
        _fallbackPosition = RandomMaxRange(projectile.Data.range);
    }

    public void SetNewTarget(BaseCharacter target, Vector3 fallbackPosition)
    {
        _targetCurrent = target;
        _fallbackPosition = fallbackPosition;
    }

    public bool HasReachedTarget(float threshold)
    {
        return Vector3.Distance(TargetPosition, transform.position) < threshold;
    }

    Vector3 RandomMaxRange(float range)
    {
        Vector2 random = Random.insideUnitCircle.normalized;
        return new Vector3(random.x, 0f, random.y) * range;
    }

    public BaseCharacter ClosestBaseCharacterInRange()
    {
        BaseCharacter closest = null;
        float closestDistance = float.MaxValue;
        bool dirty = false;

        foreach (var character in charactersInRange)
        {
            if (character == null)
            {
                dirty = true;
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

        if (dirty)
        {
            charactersInRange.RemoveAll(character => character == null);
        }

        return closest;
    }

    void OnValidate()
    {
        if (!projectile || !projectile.Data)
        {
            Debug.LogWarning($"<color=white>{name}</color> is missing reference <color=white>{nameof(projectile)}</color> or the <color=white>{nameof(Projectile)}</color> instance does not have a valid data reference.");
            return;
        }

        collider.radius = projectile.Data.range;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.TryGetComponent<BaseCharacter>(out var character) || character == _sender)
        {
            return;
        }

        charactersInRange.Add(character);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.TryGetComponent<BaseCharacter>(out var character) || character == _sender || !charactersInRange.Contains(character))
        {
            return;
        }

        charactersInRange.Remove(character);
    }
}
