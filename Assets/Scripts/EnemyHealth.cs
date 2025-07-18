using UnityEngine;
using System.Collections;


public class EnemyHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 50;
    private int currentHealth;

    [Header("Knockback Settings")]
    public float knockbackDelay = 0.15f;
    private bool isKnockedBack = false;

    [Header("Reference")]
    public RobotMovement robotMovement;

    private void Start()
    {
        currentHealth = maxHealth;

        if (robotMovement == null)
        {
            robotMovement = GetComponent<RobotMovement>();
        }
    }


    // Dipanggil saat enemy menerima damage (int).

    public void TakeDamage(int damage)
    {
        TakeDamage((float)damage);
    }

    // Dipanggil saat enemy menerima damage (float).

    public void TakeDamage(float damage)
    {
        currentHealth -= Mathf.RoundToInt(damage);

        if (!isKnockedBack)
        {
            StartCoroutine(KnockbackDelay());
        }
    }


    // Memberikan efek knockback delay dan memeriksa kematian.
  
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

    
    // Menghandle proses kematian enemy.
  
    private void Die()
    {
        Destroy(gameObject);
       
    }
}
