using System.Collections;
using UnityEngine;

/// <summary>
/// Watches an NPC and spawns a replacement prefab once the watched NPC disappears.
/// </summary>
public class NPCReplacementSpawner : MonoBehaviour
{
    [Header("Watched NPC")]
    [SerializeField] private GameObject npcToWatch;

    [Header("Replacement")]
    [SerializeField] private GameObject replacementPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Vector3 spawnOffset = Vector3.zero;
    [SerializeField] private float spawnDelay = 0.5f;
    [SerializeField] private bool spawnOnlyOnce = true;

    private bool spawnRequested = false;
    private Coroutine spawnRoutine;

    void Update()
    {
        if (spawnOnlyOnce && spawnRequested)
            return;

        bool npcGone = npcToWatch == null || !npcToWatch.activeInHierarchy;
        if (npcGone && spawnRoutine == null)
        {
            spawnRequested = true;
            spawnRoutine = StartCoroutine(SpawnReplacementRoutine());
        }
    }

    private IEnumerator SpawnReplacementRoutine()
    {
        if (spawnDelay > 0f)
            yield return new WaitForSeconds(spawnDelay);

        SpawnReplacement();
        spawnRoutine = null;
    }

    private void SpawnReplacement()
    {
        if (replacementPrefab == null)
        {
            Debug.LogWarning($"{nameof(NPCReplacementSpawner)}: Replacement prefab is not assigned.", this);
            return;
        }

        Vector3 position = spawnPoint != null ? spawnPoint.position : transform.position;
        if (npcToWatch != null && spawnPoint == null)
            position = npcToWatch.transform.position;

        position += spawnOffset;

        Quaternion rotation = spawnPoint != null ? spawnPoint.rotation : Quaternion.identity;
        Instantiate(replacementPrefab, position, rotation);
    }
}
