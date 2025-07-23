using UnityEngine;

public class HeroHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int health;
    public int maxHealth = 100;

    [Header("Sprites (Optional)")]
    public Sprite[] heroSprites;
    public SpriteRenderer heroSr;

    private HeroStateMachine stateMachine;
    private bool isDefeated = false;

    private void Awake()
    {
        health = maxHealth;
        stateMachine = GetComponent<HeroStateMachine>();
    }

    private void Update()
    {
        // Don't do anything if defeated
        if (isDefeated)
            return;

        // The sprite-changing logic can stay, but the Destroy call is removed.
        if (heroSprites != null && heroSprites.Length > 0 && heroSr != null)
        {
            float healthPercentage = (float)health / maxHealth;
            int spriteIndex = Mathf.Clamp(
                Mathf.FloorToInt(healthPercentage * heroSprites.Length - 1),
                0,
                heroSprites.Length - 1
            );
            heroSr.sprite = heroSprites[spriteIndex];
        }
    }

    /// <summary>
    /// Dipanggil attacker untuk mengurangi HP hero.
    /// </summary>
    public void TakeDamage(int amount)
    {
        if (isDefeated)
            return; // Stop if already defeated

        health -= amount;
        health = Mathf.Clamp(health, 0, maxHealth);
        Debug.Log($"{gameObject.name} HP: {health}/{maxHealth}");

        // --- THE FIX ---
        // Check for defeat and tell the state machine to handle it
        if (health <= 0)
        {
            isDefeated = true;
            if (stateMachine != null)
            {
                stateMachine.OnDefeated();
            }
            else
            {
                // Fallback if no state machine is found
                Destroy(gameObject);
            }
        }
    }
}
