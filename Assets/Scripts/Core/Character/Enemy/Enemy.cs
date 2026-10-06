using UnityEngine;

public class Enemy : BaseCharacter
{
    [SerializeField] EnemyLoot loot;
    public override void Die()
    {
        if (loot)
        {
            loot.Drop();
        }
        Destroy(gameObject);
    }
}
