
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

        float elapsed = 0f;

        while (!TargetTracker.IsTargetPositionReached(threshold))
        {
            // Accelerate projectile speed based off the player velocity parallel to  the projectile velocity.
            Vector3 senderVelocity = Sender.Rigidbody.linearVelocity;
            float scalarProjection = Vector3.Dot(senderVelocity, lastVelocity.normalized); // How fast is the sender moving in the direction of the projectile

            float lastSpeed = lastVelocity.magnitude;
            float speedDifference = lastSpeed - scalarProjection; // How much faster / slower is the projectile moving compared to sender.        

            float adjustedSpeed = speedDifference < 0 
                ? Data.initialVelocity + speedDifference * 2 
                : Data.initialVelocity
            ;

            if (speedDifference > 0)
            {

                // 1. Calculate the amount of time the return arc should take, should the sender not move

            }

            float referenceTime = Data.newTargetRange / Data.initialVelocity;
            float estimatedArrival = elapsed + (TargetTracker.TargetPosition - transform.position).magnitude / speedDifference;

            if (elapsed > referenceTime) // increase speed if after schedule
            {
                adjustedSpeed = Mathf.Lerp(Data.initialVelocity, Mathf.Max(scalarProjection, Data.initialVelocity) * 2f, (elapsed - referenceTime) / 4);
            }
            else if (estimatedArrival < referenceTime)
            {
                adjustedSpeed = Mathf.Lerp(Data.initialVelocity, 0f, referenceTime - estimatedArrival);
            }

            MoveTowardsTarget(adjustedSpeed, out Vector3 velocity);
            lastVelocity = velocity;

            elapsed += Time.deltaTime;
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
