using UnityEngine;
using UnityEngine.UI;

public class EnemyTowerHealth : MonoBehaviour
{
    public int health;
    public int maxHealth = 50;
    public Slider slider;
    public Sprite[] towerSprites;
    public SpriteRenderer towerSr;

    private bool isDestroyed = false;

    void Start()
    {
        health = maxHealth;
        slider.maxValue = maxHealth;
        slider.value = health;
    }

    void Update()
    {
        if (isDestroyed) return; // hentikan update jika sudah hancur

        slider.value = health;

        if (health <= 0)
        {
            // Destroy slider agar UI hilang
            if (slider != null)
                Destroy(slider.gameObject);

            // Set sprite terakhir (rusak parah)
            if (towerSprites.Length > 0)
                towerSr.sprite = towerSprites[towerSprites.Length - 1];

            isDestroyed = true; // hentikan update lebih lanjut
        }
        else
        {
            float healthPercentage = (float)health / maxHealth;
            int spriteIndex = Mathf.Clamp(Mathf.FloorToInt(healthPercentage * towerSprites.Length), 0, towerSprites.Length - 1);
            towerSr.sprite = towerSprites[spriteIndex];
        }
    }

    public void TakeDamage(int amount)
    {
        if (isDestroyed) return; // abaikan jika sudah hancur

        health -= amount;
        health = Mathf.Clamp(health, 0, maxHealth);
    }
}
