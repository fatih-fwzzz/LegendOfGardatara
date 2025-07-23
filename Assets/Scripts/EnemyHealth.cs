using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 50;
    private int currentHealth;

    [Header("Knockback Settings")]
    public float knockbackForce = 5f;
    public float knockbackDelay = 0.15f;
    private bool isKnockedBack = false;

    [Header("Reference")]
    public RobotMovement robotMovement;
    public Rigidbody2D rb; // tambahkan jika ingin knockback fisik

    private void Start()
    {
        currentHealth = maxHealth;

        if (robotMovement == null)
            robotMovement = GetComponent<RobotMovement>();

        if (rb == null)
            rb = GetComponent<Rigidbody2D>();
    }

    /// <summary>
    /// Dipanggil saat enemy menerima damage (int).
    /// </summary>
    public void TakeDamage(int damage)
    {
        TakeDamage((float)damage);
    }

    /// <summary>
    /// Dipanggil saat enemy menerima damage (float).
    /// </summary>
    public void TakeDamage(float damage)
    {
        currentHealth -= Mathf.RoundToInt(damage);

        if (!isKnockedBack)
        {
            StartCoroutine(KnockbackDelay());
        }
    }

    /// <summary>
    /// Dipanggil saat menerima damage dan knockback arah attacker.
    /// </summary>
    public void TakeDamage(int damage, Vector2 attackerPosition)
    {
        currentHealth -= damage;

        if (!isKnockedBack)
        {
            Vector2 knockDirection = ((Vector2)transform.position - attackerPosition).normalized;
            StartCoroutine(ApplyKnockback(knockDirection));
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// Knockback menggunakan delay (non-arah).
    /// </summary>
    private IEnumerator KnockbackDelay()
    {
        isKnockedBack = true;

        if (robotMovement != null)
            robotMovement.enabled = false;

        yield return new WaitForSeconds(knockbackDelay);

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            if (robotMovement != null)
                robotMovement.enabled = true;

            isKnockedBack = false;
        }
    }

    /// <summary>
    /// Knockback dengan arah attacker.
    /// </summary>
    private IEnumerator ApplyKnockback(Vector2 direction)
    {
        isKnockedBack = true;

        if (robotMovement != null)
            robotMovement.enabled = false;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.AddForce(direction * knockbackForce, ForceMode2D.Impulse);
        }

        yield return new WaitForSeconds(knockbackDelay);

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            if (robotMovement != null)
                robotMovement.enabled = true;

            isKnockedBack = false;
        }
    }

    /// <summary>
    /// Menghandle kematian enemy.
    /// </summary>
    private void Die()
    {
        Destroy(gameObject);
    }
}
