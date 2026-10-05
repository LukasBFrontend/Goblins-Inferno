using UnityEngine;

public class ArcherController : BaseEnemyController
{
    [Range(0f, 30f)]
    [SerializeField] float kitingDistance = 10f;
    [Range(0f, 2f)]
    [SerializeField] float stopDistance = .25f;
    [Header("Attack")]
    [SerializeField] GameObject ArrowPrefab;
    [SerializeField] Transform leftArrowSpawnPoint;
    [SerializeField] Transform rightArrowSpawnPoint;

    public void ShootArrowLeft()
    {
        ShootArrow(leftArrowSpawnPoint);
    }

    public void ShootArrowRight()
    {
        ShootArrow(rightArrowSpawnPoint);
    }

    void Awake()
    {
        Initialize();
    }

    void Update()
    {
        TrackPlayer(kitingDistance, stopDistance);
    }

    void ShootArrow(Transform origin)
    {
        GameObject ArrowInstance = Instantiate(ArrowPrefab, origin.position, Quaternion.identity);

        Projectile projectile = ArrowInstance.GetComponent<Projectile>();
        projectile.SetTarget(Game.Player);
    }
}
