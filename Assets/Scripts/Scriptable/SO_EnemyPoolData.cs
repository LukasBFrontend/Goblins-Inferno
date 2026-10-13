using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyPoolData", menuName = "Enemy/PoolData")]
public class SO_EnemyPoolData : ScriptableObject
{
    public List<PoolableData> list;
}

[System.Serializable]
public struct PoolableData
{
    public string id;
    public GameObject prefab;
    public int poolSize;
}
