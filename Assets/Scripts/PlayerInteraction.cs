using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    private CharacterController controller;

    void Start()
    {
        controller = transform.parent.GetComponent<CharacterController>();
    }

    void Update()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 1))
        {
            OnPlayerInteraction(hit);
        }
    }

    void ChangetheValues(FieldGrid fGrid)
    {
       
        Debug.Log("All Collected");

        fGrid.CompleteAction();
        if (fGrid.isCutting)
        {
            fGrid.ResetField();
            fGrid.CompleteAction();
        }
    }
    void OnPlayerInteraction(RaycastHit hit)
    {

        Collider other = hit.collider;

        if (other.CompareTag("Field") && hit.collider.gameObject.GetComponent<TileInfo>().fieldGrid.isTracking)
        {
            GameObject hitTileGameObject = hit.collider.gameObject;
            // Get the tile position for the player and try to add it
            //Vector2Int playerTile = UI_Manager.Instance.FieldGrid.GetPlayerTile();
            if (UI_Manager.Instance.FieldManager.fieldSteps[hit.collider.gameObject.GetComponent<TileInfo>().fieldGrid.fieldID] ==1)
            {
                hitTileGameObject.GetComponent<TileInfo>().OnThrowSeed(hitTileGameObject);
                //UI_Manager.Instance.FieldGrid.AddCoveredTile(hitTileGameObject);
            }
          //  UI_Manager.Instance.seedsBag.GetComponent<SeedSpawnerandSeedsBagTrigger>().OnThrowSeed(hitTileGameObject);
            if (hitTileGameObject.GetComponent<TileInfo>().fieldGrid.isCutting)
                GameManager.Instance.HarvestDeductEnergy(hitTileGameObject);
   
            if (hitTileGameObject.GetComponent<TileInfo>().fieldGrid.IsCoverageComplete())
            {

                hitTileGameObject.GetComponent<TileInfo>().fieldGrid.StopCoverageTracking();
                if (UI_Manager.Instance.FieldManager.fieldSteps[hit.collider.gameObject.GetComponent<TileInfo>().fieldGrid.fieldID] == 3)
                {
                    ChangetheValues(hitTileGameObject.GetComponent<TileInfo>().fieldGrid);
                    AudioManager.Instance.StopMusic();
                }
                else
                {
                    AudioManager.Instance.StopMusic();
                    ChangetheValues(hitTileGameObject.GetComponent<TileInfo>().fieldGrid);
                }

            }
        }
    }
}
