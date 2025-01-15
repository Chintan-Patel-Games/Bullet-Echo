using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;  // Speed of the bullet
    public float lifetime = 0.8f;  // Time before the bullet is destroyed
    public string originTag;  // The tag of the shooter (e.g., "Player" or "Enemy")

    void Start()
    {
        Destroy(gameObject, lifetime);  // Destroy bullet after its lifetime
    }

    void Update()
    {
        if (originTag == "Player")
            transform.Translate(Vector3.up * speed * Time.deltaTime); // Move bullet forward
        else if (originTag == "Enemy")
            transform.Translate(Vector3.right * speed * Time.deltaTime); // Move bullet forward
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Avoid self-collision
        if (other.CompareTag(originTag)) return;

        // Handle player bullet hitting enemy
        if (originTag == "Player" && other.CompareTag("Enemy") && other is CircleCollider2D)
        {
            Destroy(other.gameObject);  // Destroy enemy
            Destroy(gameObject);  // Destroy bullet
        }

        // Handle enemy bullet hitting player
        if (originTag == "Enemy" && other.CompareTag("Player"))
        {
            // Handle player damage or game over logic here
            Destroy(gameObject);  // Destroy bullet
            PlayerController player = other.gameObject.GetComponent<PlayerController>();
            player.TakeDamage(20); // Deal 20 damage to the player
        }
    }
}