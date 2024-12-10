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

    void ChangetheValues()
    {
        UI_Manager.Instance.FieldGrid.StopCoverageTracking();
        Debug.Log("All Collected");

        GameManager.Instance.CompleteAction();
        if (GameManager.Instance.isCutting)
        {
            GameManager.Instance.ResetValues();
        }
    }
    void OnPlayerInteraction(RaycastHit hit)
    {

        Collider other = hit.collider;

        if (other.CompareTag("Field") && UI_Manager.Instance.FieldGrid.isTracking)
        {
            GameObject hitTileGameObject = hit.collider.gameObject;
            // Get the tile position for the player and try to add it
            //Vector2Int playerTile = UI_Manager.Instance.FieldGrid.GetPlayerTile();
            UI_Manager.Instance.FieldGrid.AddCoveredTile(hitTileGameObject); // Store the tile if uncovered
            UI_Manager.Instance.seedsBag.GetComponent<SeedSpawnerandSeedsBagTrigger>().OnThrowSeed(hitTileGameObject);
            if (GameManager.Instance.isCutting)
                GameManager.Instance.HarvestDeductEnergy(hitTileGameObject);
            if (GameManager.Instance.isPlantStartGrowing)
            {
                GameManager.Instance.OnWaterTile(hitTileGameObject);
            }
            if (UI_Manager.Instance.FieldGrid.IsCoverageComplete())
            {
                if (UI_Manager.Instance.TriggerZoneCallBacks.currentStep == 3)
                {
                    if (UI_Manager.Instance.GrownPlantsToCut.Count == 0)
                    {
                        ChangetheValues();
                    }
                }
           
                else
                {
                    ChangetheValues();
                }

            }
        }
    }
}
