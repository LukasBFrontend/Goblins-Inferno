using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class WizardAttacks : MonoBehaviour
{
    [Header("Energy Explosion")]
    [SerializeField] PoolableData explosionProjectile;
    [SerializeField] int damage;
    Enemy _enemy;

    void Awake()
    {
        _enemy = GetComponent<Enemy>();
    }

    public void StartChannelingExplosion()
    {
        StartExplosionSequence(Game.Player.transform);
    }

    void StartExplosionSequence(Transform origin)
    {
        EnergyExplosionProjectile projectile = Game.ObjectPooler.Spawn<EnergyExplosionProjectile>(explosionProjectile, origin.position, Quaternion.identity);
        projectile.Initialize(_enemy, Game.Player, damage);
    }
}
