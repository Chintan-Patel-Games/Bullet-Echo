using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour
{
    [SerializeField] private Tilemap tilemap; // Reference to the Tilemap
    [SerializeField] private TileBase walkableTile; // Tile that represents walkable areas

    private int rows, columns; // Grid dimensions
    private bool[,] walkableCells; // Array to store walkability of each cell

    void Start()
    {
        InitializeGrid();
    }

    void InitializeGrid()
    {
        // Determine grid bounds
        BoundsInt bounds = tilemap.cellBounds;
        rows = bounds.size.y;
        columns = bounds.size.x;

        walkableCells = new bool[rows, columns];

        // Loop through all tiles in the Tilemap
        for (int x = 0; x < rows; x++)
        {
            for (int y = 0; y < columns; y++)
            {
                Vector3Int cellPosition = new Vector3Int(bounds.xMin + x, bounds.yMin + y, 0);
                TileBase tile = tilemap.GetTile(cellPosition);

                walkableCells[x, y] = tile == walkableTile;
            }
        }
    }

    public bool IsWalkable(Vector2 position)
    {
        Vector2Int gridPosition = new Vector2Int(Mathf.FloorToInt(position.x), Mathf.FloorToInt(position.y));

        if (gridPosition.x >= 0 && gridPosition.x < columns &&
            gridPosition.y >= 0 && gridPosition.y < rows)
        {
            bool isWalkable = walkableCells[gridPosition.x, gridPosition.y];
            return isWalkable;
        }
        return false;
    }

    public List<Vector2> GetNeighbors(Vector2 cell)
    {
        List<Vector2> neighbors = new List<Vector2>();
        
        // Define possible neighbor directions (N, S, E, W)
        Vector2[] directions = new Vector2[]
        {
            new Vector2(0, 1),  // Up
            new Vector2(0, -1), // Down
            new Vector2(1, 0),  // Right
            new Vector2(-1, 0)  // Left
        };

        foreach (Vector2 direction in directions)
        {
            Vector2 neighbor = cell + direction;

            // Ensure the neighbor is within bounds and walkable
            if (IsWalkable(neighbor))
            {
                neighbors.Add(neighbor);
            }
        }

        return neighbors;
    }

    public Vector2 GetGridPosition(Vector3 worldPosition)
    {
        Vector3Int cellPosition = tilemap.WorldToCell(worldPosition);
        return new Vector2(cellPosition.x, cellPosition.y);
    }

    public Vector3 GetWorldPosition(Vector2 gridPosition)
    {
        Vector3Int cellPosition = new Vector3Int(Mathf.FloorToInt(gridPosition.x), Mathf.FloorToInt(gridPosition.y), 0);
        Vector3 worldPosition = tilemap.GetCellCenterWorld(cellPosition);
        return worldPosition;
    }

    public bool IsValidPosition(Vector2 position)
    {
        Vector2Int gridPosition = new Vector2Int(Mathf.FloorToInt(position.x), Mathf.FloorToInt(position.y));

        bool isValid = gridPosition.x >= 0 && gridPosition.x < columns &&
                       gridPosition.y >= 0 && gridPosition.y < rows;

        return isValid;
    }
}