using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
     public EnemyPool enemyPool;
    public Transform[] spawnPoints;
    public Transform[] fieldAreas; // Array of field areas for enemy targets
    public int waveSize = 5;
    public float spawnInterval = 1f;

    private int currentWave = 1;

    public void StartWave()
    {
        StartCoroutine(SpawnWave());
    }

    private IEnumerator SpawnWave()
    {
        for (int i = 0; i < waveSize; i++)
        {
            GameObject enemyObject = enemyPool.GetEnemy();
            var Index = Random.Range(0, spawnPoints.Length);

            enemyObject.transform.position = spawnPoints[i].position;
            enemyObject.transform.rotation = spawnPoints[i].rotation;


            Enemy enemy = enemyObject.GetComponent<Enemy>();

            // Choose a random field area for each enemy
            Transform targetFieldArea = fieldAreas[i];
            enemy.Initialize(targetFieldArea);

            yield return new WaitForSeconds(spawnInterval);
        }

        //currentWave++;
    }
}


