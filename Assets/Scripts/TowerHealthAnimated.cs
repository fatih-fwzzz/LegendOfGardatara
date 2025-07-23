using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    public CameraShake cameraShake; // Drag CameraShake
    public GameObject fireEffectPrefab; // Prefab api/asap
    private GameObject fireEffectInstance; // Instance api/asap aktif

    [Header("Flash Settings")]
    public Color flashColor = Color.white; // ✅ Warna flash yang dapat dipilih di Inspector
    public float flashDuration = 0.1f; // durasi flash
    private Color originalColor;
    private Coroutine flashCoroutine;

    [Header("Defeat Scene Settings")]
    public string defeatSceneName = "DefeatScene"; // Ganti nama sesuai scene defeat Anda

    private void Start()
    {
        currentHealth = maxHealth;

        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }

        if (towerSr != null)
        {
            originalColor = towerSr.color;
        }

        UpdateTowerSprite();
    }

    private void UpdateTowerSprite()
    {
        if (towerSprites.Length == 0 || towerSr == null)
            return;

        if (currentHealth <= 0)
        {
            towerSr.sprite = towerSprites[0]; // Saat hancur tampilkan sprite element 0 sesuai permintaan
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

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }

        UpdateTowerSprite();

        // ✅ Efek shake kamera saat kena hit
        if (cameraShake != null)
        {
            cameraShake.Shake(0.15f, 0.15f);
        }

        // ✅ Flash warna custom saat kena hit
        if (flashCoroutine != null)
            StopCoroutine(flashCoroutine);
        flashCoroutine = StartCoroutine(FlashEffect());

        // ✅ Tampilkan api/asap saat health < 30%
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

        // ✅ Trigger defeat scene saat health = 0
        if (currentHealth <= 0)
        {
            Invoke(nameof(TriggerDefeatScene), 2f); // delay agar terlihat
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

    private void TriggerDefeatScene()
    {
        if (!string.IsNullOrEmpty(defeatSceneName))
        {
            SceneManager.LoadScene(defeatSceneName);
        }
        else
        {
            Debug.LogWarning("[TowerHealthAnimated] Defeat scene name belum diisi di inspector.");
        }
    }
}
