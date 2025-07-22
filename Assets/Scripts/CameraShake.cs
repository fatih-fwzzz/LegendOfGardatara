using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    public static CameraShake instance; // singleton agar mudah dipanggil

    private Vector3 originalPos;
    private Coroutine currentShake;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    /// <summary>
    /// Memanggil shake kamera dengan durasi dan kekuatan tertentu.
    /// </summary>
    /// <param name="duration">Lama shake (detik)</param>
    /// <param name="magnitude">Kekuatan shake</param>
    public void Shake(float duration, float magnitude)
    {
        if (currentShake != null)
        {
            StopCoroutine(currentShake);
        }
        currentShake = StartCoroutine(ShakeCoroutine(duration, magnitude));
    }

    private IEnumerator ShakeCoroutine(float duration, float magnitude)
    {
        originalPos = transform.localPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float percentComplete = elapsed / duration;

            float damper = 1.0f - Mathf.Clamp(4.0f * percentComplete - 3.0f, 0.0f, 1.0f);

            float x = (Random.value * 2.0f - 1.0f) * magnitude * damper;
            float y = (Random.value * 2.0f - 1.0f) * magnitude * damper;

            Vector3 targetPos = originalPos + new Vector3(x, y, 0);
            transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, 0.5f);

            yield return null;
        }

        transform.localPosition = originalPos;
        currentShake = null;
    }
}
