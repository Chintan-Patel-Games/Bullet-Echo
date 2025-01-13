using UnityEngine;
using UnityEngine.Rendering.Universal; // For Light2D
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private Vector2Int[] patrolPath;
    [SerializeField] private GridManager gridManager;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float rotationDuration = 0.2f;
    [SerializeField] private GameObject torch;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform bulletSpawnPoint;
    [SerializeField] private float shootCooldown = 0.35f;
    [SerializeField] private Color alertColor = Color.red;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Transform player;
    [SerializeField] private float detectionRange = 2f;

    private Light2D torchLight;
    private Coroutine patrolCoroutine;
    private bool isFollowingPlayer = false;
    private Queue<Vector2Int> pathToPlayer = new Queue<Vector2Int>();
    private bool canShoot = true;

    void Start()
    {
        torchLight = torch.GetComponent<Light2D>();

        if (patrolPath.Length > 0)
        {
            patrolCoroutine = StartCoroutine(PatrolPath());
        }
    }

    void Update()
    {
        if (isFollowingPlayer)
        {
            FollowPlayerWithPathfinding();

            // Shoot the player if in range
            if (Vector2.Distance(transform.position, player.position) <= detectionRange)
            {
                Shoot();
            }
        }
    }

    private IEnumerator PatrolPath()
    {
        while (!isFollowingPlayer) // Patrol only if not following the player
        {
            for (int i = 0; i < patrolPath.Length; i++)
            {
                Vector3 targetPosition = gridManager.GetWorldPosition(patrolPath[i]);

                // Rotate and move to the patrol point
                yield return StartCoroutine(SmoothRotateToAngle(GetAngleTo(targetPosition)));
                yield return StartCoroutine(MoveToPosition(targetPosition));

                yield return new WaitForSeconds(0.5f); // Pause at each patrol point
            }
        }
    }

    private void FollowPlayerWithPathfinding()
    {
        if (player == null) return;

        Vector2Int enemyGridPos = gridManager.GetGridPosition(transform.position);
        Vector2Int playerGridPos = gridManager.GetGridPosition(player.position);

        // Recalculate path if needed
        if (pathToPlayer.Count == 0 || pathToPlayer.Last() != playerGridPos)
        {
            pathToPlayer = FindPath(enemyGridPos, playerGridPos);
        }

        if (pathToPlayer.Count > 0)
        {
            Vector2Int nextGridPos = pathToPlayer.Peek();
            Vector3 nextWorldPos = gridManager.GetWorldPosition(nextGridPos);

            // Move and rotate toward the next position
            if ((transform.position - nextWorldPos).sqrMagnitude > 0.01f)
            {
                StartCoroutine(SmoothRotateToAngle(GetAngleTo(nextWorldPos)));
                StartCoroutine(MoveToPosition(nextWorldPos));
                pathToPlayer.Dequeue();
            }
        }
    }

    private IEnumerator MoveToPosition(Vector3 targetPosition)
    {
        while ((transform.position - targetPosition).sqrMagnitude > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPosition;
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
            yield return null;
        }
        transform.rotation = targetRotation;
    }

    private float GetAngleTo(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        return Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    }

    private Queue<Vector2Int> FindPath(Vector2Int start, Vector2Int target)
    {
        Queue<Vector2Int> path = new Queue<Vector2Int>();
        Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();
        Queue<Vector2Int> frontier = new Queue<Vector2Int>();
        frontier.Enqueue(start);
        cameFrom[start] = start;

        while (frontier.Count > 0)
        {
            Vector2Int current = frontier.Dequeue();

            if (current == target)
            {
                while (current != start)
                {
                    path.Enqueue(current);
                    current = cameFrom[current];
                }
                return new Queue<Vector2Int>(path.Reverse());
            }

            foreach (Vector2Int neighbor in gridManager.GetNeighbors(current))
            {
                if (!cameFrom.ContainsKey(neighbor) && gridManager.IsWalkable(neighbor))
                {
                    frontier.Enqueue(neighbor);
                    cameFrom[neighbor] = current;
                }
            }
        }
        return path;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            TriggerAlert();
        }
    }

    private void TriggerAlert()
    {
        isFollowingPlayer = true;
        torchLight.color = alertColor;

        if (patrolCoroutine != null)
        {
            StopCoroutine(patrolCoroutine);
        }
    }

    private void Shoot()
    {
        if (canShoot)
        {
            canShoot = false;
            Instantiate(bulletPrefab, bulletSpawnPoint.position, transform.rotation);
            StartCoroutine(ShootCooldown());
        }
    }

    private IEnumerator ShootCooldown()
    {
        yield return new WaitForSeconds(shootCooldown);
        canShoot = true;
    }
}