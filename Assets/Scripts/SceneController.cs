using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public GameObject[] scenes;
    public GameObject continueText;
    private int currSceneidx = 1;

    void Start()
    {
        Debug.Log($"Scene Count: {scenes.Length}");

        continueText.SetActive(false);
        scenes[0].SetActive(true);
        for (int i = 1; i < scenes.Length; i++)
            scenes[i].SetActive(i == 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKeyDown)
        {
            if (currSceneidx < scenes.Length)
            {
                Debug.Log($"Tapped! Activating Scene: {currSceneidx + 1}");
                scenes[currSceneidx].SetActive(true);

                currSceneidx++;
                if (currSceneidx == scenes.Length)
                {
                    continueText.SetActive(true);
                }
            }
            else
            {
                SceneManager.LoadSceneAsync(2);
            }
        }
    }
}
