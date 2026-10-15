using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PickupType
{
    None,
    Exp,
    Health,
}

public class BasePickup : MonoBehaviour
{
    [SerializeField] PickupType type;
    [SerializeField] int value = 10;
    [Header("Movement")]
    [SerializeField] float driftRange = 0.5f;
    [SerializeField] float driftSpeed = 1f;

    void OnTriggerEnter(Collider other)
    {
        if (!other.TryGetComponent<Player>(out var player))
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
                Debug.Log("Health restored");
                break;
            default:
                break;
        }

        Destroy(gameObject);
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
}
