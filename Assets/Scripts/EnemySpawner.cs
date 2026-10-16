using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] SO_EnemyWaveConfig data;
    float elapsed = 0f;
    int waveIndex = 0;
    

    public void Run()
    {
        elapsed += Time.deltaTime;

        if (elapsed < waveIndex * data.waveInterval)
        {
            return;
        }

        SpawnEnemies(elapsed);
        waveIndex++;
    }

    public int CalculateSpawnAmount(float elapsedTime)
    {
        float elapsedNormalized = elapsedTime / 60f;
        int waveSizeIncrease = (int)(elapsedNormalized * data.waveSizePerMinute);

        return data.baseWaveSize + waveSizeIncrease;
    }

    public void SpawnEnemies(float elapsedTime)
    {
        List<int> spawnBiases = new();
        float totalBias = 0;

        for (int i = 0; i < data.enemySpawnData.Count; i++)
        {
            var spawnData = data.enemySpawnData[i];
            int biasLower = spawnData.spawnBias.lower;
            int biasUpper = spawnData.spawnBias.upper;

            int biasActual = Random.Range(biasLower, biasUpper);
            totalBias += biasActual;

            spawnBiases.Add(biasActual);
        }

        int totalSpawnAmount = CalculateSpawnAmount(elapsedTime);

        for (int i = 0; i < data.enemySpawnData.Count; i++)
        {
            var spawnData = data.enemySpawnData[i];
            float wavePercent = spawnBiases[i] / totalBias;
            int spawnAmount = (int)(wavePercent * totalSpawnAmount);

            for (int j = 0; j < spawnAmount; j++)
            {
                Vector3 spawnPoint = Game.Player.transform.position + Utils.RandomDirection() * data.spawnDistance;

                Game.ObjectPool.Spawn<Enemy>(spawnData.poolableData, spawnPoint, Quaternion.identity);
            }
        }
    }
}
