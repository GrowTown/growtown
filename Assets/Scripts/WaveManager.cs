using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WaveManager : MonoBehaviour
{

    public static WaveManager instance;

    [Header("Spawn Settings")]
    public GameObject enemyPrefab; // Enemy to spawn
    public Transform spawnArea; // The area where enemies spawn
    public float spawnRadius = 10f; // Area radius for spawning
    public List<Transform> restrictedAreas; // List of restricted spawn areas
    public float restrictedRadius = 2f; // Radius to avoid around restricted areas

    [Header("Wave Settings")]
    public float timeBetweenWaves = 5f; // Delay between waves
    public float spawnInterval = 1f; // Delay between enemy spawns
    public List<int> waveEnemyCounts; // List defining enemies per wave

    private int currentWave = 0;
    private bool spawningWave = false;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        if (waveEnemyCounts == null || waveEnemyCounts.Count == 0)
        {
            Debug.LogError("Wave enemy counts not set!");
            return;
        }
    }

    public void StartEnemyWave()
    {
        if (spawningWave) return; // Prevent multiple coroutines from starting
        currentWave = 0; // Reset wave count if needed
        StartCoroutine(WaveLoop());
        Debug.Log("Wave Call Time ::: ");
    }

    IEnumerator WaveLoop()
    {
        currentWave = 0; // Reset wave count before starting new waves

        while (currentWave < waveEnemyCounts.Count)
        {
            yield return new WaitForSeconds(timeBetweenWaves);
            spawningWave = true;

            int enemiesToSpawn = waveEnemyCounts[currentWave];
            yield return StartCoroutine(SpawnEnemies(enemiesToSpawn));

            spawningWave = false;
            currentWave++;
        }
    }
    IEnumerator SpawnEnemies(int count)
    {
        float randomDelay = Random.Range(1f, 30f);

        yield return new WaitForSeconds(randomDelay);

        for (int i = 0; i < count; i++)
        {
            Vector3 spawnPosition = GetValidSpawnPoint();
          var enmey=  Instantiate(enemyPrefab, spawnPosition, Quaternion.identity);
            enmey.transform.GetComponent<Enemy>().Initialize(restrictedAreas[0].transform, i);
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    Vector3 GetValidSpawnPoint()
    {
        Vector3 spawnPosition;
        bool isValid;
        int attempts = 0;
        do
        {
            spawnPosition = GetRandomSpawnPoint();
            isValid = true;

            foreach (Transform restricted in restrictedAreas)
            {
                if (Vector3.Distance(spawnPosition, restricted.position) < restrictedRadius)
                {
                    isValid = false;
                    break;
                }
            }
            attempts++;
        }
        while (!isValid && attempts < 10); // Prevent infinite loops

        return spawnPosition;
    }

    Vector3 GetRandomSpawnPoint()
    {
        Vector3 randomOffset = new Vector3(
            Random.Range(-spawnRadius, spawnRadius),
            0,
            Random.Range(-spawnRadius, spawnRadius)
        );
        return spawnArea.position + randomOffset;
    }
}
