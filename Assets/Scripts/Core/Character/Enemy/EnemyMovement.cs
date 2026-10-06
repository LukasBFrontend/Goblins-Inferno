using System.Collections;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class EnemyMovement : MonoBehaviour
{

    [SerializeField] LayerMask enemyLayer;
    [Range(0f, 8f)]
    [SerializeField] float baseMoveSpeed = 4;
    [SerializeField] float stopAt = 10f;
    [SerializeField] float stopDistance = 1f;
    [SerializeField] float personalSpace = 2f;
    Rigidbody _rigidbody;
    Enemy _enemy;
    bool _isActive;
    bool _enemyIsStationary => _enemy.Rigidbody.linearVelocity.magnitude < 0.1;
    Vector3 _nudgeSignal;

    public void Nudge(Vector3 signal)
    {
        _nudgeSignal = signal;
    }

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

        CalculateNudgeSignal(personalSpace);
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

        _rigidbody.linearVelocity = (direction  + _nudgeSignal) * adjustedMoveSpeed;

        _enemy.Animator.SetBool("IsStationary", _enemyIsStationary);
        SetRotationDirection(direction);
    }

    void SetRotationDirection(Vector3 direction)
    {
        _enemy.RenderGroup.transform.localRotation = Quaternion.Euler(0, Mathf.Rad2Deg * Mathf.Atan2(direction.x, direction.z), 0);
    }

    void CalculateNudgeSignal(float distance)
    {
        Collider[] nearbyEnemyColliders = Physics.OverlapSphere(transform.position, distance, enemyLayer);

        Vector3 averagePosition = Vector3.zero;
        int enemyCount = 0;

        foreach (var collider in nearbyEnemyColliders)
        {
            if (!collider.TryGetComponent<Enemy>(out var enemy) || enemy == _enemy)
            {
                continue;
            }

            averagePosition += enemy.transform.position;
            enemyCount++;
        }

        if (enemyCount == 0)
        {
            _nudgeSignal = Vector3.zero;
            return;
        }

        averagePosition /= enemyCount;

        Vector3 delta = transform.position - averagePosition;
        float distanceToAverage = delta.magnitude;
        float strength = Mathf.InverseLerp(distance, 0f, distanceToAverage);
        strength = Mathf.SmoothStep(0f, 1f, strength);

        _nudgeSignal = delta.normalized * strength;
    }
}
