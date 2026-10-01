using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class ProjectileTargetTracker : MonoBehaviour
{
    [Tooltip("Enemies inside the collider are considered within range of the tracker.")]
    [SerializeField] CircleCollider2D collider;
    [Header("Runtime filled")]
    [SerializeField] List<Enemy> enemiesInRange = new();

    public Enemy ClosestEnemyInRange()
    {
        Enemy closest = null;
        float closestDistance = float.MaxValue;

        foreach (var enemy in enemiesInRange)
        {
            float distance = Vector2.Distance(
                transform.position,
                enemy.Rigidbody.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = enemy;
            }
        }

        return closest;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.TryGetComponent<Enemy>(out var enemy))
        {
            return;
        }

        enemiesInRange.Add(enemy);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.TryGetComponent<Enemy>(out var enemy) || !enemiesInRange.Contains(enemy))
        {
            return;
        }

        enemiesInRange.Remove(enemy);
    }
}
