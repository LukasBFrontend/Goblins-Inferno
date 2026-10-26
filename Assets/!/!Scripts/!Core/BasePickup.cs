using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePickup : MonoBehaviour, IPoolable
{
    [SerializeField] PickupType type;
    [SerializeField] int value = 10;
    [Header("Movement")]
    [SerializeField] float driftRange = 0.5f;
    [SerializeField] float driftSpeed = 1f;

    public string PoolableId { get; set; }

    void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out Player player))
        {
            return;
        }

        switch (type)
        {
            case PickupType.Exp:
                player.GainExp(value);
                break;
            case PickupType.Health:
                player.Health.Restore(value);
                break;
            default:
                break;
        }

        /*
        var pickupSound = Game.SoundFX.Default.pickupSound;
        Game.SoundFX.PlaySoundFXClip(pickupSound.audioClip, transform, pickupSound.volume);
        */

        Game.ObjectPooler.Return(this);
    }

    public void StartDriftAwayFrom(List<BasePickup> pickups)
    {
        if (pickups.Count == 0)
        {
            return;
        }

        StartCoroutine(ItemDriftRoutine(pickups));
    }

    IEnumerator ItemDriftRoutine(List<BasePickup> pickups)
    {
        const float TIMEOUT = 1f;
        float nearestDistanceSquared = 0f;
        float elapsed = 0f;

        Vector3 initialDirection = Random.insideUnitSphere;

        while (nearestDistanceSquared < driftRange * driftRange && elapsed < TIMEOUT)
        {
            BasePickup closest = null;
            nearestDistanceSquared = float.MaxValue;

            if (this == null)
            {
                yield break;
            }

            foreach (BasePickup pickup in pickups)
            {
                if (pickup == this || pickup == null)
                {
                    continue;
                }

                float distanceSquared = (pickup.transform.position - transform.position).sqrMagnitude;

                if (distanceSquared < nearestDistanceSquared)
                {
                    nearestDistanceSquared = distanceSquared;
                    closest = pickup;
                }
            }

            if (closest == null)
            {
                yield break;
            }

            Vector3 delta = nearestDistanceSquared > 0.0005f
                ? transform.position - closest.transform.position
                : initialDirection
            ;

            delta.y = 0;
            Vector3 direction = delta.normalized;

            float rangeSquared = driftRange * driftRange;
            float proximity = 1f - Mathf.Clamp01(nearestDistanceSquared / rangeSquared);

            proximity *= proximity;

            float speed = Mathf.Lerp(0f, driftSpeed, proximity);
            float step = speed * Time.deltaTime;

            transform.position += direction * step;

            elapsed += Time.deltaTime;
            yield return null;
        }
    }

    public void Reset(){ }
}

public enum PickupType
{
    None,
    Exp,
    Health,
}
