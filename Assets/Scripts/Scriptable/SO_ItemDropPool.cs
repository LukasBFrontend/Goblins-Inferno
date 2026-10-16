using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct PooledItemDropData
{
    public PoolableData data;
    public int poolQuantity;
}

[CreateAssetMenu(fileName = "ItemDropPool", menuName = "Drops/ItemDropPool")]
public class SO_ItemDropPool : ScriptableObject
{
    public List<PooledItemDropData> itemDrops;

    public List<PoolableData> GetItemsFromPool(int amount)
    {
        int totalQuantity = 0;
        List<PoolableData> drops = new();
        List<PoolableData> itemDropsFlat = new();

        foreach(var itemDrop in itemDrops)
        {
            totalQuantity += itemDrop.poolQuantity;

            for (int i = 0; i < itemDrop.poolQuantity; i++)
            {
                itemDropsFlat.Add(itemDrop.data);
            }
        }

        if (amount > totalQuantity)
        {
            Debug.LogError($"<color=white>{nameof(GetItemsFromPool)}</color> returned empty list: tried to take <color=yellow>{amount}</color> items from item drop pool containing a total of <color=yellow>{totalQuantity}</color> items");

            return drops;
        }

        for (int i = 0; i <= amount; i++)
        {
            int itemDropIndex = Random.Range(0, itemDropsFlat.Count);
            var itemDrop = itemDropsFlat[itemDropIndex];

            itemDropsFlat.RemoveAt(itemDropIndex);
            drops.Add(itemDrop);
        }

        return drops;
    }
}
