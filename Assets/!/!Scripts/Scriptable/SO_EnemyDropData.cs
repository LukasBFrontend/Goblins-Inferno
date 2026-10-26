using System.Collections.Generic;
using UnityEngine;
using Luscious;

[CreateAssetMenu(fileName = "DropData", menuName = "Drops/EnemyDropData")]
public class SO_EnemyDropData : ScriptableObject
{
    public List<DropPoolData> dropPoolDatas;

    public List<PoolableData> GenerateDrops()
    {
        List<PoolableData> drops = new();

        foreach (DropPoolData dropPool in dropPoolDatas)
        {
            float random = Random.Range(0, 100);
            int min = dropPool.itemsToTakeRange.lower - 1;
            int max = dropPool.itemsToTakeRange.upper;

            if (random < dropPool.dropChance)
            {
                int itemsToTake = Mathf.RoundToInt(Random.Range(min, max));
                drops.AddRange(dropPool.itemPool.GetItemsFromPool(itemsToTake));
            }
        }

        return drops;
    }
}

[System.Serializable]
public struct DropPoolData
{
    public SO_ItemDropPool itemPool;
    [Range(0, 100)]
    public float dropChance;
    public Misc.Range itemsToTakeRange;
}
