using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour
{
    public Tilemap tilemap; // Assign your Tilemap in the Inspector
    public TileBase walkableTile; // Assign the walkable tile in the Inspector
    public TileBase nonWalkableTile; // Assign the non-walkable tile in the Inspector

    private int rows, columns; // Grid size
    private bool[,] walkableCells; // Walkable status of each cell

    void Start()
    {
        InitializeGrid();
    }

    void InitializeGrid()
    {
        // Determine grid bounds
        BoundsInt bounds = tilemap.cellBounds;
        rows = bounds.size.x;
        columns = bounds.size.y;

        walkableCells = new bool[rows, columns];

        // Loop through all tiles in the Tilemap
        for (int x = 0; x < rows; x++)
        {
            for (int y = 0; y < columns; y++)
            {
                Vector3Int cellPosition = new Vector3Int(bounds.xMin + x, bounds.yMin + y, 0);
                TileBase tile = tilemap.GetTile(cellPosition);

                if (tile == walkableTile)
                {
                    walkableCells[x, y] = true;
                }
                else if (tile == nonWalkableTile)
                {
                    walkableCells[x, y] = false;
                }
            }
        }
    }

    public bool IsWalkable(Vector3 worldPosition)
    {
        Vector3Int cellPosition = tilemap.WorldToCell(worldPosition);

        if (cellPosition.x >= 0 && cellPosition.x < rows && cellPosition.y >= 0 && cellPosition.y < columns)
        {
            return walkableCells[cellPosition.x, cellPosition.y];
        }

        return false; // Out of bounds
    }

    public Vector3 GetWorldPosition(Vector2Int gridPosition)
    {
        Vector3Int cellPosition = new Vector3Int(gridPosition.x, gridPosition.y, 0);
        return tilemap.GetCellCenterWorld(cellPosition);
    }

}