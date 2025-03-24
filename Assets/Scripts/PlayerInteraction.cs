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

        UI_Manager.Instance.TriggerZoneCallBacks.CompleteAction();
        if (GameManager.Instance.isCutting)
        {
            GameManager.Instance.ResetValues();
            UI_Manager.Instance.TriggerZoneCallBacks.CompleteAction();
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
            if (UI_Manager.Instance.FieldManager.CurrentStepID ==1)
            {
                UI_Manager.Instance.FieldGrid.AddCoveredTile(hitTileGameObject);
            }
            UI_Manager.Instance.seedsBag.GetComponent<SeedSpawnerandSeedsBagTrigger>().OnThrowSeed(hitTileGameObject);
            if (GameManager.Instance.isCutting)
                GameManager.Instance.HarvestDeductEnergy(hitTileGameObject);
   
            if (UI_Manager.Instance.FieldGrid.IsCoverageComplete())
            {

                if (UI_Manager.Instance.FieldManager.CurrentStepID == 3)
                {
                    ChangetheValues();
                }
                else
                {
                    ChangetheValues();
                }

            }
        }
    }
}
