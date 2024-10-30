using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldGrid : MonoBehaviour
{
    public GameObject cellPrefab;
    public int rows = 5;
    public int columns = 7;
    public float cellSpacing = 1.1f;

    private HashSet<Vector2Int> coveredTiles = new HashSet<Vector2Int>();
    private PlayerAction currentAction;
    private bool isTracking = false;

    private void Start()
    {
        GenerateGrid();
    }

    private void GenerateGrid()
    {
        Vector3 gridOffset = new Vector3((columns - 1) * cellSpacing / 2, 0, (rows - 1) * cellSpacing / 2);

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                Vector3 cellPosition = new Vector3(col * cellSpacing, 0.02f, row * cellSpacing) - gridOffset;
                GameObject cell = Instantiate(cellPrefab, transform.position + cellPosition, Quaternion.identity);
                cell.transform.SetParent(transform);
            }
        }
    }

    public void StartCoverageTracking(PlayerAction action)
    {
        coveredTiles.Clear();
        currentAction = action;
        isTracking = true;
        UI_Manager.Instance.StartActionAnimation(action);  // Start animation for the action
    }

    public void StopCoverageTracking()
    {
        isTracking = false;
        UI_Manager.Instance.StopCurrentAction(); // Stop the action animation
    }

    private void Update()
    {
        if (isTracking)
        {
            Vector2Int playerTile = GetPlayerTile();
            if (!coveredTiles.Contains(playerTile))
            {
                coveredTiles.Add(playerTile);
                CheckIfCoverageComplete();
            }
        }
    }

    private Vector2Int GetPlayerTile()
    {
        Vector3 playerPos = UI_Manager.Instance.CharacterMovements.transform.position;
        int row = Mathf.RoundToInt(playerPos.z / cellSpacing);
        int col = Mathf.RoundToInt(playerPos.x / cellSpacing);
        return new Vector2Int(col, row);  // Ensure row/col order matches your grid
    }

    private void CheckIfCoverageComplete()
    {
        if (coveredTiles.Count >= rows * columns)
        {
            StopCoverageTracking();
            GameManager.Instance.CompleteAction();
        }
    }
}




