using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ArenaTrigger : MonoBehaviour
{
    [Header("References")]
    public GameObject bossPrefab;
    public Transform bossSpawnPoint;
    public GameObject gunPrefab;
    public Transform playerHand;
    public AudioSource arenaMusic;
    public AudioSource ambientMusic;

    private bool activated = false;

    private void OnTriggerEnter(Collider other)
    {
        if (activated) return;
        if (!other.CompareTag("Player")) return;

        activated = true;
        Debug.Log("[ArenaTrigger] Player entered arena trigger!");

        // 🎵 Switch music
        if (ambientMusic != null && ambientMusic.isPlaying)
            ambientMusic.Stop();
        if (arenaMusic != null)
            arenaMusic.Play();

        // 🔫 Equip player gun
        EquipPlayerGun(other);

        // 👹 Spawn boss
        SpawnBoss(other);

        // Disable the trigger to prevent reactivation
        gameObject.SetActive(false);
    }

    private void EquipPlayerGun(Collider player)
    {
        Transform hand = playerHand != null ? playerHand : player.transform;
        if (gunPrefab == null || hand == null)
        {
            Debug.LogWarning("[ArenaTrigger] Missing gunPrefab or playerHand reference!");
            return;
        }

        // Prevent duplicates (if player already has a gun)
        if (hand.Find(gunPrefab.name) != null)
        {
            Debug.Log("[ArenaTrigger] Player already has gun equipped!");
            return;
        }

        GameObject gun = Instantiate(gunPrefab, hand.position, hand.rotation, hand);
        gun.name = gunPrefab.name;
        Debug.Log("[ArenaTrigger] Gun equipped on player!");
    }

    private void SpawnBoss(Collider player)
    {
        if (bossPrefab == null || bossSpawnPoint == null)
        {
            Debug.LogError("[ArenaTrigger] Missing Boss Prefab or Spawn Point reference!");
            return;
        }

        Debug.Log("[ArenaTrigger] Spawning boss...");
        GameObject spawnedBoss = Instantiate(bossPrefab, bossSpawnPoint.position, bossSpawnPoint.rotation);
        spawnedBoss.name = "SpawnedBoss";

        // ✅ Link player to boss
        BossController bossCtrl = spawnedBoss.GetComponent<BossController>();
        if (bossCtrl != null)
        {
            bossCtrl.player = player.transform;
            Debug.Log("[ArenaTrigger] Boss linked to player!");
        }
        else
        {
            Debug.LogWarning("[ArenaTrigger] BossController not found on spawned prefab!");
        }

        // ✅ NavMesh snap fix
        NavMeshAgent agent = spawnedBoss.GetComponent<NavMeshAgent>();
        if (agent != null && !agent.isOnNavMesh)
        {
            if (NavMesh.SamplePosition(spawnedBoss.transform.position, out NavMeshHit hit, 5f, NavMesh.AllAreas))
            {
                spawnedBoss.transform.position = hit.position;
                agent.Warp(hit.position);
                Debug.Log("[ArenaTrigger] Boss snapped to NavMesh!");
            }
        }

        Debug.Log("[ArenaTrigger] Boss instantiated successfully!");
    }
}
