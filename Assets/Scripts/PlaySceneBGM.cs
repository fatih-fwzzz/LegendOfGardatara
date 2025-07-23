using UnityEngine;

public class PlaySceneBGM : MonoBehaviour
{

    public enum SceneBGM
    {
        MainMenu,
        Story,
        Arena,
        Defeat,
        Win
    }

    [Header("BGM Settings")]
    public SceneBGM sceneBGM;

    void Start()
    {
        if (AudioManager.Instance == null)
        {
            Debug.LogWarning("AudioManager not found in scene.");
            return;
        }

        //BGM mana yang akan dimainkan berdasarkan scene
        switch (sceneBGM)
        {
            case SceneBGM.MainMenu:
                AudioManager.Instance.PlayBGM(AudioManager.Instance.mainMenuBGM);
                break;

                case SceneBGM.Story:
            AudioManager.Instance.PlayBGM(AudioManager.Instance.storyBGM);
            break;

            case SceneBGM.Arena:
                AudioManager.Instance.PlayBGM(AudioManager.Instance.arenaBGM);
                break;

            case SceneBGM.Defeat:
            AudioManager.Instance.PlayBGM(AudioManager.Instance.defeatBGM);
            break;

            case SceneBGM.Win:
            AudioManager.Instance.PlayBGM(AudioManager.Instance.winBGM);
            break;
        }
    }
}
