using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Projectile : MonoBehaviour
{
    [SerializeField] SO_ProjectileData projectileData;
    [SerializeField] bool logOnTargetHit;
    BaseCharacter _target;

    public void SetTarget(BaseCharacter target)
    {
        _target = target;

        switch (projectileData.arcMode)
        {
            case ProjectileArcMode.None:
                StartCoroutine(MoveRoutine(target));
                break;
            case ProjectileArcMode.Track:
                StartCoroutine(TrackRoutine(target));
                break;
            case ProjectileArcMode.Boomerang:
                StartCoroutine(BoomerangRoutine(target));
                break;
            case ProjectileArcMode.Bounce:
                StartCoroutine(BounceRoutine(target));
                break;
        }
    }

    IEnumerator MoveRoutine(BaseCharacter target)
    {
        Vector3 direction = PointToTarget(target);
        float elapsed = 0f;


        while (elapsed < projectileData.lifeTime)
        {
            float step = projectileData.initialVelocity * Time.deltaTime;
            transform.position += direction * step;

            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }

    IEnumerator TrackRoutine(BaseCharacter target)
    {
        while (Vector3.Distance(target.ColliderCenter, transform.position) > projectileData.targetReachedThreshold)
        {
            PointToTarget(target);

            float step = projectileData.initialVelocity * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, target.ColliderCenter, step);
            yield return null;
        }
    }
    IEnumerator BoomerangRoutine(BaseCharacter initialTarget)
    {
        throw new NotImplementedException();
    }

    IEnumerator BounceRoutine(BaseCharacter target)
    {
        throw new NotImplementedException();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent<BaseCharacter>(out var character) || character != _target)
        {
            return;
        }

        if (logOnTargetHit)
        {
            Debug.Log($"Projectile <color=white>{name}</color> found its target {_target.name}");
        }
    }

    Vector3 PointToTarget(BaseCharacter target)
    {
        Vector3 direction = (target.ColliderCenter - transform.position).normalized;
        direction.y = 0;

        transform.rotation = Quaternion.Euler(0, Mathf.Rad2Deg * Mathf.Atan2(direction.x, direction.z), 0);

        return direction;
    }
}
