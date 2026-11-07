
using System.Collections;
using UnityEngine;

public sealed class BoomerangProjectile : BaseProjectile
{
    [SerializeField] private float velocityCompensation = 1f;
    [SerializeField] private float minSpeedMultiplier = 0f;
    [SerializeField] private float maxSpeedMultiplier = 3f;

    public override IEnumerator ProjectileRoutine(BaseCharacter initialTarget)
    {
        Vector3 startPosition = Sender.transform.position;
        Vector3 targetRelative;

        if (initialTarget != null)
        {
            Vector3 direction = (initialTarget.transform.position - startPosition).normalized;
            targetRelative = direction * Data.newTargetRange;
        }
        else
        {
            targetRelative = RandomMaxRange(transform, Data.newTargetRange) - startPosition;
        }

        TargetTracker.SetTarget(transform.position + targetRelative);

        float threshold = Data.targetReachedThreshold;
        Vector3 lastSenderPosition = Sender.transform.position;

        // Outbound journey.
        while (!TargetTracker.IsTargetPositionReached(threshold))
        {
            UpdateTargetPosition(startPosition, lastSenderPosition);
            MoveWithSenderVelocityCompensation(ref lastSenderPosition, false);
            yield return null;
        }

        // Return journey.
        TargetTracker.SetTarget(Sender);

        while (!TargetTracker.IsTargetPositionReached(threshold))
        {
            UpdateTargetPosition(startPosition, lastSenderPosition);
            MoveWithSenderVelocityCompensation(ref lastSenderPosition, true);
            yield return null;
        }

        Game.ObjectPooler.Return(this);
    }

    private void UpdateTargetPosition(Vector3 startPosition, Vector3 lastSenderPosition)
    {
        Vector3 moveDelta = Sender.transform.position - lastSenderPosition;
        Vector3 currentTargetRelative = TargetTracker.TargetPosition - startPosition;
        Vector3 projection = Vector3.Project(moveDelta, currentTargetRelative);

        TargetTracker.SetTarget(TargetTracker.TargetPosition + projection);
    }

    private void MoveWithSenderVelocityCompensation(ref Vector3 lastSenderPosition, bool returning)
    {
        Vector3 senderVelocity = (Sender.transform.position - lastSenderPosition) / Time.deltaTime;
        Vector3 targetDirection = returning
            ? (Sender.transform.position - transform.position).normalized
            : (TargetTracker.TargetPosition - transform.position).normalized;

        float directionalVelocity = Vector3.Dot(senderVelocity, targetDirection);
        float speed = CalculateCompensatedSpeed(directionalVelocity);

        MoveTowardsTarget(speed, out Vector3 velocity);

        lastSenderPosition = Sender.transform.position;
    }

    private float CalculateCompensatedSpeed(float directionalVelocity)
    {
        float speed = Data.initialVelocity + directionalVelocity * velocityCompensation;

        return Mathf.Clamp(
            speed,
            Data.initialVelocity * minSpeedMultiplier,
            Data.initialVelocity * maxSpeedMultiplier
        );
    }

    private void OnTriggerEnter(Collider other)
    {
        if (
            !other.TryGetComponent(out BaseCharacter character)
            || character == Sender
            || (Data.targetsLayerMask & (1 << other.gameObject.layer)) == 0
            || !gameObject.activeSelf
        )
        {
            return;
        }

        RegisterHit(character);
    }

    private void MoveTowardsTarget(float speed, out Vector3 velocity)
    {
        Vector3 targetPosition = TargetTracker.TargetPosition;
        PointToPosition(targetPosition);

        float step = speed * Time.deltaTime;
        Vector3 newPosition = Vector3.MoveTowards(transform.position, targetPosition, step);

        velocity = (newPosition - transform.position) / Time.deltaTime;

        if (velocity.x == float.NaN)
        {
            return;
        }
        transform.position = newPosition;
    }
}
