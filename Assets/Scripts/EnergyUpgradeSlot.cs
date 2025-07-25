using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnergyUpgradeSlot : MonoBehaviour
{
    [Header("Dependencies")]
    public EnergyManager energyManager;
    public Button upgradeButton;        

    [Header("Upgrade Settings")]
    public int upgradeCost = 40;         
    public TextMeshProUGUI priceText;    
    public TextMeshProUGUI levelText;    

    private int currentLevel = 1;       

    private void Start()
    {
        priceText.text = upgradeCost + "$";
        UpdateLevelText();
    }

    private void Update()
    {
        if (energyManager != null && upgradeButton != null)
        {
            upgradeButton.interactable = energyManager.currentEnergy >= upgradeCost;
        }
    }

    public void TryUpgradeEnergy()
    {
        AudioManager.Instance.PlaySFX(AudioManager.Instance.upgradeSFX);
        Debug.Log($"[UpgradeSlot] Trying upgrade. CurrentEnergy: {energyManager.currentEnergy}, Cost: {upgradeCost}");

        if (energyManager.TryUseEnergy(upgradeCost))
        {
            bool upgraded = energyManager.UpgradeMaxEnergyStage();

            if (upgraded)
            {
                currentLevel++;
                UpdateLevelText();

                upgradeCost += 40;
                priceText.text = upgradeCost + "$";

                Debug.Log($"[UpgradeSlot] Upgrade successful. New Level: {currentLevel}, New Cost: {upgradeCost}");
            }
            else
            {
                Debug.Log("[UpgradeSlot] Already at maximum stage.");
            }
        }
        else
        {
            Debug.Log("[UpgradeSlot] Not enough energy for upgrade.");
        }
    }

    private void UpdateLevelText()
    {
        if (levelText != null)
        {
            levelText.text = "Level " + currentLevel;
        }
    }
}
