using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SeedSpawnerandSeedsBagTrigger : MonoBehaviour
{
    private bool isHandInBag = false; 
    
    private bool isShotGunInHand = false;
    GameObject spawnObject;
  
   // internal GameObject CoveredTile;
    internal List<GameObject> CoveredTileForSpawn=new List<GameObject>();

    public void OnHandInBag()
    {
        isHandInBag = true;
        SpawnSeedInHand();
    }


    public void OnGunInHand()
    {
        SpawnShotGunInHand();
    }
    private void SpawnShotGunInHand()
    {
        if (!isShotGunInHand && UI_Manager.Instance.shotGun != null)
        {
            // Instantiate shotGun at hand position
            spawnObject = Instantiate(UI_Manager.Instance.shotGun, UI_Manager.Instance.shotGunSpawnPoint.transform.position,
                                        UI_Manager.Instance.shotGunSpawnPoint.transform.rotation, UI_Manager.Instance.shotGunSpawnPoint.transform);
            isShotGunInHand = true;
            Debug.Log("ShotGun spawned in hand");
        }
        else
        {
            Destroy(spawnObject);
            isShotGunInHand = false;
        }
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
  /*  public void OnThrowSeed()
    {
        *//*if (isHandInBag && UI_Manager.Instance.seed!= null)
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
        }*//*
    }*/
  
}


