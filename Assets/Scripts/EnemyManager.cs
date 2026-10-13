using System.Collections;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] SO_EnemyPoolData enemyPoolData;
    [SerializeField] float enemySpawnInterval = 20f;

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
            Game.ObjectPool.Spawn<Enemy>(enemyPoolData, new (Random.Range(-10f, 10f), 0 , Random.Range(-10f, 10f)), Quaternion.identity);
        }
    }
}
