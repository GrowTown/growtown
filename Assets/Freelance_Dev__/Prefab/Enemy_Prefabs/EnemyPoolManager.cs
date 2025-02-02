using System.Collections.Generic;
using UnityEngine;

public class EnemyPoolManager : MonoBehaviour
{
    [Header("Enemy Settings")]
    public GameObject enemyPrefab; // Assign your enemy prefab in Inspector
    public int initialEnemyCount = 10; // Number of enemies in pool initially

    [Header("Spawn Settings")]
    public Transform[] spawnPoints; // Assign spawn points in Inspector
    private List<GameObject> enemyPool = new List<GameObject>();

    [Header("Enemy Attack Settings")]
    public Transform[] enemyAttackFieldAreas; // Array of field areas for enemy targets

    void Start()
    {
        InitializePool();
    }

    // Initializes the pool with inactive enemies
    private void InitializePool()
    {
        for (int i = 0; i < initialEnemyCount; i++)
        {
            GameObject enemy = Instantiate(enemyPrefab);
            enemy.SetActive(false);
            enemyPool.Add(enemy);
        }
    }

    // Gets an inactive enemy from the pool
    public GameObject GetEnemy()
    {
        foreach (var enemy in enemyPool)
        {
            if (!enemy.activeInHierarchy)
            {
                return enemy;
            }
        }

        // If all enemies are in use, instantiate a new one and add to pool
        GameObject newEnemy = Instantiate(enemyPrefab);
        newEnemy.SetActive(false);
        enemyPool.Add(newEnemy);
        Debug.Log("EnemyPool Count: " + enemyPool.Count);
        return newEnemy;
    }

    // Resets all enemies (use this after a wave ends)
    public void ResetEnemies()
    {
        foreach (var enemy in enemyPool)
        {
            enemy.SetActive(false);
        }
    }
}
