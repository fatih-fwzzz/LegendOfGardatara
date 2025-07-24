using UnityEngine;

public class HeroHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int health;
    public int maxHealth = 100;

    [Header("Sprites (Optional)")]
    public Sprite[] heroSprites;
    public SpriteRenderer heroSr;

    private void Start()
    {
        health = maxHealth;
    }

    private void Update()
    {
        if (health <= 0)
        {
            Destroy(gameObject);
        }
        else if (heroSprites != null && heroSprites.Length > 0 && heroSr != null)
        {
            float healthPercentage = (float)health / maxHealth;
            int spriteIndex = Mathf.Clamp(Mathf.FloorToInt(healthPercentage * heroSprites.Length - 1), 0, heroSprites.Length - 1);
            heroSr.sprite = heroSprites[spriteIndex];
        }
    }

    /// <summary>
    /// Dipanggil attacker untuk mengurangi HP hero.
    /// </summary>
    public void TakeDamage(int amount)
    {
        health -= amount;
        health = Mathf.Clamp(health, 0, maxHealth);
        Debug.Log($"{gameObject.name} HP: {health}/{maxHealth}");
    }
}
