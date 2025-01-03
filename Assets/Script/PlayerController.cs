// using UnityEngine;
// using UnityEngine.Tilemaps;

// public class PlayerMovement : MonoBehaviour
// {
//     public float moveStep = 1f; // Distance to move in each step
//     public float rotationStep = 90f; // Rotation angle in degrees per key press
//     public Tilemap tilemap; // Assign your Tilemap in the Inspector
//     public TileBase walkableTile; // Assign your walkable tile in the Inspector

//     void Update()
//     {
//         if (Input.GetKeyDown(KeyCode.W)) TryMoveForward();
//         if (Input.GetKeyDown(KeyCode.A)) Rotate(rotationStep);
//         if (Input.GetKeyDown(KeyCode.D)) Rotate(-rotationStep);
//     }

//     void TryMoveForward()
//     {
//         Vector3 forward = transform.up;
//         Vector3 targetPosition = transform.position + forward * moveStep;

//         // Check if the target position is walkable
//         if (IsWalkable(targetPosition))
//         {
//             transform.position = targetPosition; // Move the player
//         }
//         else
//         {
//             Debug.Log("Cannot move to non-walkable tile.");
//         }
//     }

//     void Rotate(float angle)
//     {
//         transform.Rotate(0, 0, angle);
//     }

//     bool IsWalkable(Vector3 worldPosition)
//     {
//         // Convert world position to cell position
//         Vector3Int cellPosition = tilemap.WorldToCell(worldPosition);

//         // Get the tile at the cell position
//         TileBase tile = tilemap.GetTile(cellPosition);

//         // Check if the tile is walkable
//         return tile == walkableTile;
//     }
// }

using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 2f; // Speed of the movement (units per second)
    public Tilemap tilemap; // Assign your Tilemap in the Inspector
    public TileBase walkableTile; // Assign your walkable tile in the Inspector

    private bool isMoving = false; // To track if the player is currently moving

    void Update()
    {
        if (!isMoving) // Allow input only when not moving
        {
            if (Input.GetKey(KeyCode.W)) StartCoroutine(Move(transform.up));
            else if (Input.GetKey(KeyCode.A)) StartCoroutine(Rotate(90f));
            else if (Input.GetKey(KeyCode.D)) StartCoroutine(Rotate(-90f));
        }
    }

    System.Collections.IEnumerator Move(Vector3 direction)
    {
        Vector3 targetPosition = transform.position + direction;

        // Check if the target position is walkable
        if (IsWalkable(targetPosition))
        {
            isMoving = true;

            // Gradually move to the target position
            while ((transform.position - targetPosition).sqrMagnitude > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
                yield return null; // Wait for the next frame
            }

            transform.position = targetPosition; // Snap to the target position
        }

        isMoving = false;
    }

    System.Collections.IEnumerator Rotate(float angle)
    {
        isMoving = true;

        // Calculate the target rotation
        Quaternion startRotation = transform.rotation;
        Quaternion targetRotation = Quaternion.Euler(0, 0, transform.eulerAngles.z + angle);

        // Gradually rotate to the target rotation
        float elapsedTime = 0f;
        float rotationDuration = 0.2f; // Time it takes to rotate
        while (elapsedTime < rotationDuration)
        {
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, elapsedTime / rotationDuration);
            elapsedTime += Time.deltaTime;
            yield return null; // Wait for the next frame
        }

        transform.rotation = targetRotation; // Snap to the target rotation
        isMoving = false;
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