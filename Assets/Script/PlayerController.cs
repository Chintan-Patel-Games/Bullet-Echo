using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerMovement : MonoBehaviour
{
    public float moveStep = 1f; // Distance to move in each step
    public float rotationStep = 90f; // Rotation angle in degrees per key press
    public Tilemap tilemap; // Assign your Tilemap in the Inspector
    public TileBase walkableTile; // Assign your walkable tile in the Inspector

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.W)) TryMoveForward();
        if (Input.GetKeyDown(KeyCode.A)) Rotate(rotationStep);
        if (Input.GetKeyDown(KeyCode.D)) Rotate(-rotationStep);
    }

    void TryMoveForward()
    {
        Vector3 forward = transform.up;
        Vector3 targetPosition = transform.position + forward * moveStep;

        // Check if the target position is walkable
        if (IsWalkable(targetPosition))
        {
            transform.position = targetPosition; // Move the player
        }
        else
        {
            Debug.Log("Cannot move to non-walkable tile.");
        }
    }

    void Rotate(float angle)
    {
        transform.Rotate(0, 0, angle);
    }

    bool IsWalkable(Vector3 worldPosition)
    {
        // Convert world position to cell position
        Vector3Int cellPosition = tilemap.WorldToCell(worldPosition);

        // Get the tile at the cell position
        TileBase tile = tilemap.GetTile(cellPosition);

        // Check if the tile is walkable
        return tile == walkableTile;
    }
}