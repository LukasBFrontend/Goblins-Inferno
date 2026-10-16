using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class WizardAttacks : MonoBehaviour
{
    [Header("Energy Explosion")]
    [SerializeField] PoolableData explosionProjectile;
    [SerializeField] int damage;
    Enemy _archer;

    void Awake()
    {
        _archer = GetComponent<Enemy>();
    }

    public void StartChannelingExplosion()
    {
        StartExplosionSequence(Game.Player.transform);
    }

    private void StartExplosionSequence(Transform origin)
    {
        Projectile projectile = Game.ObjectPool.Spawn<Projectile>(explosionProjectile, origin.position, Quaternion.identity);
        projectile.Initialize(_archer, Game.Player, damage);
    }
}
