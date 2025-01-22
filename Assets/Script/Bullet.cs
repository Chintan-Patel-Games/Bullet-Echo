using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 10f;  // Speed of the bullet
    [SerializeField] private float lifetime = 0.8f;  // Time before the bullet is destroyed

    private bool isPlayerBullet;  // Determines if the bullet belongs to the player

    public void Initialize(bool playerBullet)
    {
        isPlayerBullet = playerBullet; // Set ownership when bullet is instantiated
    }

    void Start()
    {
        Destroy(gameObject, lifetime);  // Destroy bullet after its lifetime
    }

    void Update()
    {
        // Move the bullet based on its ownership
        if (isPlayerBullet)
            transform.Translate(Vector3.up * speed * Time.deltaTime); // Player bullet moves upward
        else
            transform.Translate(Vector3.right * speed * Time.deltaTime); // Enemy bullet moves rightward
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Handle player bullet hitting enemy
        if (isPlayerBullet && other.TryGetComponent<EnemyAI>(out var enemy))
        {
            Destroy(enemy.gameObject);  // Destroy enemy
            Destroy(gameObject);  // Destroy bullet
        }

        // Handle enemy bullet hitting player
        if (!isPlayerBullet && other.TryGetComponent<PlayerController>(out var player))
        {
            player.Die();  // Trigger player death
            Destroy(gameObject);  // Destroy bullet
        }
    }
}