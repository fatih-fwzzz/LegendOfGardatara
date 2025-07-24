using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TowerHealthAnimated : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100;
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

    [Header("Defeat Settings")]
    public float defeatDelay = 2f; // delay sebelum defeat popup muncul

    private bool hasDefeated = false;
    private DefeatManager defeatManager; // reference ke DefeatManager

    private void Start()
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

        // Cari DefeatManager secara otomatis di scene
        defeatManager = FindAnyObjectByType<DefeatManager>();
        if (defeatManager == null)
        {
            Debug.LogWarning("[TowerHealthAnimated] DefeatManager tidak ditemukan di scene, defeat popup tidak akan muncul saat tower hancur.");
        }
    }

    private void UpdateTowerSprite()
    {
        if (towerSprites.Length == 0 || towerSr == null)
            return;

        if (currentHealth <= 0)
        {
            towerSr.sprite = towerSprites[0];
        }
        else
        {
            float healthPercent = (float)currentHealth / maxHealth;
            int spriteIndex = Mathf.Clamp(Mathf.FloorToInt(healthPercent * towerSprites.Length), 0, towerSprites.Length - 1);
            towerSr.sprite = towerSprites[spriteIndex];
        }
    }

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (healthSlider != null)
            healthSlider.value = currentHealth;

        UpdateTowerSprite();

        // Kamera shake
        if (cameraShake != null)
            cameraShake.Shake(0.15f, 0.15f);

        // Flash saat terkena damage
        if (flashCoroutine != null)
            StopCoroutine(flashCoroutine);
        flashCoroutine = StartCoroutine(FlashEffect());

        // Tampilkan api saat health < 30%
        if (currentHealth < maxHealth * 0.3f)
        {
            if (fireEffectInstance == null && fireEffectPrefab != null)
            {
                fireEffectInstance = Instantiate(fireEffectPrefab, transform.position, Quaternion.identity, transform);
            }
        }

        // Trigger defeat jika health 0
        if (currentHealth <= 0 && !hasDefeated)
        {
            hasDefeated = true;
            StartCoroutine(TriggerDefeatAfterDelay());
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

    private IEnumerator TriggerDefeatAfterDelay()
    {
        yield return new WaitForSeconds(defeatDelay);

        // Play defeat BGM 
        if (AudioManager.Instance != null && AudioManager.Instance.defeatBGM != null)
        {
            AudioManager.Instance.PlayBGM(AudioManager.Instance.defeatBGM);
        }

        // Trigger defeat popup jika DefeatManager ketemu
        if (defeatManager != null)
        {
            defeatManager.TriggerDefeat();
        }
        else
        {
            Debug.LogWarning("defeat manager nggak ketemu");
        }
    }
}
