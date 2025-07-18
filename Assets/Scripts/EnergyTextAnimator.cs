using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class EnergyTextAnimator : MonoBehaviour
{
    public TextMeshProUGUI energyText;
    public float popScale = 1f;
    public float animDuration = 0.3f;
    public Color glowColor = Color.yellow; // warna glow saat animasi

    public CanvasGroup glowCanvasGroup;
    public Image glowImage; // glow image di energyText
    public Color glowImageColor = Color.yellow; // warna glow saat animasi text
    public float glowMaxAlpha = 0.8f;

    private Color originalColor;
    private Vector3 originalScale;
    private Color originalGlowImageColor;

    private void Awake()
    {
        if (energyText == null)
            energyText = GetComponent<TextMeshProUGUI>();

        originalColor = energyText.color;
        originalScale = energyText.transform.localScale;

        if (glowImage != null)
            originalGlowImageColor = glowImage.color;

        if (glowCanvasGroup != null)
            glowCanvasGroup.alpha = 0f;
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

        while (timer < animDuration)
        {
            timer += Time.deltaTime;
            float t = timer / animDuration;

            energyText.transform.localScale = Vector3.Lerp(originalScale, targetScale, t);
            energyText.color = Color.Lerp(originalColor, glowColor, t);

            if (glowCanvasGroup != null)
                glowCanvasGroup.alpha = Mathf.Lerp(0f, glowMaxAlpha, t);

            if (glowImage != null)
                glowImage.color = Color.Lerp(originalGlowImageColor, glowImageColor, t);

            yield return null;
        }

        timer = 0f;
        while (timer < animDuration)
        {
            timer += Time.deltaTime;
            float t = timer / animDuration;

            energyText.transform.localScale = Vector3.Lerp(targetScale, originalScale, t);
            energyText.color = Color.Lerp(glowColor, originalColor, t);

            if (glowCanvasGroup != null)
                glowCanvasGroup.alpha = Mathf.Lerp(glowMaxAlpha, 0f, t);

            if (glowImage != null)
                glowImage.color = Color.Lerp(glowImageColor, originalGlowImageColor, t);

            yield return null;
        }

        energyText.transform.localScale = originalScale;
        energyText.color = originalColor;

        if (glowCanvasGroup != null)
            glowCanvasGroup.alpha = 0f;

        if (glowImage != null)
            glowImage.color = originalGlowImageColor;
    }
}

