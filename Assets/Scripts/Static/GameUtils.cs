
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class GameUtils
{
    /// <summary>
    /// Calculates the unit direction from the closest enemy to the player by comparing between the player and the list of enemies.
    /// </summary>
    public static Vector3 ClosestEnemyToPlayerDir(Player player)
    {
        if (player == null)
        {
            return Vector3.zero;
        }

        List<Enemy> enemies = Object.FindObjectsByType<Enemy>(FindObjectsInactive.Exclude).ToList();
        Vector3 playerPosition = player.transform.position;

        if (enemies.Count == 0)
        {
            float randomAngle = Random.Range(0f, 2 * Mathf.PI);
            return new Vector3(Mathf.Cos(randomAngle), 0, Mathf.Sin(randomAngle));
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

        return (closestEnemy.transform.position - playerPosition).normalized;
    }

    /// <summary>
    /// Calculates the unit direction from the player to the enemy.
    /// </summary>
    public static Vector2 PlayerToEnemyDir(Player player, Enemy enemy)
    {
        if (player == null)
        {
            return Vector2.zero;
        }

        Vector2 position = enemy.Rigidbody.position;
        Vector2 targetPosition = player.Rigidbody.position;
        return (targetPosition - position).normalized;
    }
}
