using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerMovement : MonoBehaviour
{
    public float moveStep = 1f; // Distance to move in each step
    public float rotationStep = 90f; // Rotation angle in degrees per key press

    void Update()
    {
        // Handle player movement input
        if (Input.GetKeyDown(KeyCode.W)) MoveForward();
        if (Input.GetKeyDown(KeyCode.A)) Rotate(rotationStep);
        if (Input.GetKeyDown(KeyCode.D)) Rotate(-rotationStep);
    }

    void MoveForward()
    {
        Vector3 forward = transform.up;
        transform.position += forward * moveStep; // Move the player
    }

    void Rotate(float angle)
    {
        // Rotate the player around the Z-axis
        transform.Rotate(0, 0, angle);
    }
}