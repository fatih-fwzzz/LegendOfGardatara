using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneController : MonoBehaviour
{
    public GameObject[] scenes;
    public GameObject continueText;
    private int currSceneidx = 0;
    public float fadeDuration = 1f;
    public float slideDuration = 1f;

    private bool readyToContinue = false;
    private bool userTapped = false;

    private void Start()
    {
        continueText.SetActive(false);
        foreach (GameObject scene in scenes)
            scene.SetActive(false);

        StartCoroutine(PlaySlides());
    }

    private IEnumerator PlaySlides()
    {
        while (currSceneidx < scenes.Length)
        {
            GameObject currentSlide = scenes[currSceneidx];
            currentSlide.SetActive(true);

            SpriteRenderer sr = currentSlide.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                Color c = sr.color;
                c.a = 0f;
                sr.color = c;

                float timer = 0f;
                while (timer < fadeDuration)
                {
                    timer += Time.deltaTime;
                    c.a = Mathf.Clamp01(timer / fadeDuration);
                    sr.color = c;
                    yield return null;
                }
                c.a = 1f;
                sr.color = c;
            }

            float elapsed = 0f;
            while (elapsed < slideDuration)
            {
                if (userTapped)
                {
                    userTapped = false;
                    break; // skip waktu tunggu jika tap
                }
                elapsed += Time.deltaTime;
                yield return null;
            }

            currSceneidx++;
        }

        continueText.SetActive(true);
        readyToContinue = true;

        StartCoroutine(BlinkContinueText());
    }

    private IEnumerator BlinkContinueText()
    {
        CanvasGroup cg = continueText.GetComponent<CanvasGroup>();
        if (cg == null)
        {
            cg = continueText.AddComponent<CanvasGroup>();
        }

        while (true)
        {
            // Fade In
            float timer = 0f;
            while (timer < 0.5f)
            {
                timer += Time.deltaTime;
                cg.alpha = Mathf.Lerp(0f, 1f, timer / 0.5f);
                yield return null;
            }
            cg.alpha = 1f;

            yield return new WaitForSeconds(0.3f);

            // Fade Out
            timer = 0f;
            while (timer < 0.5f)
            {
                timer += Time.deltaTime;
                cg.alpha = Mathf.Lerp(1f, 0f, timer / 0.5f);
                yield return null;
            }
            cg.alpha = 0f;

            yield return new WaitForSeconds(0.3f);
        }
    }

    private void Update()
    {
        if (readyToContinue && Input.anyKeyDown)
        {
            SceneManager.LoadSceneAsync(2); 
        }
        else if (!readyToContinue && Input.anyKeyDown)
        {
            userTapped = true; 
        }
    }
}