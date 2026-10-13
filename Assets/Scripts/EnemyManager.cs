using System.Collections;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] SO_EnemyPoolData enemyPoolData;
    [SerializeField] float spawnDistance = 10f;
    [SerializeField] float enemySpawnInterval = 10f;

    void Start()
    {
        StartCoroutine(EnemySpawnRoutine());
    }

    IEnumerator EnemySpawnRoutine()
    {
        while (true)
        {
            SpawnEnemies();
            yield return new WaitForSeconds(enemySpawnInterval);
        }
    }

    public void SpawnEnemies()
    {
        foreach (var enemyPoolData in enemyPoolData.list)
        {
            Range range = enemyPoolData.enemiesToSpawnRange;
            int spawnAmount = Random.Range(range.lower - 1, range.upper);


            for (int i = 0; i < spawnAmount; i++)
            {
                Vector3 spawnPoint = Game.Player.transform.position + Utils.RandomDirection() * spawnDistance;

                Game.ObjectPool.Spawn<Enemy>(enemyPoolData, spawnPoint, Quaternion.identity);
            }
        }
    }
}
