using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponTargetTracker : MonoBehaviour
{
    [SerializeField] Weapon weapon;
    [SerializeField] SphereCollider collider;
    [Header("Runtime filled")]
    [SerializeField] List<Enemy> enemiesInRange = new();
    public List<Enemy> EnemiesInRange => enemiesInRange;

    public void SetScale(float radius)
    {
        transform.localScale = new(radius, radius, radius);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent<Enemy>(out var enemy))
        {
            return;
        }

        enemiesInRange.Add(enemy);
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.TryGetComponent<Enemy>(out var enemy))
        {
            return;
        }

        enemiesInRange.Remove(enemy);
    }
}
