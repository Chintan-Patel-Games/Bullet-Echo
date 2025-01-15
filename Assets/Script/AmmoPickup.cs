using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    [SerializeField] private int ammoAmount = 5; // Amount of ammo to add to the player

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the object colliding is the player
        PlayerController player = collision.GetComponent<PlayerController>();
        if (player != null)
        {
            // Add ammo to the player
            player.AddAmmo(ammoAmount);

            // Destroy the pickup game object
            Destroy(gameObject);
        }
    }
}