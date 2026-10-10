using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class ArcherAttacks : MonoBehaviour
{
    [Header("Shoot Arrow")]
    [SerializeField] GameObject arrowPrefab;
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
        GameObject arrowInstance = Instantiate(arrowPrefab, origin.position, Quaternion.identity);

        Projectile projectile = arrowInstance.GetComponent<Projectile>();
        projectile.Initialize(_archer, Game.Player, damage);
    }
}
