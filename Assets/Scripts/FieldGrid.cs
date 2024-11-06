using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldGrid : MonoBehaviour
{
    public GameObject cellPrefab;
    public int rows = 5;
    public int columns = 7;
    public float cellSpacing = 1.1f;

    internal HashSet<Vector2Int> coveredTiles = new HashSet<Vector2Int>();
    internal List<GameObject> tiles = new List<GameObject>();
    private PlayerAction currentAction;
    internal bool isTracking = false;

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
                tiles.Add(cell);
            }
        }
    }

    /// <summary>
    /// start the action
    /// </summary>
    /// <param name="action"></param>
    public void StartCoverageTracking(PlayerAction action)
    {
        coveredTiles.Clear();
        currentAction = action;
        isTracking = true;
        UI_Manager.Instance.StartActionAnimation(action);  // Start animation for the action
    }

    /// <summary>
    /// Stoping action
    /// </summary>
    public void StopCoverageTracking()
    {
        isTracking = false;
        UI_Manager.Instance.StopCurrentAction(); // Stop the action animation
    }

    /// <summary>
    /// Adding the player covered tiles
    /// </summary>
    /// <param name="tilePosition"></param>
    /// <param name="tileGo"></param>
    public void AddCoveredTile(Vector2Int tilePosition,GameObject tileGo)
    {
        if (!coveredTiles.Contains(tilePosition))
        {
            coveredTiles.Add(tilePosition);
            tileGo.GetComponent<MeshRenderer>().material.SetColor("_BaseColor", new Color(0.9098039f, 0.6431373f, 0.6431373f, 1f));

            Debug.Log("Tile added at position: " + tilePosition);
        }
    }

    /// <summary>
    /// Checking the player covered all tiles are not
    /// </summary>
    /// <returns></returns>
    public bool IsCoverageComplete()
    {
        return coveredTiles.Count >= rows * columns;
    }

    /// <summary>
    /// Getting the tile according to player position on the grid
    /// </summary>
    /// <returns></returns>
    internal Vector2Int GetPlayerTile()
    {
        Vector3 playerPos = UI_Manager.Instance.CharacterMovements.transform.position;
        int row = Mathf.RoundToInt(playerPos.z / cellSpacing);
        int col = Mathf.RoundToInt(playerPos.x / cellSpacing);
        return new Vector2Int(col, row);  // Ensure row/col order matches your grid
    }

    internal void CheckIfCoverageComplete()
    {
        if (coveredTiles.Count >= rows * columns)
        {
            StopCoverageTracking();
            GameManager.Instance.CompleteAction();
        }
    }
}




