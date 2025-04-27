//using CandyCoded.HapticFeedback;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AudioManager : MonoBehaviour
{

    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource backgroundMusicSource;
    public AudioSource sfxSource;

    [Header("Audio Clip")]
    public AudioClip bgMusic;
    public AudioClip sfxMusic;
    public AudioClip cleaningMusic;
    public AudioClip wateringMusic;
    public AudioClip levelUpMusic;
    public AudioClip cuttingMusic;
    public AudioClip buyMusic;
    public AudioClip sellMusic;

    [Header("Volume Settings (0–1 Internally)")]
    [Range(0, 1)] public float musicVolume = 1f;
    [Range(0, 1)] public float sfxVolume = 1f;

    [Header("UI References")]
    [SerializeField] private Slider _sliderBG;
    [SerializeField] private Slider _sliderSFX;
    [SerializeField] private TextMeshProUGUI _musicValueText;
    [SerializeField] private TextMeshProUGUI _sfxValueText;

   // public HapticFeedbackController hapticFeedbackController;

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
        LoadVolumeSettings();
        SetMusicVolume(musicVolume);
        SetSFXVolume(sfxVolume);
        PlayBackgroundMusic();
    }

    public void OnMusicVolumeSliderChanged(float volume)
    {
        float normalizedVolume = volume / 100f; 
        SetMusicVolume(normalizedVolume);
        _musicValueText.text = ((int)volume).ToString(); 
    }

    public void OnSFXVolumeSliderChanged(float volume)
    {
        float normalizedVolume = volume / 100f; 
        SetSFXVolume(normalizedVolume);
        _sfxValueText.text = ((int)volume).ToString(); 
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
            // hapticFeedbackController.LightFeedback();
        }
    }

    public void PlayMusic(AudioClip clip, float volume)
    {
        if (sfxSource != null && clip != null)
        {
            sfxSource.clip = clip;
            sfxSource.volume = volume;
            sfxSource.Play();
        }
    }

    public void StopMusic()
    {
        if (sfxSource != null && sfxSource.isPlaying)
        {
            sfxSource.Stop();
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
        if (sfxSource != null)
        {
            sfxSource.volume = sfxVolume;
        }

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

        float musicSliderValue = musicVolume * 100f;
        float sfxSliderValue = sfxVolume * 100f;

        _sliderBG.value = musicSliderValue;
        _sliderSFX.value = sfxSliderValue;

        _musicValueText.text = ((int)musicSliderValue).ToString();
        _sfxValueText.text = ((int)sfxSliderValue).ToString();
    }
}
