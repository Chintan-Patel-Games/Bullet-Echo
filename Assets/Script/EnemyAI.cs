using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private Vector2Int[] patrolPath; // Array of grid positions for patrol
    [SerializeField] private GridManager gridManager;
    private int currentTargetIndex = 0;

    public float moveSpeed = 2f; // Speed of movement
    private bool isMoving = false; // To track if the enemy is currently moving

    void Start()
    {
        MoveToNextPoint(); // Start the patrol
    }

    void Update()
    {
        if (!isMoving)
        {
            StartCoroutine(MoveToNextPoint());
        }
    }

    System.Collections.IEnumerator MoveToNextPoint()
    {
        if (patrolPath.Length == 0) yield break;

        isMoving = true;

        // Get the target cell position
        Vector2Int targetCell = patrolPath[currentTargetIndex];
        Vector3 targetPosition = gridManager.GetWorldPosition(targetCell);

        // Smoothly move to the target position
        while ((transform.position - targetPosition).sqrMagnitude > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
            yield return null; // Wait for the next frame
        }

        transform.position = targetPosition; // Snap to the target position

        // Move to the next patrol point
        currentTargetIndex = (currentTargetIndex + 1) % patrolPath.Length;

        isMoving = false;
    }
}