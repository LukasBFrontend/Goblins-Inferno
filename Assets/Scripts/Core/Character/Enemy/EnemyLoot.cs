using System.Collections.Generic;
using UnityEngine;

public class EnemyLoot : MonoBehaviour
{
    [SerializeField] SO_EnemyDropData enemyDropData;

    public void Drop()
    {
        List<PoolableData> pickupDatas = enemyDropData.GenerateDrops();
        List<BasePickup> pickups = new();

        foreach(var pickupData in pickupDatas)
        {
            var result = Game.ObjectPool.Spawn<BasePickup>(pickupData, transform.position, Quaternion.identity);
            pickups.Add(result);
        }

        foreach (var pickup in pickups)
        {
            pickup.StartDriftAwayFrom(pickups);
        }
    }
}
