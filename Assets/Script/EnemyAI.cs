using UnityEngine;
using UnityEngine.Rendering.Universal; // For Light2D
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private Vector2Int[] patrolPath; // Array of grid positions for patrol
    [SerializeField] private GridManager gridManager; // Reference to your grid manager
    [SerializeField] private float moveSpeed = 2f; // Speed of movement
    [SerializeField] private float rotationDuration = 0.2f; // Time taken to rotate to the next direction
    [SerializeField] private GameObject torch; // Torch Light GameObject (Light2D)
    [SerializeField] private GameObject bulletPrefab; // Assign the bullet prefab
    [SerializeField] private Transform bulletSpawnPoint; // A child object to determine where bullets spawn
    [SerializeField] private float shootCooldown = 0.35f; // Time between shots
    [SerializeField] private Color alertColor = new Color(1f, 0f, 0f, 1f); // Red
    [SerializeField] private Color normalColor = new Color(1f, 1f, 1f, 1f); // White
    [SerializeField] private Transform player; // Assign the player's Transform in the Inspector
    [SerializeField] private float detectionRange = 2f; // Enemy's detection range

    private Light2D torchLight; // Reference to the Light2D
    private PolygonCollider2D torchCollider; // Reference to the PolygonCollider2D for torch
    private bool isFollowingPlayer = false;
    private int currentTargetIndex = 0;
    private bool isMoving = false; // To track if the enemy is currently moving
    private bool canShoot = true;
    private Queue<Vector2Int> pathToPlayer = new Queue<Vector2Int>();

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
            if (IsPlayerOutOfRange())
            {
                StopFollowingPlayer();
                return;
            }

            FollowPlayerWithPathfinding();
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

    void FollowPlayerWithPathfinding()
    {
        if (player == null) return;

        // Check if the path to the player is empty or needs recalculating
        if (pathToPlayer.Count == 0)
        {
            Vector2Int enemyGridPos = gridManager.GetGridPosition(transform.position);
            Vector2Int playerGridPos = gridManager.GetGridPosition(player.position);

            pathToPlayer = FindPath(enemyGridPos, playerGridPos);

            if (pathToPlayer.Count == 0)
            {
                return; // No valid path found, don't continue
            }
        }

        if (pathToPlayer.Count > 0)
        {
            // Get the next position to move toward
            Vector2Int nextGridPos = pathToPlayer.Peek();
            Vector3 nextWorldPos = gridManager.GetWorldPosition(nextGridPos);

            // Calculate the direction to the next position
            Vector3 directionToTarget = (nextWorldPos - transform.position).normalized;

            // Rotate smoothly towards the next position
            float targetAngle = Mathf.Atan2(directionToTarget.y, directionToTarget.x) * Mathf.Rad2Deg;
            StartCoroutine(SmoothRotateToAngle(targetAngle));

            // Move toward the next grid position
            if (!isMoving)
            {
                StartCoroutine(MoveToPosition(nextWorldPos));
                pathToPlayer.Dequeue(); // Remove the current grid position from the path
            }
        }

        // Shoot the player if within range
        Shoot();
    }

    private bool IsPlayerOutOfRange()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        return distanceToPlayer > detectionRange;
    }

    private void StopFollowingPlayer()
    {
        isFollowingPlayer = false;

        if (torchLight != null)
        {
            torchLight.color = normalColor;
        }

        StartCoroutine(PatrolPath());
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
            TriggerAlert();
        }
    }

    void Shoot()
    {
        // Check if the enemy can shoot, and if so, shoot and start cooldown
        if (canShoot)
        {
            canShoot = false;

            // Instantiate the bullet at the spawn point and in the player's current rotation
            Instantiate(bulletPrefab, bulletSpawnPoint.position, transform.rotation);

            // Start cooldown timer
            StartCoroutine(ShootCooldown());
        }
    }

    private IEnumerator ShootCooldown()
    {
        yield return new WaitForSeconds(shootCooldown);
        canShoot = true; // Enemy can shoot again after the cooldown period
    }

    private Queue<Vector2Int> FindPath(Vector2Int start, Vector2Int target)
    {
        Queue<Vector2Int> path = new Queue<Vector2Int>();
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();
        Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();

        // Check if the start or target positions are out of bounds
        if (!gridManager.IsValidPosition(start) || !gridManager.IsValidPosition(target))
        {
            return path;
        }

        Queue<Vector2Int> frontier = new Queue<Vector2Int>();
        frontier.Enqueue(start);

        while (frontier.Count > 0)
        {
            Vector2Int current = frontier.Dequeue();

            if (current == target)
            {
                // Trace back the path
                Vector2Int step = target;
                while (step != start)
                {
                    path.Enqueue(step);
                    step = cameFrom[step];
                }

                path.Enqueue(start); // Optional: include the starting position
                path = new Queue<Vector2Int>(path.Reverse()); // Reverse the path
                break;
            }

            foreach (Vector2Int neighbor in gridManager.GetNeighbors(current))
            {
                if (!visited.Contains(neighbor) && gridManager.IsWalkable(neighbor))
                {
                    visited.Add(neighbor);
                    cameFrom[neighbor] = current;
                    frontier.Enqueue(neighbor);
                }
            }
        }
        return path;
    }
}