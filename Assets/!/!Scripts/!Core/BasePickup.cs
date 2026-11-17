using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePickup : MonoBehaviour, IPoolable
{
    [SerializeField] PickupType type;
    [SerializeField] int value = 10;
    [Header("Movement")]
    [SerializeField] float pickupRange = 0.15f;
    [SerializeField] bool gravitateTowardsPlayer;
    [SerializeField] float driftRange = 0.5f;
    [SerializeField] float driftSpeed = 1f;

    private Coroutine _driftAwayCurrent;
    private Coroutine _driftTowardsCurrent;

    public string PoolableId { get; set; }

    void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent(out Player player))
        {
            return;
        }

        if (gravitateTowardsPlayer)
        {
            if (_driftAwayCurrent != null)
            {
                StopCoroutine(_driftAwayCurrent);
            }

            _driftTowardsCurrent = StartCoroutine(DriftTowards(player));
        }
        else
        {
            RegisterPickUp(player);
        }

    }

    void OnTriggerExit(Collider other)
    {
        if (!other.TryGetComponent(out Player player) || _driftTowardsCurrent == null)
        {
            return;
        }

        StopCoroutine(_driftTowardsCurrent);
    }

    public void StartDriftAwayFrom(List<BasePickup> pickups)
    {
        if (pickups.Count == 0 || _driftTowardsCurrent != null)
        {
            return;
        }

        _driftAwayCurrent = StartCoroutine(DriftAwayRoutine(pickups));
    }

    private void RegisterPickUp(Player player)
    {
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

        var pickupSound = Game.SoundFX.Default.pickupSound;
        Game.SoundFX.PlayRandomSoundFXClip(pickupSound, transform.position);


        Game.ObjectPooler.Return(this);        
    }

    private IEnumerator DriftTowards(Player player)
    {
        Vector3 targetOffset = player.transform.position - transform.position;
        float distanceSquared = targetOffset.sqrMagnitude;

        while (distanceSquared > Mathf.Pow(pickupRange, 2))
        {
            targetOffset = player.transform.position - transform.position;
            distanceSquared = targetOffset.sqrMagnitude;

            float t = 3 * Mathf.Cos(Mathf.Sqrt(distanceSquared)) + 0.5f;
            float speed = Mathf.Lerp(0.5f * driftSpeed, 2 * driftSpeed, t);
            
            transform.position = Vector3.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);

            yield return null;
        }

        RegisterPickUp(player);
    }

    IEnumerator DriftAwayRoutine(List<BasePickup> pickups)
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
