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
    public int BulletCount { get; private set; } = 5; // Example default bullet count

    [SerializeField] private GameObject enemiesParent; // Array of enemies in the level
    [SerializeField] private LevelManager levelManager; // Reference to the LevelManager
    [SerializeField] private UIController uiController; // Reference to the UIController

    private bool canShoot = true;
    private bool isDead = false;
    private bool isMoving = false; // To track if the player is currently moving
    private bool isRotating = false; // To track if the player is currently rotating

    void Update()
    {
        if (!isMoving && !isRotating && !isDead) // Allow input only when not moving, rotating, or dead
        {
            if (Input.GetKey(KeyCode.W)) StartCoroutine(Move(transform.up));
            else if (Input.GetKey(KeyCode.S)) StartCoroutine(Move(-transform.up));
            else if (Input.GetKey(KeyCode.A)) StartCoroutine(Rotate(90f));
            else if (Input.GetKey(KeyCode.D)) StartCoroutine(Rotate(-90f));
        }

        // Handle shooting
        if (Input.GetKey(KeyCode.Space) && canShoot && !isRotating && !isDead)
        {
            Shoot();
        }

        AreAllEnemiesDead();
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
        if (tile == finishTile && AreAllEnemiesDead())
        {
            uiController.TriggerGamewin(); // Call CompleteLevel method from LevelManager
        }
    }

    public bool AreAllEnemiesDead()
    {
        // Get all children of the "Enemies" parent GameObject
        Transform[] enemyTransforms = enemiesParent.GetComponentsInChildren<Transform>();

        foreach (Transform enemyTransform in enemyTransforms)
        {
            // Ignore the parent GameObject itself
            if (enemyTransform != enemiesParent)
            {
                EnemyAI enemy = enemyTransform.GetComponent<EnemyAI>();
                if (enemy != null) // Check if the enemy is alive
                {
                    return false; // If at least one enemy is alive, return false
                }
            }
        }

        return true; // If all enemies are dead, return true
    }

    void Shoot()
    {
        if (BulletCount <= 0)
        {
            SoundManager.Instance.PlaySFX(SFXList.No_Ammo);
            return;
        }

        canShoot = false;

        // Instantiate the bullet at the spawn point and in the player's current rotation
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, transform.rotation);
        bullet.GetComponent<Bullet>().Initialize(true); // For player bullets

        SoundManager.Instance.PlaySFX(SFXList.Shoot);

        // Decrease the bullet count
        BulletCount--;

        // Start cooldown timer
        StartCoroutine(ShootCooldown());
    }

    private IEnumerator ShootCooldown()
    {
        yield return new WaitForSeconds(shootCooldown);
        canShoot = true;
    }

    // Method to add ammo
    public void AddAmmo(int amount)
    {
        BulletCount += amount;
    }

    public void Die()
    {
        if (isDead) return; // Avoid multiple death triggers
        isDead = true;

        gameObject.SetActive(false); // Disable the player object

        // Trigger Game Over UI
        if (uiController != null)
        {
            uiController.TriggerGameOver();
        }
    }
}