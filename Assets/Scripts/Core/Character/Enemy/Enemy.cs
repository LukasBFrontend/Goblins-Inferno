using UnityEngine;

public class Enemy : BaseCharacter, IPoolable
{
    [SerializeField] EnemyLoot loot;
    public string PoolId { get; set;}
    public GameObject GameObject => gameObject;

    public override void Die()
    {
        if (loot)
        {
            loot.Drop();
        }

        ReturnToPool();
    }

    void IPoolable.SetPosition(Vector3 newPosition)
    {
        transform.position = newPosition;
    }

    void IPoolable.Reset()
    {
        Health.Reset();
    }

    public void ReturnToPool()
    {
        Game.ObjectPoolManager.Return(this);
    }
}
