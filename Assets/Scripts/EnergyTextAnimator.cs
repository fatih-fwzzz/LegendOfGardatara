using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class EnergyTextAnimator : MonoBehaviour
{
    public TextMeshProUGUI energyText;
    public float popScale = 0.8f;
    public float animDuration = 0.3f;
    public Color glowColor = Color.yellow; 

    public Image glowImage; 
    public Color glowImageColor = Color.yellow; 

    private Color originalColor;
    private Vector3 originalScale;
    private Color originalGlowImageColor;
    private Vector3 originalGlowImageScale;

    private void Awake()
    {
        if (energyText == null)
            energyText = GetComponent<TextMeshProUGUI>();

        originalColor = energyText.color;
        originalScale = energyText.transform.localScale;

        if (glowImage != null)
        {
            originalGlowImageColor = glowImage.color;
            originalGlowImageScale = glowImage.transform.localScale;
            glowImage.enabled = true; 
        }
    }

    public void PlayPopAnimation()
    {
        StopAllCoroutines();
        StartCoroutine(PopCoroutine());
    }

    private IEnumerator PopCoroutine()
    {
        float timer = 0f;
        Vector3 targetScale = originalScale * popScale;
        Vector3 targetGlowScale = originalGlowImageScale * popScale;

        while (timer < animDuration)
        {
            timer += Time.deltaTime;
            float t = timer / animDuration;

            energyText.transform.localScale = Vector3.Lerp(originalScale, targetScale, t);
            energyText.color = Color.Lerp(originalColor, glowColor, t);

            if (glowImage != null)
            {
                glowImage.transform.localScale = Vector3.Lerp(originalGlowImageScale, targetGlowScale, t);
                glowImage.color = Color.Lerp(originalGlowImageColor, glowImageColor, t);
            }

            yield return null;
        }

        timer = 0f;
        while (timer < animDuration)
        {
            timer += Time.deltaTime;
            float t = timer / animDuration;

            energyText.transform.localScale = Vector3.Lerp(targetScale, originalScale, t);
            energyText.color = Color.Lerp(glowColor, originalColor, t);

            if (glowImage != null)
            {
                glowImage.transform.localScale = Vector3.Lerp(targetGlowScale, originalGlowImageScale, t);
                glowImage.color = Color.Lerp(glowImageColor, originalGlowImageColor, t);
            }

            yield return null;
        }

        // Ensure exact reset after lerp
        energyText.transform.localScale = originalScale;
        energyText.color = originalColor;

        if (glowImage != null)
        {
            glowImage.transform.localScale = originalGlowImageScale;
            glowImage.color = originalGlowImageColor;
        }
    }
}
