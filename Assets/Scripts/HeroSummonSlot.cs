using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class HeroSummonSlot : MonoBehaviour
{
    [Header("Hero Settings")]
    public GameObject heroPrefab;
    public Transform[] spawnPoints; // ✅ Ganti dari single spawnPoint menjadi array
    public int summonCost = 50;
    public float cooldownTime = 5f; // lama cooldown

    [Header("UI Components")]
    public Image heroImage;
    public TextMeshProUGUI priceText;
    public Slider cooldownSlider; // Slider cooldown berjalan

    [Header("Dependencies")]
    public EnergyManager energyManager; // drag manual via Inspector

    private Button button;
    private bool isCooldown = false;

    void Start()
    {
        if (energyManager == null)
            Debug.LogError("[HeroSummonSlot] EnergyManager belum diassign di Inspector!");

        button = GetComponent<Button>() ?? GetComponentInChildren<Button>();
        if (button != null)
            button.onClick.AddListener(TrySummonHero);
        else
            Debug.LogError("[HeroSummonSlot] Button tidak ditemukan!");

        if (priceText != null)
            priceText.text = summonCost + "$";

        if (cooldownSlider != null)
        {
            cooldownSlider.minValue = 0;
            cooldownSlider.maxValue = cooldownTime;
            cooldownSlider.value = 0;
            cooldownSlider.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (energyManager == null) return;

        if (!isCooldown)
        {
            bool canAfford = energyManager.currentEnergy >= summonCost;
            if (button != null) button.interactable = canAfford;
            if (heroImage != null) heroImage.color = canAfford ? Color.white : Color.gray;
        }
    }

    void TrySummonHero()
    {
        if (isCooldown || energyManager == null) return;

        if (energyManager.TryUseEnergy(summonCost))
        {
            Debug.Log($"[HeroSummonSlot] Hero summoned, energy used: {summonCost}");

            if (heroPrefab != null && spawnPoints != null && spawnPoints.Length > 0)
            {
                // ✅ Pilih spawn point secara random
                int randomIndex = Random.Range(0, spawnPoints.Length);
                Transform spawnPoint = spawnPoints[randomIndex];

                Instantiate(heroPrefab, spawnPoint.position, Quaternion.identity);
            }
            else
            {
                Debug.LogError("[HeroSummonSlot] HeroPrefab belum diassign atau SpawnPoints kosong!");
            }

            StartCoroutine(CooldownCoroutine());
        }
        else
        {
            Debug.Log("[HeroSummonSlot] Tidak cukup energy untuk summon.");
        }
    }

    IEnumerator CooldownCoroutine()
    {
        isCooldown = true;
        if (button != null) button.interactable = false;
        if (heroImage != null) heroImage.color = Color.gray;

        if (cooldownSlider != null)
        {
            cooldownSlider.gameObject.SetActive(true);
            cooldownSlider.value = 0f;
        }

        float elapsed = 0f;
        while (elapsed < cooldownTime)
        {
            elapsed += Time.deltaTime;
            if (cooldownSlider != null)
                cooldownSlider.value = elapsed;
            yield return null;
        }

        if (cooldownSlider != null)
            cooldownSlider.gameObject.SetActive(false);

        isCooldown = false;

        if (energyManager != null && energyManager.currentEnergy >= summonCost)
        {
            if (button != null) button.interactable = true;
            if (heroImage != null) heroImage.color = Color.white;
        }
        else
        {
            if (button != null) button.interactable = false;
            if (heroImage != null) heroImage.color = Color.gray;
        }
    }
}
