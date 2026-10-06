using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Range
{
    public int lower;
    public int upper;
}

[System.Serializable]
public struct DropPoolData
{
    public SO_ItemDropPool itemPool;
    [Range(0, 100)]
    public float dropChance;
    public Range itemsToTakeRange;
}

[CreateAssetMenu(fileName = "DropData", menuName = "Drops/EnemyDropData")]
public class SO_EnemyDropData : ScriptableObject
{
    public List<DropPoolData> dropPoolDatas;

    public List<GameObject> GenerateDrops()
    {
        List<GameObject> drops = new();

        foreach (var dropPool in dropPoolDatas)
        {
            float random = Random.Range(0, 100);
            int min = dropPool.itemsToTakeRange.lower;
            int max = dropPool.itemsToTakeRange.upper + 1;

            if (random < dropPool.dropChance)
            {
                int itemsToTake = Mathf.RoundToInt(Random.Range(min, max));
                drops.AddRange(dropPool.itemPool.GetItemsFromPool(itemsToTake));
            }
        }

        return drops;
    }
}
