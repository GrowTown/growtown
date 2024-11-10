using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileInfo : MonoBehaviour
{
    public Transform[] spawnPoints; 
    private bool seedsSpawned = false; 
    private bool plantSpawned = false; 


    public void OnPlayerEnter()
    {
        if (!seedsSpawned)
        {
            SpawnSeeds();
            seedsSpawned = true;
           
        }
        
    }

    private void SpawnSeeds()
    {
        // Spawn a seed at each spawn point
        foreach (Transform spawnPoint in spawnPoints)
        {
            Instantiate(UI_Manager.Instance.seed, spawnPoint.position, Quaternion.identity);
        }
    }

    internal void SpawnPlant()
    {
        if(!plantSpawned)
        {
            foreach (Transform spawnPoint in spawnPoints)
            {
                var instance=Instantiate(UI_Manager.Instance.plantHolder,spawnPoint.transform.position, Quaternion.identity);
                //instance.transform.SetParent(UI_Manager.Instance.Canvas.transform);

            }
            plantSpawned = true;
        }
        
    }
}


