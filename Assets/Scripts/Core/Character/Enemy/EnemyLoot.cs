using System.Collections.Generic;
using UnityEngine;

public class EnemyLoot : MonoBehaviour
{
    [SerializeField] SO_EnemyDropData enemyDropData;

    public void Drop()
    {
        List<PoolableData> pickupDatas = enemyDropData.GenerateDrops();
        List<BasePickup> pickups = new();

        foreach(PoolableData pickupData in pickupDatas)
        {
            BasePickup result = Game.ObjectPooler.Spawn<BasePickup>(pickupData, transform.position, Quaternion.identity);
            pickups.Add(result);
        }

        foreach (BasePickup pickup in pickups)
        {
            pickup.StartDriftAwayFrom(pickups);
        }
    }
}
