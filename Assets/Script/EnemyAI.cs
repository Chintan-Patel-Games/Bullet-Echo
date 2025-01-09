using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal; // For Light2D

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private Vector2Int[] patrolPath; // Array of grid positions for patrol
    [SerializeField] private GridManager gridManager;
    [SerializeField] private float moveSpeed = 2f; // Speed of movement
    [SerializeField] private float rotationDuration = 0.2f; // Time taken to rotate to the next direction
    [SerializeField] private GameObject torch; // Torch Light GameObject (Light2D)
    public Color alertColor = new Color(1f, 0f, 0f, 1f); // Red
    public Color normalColor = new Color(1f, 1f, 1f, 1f); // White
    public Transform player; // Assign the player's Transform in the Inspector

    private Light2D torchLight; // Reference to the Light2D
    private PolygonCollider2D torchCollider; // Reference to the PolygonCollider2D for torch
    private bool isFollowingPlayer = false;
    private int currentTargetIndex = 0;
    private bool isMoving = false; // To track if the enemy is currently moving

    void Start()
    {
        torchLight = torch.GetComponent<Light2D>();
        torchCollider = torch.GetComponent<PolygonCollider2D>();

        if (patrolPath.Length > 0)
        {
            StartCoroutine(PatrolPath());
        }
    }

    void Update()
    {
        if (isFollowingPlayer)
        {
            FollowPlayer();
        }
    }

    private IEnumerator PatrolPath()
    {
        while (!isFollowingPlayer) // Patrol only if not following the player
        {
            if (patrolPath.Length == 0) yield break;

            // Get the next target position in the patrol path
            Vector2Int targetCell = patrolPath[currentTargetIndex];
            Vector3 targetPosition = gridManager.GetWorldPosition(targetCell);

            // Rotate toward the target position
            Vector3 directionToTarget = (targetPosition - transform.position).normalized;
            float targetAngle = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg;

            yield return StartCoroutine(SmoothRotateToAngle(targetAngle));

            // Move to the target position
            yield return StartCoroutine(MoveToPosition(targetPosition));

            // Update the target index for the next patrol point
            currentTargetIndex = (currentTargetIndex + 1) % patrolPath.Length;

            yield return new WaitForSeconds(0.5f); // Optional pause at each patrol point
        }
    }

    private IEnumerator MoveToPosition(Vector3 targetPosition)
    {
        isMoving = true;

        while ((transform.position - targetPosition).sqrMagnitude > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            yield return null; // Wait for the next frame
        }

        transform.position = targetPosition; // Snap to the target position
        isMoving = false;
    }

    private IEnumerator SmoothRotateToAngle(float targetAngle)
    {
        Quaternion startRotation = transform.rotation;
        Quaternion targetRotation = Quaternion.Euler(0, 0, targetAngle);

        float elapsedTime = 0f;

        while (elapsedTime < rotationDuration)
        {
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, elapsedTime / rotationDuration);
            elapsedTime += Time.deltaTime;
            yield return null; // Wait for the next frame
        }

        transform.rotation = targetRotation; // Snap to the final rotation
    }

    void FollowPlayer()
    {
        if (player == null) return;

        // Rotate toward the player
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        float targetAngle = Mathf.Atan2(directionToPlayer.y, directionToPlayer.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, targetAngle);

        // Move toward the player
        transform.position = Vector3.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
    }

    public void TriggerAlert()
    {
        isFollowingPlayer = true;

        if (torchLight != null)
        {
            torchLight.color = alertColor; // Change torch color to red
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the player has entered the torch's detection area
        if (other.CompareTag("Player")) // Ensure the player GameObject has the "Player" tag
        {
            Debug.Log("Player detected!");
            TriggerAlert();
        }
    }
}