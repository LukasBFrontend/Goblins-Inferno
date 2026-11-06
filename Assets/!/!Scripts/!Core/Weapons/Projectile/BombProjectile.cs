
using System.Collections;
using UnityEngine;

public sealed class BombProjectile: BaseProjectile
{
    private WaitForSeconds _bombWaitForSeconds;

    public override IEnumerator ProjectileRoutine(BaseCharacter target)
    {
        Vector3 direction = PointToPosition(target != null
            ? target.ColliderCenter
            : RandomMaxRange(transform, Data.newTargetRange)
        );
        direction.y = .5f; // Throw at an upwards angle.

        Rigidbody.AddForce(direction * Data.initialVelocity, ForceMode.Impulse);

        yield return _bombWaitForSeconds;

        StartCoroutine(BombExplosionRoutine());
    }

    private void OnEnable()
    {
        _bombWaitForSeconds = new(Data.lifeTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (
            !Data.destroyOnContact
            || (Data.targetsLayerMask & (1 << collision.gameObject.layer)) == 0
            || !gameObject.activeSelf
        )
        {
            return;
        }

        StartCoroutine(BombExplosionRoutine());
    }

    private void OnValidate()
    {
        if (Rigidbody == null)
        {
            Debug.LogWarning($"Bomb projectile is missing particleSystemControls or rigidbody");
        }
    }

    private IEnumerator BombExplosionRoutine()
    {
        if (gameObject == null)
        {
            yield break;
        }

        HitTargetsWithinRange();
        Instantiate(Data.onDestroyVFX, transform.position, Quaternion.identity);

        if (Data.onDestroy.clips.Count > 0)
        {
            Game.SoundFX.PlayRandomSoundFXClip(Data.onDestroy, transform.position);
        }
        Game.ObjectPooler.Return(this);
    }

    private void HitTargetsWithinRange()
    {
        foreach (BaseCharacter character in TargetTracker.InRange(Data.newTargetRange, Data.targetsLayerMask))
        {
            RegisterHit(character);
        }
    }
}
