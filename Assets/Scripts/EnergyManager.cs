using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class EnergyManager : MonoBehaviour
{
    [Header("Energy Settings")]
    public float currentEnergy = 0f;
    public float maxEnergy;
    public float regenAmount = 1f;

    [Header("Stage Settings")]
    public int maxEnergyStage = 1;
    public int maxStageLimit = 8;

    [Header("UI")]
    public TextMeshProUGUI energyText;

    [Header("Animator (Optional)")]
    public EnergyTextAnimator energyTextAnimator;

    [Header("Regeneration Settings")]
    public float baseInterval = 0.2f;
    private float regenInterval;
    private Coroutine regenCoroutine;
    private Coroutine uiUpdateCoroutine;

    private bool energyChanged = false; // flag update UI stabil

    void Start()
    {
        CalculateMaxEnergy();
        UpdateEnergyText();
        UpdateRegenSpeed();
        uiUpdateCoroutine = StartCoroutine(UpdateEnergyTextCoroutine());
    }

    private IEnumerator RegenerateEnergyCoroutine()
    {
        while (true)
        {
            currentEnergy += regenAmount;
            if (currentEnergy > maxEnergy)
                currentEnergy = maxEnergy;

            energyChanged = true; // flag UI perlu update

            yield return new WaitForSeconds(regenInterval);
        }
    }

    private IEnumerator UpdateEnergyTextCoroutine()
    {
        while (true)
        {
            if (energyChanged)
            {
                UpdateEnergyText();
                energyChanged = false;
            }
            yield return new WaitForSeconds(0.1f); // update UI stabil setiap 0.1 detik
        }
    }

    public bool TryUseEnergy(float amount)
    {
        if (currentEnergy >= amount)
        {
            currentEnergy -= amount;
            energyChanged = true; // tandai perlu update
            Debug.Log($"[EnergyManager] Energy used: {amount}, remaining: {currentEnergy}");
            return true;
        }
        else
        {
            Debug.Log("[EnergyManager] Not enough energy.");
            return false;
        }
    }

    private void CalculateMaxEnergy()
    {
        switch (maxEnergyStage)
        {
            case 1: maxEnergy = 100f; break;
            case 2: maxEnergy = 150f; break;
            case 3: maxEnergy = 200f; break;
            case 4: maxEnergy = 250f; break;
            case 5: maxEnergy = 300f; break;
            case 6: maxEnergy = 350f; break;
            case 7: maxEnergy = 400f; break;
            case 8: maxEnergy = 450f; break;
            default: maxEnergy = 450f; break;
        }
        Debug.Log($"[EnergyManager] Calculated MaxEnergy for Stage {maxEnergyStage}: {maxEnergy}");
    }

    public bool UpgradeMaxEnergyStage()
    {
        if (maxEnergyStage < maxStageLimit)
        {
            maxEnergyStage++;
            CalculateMaxEnergy();
            energyChanged = true;
            PlayEnergyPopAnimation();
            UpdateRegenSpeed();
            Debug.Log($"[EnergyManager] Upgraded to Stage {maxEnergyStage}, maxEnergy now {maxEnergy}");
            return true;
        }
        else
        {
            Debug.Log("[EnergyManager] Already at maximum stage.");
            return false;
        }
    }

    public void UpdateEnergyText()
    {
        if (energyText != null)
            energyText.text = $"{(int)currentEnergy} / {(int)maxEnergy}";
    }

    private void PlayEnergyPopAnimation()
    {
        if (energyTextAnimator != null)
        {
            energyTextAnimator.PlayPopAnimation();
        }
    }

    private void UpdateRegenSpeed()
    {
        regenInterval = Mathf.Max(0.05f, baseInterval / maxEnergyStage);

        if (regenCoroutine != null)
            StopCoroutine(regenCoroutine);
        regenCoroutine = StartCoroutine(RegenerateEnergyCoroutine());

        Debug.Log($"[EnergyManager] Regen interval updated to {regenInterval} seconds per tick at Stage {maxEnergyStage}");
    }
}
