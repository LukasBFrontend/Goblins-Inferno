using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class EnemyMovement : MonoBehaviour
{
    [SerializeField] LayerMask enemyLayer;
    [Space()]
    [SerializeField] EnemyMovementType movementType;
    [Range(0f, 8f)]
    [SerializeField] float baseMoveSpeed = 4f;
    [SerializeField] float stopAt = 10f;
    [SerializeField] float stopDistance = 1f;
    [SerializeField] float personalSpace = 2f;
    [Space()]
    [SerializeField] KitingData kitingBehavior;

    Rigidbody _rigidbody;
    Enemy _enemy;
    bool _isActive;
    bool _enemyIsStationary => _enemy.Rigidbody.linearVelocity.magnitude < 0.1f;

    bool _isRetreating;
    bool _canStartFleeing = true;
    Vector3 _nudgeSignal;
    Coroutine _retreatCoroutine;

    void Awake()
    {
        _enemy = GetComponent<Enemy>();
        _rigidbody = _enemy.Rigidbody;

        _isActive = true;
        GameEvents.GameOverEvent.AddListener(OnGameOver);
    }

    void OnDestroy()
    {
        GameEvents.GameOverEvent.RemoveListener(OnGameOver);
    }

    void Update()
    {
        if (!_isActive)
        {
            return;
        }

        CalculateNudgeSignal(personalSpace);
        TrackPlayer(stopAt, stopDistance, kitingBehavior.kitingDistance);
    }

    void OnValidate()
    {
        if (movementType == EnemyMovementType.Melee)
        {
            kitingBehavior.kitingDistance = 0f;
            kitingBehavior.retreatCooldown = 0f;
        }
    }

    void OnGameOver()
    {
        _isActive = false;

        if (_retreatCoroutine != null)
        {
            StopCoroutine(_retreatCoroutine);
            _retreatCoroutine = null;
        }

        _isRetreating = false;
        _rigidbody.linearVelocity = Vector3.zero;
    }

    void TrackPlayer(float distanceAway, float stopDistance, float kitingDistance)
    {
        Vector3 direction = _enemy.DirectionTo(Game.Player);
        float distanceToPlayer = _enemy.DistanceTo(Game.Player);

        if (
            distanceToPlayer < kitingDistance && !_isRetreating && _canStartFleeing
        )
        {
            _retreatCoroutine = StartCoroutine(RetreatRoutine(kitingDistance));
        }

        float t = stopDistance > 0f ? (distanceToPlayer - distanceAway) / stopDistance: 1f;

        float adjustedMoveSpeed = Mathf.Lerp(0f, baseMoveSpeed, t);

        if (_isRetreating)
        {
            direction *= -1f;
            adjustedMoveSpeed = baseMoveSpeed;
        }

        _rigidbody.linearVelocity = (direction + _nudgeSignal).normalized * adjustedMoveSpeed;

        _enemy.Animator.SetBool("IsStationary", _enemyIsStationary);

        SetRotationDirection(direction);
    }

    void SetRotationDirection(Vector3 direction)
    {
        _enemy.RenderTransform.localRotation = Quaternion.Euler(0f, Mathf.Rad2Deg * Mathf.Atan2(direction.x, direction.z), 0f);
    }

    void CalculateNudgeSignal(float distance)
    {
        Collider[] hitColliders = new Collider[4];

        int numColliders = Physics.OverlapSphereNonAlloc(transform.position, distance, hitColliders, enemyLayer);

        Vector3 averagePosition = Vector3.zero;
        int enemyCount = 0;

        for (int i = 0; i < numColliders; i++)
        {
            if (!hitColliders[i].TryGetComponent(out BaseCharacter enemy) || enemy == _enemy)
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

    IEnumerator RetreatRoutine(float kitingDistance)
    {
        _isRetreating = true;
        _canStartFleeing = false;

        float escapeDistance = kitingDistance + kitingBehavior.retreatMargin;

        while (_isActive && _enemy.DistanceTo(Game.Player) < escapeDistance)
        {
            yield return null;
        }

        _isRetreating = false;
        _retreatCoroutine = null;

        if (!_isActive)
        {
            yield break;
        }

        yield return new WaitForSeconds(kitingBehavior.retreatCooldown);

        if (_isActive)
        {
            _canStartFleeing = true;
        }
    }
}

public enum EnemyMovementType
{
    Melee,
    Ranged,
}

[System.Serializable]
struct KitingData
{
    public float kitingDistance;
    public float retreatMargin;
    public float retreatCooldown;
}
