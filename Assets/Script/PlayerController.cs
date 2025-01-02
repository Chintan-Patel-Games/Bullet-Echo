using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveStep = 1f;        // Distance to move in each step
    public float rotationStep = 90f;  // Rotation angle in degrees per key press

    void Update()
    {
        // Forward Movement
        if (Input.GetKeyDown(KeyCode.W)) MoveForward();

        // Rotation
        if (Input.GetKeyDown(KeyCode.D)) Rotate(-rotationStep);
        if (Input.GetKeyDown(KeyCode.A)) Rotate(rotationStep);
    }

    void MoveForward()
    {
        // Move the player forward in the direction it is facing
        Vector3 forward = transform.up; // In 2D, "up" is the forward direction
        transform.position += forward * moveStep;
    }

    void Rotate(float angle)
    {
        // Rotate the player around the Z-axis
        transform.Rotate(0, 0, angle);
    }
}