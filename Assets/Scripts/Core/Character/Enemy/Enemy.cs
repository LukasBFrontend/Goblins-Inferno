using UnityEngine;

public class Enemy : BaseCharacter, IPoolable
{
    [Header("Enemy")]
    [SerializeField] EnemyLoot loot;
    [SerializeField] int scoreValue = 100;

    public string PoolableId { get; set; }

    public override void Die()
    {
        if (loot)
        {
            loot.Drop();
        }

        Game.ObjectPooler.Return(this);
        Game.AddPoints(scoreValue);
    }

    void IPoolable.Reset()
    {
        Health.Reset();
    }
}
