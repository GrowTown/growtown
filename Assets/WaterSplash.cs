using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterSplash : MonoBehaviour
{

    private ParticleSystem waterParticles; // Assign the water particle system in the Inspector
    public GameObject splashEffectPrefab; // Assign the splash effect prefab in the Inspector

    private Dictionary<Vector3, GameObject> activeSplashes = new Dictionary<Vector3, GameObject>();
    private float positionTolerance = 0.1f; // Tolerance to prevent multiple spawns near the same location
    private float splashLifetime = 2f;     // Time to keep splash active without collision

    void Start()
    {
        if (waterParticles == null)
        {
            waterParticles = GetComponent<ParticleSystem>();
        }
    }

    void OnParticleCollision(GameObject other)
    {
        // Get collision points
        ParticleCollisionEvent[] collisionEvents = new ParticleCollisionEvent[16];
        int numCollisionEvents = waterParticles.GetCollisionEvents(other, collisionEvents);

        HashSet<Vector3> currentCollisionPositions = new HashSet<Vector3>();

        for (int i = 0; i < numCollisionEvents; i++)
        {
            Vector3 collisionPosition = collisionEvents[i].intersection;
            Quaternion collisionRotation = Quaternion.LookRotation(collisionEvents[i].normal);

            currentCollisionPositions.Add(collisionPosition);

            // Check if a splash already exists nearby
            if (!HasNearbySplash(collisionPosition))
            {
                // Instantiate the splash effect
                GameObject splash = Instantiate(splashEffectPrefab, collisionPosition, collisionRotation);

                // Store the splash effect
                activeSplashes[collisionPosition] = splash;
            }
        }

        // Cleanup splashes no longer colliding
        CleanupInactiveSplashes(currentCollisionPositions);
    }

    private bool HasNearbySplash(Vector3 position)
    {
        foreach (var splashPosition in activeSplashes.Keys)
        {
            if (Vector3.Distance(splashPosition, position) <= positionTolerance)
            {
                return true; // Found a nearby splash
            }
        }
        return false; // No nearby splash found
    }

    private void CleanupInactiveSplashes(HashSet<Vector3> currentCollisionPositions)
    {
        // Get all splash positions that are no longer colliding
        List<Vector3> positionsToRemove = new List<Vector3>();

        foreach (var splashPosition in activeSplashes.Keys)
        {
            if (!IsPositionColliding(splashPosition, currentCollisionPositions))
            {
                // Mark the splash for removal
                positionsToRemove.Add(splashPosition);
            }
        }

        // Destroy and remove inactive splashes
        foreach (var position in positionsToRemove)
        {
            Destroy(activeSplashes[position]);
            activeSplashes.Remove(position);
        }
    }

    private bool IsPositionColliding(Vector3 position, HashSet<Vector3> currentCollisionPositions)
    {
        foreach (var collisionPosition in currentCollisionPositions)
        {
            if (Vector3.Distance(position, collisionPosition) <= positionTolerance)
            {
                return true; // Position is still colliding
            }
        }
        return false; // No collision at this position
    }
}





