using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CannonButton : MonoBehaviour, IPointerDownHandler
{
    [Header("Cannon Spawn Settings")]
    public GameObject cannonPrefab;
    public Transform spawnPoint;

    [Header("Cooldown Settings")]
    public float cooldownDuration = 30f;
    private float cooldownTimer = 0f;
    private bool isCooldown = true;

    [Header("UI Elements")]
    public Image cooldownOverlay;
    public Image skillImage;

    [Header("Blink Settings")]
    public float blinkSpeed = 4f;
    private bool shouldBlink = false;

    void Start()
    {
       
        cooldownTimer = cooldownDuration;
        cooldownOverlay.fillAmount = 1f;
    }

    void Update()
    {
        if (isCooldown)
        {
            cooldownTimer -= Time.deltaTime;
            float fillAmount = cooldownTimer / cooldownDuration;
            cooldownOverlay.fillAmount = Mathf.Clamp01(fillAmount);

            if (cooldownTimer <= 0f)
            {
                isCooldown = false;
                cooldownOverlay.fillAmount = 0f;
                shouldBlink = true;
            }
        }

        
        if (shouldBlink && skillImage != null)
        {
            float alpha = Mathf.PingPong(Time.time * blinkSpeed, 0.5f) + 0.5f;
            Color newColor = skillImage.color;
            newColor.a = alpha;
            skillImage.color = newColor;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
       
        if (!shouldBlink)
            return;

 
        Instantiate(cannonPrefab, spawnPoint.position, spawnPoint.rotation);

       
        StartCooldown();
    }

    void StartCooldown()
    {
        isCooldown = true;
        cooldownTimer = cooldownDuration;
        cooldownOverlay.fillAmount = 1f;
        shouldBlink = false;

        
        if (skillImage != null)
        {
            Color resetColor = skillImage.color;
            resetColor.a = 1f;
            skillImage.color = resetColor;
        }
    }
}
