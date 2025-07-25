using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class WinPopupManager : MonoBehaviour
{
    [Header("References")]
    public EnemyTowerHealth enemyTowerHealth;
    public HeroStateMachine heroStateMachine;
    public BossController bossController;

    [Header("Win Popup")]
    public GameObject winPopupPanel;              // drag WinPopupPanel
    public TextMeshProUGUI winText;              // drag TMP Text "YOU WIN!"
    public Button mainMenuButton;                // drag MainMenuButton

    [Header("Audio")]
    public AudioClip winBGM;

    private bool hasWon = false;

    private void Start()
    {
        if (winPopupPanel != null)
            winPopupPanel.SetActive(false);

        // Auto-assign MainMenuButton jika lupa assign
        if (mainMenuButton == null)
        {
            GameObject foundButton = GameObject.Find("MainMenuButton");
            if (foundButton != null)
            {
                mainMenuButton = foundButton.GetComponent<Button>();
                Debug.Log("[WinPopupManager] MainMenuButton auto-assigned via GameObject.Find.");
            }
            else
            {
                Debug.LogWarning("[WinPopupManager] MainMenuButton not assigned or found in the scene.");
            }
        }

        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(ReturnToMainMenu);
    }

    private void Update()
    {
        if (!hasWon && enemyTowerHealth.currentHealth <= 0)
        {
            WinCondition();
        }
    }

    private void WinCondition()
    {
        hasWon = true;

        Debug.Log("[WinPopupManager] Victory triggered.");

        // Hero berhenti idle
        if (heroStateMachine != null)
            heroStateMachine.SetState(HeroState.Idle);

        // Boss kabur
        if (bossController != null)
            bossController.Flee();

        // Play BGM Win
        if (AudioManager.Instance != null && winBGM != null)
            AudioManager.Instance.PlayBGM(winBGM);

        // Setelah 3 detik tampilkan win popup
        Invoke(nameof(ShowWinPopup), 3f);
    }

    private void ShowWinPopup()
    {
        if (winPopupPanel != null)
            winPopupPanel.SetActive(true);

        if (winText != null)
            winText.text = "YOU WIN!";

        Time.timeScale = 0f;
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Main Menu");
    }
}
