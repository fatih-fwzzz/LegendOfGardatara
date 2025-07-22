using UnityEngine;

public class CannonProjectile : MonoBehaviour
{
    [Header("Settings")]
    public float speed = 5f;
    public float damage = 50f;
    public GameObject explosionEffectPrefab; // opsional buat efek

    private Rigidbody2D rb;
    private bool hasCollided = false; // agar tidak double trigger

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Meluncur dengan lintasan busur ke kanan atas
        Vector2 launchDirection = new Vector2(0.7f, 1f).normalized;
        rb.linearVelocity = launchDirection * speed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (hasCollided)
            return;

        string collidedObjectName = collision.gameObject.name;
        Debug.Log($" Cannon collided with: {collidedObjectName}");

        // Jika terkena Enemy, berikan damage
        if (collision.gameObject.CompareTag("Enemy"))
        {
            hasCollided = true;

            EnemyHealth enemyHealth = collision.gameObject.GetComponentInParent<EnemyHealth>();
            if (enemyHealth != null)
            {
                Debug.Log($" Applying {damage} damage to {collidedObjectName}");
                enemyHealth.TakeDamage((int)damage);
            }
            else
            {
                Debug.LogWarning($" EnemyHealth not found on {collidedObjectName}");
            }

            ExplodeAndDestroy();
        }
        // Jika mengenai TowerPlayer, abaikan (tidak meledak, tidak hilang)
        else if (collision.gameObject.CompareTag("TowerPlayer"))
        {
            Debug.Log("Cannon hit TowerPlayer, ignoring collision.");
            // Tidak melakukan apa-apa, cannon tetap berjalan
        }
        // Jika terkena objek lain (ground, obstacle), meledak & hilang tanpa damage
        else
        {
            hasCollided = true;
            Debug.Log($" Cannon hit non-enemy object: {collidedObjectName}");
            ExplodeAndDestroy();
        }
    }

    private void ExplodeAndDestroy()
    {
        // Spawn efek ledakan jika ada
        if (explosionEffectPrefab != null)
        {
            Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}
