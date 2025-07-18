using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class CannonButtonManager : MonoBehaviour
{
    [Header("References")]
    public Button cannonButton;
    public GameObject cannonPrefab;
    public Transform spawnPoint; // tempat spawn meriam
    public TextMeshProUGUI buttonText; // TMP untuk countdown

    [Header("Cooldown Settings")]
    public float cooldownTime = 30f;
    private bool isCooldown = false;

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

   
    // Dipanggil saat tombol ditekan
 
    public void ActivateCannon()
    {
        if (!isCooldown)
        {
            Debug.Log("Cannon Button pressed, spawning cannon...");
            SpawnCannon();
            StartCoroutine(CooldownRoutine());
        }
        else
        {
            Debug.Log("Cannon Button pressed, but still on cooldown.");
        }
    }

 
    // Spawn cannonPrefab di spawnPoint
   
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

    
    // Menjalankan cooldown tombol dengan countdown text
   
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
    }
}
