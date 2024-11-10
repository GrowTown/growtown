using System.Collections.Generic;
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

    void OnPlayerInteraction(RaycastHit hit)
    {
        Collider other = hit.collider;

        if (other.CompareTag("Field") && UI_Manager.Instance.FieldGrid.isTracking)
        {
            GameObject hitTileGameObject = hit.collider.gameObject;
            // Get the tile position for the player and try to add it
            //Vector2Int playerTile = UI_Manager.Instance.FieldGrid.GetPlayerTile();
            UI_Manager.Instance.FieldGrid.AddCoveredTile( hitTileGameObject); // Store the tile if uncovered
            UI_Manager.Instance.seedsBag.GetComponent<SeedSpawnerandSeedsBagTrigger>().CoveredTile = hitTileGameObject;
            // Check if grid coverage is complete to show completion popup
            if (UI_Manager.Instance.FieldGrid.IsCoverageComplete())
            {
                Debug.Log("All Collected");
                UI_Manager.Instance.FieldGrid.StopCoverageTracking();
                GameManager.Instance.CompleteAction();
            }
        }
    }
}
