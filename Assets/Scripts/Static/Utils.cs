
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Utils
{
    /// <summary>
    /// Calculates the unit direction from the closest enemy to the player by comparing between the player and the list of enemies.
    /// </summary>
    public static Enemy ClosestEnemy(Player player)
    {
        Vector3 playerPosition = player.transform.position;
        List<Enemy> enemies = Object.FindObjectsByType<Enemy>(FindObjectsInactive.Exclude).ToList();

        if (enemies.Count == 0)
        {
            return null;
        }

        enemies.Sort((enemy, prevEnemy) =>
        {
            return Vector3.Distance
            (
                enemy.Rigidbody.position, playerPosition) < Vector3.Distance(prevEnemy.Rigidbody.position, playerPosition
            )
            ? -1
            : 1;
        });

        Enemy closestEnemy = enemies[0];

        return closestEnemy;
    }

    public static Vector3 RandomDirection()
    {
        float randomAngle = Random.Range(0f, 2 * Mathf.PI);
        return new Vector3(Mathf.Cos(randomAngle), 0, Mathf.Sin(randomAngle)).normalized;
    }
}
