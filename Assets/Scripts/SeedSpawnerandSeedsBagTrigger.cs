using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SeedSpawnerandSeedsBagTrigger : MonoBehaviour
{
    private bool isHandInBag = false; 
    internal bool isTileHasSeed = false; 
    GameObject spawnObject;
    internal GameObject CoveredTile;

    public void OnHandInBag()
    {
        isHandInBag = true;
        SpawnSeedInHand();
    }

    private void SpawnSeedInHand()
    {
        if (isHandInBag && UI_Manager.Instance.seed!= null)
        {
            // Instantiate seed at hand position
          spawnObject = Instantiate(UI_Manager.Instance.seed, UI_Manager.Instance.seedSpawnPoint.transform.position, Quaternion.identity);
            Debug.Log("Seed spawned in hand");
        }
    }

    // This method will be called by the animation event for the throw
    public void OnThrowSeed()
    {
        if (isHandInBag && UI_Manager.Instance.seed!= null)
        {
             Destroy(spawnObject);
            // Instantiate seed at tile position
            if (CoveredTile != null)
            {
                isTileHasSeed=true;
                CoveredTile.GetComponent<TileInfo>().OnPlayerEnter();
                Debug.Log("Seed spawned on tile");
                SpawnPlantWithDelay(CoveredTile);
            }
            

             CoveredTile = null;
            
            // Reset flag
            isHandInBag = false;
        }
    }

    private void SpawnPlantWithDelay(GameObject tilego)
    {
      
        if (UI_Manager.Instance.plantHolder != null)
        {
            UI_Manager.Instance.isPlanted=true;
            tilego.GetComponent<TileInfo>().SpawnPlant();
            Debug.Log("Plant spawned after delay");
        }
     
    }

}


