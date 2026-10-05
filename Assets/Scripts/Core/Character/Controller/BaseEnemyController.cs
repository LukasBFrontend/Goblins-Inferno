using UnityEngine;

[RequireComponent(typeof(Enemy))]
public abstract class BaseEnemyController : MonoBehaviour
{
    [Range(0.5f, 50f)]
    [SerializeField] float baseMoveSpeed = 4;
    Rigidbody _rigidbody;
    Enemy _enemy;
    protected bool EnemyIsStationary => _enemy.Rigidbody.linearVelocity.magnitude < 0.1;

    protected void Initialize()
    {
        _enemy = GetComponent<Enemy>();
        _rigidbody = _enemy.Rigidbody;
    }

    protected void TrackPlayer()
    {
        Vector3 dir = _enemy.DirectionTo(Game.Player);
        _rigidbody.linearVelocity = dir * baseMoveSpeed;
        SetRotationDirection(dir);

        _enemy.Animator.SetFloat("MoveSpeed", baseMoveSpeed);
    }

    protected void TrackPlayer(float distanceAway, float stopDistance)
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
