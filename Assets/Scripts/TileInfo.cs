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
            GameManager.Instance.ForCropSeedDEduction();


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

            // Define unique rotations for the plants
            Quaternion[] rotations = new Quaternion[]
            {
            Quaternion.Euler(0, 0, 0),    // Default rotation
            Quaternion.Euler(0, 45, 0),  // Rotated 90 degrees on the Y axis
            Quaternion.Euler(0, 90, 0), // Rotated 180 degrees on the Y axis
            Quaternion.Euler(0, 270, 0)  // Rotated 270 degrees on the Y axis
            };

            int index = 0;
            foreach (Transform spawnPoint in spawnPoints)
            {
                // Use the index to assign a rotation
                var rotation = rotations[index % rotations.Length];

                // Instantiate the plant with the specific rotation
                var instance = Instantiate(UI_Manager.Instance.plantHolder, spawnPoint.position, rotation);

                // Add the instance to the respective lists
                UI_Manager.Instance.spawnPlantsForGrowth[tilego].Add(instance);
                UI_Manager.Instance.spawnPlantsForInitialGrowth.Add(instance);
                UI_Manager.Instance.GrownPlantsToCut.Add(instance);

                // Increment the index for the next rotation
                index++;
            }

            plantSpawned = true;
        }
    }




}


