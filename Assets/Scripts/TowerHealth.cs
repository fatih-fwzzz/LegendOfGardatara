using UnityEngine;
using UnityEngine.UI;

public class TowerHealth : MonoBehaviour
{
    public int health;
    public int maxHealth = 50;
    public Slider slider;
    public Sprite[] towerSprites;
    public SpriteRenderer towerSr;

    void Start()
    {
        health = maxHealth;
        slider.maxValue = maxHealth;
    }

    void Update()
    {
        slider.value = health;
        if (health <= 0)
        {
            Destroy(slider.gameObject);
            Destroy(gameObject);
        }
        else
        {
            float healthPercentage = (float)health / maxHealth;
            int spriteIndex = Mathf.Clamp(Mathf.FloorToInt(healthPercentage * towerSprites.Length - 1), 0, towerSprites.Length - 1);
            towerSr.sprite = towerSprites[spriteIndex];
        }
    }

    // ✅ Tambahkan ini untuk menerima damage dari hero
    public void TakeDamage(int amount)
    {
        health -= amount;
        health = Mathf.Clamp(health, 0, maxHealth);
    }
}
