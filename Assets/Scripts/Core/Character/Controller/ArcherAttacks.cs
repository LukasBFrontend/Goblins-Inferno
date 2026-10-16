using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class ArcherAttacks : MonoBehaviour
{
    [Header("Shoot Arrow")]
    [SerializeField] PoolableData arrowProjectile;
    [SerializeField] Transform leftArrowSpawnPoint;
    [SerializeField] Transform rightArrowSpawnPoint;
    [SerializeField] int damage;
    Enemy _archer;

    void Awake()
    {
        _archer = GetComponent<Enemy>();
    }

    public void ShootArrowLeft()
    {
        ShootArrow(leftArrowSpawnPoint);
    }

    public void ShootArrowRight()
    {
        ShootArrow(rightArrowSpawnPoint);
    }

    void ShootArrow(Transform origin)
    {
        Projectile projectile = Game.ObjectPooler.Spawn<Projectile>(arrowProjectile, origin.position, Quaternion.identity);
        projectile.Initialize(_archer, Game.Player, damage);
    }
}
