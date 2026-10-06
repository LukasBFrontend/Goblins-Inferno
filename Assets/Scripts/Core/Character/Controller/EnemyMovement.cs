using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class EnemyMovement : MonoBehaviour
{

    [Range(0f, 8f)]
    [SerializeField] float baseMoveSpeed = 4;
    [Range(0f, 20f)]
    [SerializeField] float stopAt = 10f;
    [Range(0f, 2f)]
    [SerializeField] float stopDistance = 1f;
    Rigidbody _rigidbody;
    Enemy _enemy;
    bool _isActive;
    protected bool EnemyIsStationary => _enemy.Rigidbody.linearVelocity.magnitude < 0.1;

    void Awake()
    {
        _enemy = GetComponent<Enemy>();
        _rigidbody = _enemy.Rigidbody;

        _isActive = true;
        GameEvents.GameOverEvent.AddListener(OnGameOver);
    }

    void Update()
    {
        if (!_isActive)
        {
            return;
        }

        TrackPlayer(stopAt, stopDistance);
    }

    void OnGameOver()
    {
        _isActive = false;
        _rigidbody.linearVelocity = Vector3.zero;
    }

    void TrackPlayer(float distanceAway, float stopDistance)
    {
        Vector3 direction = _enemy.DirectionTo(Game.Player);
        float distanceToPlayer = _enemy.DistanceTo(Game.Player);

        float t = (distanceToPlayer - distanceAway) / stopDistance;
        float adjustedMoveSpeed = Mathf.Lerp(0f, baseMoveSpeed, t);

        _rigidbody.linearVelocity = direction * adjustedMoveSpeed;

        _enemy.Animator.SetBool("IsStationary", EnemyIsStationary);
        SetRotationDirection(direction);
    }

    void SetRotationDirection(Vector3 direction)
    {
        _enemy.RenderGroup.transform.localRotation = Quaternion.Euler(0, Mathf.Rad2Deg * Mathf.Atan2(direction.x, direction.z), 0);
    }
}
