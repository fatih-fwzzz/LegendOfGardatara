using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class CannonButtonManager : MonoBehaviour
{
    [Header("References")]
    public Button cannonButton;
    public GameObject cannonPrefab;
    public Transform spawnPoint;
    public TextMeshProUGUI buttonText;

    [Header("Cooldown Settings")]
    public float cooldownTime = 30f;
    private bool isCooldown = false;

    [Header("Blink Settings")]
    public float blinkSpeed = 2f;    
    public float blinkAlphaMin = 0.3f; 

    private Coroutine blinkCoroutine;

    private void Start()
    {
        if (cannonButton != null)
        {
            cannonButton.onClick.AddListener(ActivateCannon);
        }
        else
        {
            Debug.LogWarning("Cannon Button belum diassign!");
        }

        if (buttonText != null)
        {
            buttonText.text = "FIRE!";
        }
    }

    public void ActivateCannon()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(AudioManager.Instance.shootCannonSFX);

        if (!isCooldown)
        {
            Debug.Log("Cannon Button pressed, spawning cannon...");
            StopBlinking(); 
            SpawnCannon();
            StartCoroutine(CooldownRoutine());
        }
        else
        {
            Debug.Log("Cannon Button pressed, but still on cooldown.");
        }
    }

    private void SpawnCannon()
    {
        if (cannonPrefab != null && spawnPoint != null)
        {
            Instantiate(cannonPrefab, spawnPoint.position, Quaternion.identity);
            Debug.Log("Cannon spawned at: " + spawnPoint.position);
        }
        else
        {
            Debug.LogWarning("CannonPrefab atau SpawnPoint belum diassign!");
        }
    }

    private IEnumerator CooldownRoutine()
    {
        isCooldown = true;
        cannonButton.interactable = false;

        float timer = cooldownTime;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            if (buttonText != null)
            {
                buttonText.text = "Cooldown: " + Mathf.Ceil(timer) + "s";
            }
            yield return null;
        }

        if (buttonText != null)
        {
            buttonText.text = "FIRE!";
        }

        cannonButton.interactable = true;
        isCooldown = false;

      
        StartBlinking();
    }

    private void StartBlinking()
    {
        if (blinkCoroutine != null)
            StopCoroutine(blinkCoroutine);

        blinkCoroutine = StartCoroutine(BlinkButtonRoutine());
    }

    private void StopBlinking()
    {
        if (blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
            blinkCoroutine = null;
        }

       
        Image buttonImage = cannonButton.GetComponent<Image>();
        if (buttonImage != null)
        {
            Color originalColor = buttonImage.color;
            originalColor.a = 1f;
            buttonImage.color = originalColor;
        }
    }

    private IEnumerator BlinkButtonRoutine()
    {
        Image buttonImage = cannonButton.GetComponent<Image>();
        if (buttonImage == null)
        {
            Debug.LogWarning("CannonButton tidak memiliki komponen Image untuk efek blink.");
            yield break;
        }

        Color originalColor = buttonImage.color;

        while (true) 
        {
            float alpha = (Mathf.Sin(Time.time * blinkSpeed) * 0.5f + 0.5f) * (1f - blinkAlphaMin) + blinkAlphaMin;
            Color newColor = originalColor;
            newColor.a = alpha;
            buttonImage.color = newColor;
            yield return null;
        }
    }
}
