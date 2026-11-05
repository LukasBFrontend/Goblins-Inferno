
using System.Collections;
using UnityEngine;

public sealed class BoomerangProjectile : BaseProjectile
{
    public override IEnumerator ProjectileRoutine(BaseCharacter initialTarget)
    {
        TargetTracker.Initialize(initialTarget);
        float threshold = Data.targetReachedThreshold;

        while (!TargetTracker.IsTargetPositionReached(threshold))
        {
            MoveTowardsTarget();
            yield return null;
        }

        // Return to sender.
        Vector3 fallbackPosition = Sender.ColliderCenter;
        TargetTracker.SetNewTarget(Sender, fallbackPosition);

        while (!TargetTracker.IsTargetPositionReached(threshold))
        {
            MoveTowardsTarget();
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

    private void MoveTowardsTarget()
    {
        Vector3 targetPosition = TargetTracker.TargetPosition;
        PointToPosition(targetPosition);

        float step = Data.initialVelocity * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);
    }
}
