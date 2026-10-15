using UnityEngine;

public class Enemy : BaseCharacter, IPoolable
{
    [SerializeField] EnemyLoot loot;
    [SerializeField] int scoreValue = 100;
    public string Id { get; set; }

    public override void Die()
    {
        if (loot)
        {
            loot.Drop();
        }

        Game.ObjectPool.Return(this);
        Game.AddPoints(scoreValue);
    }

    void IPoolable.Reset()
    {
        Health.Reset();
    }
}
