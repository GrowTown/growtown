using System.Collections;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance;

    [Header("Wave Settings")]
    public int totalWaves = 5;
    public int[] enemiesToSpawnPerWave; // Each wave has a unique enemy count
    public float waveInterval = 10f;
    public float minSpawnDelay = 0.5f;
    public float maxSpawnDelay = 2f;

    [Header("Boss Settings")]
    public GameObject enemyBossPrefab;
    public Transform[] bossSpawnPoints;

    private int currentWave = 0;
    private EnemyPoolManager enemyPoolManager;
    private GameObject boss;
    private bool isSpawning = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        enemyPoolManager = GetComponent<EnemyPoolManager>();

        // Initialize enemies per wave if not set in Inspector
        if (enemiesToSpawnPerWave == null || enemiesToSpawnPerWave.Length == 0)
        {
            enemiesToSpawnPerWave = new int[totalWaves];
            for (int i = 0; i < totalWaves; i++)
            {
                enemiesToSpawnPerWave[i] = 5 + (i * 2); // Default scaling pattern
            }
        }
    }

    public void StartEnemyWaves()
    {
        StartCoroutine(StartWaves());
    }

    private IEnumerator StartWaves()
    {
        while (currentWave < totalWaves)
        {
            enemyPoolManager.ResetEnemies(); // Reset enemies at the start of each wave

            yield return new WaitForSeconds(waveInterval); // Delay before new wave starts

            int enemiesToSpawn = enemiesToSpawnPerWave[currentWave];
            Debug.Log($"Starting Wave {currentWave + 1}, Spawning {enemiesToSpawn} Enemies");

            yield return StartCoroutine(SpawnEnemies(enemiesToSpawn)); // Spawn wave-specific count

            currentWave++;

            // If last wave, spawn boss after a short delay
            if (currentWave == totalWaves)
            {
                yield return new WaitForSeconds(5f);
                SpawnBoss();
            }
        }
    }

    private IEnumerator SpawnEnemies(int count)
    {
        if (isSpawning) yield break; // Prevent multiple spawns at the same time
        isSpawning = true;

        Transform[] spawnPoints = enemyPoolManager.spawnPoints;
        int spawnedEnemies = 0;

        while (spawnedEnemies < count)
        {
            GameObject enemyObject = enemyPoolManager.GetEnemy();
            if (enemyObject != null)
            {
                Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
                enemyObject.transform.position = spawnPoint.position;
                enemyObject.transform.rotation = spawnPoint.rotation;
                enemyObject.SetActive(true);

                Enemy enemy = enemyObject.GetComponent<Enemy>();
                if (enemy != null)
                {
                    Transform targetFieldArea = enemyPoolManager.enemyAttackFieldAreas[Random.Range(0, enemyPoolManager.enemyAttackFieldAreas.Length)];
                    enemy.Initialize(targetFieldArea, spawnedEnemies);
                }

                spawnedEnemies++;
            }

            yield return new WaitForSeconds(Random.Range(minSpawnDelay, maxSpawnDelay)); // Random delay per enemy
        }

        isSpawning = false;
    }

    private void SpawnBoss()
    {
        if (enemyBossPrefab != null)
        {
            Transform randomSpawnPoint = bossSpawnPoints[Random.Range(0, bossSpawnPoints.Length)];

            if (boss != null)
            {
                boss.transform.position = randomSpawnPoint.position;
                boss.transform.rotation = randomSpawnPoint.rotation;
                boss.SetActive(true);
            }
            else
            {
                boss = Instantiate(enemyBossPrefab, randomSpawnPoint.position, randomSpawnPoint.rotation);
            }

            Enemy bossEnemy = boss.GetComponent<Enemy>();
            if (bossEnemy != null)
            {
                Transform targetFieldArea = enemyPoolManager.enemyAttackFieldAreas[Random.Range(0, enemyPoolManager.enemyAttackFieldAreas.Length)];
                bossEnemy.Initialize(targetFieldArea, 0);
            }

            Debug.Log("Boss Spawned!");
        }
    }
}
