using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    public AudioSource bgmSource;
    public AudioSource sfxSource;

    [Header("Audio Clips")]
    public AudioClip bgmLoop;
    public AudioClip sfxItem;
    public AudioClip sfxShoot;
    public AudioClip sfxZombieDeath;
    public AudioClip sfxGameOver;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // DontDestroyOnLoad(gameObject); // Optional, handled per-scene via UI.prefab currently
            
            if (bgmSource != null && bgmLoop != null)
            {
                bgmSource.clip = bgmLoop;
                bgmSource.loop = true;
                bgmSource.Play();
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayBGM(AudioClip clip)
    {
        if (bgmSource == null || clip == null) return;
        bgmSource.clip = clip;
        bgmSource.loop = true;
        bgmSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        if (sfxSource == null || clip == null) return;
        sfxSource.PlayOneShot(clip);
    }

    public void PlayItemSFX()
    {
        PlaySFX(sfxItem);
    }

    public void PlayShootSFX()
    {
        PlaySFX(sfxShoot);
    }

    public void PlayZombieDeathSFX()
    {
        PlaySFX(sfxZombieDeath);
    }

    public void PlayGameOverSFX()
    {
        PlaySFX(sfxGameOver);
        if (bgmSource != null)
        {
            bgmSource.Stop(); // Stop BGM on game over
        }
    }
}
