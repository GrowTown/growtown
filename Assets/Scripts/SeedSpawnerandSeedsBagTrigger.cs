using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SeedSpawnerandSeedsBagTrigger : MonoBehaviour
{
    private bool isHandInBag = false; 
    internal bool isTileHasSeed = false; 
    GameObject spawnObject;
   // internal GameObject CoveredTile;
    internal List<GameObject> CoveredTileForSpawn=new List<GameObject>();

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
        Destroy(spawnObject);
    }

    // This method will be called by the animation event for the throw
    public void OnThrowSeed()
    {
        /*if (isHandInBag && UI_Manager.Instance.seed!= null)
        {
             Destroy(spawnObject);
            // Instantiate seed at tile position
            if (CoveredTile != null)
            {
                CoveredTileForSpawn.Add(CoveredTile);
            }

            if (CoveredTileForSpawn.Contains(CoveredTile)&&!isTileHasSeed)
            {
                isTileHasSeed = true;
                CoveredTile.GetComponent<TileInfo>().OnPlayerEnter();
                Debug.Log("Seed spawned on tile");
                SpawnPlantWithDelay(CoveredTile);
            }
             CoveredTile = null;
            
            // Reset flag
            isHandInBag = false;
        }*/
    }
    public void OnThrowSeed(GameObject coveredTile)
    {
        if (UI_Manager.Instance.seed!= null&& GameManager.Instance.isThroughingseeds)
        {
            if (coveredTile != null && !isTileHasSeed)
            {
                isTileHasSeed = true;
                coveredTile.GetComponent<TileInfo>().OnPlayerEnter();
                Debug.Log("Seed spawned on tile");
                SpawnPlantWithDelay(coveredTile);
            }
             coveredTile = null;
        }
    }

    private void SpawnPlantWithDelay(GameObject tilego)
    {
      
        if (UI_Manager.Instance.plantHolder != null)
        {
            UI_Manager.Instance.isPlanted=true;
            tilego.GetComponent<TileInfo>().SpawnPlant(tilego);
            Debug.Log("Plant spawned after delay");
        }
     
    }

}


