using System.Collections.Generic;
using UnityEngine;
using Luscious;

[CreateAssetMenu(fileName = "EnemyWaveConfig", menuName = "Enemy/WaveConfig")]
public class SO_EnemyWaveConfig : ScriptableObject
{
    public float initialWait = 10f;
    public float spawnDistance = 15f;
    [Header("Size")]
    public int baseWaveSize = 3;
    public int waveSizePerMinute = 3;
    public int maxWaveSize = 10;
    [Header("Timing")]
    public float waveInterval = 10f;
    public List<EnemySpawnData> enemySpawnData;
}

[System.Serializable]
public struct EnemySpawnData
{
    public PoolableData poolableData;
    public Misc.Range spawnBias;
}
