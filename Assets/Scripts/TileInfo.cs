using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileInfo : MonoBehaviour
{
    public Transform[] spawnPoints; 
    internal bool seedsSpawned = false; 
    internal bool plantSpawned = false;
    internal bool plantgrowth = false;
    internal bool isCuttingStarted = false;


    public void OnPlayerEnter()
    {
        if (!seedsSpawned)
        {
            if (!GameManager.Instance.HasEnoughPoints(5, 0)) return;
           SpawnSeeds();
            seedsSpawned = true;
            GameManager.Instance.DeductEnergyPoints(5);


        }
        
    }

    private void SpawnSeeds()
    {
        // Spawn a seed at each spawn point
        foreach (Transform spawnPoint in spawnPoints)
        {
            var instace=Instantiate(UI_Manager.Instance.seed, spawnPoint.position, Quaternion.identity);
            UI_Manager.Instance.spawnedSeed.Add(instace);
        }
    }

    /* internal void SpawnPlant(GameObject tilego)
     {
         if(!plantSpawned)
         {
             foreach (Transform spawnPoint in spawnPoints)
             {
                 var instance = Instantiate(UI_Manager.Instance.plantHolder, spawnPoint.position, Quaternion.identity);
                 UI_Manager.Instance.spawnPlantsForGrowth.Add(tilego,instance);
             }
             plantSpawned = true;
         }

     }*/

    internal void SpawnPlant(GameObject tilego)
    {
        if (!plantSpawned)
        {
            
            if (!UI_Manager.Instance.spawnPlantsForGrowth.ContainsKey(tilego))
            {
                UI_Manager.Instance.spawnPlantsForGrowth[tilego] = new List<GameObject>();
            }

        
            foreach (Transform spawnPoint in spawnPoints)
            {
                var instance = Instantiate(UI_Manager.Instance.plantHolder, spawnPoint.position, Quaternion.identity);
                UI_Manager.Instance.spawnPlantsForGrowth[tilego].Add(instance); 
                UI_Manager.Instance.spawnPlantsForInitialGrowth.Add(instance);
                UI_Manager.Instance.GrownPlantsToCut.Add(instance);

            }

            plantSpawned = true;
        }
    }

  

}


