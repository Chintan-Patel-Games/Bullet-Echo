using UnityEngine;
using UnityEngine.Rendering.Universal; // For Light2D
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private Vector2[] patrolPath; // Patrol points
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float rotationDuration = 0.2f;
    [SerializeField] private GameObject torch;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform bulletSpawnPoint;
    [SerializeField] private float shootCooldown = 0.4f;
    [SerializeField] private Color alertColor = Color.red;
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Transform player;

    private Light2D torchLight;
    private bool canShoot = true;

    void Start()
    {
        torchLight = torch.GetComponent<Light2D>();
        if (patrolPath.Length > 0)
            StartCoroutine(PatrolPath());
    }

    private IEnumerator PatrolPath()
    {
        while (true) // Patrol continuously
        {
            foreach (var target in patrolPath)
            {
                // Rotate and move to the patrol point
                yield return StartCoroutine(SmoothRotateToAngle(GetAngleTo(target)));
                yield return StartCoroutine(MoveToPosition(target));

                yield return new WaitForSeconds(0.5f); // Pause at each patrol point
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

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            TriggerAlert();
            Shoot();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            ResetAlert();
            canShoot = false;
        }
    }

    private void TriggerAlert()
    {
        torchLight.color = alertColor; // Change torch light color to alert
    }

    private void ResetAlert()
    {
        torchLight.color = normalColor; // Reset torch light color to normal
    }

    private void Shoot()
    {
        if (canShoot)
        {
            canShoot = false;
            GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, transform.rotation);
            bullet.GetComponent<Bullet>().Initialize(false); // For enemy bullets
            SoundManager.Instance.PlaySFX(SFXList.Shoot);
            StartCoroutine(ShootCooldown());
        }
    }

    private IEnumerator ShootCooldown()
    {
        yield return new WaitForSeconds(shootCooldown);
        canShoot = true;
    }
}