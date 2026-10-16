using System.Collections.Generic;
using UnityEngine;

public class WeaponTargetTracker : MonoBehaviour
{
    [SerializeField] Weapon weapon;
    [SerializeField] SphereCollider collider;
    [Header("Runtime filled")]
    [SerializeField] List<Enemy> enemiesInRange = new();
    
    bool _dirty;
    
    public List<Enemy> EnemiesInRange => enemiesInRange;

    public void MarkDirty()
    {
        _dirty = true;
    }

    public void SetAttackSize(float radius)
    {
        transform.localScale = new(radius, radius, radius);
    }

    void Update()
    {
        if (!_dirty)
        {
            return;
        }

        enemiesInRange.RemoveAll(enemy => enemy == null || !enemy.gameObject.activeSelf);
        _dirty = false;
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
