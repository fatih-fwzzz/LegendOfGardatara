using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class EnemyTowerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 50;
    public int currentHealth;

    [Header("Sprite Settings")]
    public SpriteRenderer towerSr;
    public Sprite[] towerSprites;

    [Header("UI")]
    public Slider healthSlider;

    [Header("Effects")]
    public CameraShake cameraShake;
    public GameObject fireEffectPrefab;
    private GameObject fireEffectInstance;

    [Header("Flash Settings")]
    public Color flashColor = Color.white;
    public float flashDuration = 0.1f;
    private Color originalColor;
    private Coroutine flashCoroutine;

    private bool isDestroyed = false;

    void Start()
    {
        currentHealth = maxHealth;

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }

        if (towerSr != null)
            originalColor = towerSr.color;

        UpdateTowerSprite();
    }

    void UpdateTowerSprite()
    {
        if (towerSprites.Length == 0 || towerSr == null)
            return;

        if (currentHealth <= 0)
        {
            towerSr.sprite = towerSprites[0]; // Saat hancur tampilkan sprite element 0
        }
        else
        {
            float healthPercent = (float)currentHealth / maxHealth;
            int spriteIndex = Mathf.Clamp(
                Mathf.FloorToInt(healthPercent * towerSprites.Length),
                0,
                towerSprites.Length - 1
            );
            towerSr.sprite = towerSprites[spriteIndex];
        }
    }

    public void TakeDamage(int amount)
    {
        if (isDestroyed)
            return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (healthSlider != null)
            healthSlider.value = currentHealth;

        UpdateTowerSprite();

        // Efek shake
        if (cameraShake != null)
            cameraShake.Shake(0.15f, 0.15f);

        // Flash warna saat kena hit
        if (flashCoroutine != null)
            StopCoroutine(flashCoroutine);
        flashCoroutine = StartCoroutine(FlashEffect());

        // Tampilkan api/asap jika health < 30%
        if (currentHealth < maxHealth * 0.3f)
        {
            if (fireEffectInstance == null && fireEffectPrefab != null)
            {
                fireEffectInstance = Instantiate(
                    fireEffectPrefab,
                    transform.position,
                    Quaternion.identity,
                    transform
                );
            }
        }

        // Set isDestroyed tanpa Destroy tower
        if (currentHealth <= 0)
        {
            isDestroyed = true;
            Destroy(healthSlider.gameObject);
            Destroy(gameObject);
            Debug.Log("[EnemyTowerHealth] Tower musuh hancur.");
        }
    }

    private IEnumerator FlashEffect()
    {
        if (towerSr != null)
        {
            towerSr.color = flashColor;
            yield return new WaitForSeconds(flashDuration);
            towerSr.color = originalColor;
        }
    }
}
