using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class DefeatManager : MonoBehaviour
{
    [Header("References")]
    public GameObject defeatPopupPanel;
    public Button returnButton;         
    public Button retryButton;         
    public TextMeshProUGUI defeatText;  

    private bool hasLost = false;

    private void Start()
    {
        if (defeatPopupPanel != null)
            defeatPopupPanel.SetActive(false);

        if (returnButton != null)
            returnButton.onClick.AddListener(ReturnToMainMenu);

        if (retryButton != null)
            retryButton.onClick.AddListener(RetryLevel);
    }

    public void TriggerDefeat()
    {
        if (hasLost) return;
        hasLost = true;

        Time.timeScale = 0f; // pause game
        if (defeatText != null)
            defeatText.text = "Defeat";
        if (defeatPopupPanel != null)
            defeatPopupPanel.SetActive(true);

       
        AudioManager.Instance.PlayBGM(AudioManager.Instance.defeatBGM);
    }

    private void ReturnToMainMenu()
    {
        Time.timeScale = 1f; // resume sebelum load
        SceneManager.LoadScene("Main Menu"); 
    }

    private void RetryLevel()
    {
        Time.timeScale = 1f; // resume sebelum reload
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
}
