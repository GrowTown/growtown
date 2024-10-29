using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldGrid : MonoBehaviour
{

    public GameObject cellPrefab; // Prefab to be used for each cell in the grid
    public int rows = 5; // Number of rows
    public int columns = 7; // Number of columns
    public float cellSpacing = 1.1f; // Spacing between cells

    void Start()
    {
        GenerateGrid();
    }

    void GenerateGrid()
    {

        Vector3 gridOffset = new Vector3(
            (columns - 1) * cellSpacing / 2,
            0,
            (rows - 1) * cellSpacing / 2
        );
        // Loop through rows and columns to place each cell
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                // Calculate position for each cell
                Vector3 cellPosition = new Vector3(col * cellSpacing, 0.02f, row * cellSpacing)-gridOffset;

                // Instantiate a cell at the calculated position
                GameObject cell = Instantiate(cellPrefab, transform.position+cellPosition, Quaternion.identity);

                // Set cell as a child of GridManager for cleaner hierarchy
                cell.transform.SetParent(transform);
            }
        }
    }
}


