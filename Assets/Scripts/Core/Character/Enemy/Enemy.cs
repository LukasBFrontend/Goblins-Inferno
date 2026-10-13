using UnityEngine;

public class Enemy : BaseCharacter, IPoolable
{
    [SerializeField] EnemyLoot loot;
    public string Id { get; set; }
    public GameObject GameObject => gameObject;

    public override void Die()
    {
        if (loot)
        {
            loot.Drop();
        }

        Game.ObjectPool.Return(this);
    }

    void IPoolable.Reset()
    {
        Health.Reset();
    }
}
