using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SeedSpawnerandSeedsBagTrigger : MonoBehaviour
{
    private bool isHandInBag = false; // To track hand in bag
    internal bool isTileHasSeed = false; // To track hand in bag
    GameObject spawnObject;
    internal GameObject CoveredTile;
    // Method called when hand enters bag trigger
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
                var insta=Instantiate(UI_Manager.Instance.seed, CoveredTile.transform.GetChild(4).position, Quaternion.identity);
                var insta1=Instantiate(UI_Manager.Instance.seed, CoveredTile.transform.GetChild(5).position, Quaternion.identity);
                var insta2=Instantiate(UI_Manager.Instance.seed, CoveredTile.transform.GetChild(6).position, Quaternion.identity);
                var insta3=Instantiate(UI_Manager.Instance.seed, CoveredTile.transform.GetChild(7).position, Quaternion.identity);
                Debug.Log("Seed spawned on tile");
                SpawnPlantWithDelay(CoveredTile.transform);
            }
            

             CoveredTile = null;
            
            // Reset flag
            isHandInBag = false;
        }
    }

    private void SpawnPlantWithDelay(Transform tileTransform)
    {
      
        if (UI_Manager.Instance.plantHolder != null)
        {
            UI_Manager.Instance.isPlanted=true;
            // Instantiate plant at the position of the first seed
            Instantiate(UI_Manager.Instance.plantHolder, tileTransform.GetChild(4).position, Quaternion.identity);
            Instantiate(UI_Manager.Instance.plantHolder, tileTransform.GetChild(5).position, Quaternion.identity);
            Instantiate(UI_Manager.Instance.plantHolder, tileTransform.GetChild(6).position, Quaternion.identity);
            Instantiate(UI_Manager.Instance.plantHolder, tileTransform.GetChild(7).position, Quaternion.identity);
            Debug.Log("Plant spawned after delay");
        }
     
    }

}


