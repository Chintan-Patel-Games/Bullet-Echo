using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour
{
    [SerializeField] private Tilemap tilemap;
    [SerializeField] private TileBase walkableTile;

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

    public bool IsWalkable(Vector2Int position)
    {
        if (position.x >= 0 && position.x < columns &&
            position.y >= 0 && position.y < rows)
        {
            return walkableCells[position.x, position.y];
        }
        return false; // Out-of-bounds cells are not walkable
    }

    public List<Vector2Int> GetNeighbors(Vector2Int cell)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();

        // Define possible neighbor directions (N, S, E, W)
        Vector2Int[] directions = new Vector2Int[]
        {
            new Vector2Int(0, 1),  // Up
            new Vector2Int(0, -1), // Down
            new Vector2Int(1, 0),  // Right
            new Vector2Int(-1, 0)  // Left
        };

        foreach (Vector2Int direction in directions)
        {
            Vector2Int neighbor = cell + direction;

            // Ensure the neighbor is within bounds and walkable
            if (IsWalkable(neighbor))
            {
                neighbors.Add(neighbor);
            }
        }

        return neighbors;
    }

    public Vector2Int GetGridPosition(Vector3 worldPosition)
    {
        Vector3Int cellPosition = tilemap.WorldToCell(worldPosition);
        return new Vector2Int(cellPosition.x, cellPosition.y);
    }

    public Vector3 GetWorldPosition(Vector2Int gridPosition)
    {
        Vector3Int cellPosition = new Vector3Int(gridPosition.x, gridPosition.y, 0);
        return tilemap.GetCellCenterWorld(cellPosition);
    }

    public bool IsValidPosition(Vector2Int position)
    {
        int width = columns;
        int height = rows;

        return position.x >= 0 && position.x < width && position.y >= 0 && position.y < height;
    }
}