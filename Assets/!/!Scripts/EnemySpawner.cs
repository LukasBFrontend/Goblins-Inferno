using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : Singleton<EnemySpawner>
{
    [Space()]
    [SerializeField] SO_EnemyWaveConfig data;

    float _elapsed = 0f;
    int _waveIndex = 0;

    void Start()
    {
        _elapsed = -data.initialWait;
    }

    public void Run()
    {
        _elapsed += Time.deltaTime;

        if (_elapsed < _waveIndex * data.waveInterval)
        {
            return;
        }

        SpawnEnemies(_elapsed);
        _waveIndex++;
    }

    public int CalculateSpawnAmount(float elapsedTime)
    {
        float elapsedNormalized = elapsedTime / 60f;
        int waveSizeIncrease = (int)(elapsedNormalized * data.waveSizePerMinute);

        return Mathf.Clamp(data.baseWaveSize + waveSizeIncrease, 1, data.maxWaveSize);
    }

    public void SpawnEnemies(float elapsedTime)
    {
        List<int> spawnBiases = new();
        float totalBias = 0;

        for (int i = 0; i < data.enemySpawnData.Count; i++)
        {
            EnemySpawnData spawnData = data.enemySpawnData[i];
            int biasLower = spawnData.spawnBias.lower;
            int biasUpper = spawnData.spawnBias.upper;

            int biasActual = Random.Range(biasLower, biasUpper);
            totalBias += biasActual;

            spawnBiases.Add(biasActual);
        }

        int totalSpawnAmount = CalculateSpawnAmount(elapsedTime);

        for (int i = 0; i < data.enemySpawnData.Count; i++)
        {
            EnemySpawnData spawnData = data.enemySpawnData[i];
            float wavePercent = spawnBiases[i] / totalBias;
            int spawnAmount = (int)(wavePercent * totalSpawnAmount);

            for (int j = 0; j < spawnAmount; j++)
            {
                Vector3 spawnPoint = Game.Player.transform.position + Utils.RandomDirection() * data.spawnDistance;

                Enemy enemy = Game.ObjectPooler.Spawn<Enemy>(spawnData.poolableData, spawnPoint, Quaternion.identity);
                enemy.Movement.Enable();
            }
        }
    }
}
