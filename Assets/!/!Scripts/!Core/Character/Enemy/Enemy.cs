using UnityEngine;

public class Enemy : BaseCharacter, IPoolable
{
    [Header("Enemy")]
    [SerializeField] EnemyLoot loot;
    [SerializeField] EnemyMovement movement;
    [SerializeField] int scoreValue = 100;

    public EnemyMovement Movement => movement;

    public string PoolableId { get; set; }

    public override void Die()
    {
        if (loot)
        {
            loot.Drop();
        }

        Game.ObjectPooler.Return(this);
        GameSaveState.Score.AddPoints(scoreValue);
    }

    void IPoolable.Reset()
    {
        Health.Reset();
        Movement.Reset();
    }
}
