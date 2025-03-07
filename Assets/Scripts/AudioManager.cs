using CandyCoded.HapticFeedback;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource backgroundMusicSource;
    public AudioSource sfxSource;

    [Header("Audio Clip")]
    public AudioClip bgMusic;
    public AudioClip sfxMusic;

    [Header("Volume Settings")]
    [Range(0, 1)] public float musicVolume = 1f;
    [Range(0, 1)] public float sfxVolume = 1f;

    public HapticFeedbackController hapticFeedbackController;

    private const string MusicVolumeKey = "MusicVolume";
    private const string SFXVolumeKey = "SFXVolume";

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

    private void Start()
    {
        PlayBackgroundMusic();

    }


    public void PlayBackgroundMusic()
    {
        if (backgroundMusicSource != null && bgMusic != null)
        {
            backgroundMusicSource.clip = bgMusic;
            backgroundMusicSource.loop = true;
            backgroundMusicSource.volume = musicVolume;
            backgroundMusicSource.Play();
        }
    }

    public void PlaySFX()
    {
        if (sfxSource != null && sfxMusic != null)
        {
            sfxSource.PlayOneShot(sfxMusic, sfxVolume);
            hapticFeedbackController.LightFeedback();
        }
    }

    public void StopBackgroundMusic()
    {
        if (backgroundMusicSource != null)
        {
            backgroundMusicSource.Stop();
        }
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        if (backgroundMusicSource != null)
        {
            backgroundMusicSource.volume = musicVolume;
        }

        PlayerPrefs.SetFloat(MusicVolumeKey, musicVolume);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);

        // Save to PlayerPrefs
        PlayerPrefs.SetFloat(SFXVolumeKey, sfxVolume);
        PlayerPrefs.Save();
    }

    private void LoadVolumeSettings()
    {
        if (PlayerPrefs.HasKey(MusicVolumeKey))
        {
            musicVolume = PlayerPrefs.GetFloat(MusicVolumeKey);
        }
        if (PlayerPrefs.HasKey(SFXVolumeKey))
        {
            sfxVolume = PlayerPrefs.GetFloat(SFXVolumeKey);
        }
    }

}
