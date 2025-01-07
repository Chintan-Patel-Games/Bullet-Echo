using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;  // Speed of the bullet
    public float lifetime = 0.8f;  // Time before the bullet is destroyed

    void Start()
    {
        Destroy(gameObject, lifetime);  // Destroy bullet after its lifetime
    }

    void Update()
    {
        // Move bullet forward
        transform.Translate(Vector3.up * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") && other as CircleCollider2D)
        {
            Debug.Log("Enemy hit!");
            Destroy(other.gameObject);  // Destroy enemy
            Destroy(gameObject);  // Destroy bullet
        }
    }
}