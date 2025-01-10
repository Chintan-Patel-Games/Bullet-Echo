using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 2f; // Speed of the movement (units per second)
    [SerializeField] private Tilemap tilemap; // Assign your Tilemap
    [SerializeField] private TileBase walkableTile; // Assign your walkable tile
    [SerializeField] private TileBase finishTile; // Assign your walkable tile

    [Header("Shooting Settings")]
    [SerializeField] private GameObject bulletPrefab;  // Assign the bullet prefab
    [SerializeField] private Transform bulletSpawnPoint;  // A child object to determine where bullets spawn
    [SerializeField] private float shootCooldown = 0.35f;  // Time between shots

    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 100; // Maximum health of the player
    private int currentHealth;

    [SerializeField] private LevelManager levelManager; // Reference to the LevelManager

    private bool canShoot = true;
    private bool isMoving = false; // To track if the player is currently moving
    private bool isRotating = false; // To track if the player is currently rotating


    void Start()
    {
        currentHealth = maxHealth; // Initialize health to maximum
    }

    void Update()
    {
        if (!isMoving && !isRotating) // Allow input only when not moving or rotating
        {
            if (Input.GetKey(KeyCode.W)) StartCoroutine(Move(transform.up));
            else if (Input.GetKey(KeyCode.A)) StartCoroutine(Rotate(90f));
            else if (Input.GetKey(KeyCode.D)) StartCoroutine(Rotate(-90f));
        }

        // Handle shooting
        if (Input.GetKey(KeyCode.Space) && canShoot && !isRotating)
        {
            Shoot();
        }
    }

    private IEnumerator Move(Vector3 direction)
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

            OnFinishTile();
        }

        isMoving = false;
    }

    private IEnumerator Rotate(float angle)
    {
        isRotating = true;

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
        isRotating = false;
    }

    bool IsWalkable(Vector3 worldPosition)
    {
        // Convert world position to cell position
        Vector3Int cellPosition = tilemap.WorldToCell(worldPosition);

        // Get the tile at the cell position
        TileBase tile = tilemap.GetTile(cellPosition);

        // Check if the tile is walkable
        return tile == walkableTile || tile == finishTile;
    }

    private void OnFinishTile()
    {
        // Convert the player's position to cell position
        Vector3Int cellPosition = tilemap.WorldToCell(transform.position);

        // Get the tile at the player's position
        TileBase tile = tilemap.GetTile(cellPosition);

        // Check if the tile is the finish tile
        if (tile == finishTile)
        {
            Debug.Log("Player reached the finish tile!");
            levelManager.CompleteLevel(); // Call CompleteLevel method from LevelManager
        }
    }

    void Shoot()
    {
        canShoot = false;

        // Instantiate the bullet at the spawn point and in the player's current rotation
        Instantiate(bulletPrefab, bulletSpawnPoint.position, transform.rotation);

        // Start cooldown timer
        StartCoroutine(ShootCooldown());
    }

    private IEnumerator ShootCooldown()
    {
        yield return new WaitForSeconds(shootCooldown);
        canShoot = true;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log("Player Health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Player has died!");
        // Add your game over logic here, e.g., restart the level or show a game over screen
        gameObject.SetActive(false); // Disable the player object
    }
}