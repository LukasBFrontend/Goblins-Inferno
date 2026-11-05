
using System.Collections;
using UnityEngine;

public sealed class ArrowProjectile : BaseProjectile
{
    public override IEnumerator ProjectileRoutine(BaseCharacter target)
    {
        if (TargetTracker)
        {
            TargetTracker.gameObject.SetActive(false);
        }

        float elapsed = 0f;

        Vector3 direction = PointToPosition(target != null
            ? target.ColliderCenter
            : RandomMaxRange(transform, Data.newTargetRange)
        );

        while (elapsed < Data.lifeTime)
        {
            if (CharactersHit.Count >= 1)
            {
                break;
            }

            float step = Data.initialVelocity * Time.deltaTime;
            transform.position += direction * step;

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
}
