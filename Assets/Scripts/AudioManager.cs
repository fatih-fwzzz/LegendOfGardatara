using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource bgmSource;
    public AudioSource sfxSource;

    [Header("Audio Clips")]
    public AudioClip mainMenuBGM;
    public AudioClip arenaBGM;
    public AudioClip storyBGM;
    public AudioClip defeatBGM;
    public AudioClip winBGM;
    public AudioClip clickSFX;
    public AudioClip hitSFX;
    public AudioClip upgradeSFX;
    public AudioClip deploySFX;
    public AudioClip shootCannonSFX;
    

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayBGM(AudioClip clip)
    {
        bgmSource.clip = clip;
        bgmSource.loop = true;
        bgmSource.Play();
    }

    public void StopBGM()
    {
        bgmSource.Stop();
    }

    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }
}
