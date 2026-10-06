using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct PooledItemDropData
{
    public GameObject prefab;
    public int poolQuantity;
}

[CreateAssetMenu(fileName = "ItemDropPool", menuName = "Drops/ItemDropPool")]
public class SO_ItemDropPool : ScriptableObject
{
    public List<PooledItemDropData> itemDrops;

    public List<GameObject> GetItemsFromPool(int amount)
    {
        int totalQuantity = 0;
        List<GameObject> drops = new();
        List<GameObject> itemDropsFlat = new();

        foreach(var itemDrop in itemDrops)
        {
            totalQuantity += itemDrop.poolQuantity;

            for (int i = 0; i < itemDrop.poolQuantity; i++)
            {
                itemDropsFlat.Add(itemDrop.prefab);
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
            GameObject itemDrop = itemDropsFlat[itemDropIndex];

            itemDropsFlat.RemoveAt(itemDropIndex);
            drops.Add(itemDrop);
        }

        return drops;
    }
}
