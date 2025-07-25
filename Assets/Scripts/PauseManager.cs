using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject pausePopupPanel; 
    public Button pauseButton;         
    public Button continueButton;     
    public Button escapeButton;
    public Button retryButton; 

    private bool isPaused = false;

    private void Start()
    {
        if (pauseButton != null)
            pauseButton.onClick.AddListener(TogglePause);

        if (continueButton != null)
            continueButton.onClick.AddListener(ResumeGame);

        if (escapeButton != null)
            escapeButton.onClick.AddListener(EscapeGame);

        if (retryButton != null)
            retryButton.onClick.AddListener(RetryGame);

        if (pausePopupPanel != null)
            pausePopupPanel.SetActive(false);

        Time.timeScale = 1f; // supaya tdk pause waktu start
    }

    private void TogglePause()
    {
        if (!isPaused)
            PauseGame();
        else
            ResumeGame();
    }

    private void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f; // freeze semua animasi
        if (pausePopupPanel != null)
            pausePopupPanel.SetActive(true);
    }

    private void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = 1f; // resume
        if (pausePopupPanel != null)
            pausePopupPanel.SetActive(false);
    }

    private void EscapeGame()
    {
        Time.timeScale = 1f; // kembali normal
        SceneManager.LoadScene("Main Menu");
    }

    private void RetryGame()
    {
        Time.timeScale = 1f; // resume waktu sebelum reload
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}