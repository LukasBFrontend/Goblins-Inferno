using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyWaveConfig", menuName = "Enemy/WaveConfig")]
public class SO_EnemyWaveConfig : ScriptableObject
{
    public float spawnDistance = 15f;
    [Header("Size")]
    public int baseWaveSize = 3;
    public int waveSizePerMinute = 3;
    [Header("Timing")]
    public float waveInterval = 10f;
    public List<EnemySpawnData> enemySpawnData;
}

[System.Serializable]
public struct PoolableData
{
    public string id;
    public GameObject prefab;
}

[System.Serializable]
public struct EnemySpawnData
{
    public PoolableData poolableData;
    public Range spawnBias;
}
