using System.Collections.Generic;
using UnityEngine;

public class EnemyLoot : MonoBehaviour
{
    [SerializeField] SO_EnemyDropData enemyDropData;

    public void Drop()
    {
        List<GameObject> pickupPrefabs = enemyDropData.GenerateDrops();
        List<BasePickup> pickups = new();

        foreach(var pickupPrefab in pickupPrefabs)
        {
            GameObject instance = Instantiate(pickupPrefab, transform.position, Quaternion.identity);
            pickups.Add(instance.GetComponent<BasePickup>());
        }

        foreach (var pickup in pickups)
        {
            pickup.StartDriftAwayFrom(pickups);
        }
    }
}
