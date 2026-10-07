using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class ArcherAttacks : MonoBehaviour
{
    [Header("Shoot Arrow")]
    [SerializeField] GameObject ArrowPrefab;
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
        GameObject ArrowInstance = Instantiate(ArrowPrefab, origin.position, Quaternion.identity);

        Projectile projectile = ArrowInstance.GetComponent<Projectile>();
        projectile.Initialize(_archer, Game.Player, damage);
    }
}
