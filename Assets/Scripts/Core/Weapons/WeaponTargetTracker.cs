using UnityEngine;

public class WeaponTargetTracker : MonoBehaviour
{
    [Tooltip("Enemies inside the collider are considered within range of the tracker.")]
    [SerializeField] Collider collider;
    [SerializeField] Transform transformTarget;
    SO_WeaponData _weapon;

    public void Initialize(SO_WeaponData weapon)
    {
        _weapon = weapon;
    }

    private void Update()
    {
        Vector3 closestDir = GameUtils.ClosestEnemyToPlayerDir(Player.Instance);
        float attackAngle = Mathf.Rad2Deg * Mathf.Atan2(closestDir.x, closestDir.z );
        _weapon.Attack(Player.Instance.Stats.AttackCooldown, transformTarget, attackAngle);
    }

    private void Awake()
    {
        if (collider == null)
        {
            Debug.LogWarning($"WeaponTargetTracker assigned to {name} is missing a Collider reference and will not function as intended.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent<Enemy>(out var enemy))
        {
            return;
        }

        _weapon.EnterIntoRange(enemy);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.TryGetComponent<Enemy>(out var enemy))
        {
            return;
        }

        _weapon.ExitFromRange(enemy);
    }
}
