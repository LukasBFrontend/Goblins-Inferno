
using System.Collections;
using UnityEngine;

public sealed class BoomerangProjectile : BaseProjectile
{
    public override IEnumerator ProjectileRoutine(BaseCharacter initialTarget)
    {
        Vector3 startPosition = Sender.transform.position;
        Vector3 targetPosition;
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

        targetPosition = transform.position + targetRelative;
        TargetTracker.SetTarget(targetPosition);

        float threshold = Data.targetReachedThreshold;

        Vector3 lastSenderPosition = Sender.transform.position;
        Vector3 lastVelocity = Vector3.zero;

        while (!TargetTracker.IsTargetPositionReached(threshold))
        {
            Vector3 moveDelta = Sender.transform.position - lastSenderPosition;
            Vector3 currentTargetRelative = TargetTracker.TargetPosition - startPosition;
            Vector3 projection = Vector3.Project(moveDelta, currentTargetRelative);

            lastSenderPosition = Sender.transform.position;

            TargetTracker.SetTarget(TargetTracker.TargetPosition + projection);
            MoveTowardsTarget(Data.initialVelocity, out Vector3 velocity);
            lastVelocity = velocity;

            yield return null;
        }

        // Return to sender.
        TargetTracker.SetTarget(Sender);

        while (!TargetTracker.IsTargetPositionReached(threshold))
        {
            // Accelerate projectile speed based off the player velocity parallel to  the projectile velocity.
            float scalarProjection = Vector3.Dot(Sender.Rigidbody.linearVelocity, lastVelocity.normalized);

            float lastSpeed = lastVelocity.magnitude;
            float speedDifference = scalarProjection - lastSpeed;

            Debug.Log($"scalarProjection: <color=white>{scalarProjection}</color> lastSpeed: <color=white>{lastSpeed}</color> speedDifference: <color=white>{speedDifference}</color>");

            float adjustedSpeed = speedDifference > 0 
                ? Data.initialVelocity + speedDifference * 2 
                : Data.initialVelocity
            ;

            MoveTowardsTarget(adjustedSpeed, out Vector3 velocity);
            lastVelocity = velocity;

            yield return null;
        }

        Game.ObjectPooler.Return(this);
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

        transform.position = newPosition;
    }
}
