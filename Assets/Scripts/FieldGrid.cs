using System.Collections.Generic;
using UnityEngine;

public class FieldGrid : MonoBehaviour
{
    public GameObject cellPrefab;
    public int rows = 5;
    public int columns = 7;
    public float cellSpacing = 0.01f;

    //internal HashSet<Vector3> coveredTiles = new HashSet<Vector3>();
    internal List<GameObject> tiles = new List<GameObject>();
    internal List<GameObject> coveredtiles = new List<GameObject>();
    private PlayerAction currentAction;
    internal bool isTracking = false;
    internal bool checkedOnce;

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
                Vector3 cellPosition = new Vector3(col * cellSpacing, 0f, row * cellSpacing) - gridOffset;
                GameObject cell = Instantiate(cellPrefab, transform.position + cellPosition, Quaternion.identity);
                cell.transform.SetParent(transform);
                tiles.Add(cell);
            }
        }
    }

    /// <summary>
    /// start the action
    /// </summary>StartCovering
    /// <param name="action"></param>
    public void StartCoverageTracking(PlayerAction action)
    {
        if (UI_Manager.Instance.FieldGrid.IsCoverageComplete())
            coveredtiles.Clear();
        currentAction = action;
        isTracking = true;
        GameManager.Instance.StartActionAnimation(action);  // Start animation for the action
    }

    /// <summary>
    /// Stoping action
    /// </summary>
    public void StopCoverageTracking()
    {
        isTracking = false;
        GameManager.Instance.StopCurrentAnimations(); // Stop the action animation
    }

   
    /// <summary>
    /// Adding the player covered tiles
    /// </summary>
    /// <param name="tilePosition"></param>
    /// <param name="tileGo"></param>
    public void AddCoveredTile(/*Vector2Int tilePosition,*/GameObject tileGo)
    {
        if (!checkedOnce)
        {
            checkedOnce = true;
          if (!GameManager.Instance.HasEnoughPoints(5, 10)) return;
      
        }
        if(UI_Manager.Instance.TriggerZoneCallBacks.currentStep == 1)
        {
            if (!GameManager.Instance.HasNotEnoughSeed(1) && GameManager.Instance.isThroughingseeds) return;
        }
        /*  if (!coveredTiles.Contains(tilePosition))
          {
              coveredTiles.Add(tilePosition);
              tileGo.GetComponent<MeshRenderer>().material.SetColor("_BaseColor", new Color(0.9098039f, 0.6431373f, 0.6431373f, 1f));

              Debug.Log("Tile added at position: " + tilePosition);
          }*/

        if (!coveredtiles.Contains(tileGo))
        {

            switch (currentAction)
            {
                case PlayerAction.Clean:
                    coveredtiles.Add(tileGo);
                    tileGo.GetComponent<MeshRenderer>().material.SetColor("_BaseColor", new Color(1f, 0.9188f, 0.9188f, 1f));
                    Debug.Log("Cleanig");
                    break;
                case PlayerAction.Seed:
                    if (UI_Manager.Instance.seedsBag.GetComponent<SeedSpawnerandSeedsBagTrigger>().isTileHasSeed)
                    {
                        coveredtiles.Add(tileGo);
                        tileGo.GetComponent<MeshRenderer>().material.SetColor("_BaseColor", new Color(0.7095f, 0.7095f, 0.7095f, 1f));
                        UI_Manager.Instance.seedsBag.GetComponent<SeedSpawnerandSeedsBagTrigger>().isTileHasSeed = false;
                    }
                    // Debug.Log("Seeding");
                    break;
                case PlayerAction.Water:
                    coveredtiles.Add(tileGo);
                    tileGo.GetComponent<MeshRenderer>().material.SetColor("_BaseColor", new Color(0.9098039f, 0.6431373f, 0.6431373f, 1f));
                    break;
                case PlayerAction.Harvest:
                    coveredtiles.Add(tileGo);
                    tileGo.GetComponent<MeshRenderer>().material.SetColor("_BaseColor", new Color(1f, 1f, 1f, 1f));
                    break;
            }

             
            Debug.Log("Tile added: " + tileGo);
        }
    }

    /// <summary>
    /// Checking the player covered all tiles are not
    /// </summary>
    /// <returns></returns>
    public bool IsCoverageComplete()
    {
        return coveredtiles.Count >= rows * columns;
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

}




