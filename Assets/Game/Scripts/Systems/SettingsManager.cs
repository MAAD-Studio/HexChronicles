using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// Manages game settings such as audio volumes, resolution, and fullscreen mode,
/// providing methods to save, load, and apply these settings.
/// </summary>
public class SettingsManager : Singleton<SettingsManager>
{
    #region Fields
    [SerializeField] private AudioMixer audioMixer; // Assign this in the inspector
    public float MasterVolume { get; private set; }
    public float MusicVolume { get; private set; }
    public float SfxVolume { get; private set; }
    public int QualityLevel { get; private set; }
    public Resolution GameResolution { get; private set; }
    public bool IsFullScreen { get; private set; }
    #endregion

    #region Unity Lifecycle
    public override void Awake()
    {
        base.Awake();

        if (audioMixer == null)
        {
            audioMixer = Resources.Load<AudioMixer>("Audio/AudioMixer");
        }
        Debug.Assert(audioMixer != null, "SettingsManager can't find AudioMixer");

        LoadAllSettings();
    }

    /*
    void OnEnable()
    {
        EventBus.Instance.Subscribe<AudioSettingsChangedEvent>(ApplyAudioSettings);
        EventBus.Instance.Subscribe<ResolutionChangedEvent>(ApplyResolutionSettings);
    }

    void OnDisable()
    {
        EventBus.Instance.Unsubscribe<AudioSettingsChangedEvent>(ApplyAudioSettings);
        EventBus.Instance.Unsubscribe<ResolutionChangedEvent>(ApplyResolutionSettings);
    }
    */
    #endregion

    #region Settings Loading and Saving
    /// <summary>
    /// Loads all settings from PlayerPrefs and applies them.
    /// </summary>
    public void LoadAllSettings()
    {
        LoadAudioSettings();
        LoadIsFullScreen();
        LoadResolution();
        LoadQualityLevel();
        ApplySettings();
    }

    /// <summary>
    /// Loads the audio settings from PlayerPrefs.
    /// </summary>
    private void LoadAudioSettings()
    {
        MasterVolume = PlayerPrefs.GetFloat("masterVolume", 1.0f);
        MusicVolume = PlayerPrefs.GetFloat("musicVolume", 1.0f);
        SfxVolume = PlayerPrefs.GetFloat("sfxVolume", 1.0f);
    }

    /// <summary>
    /// Saves the audio settings to PlayerPrefs and applies them.
    /// </summary>
    /// <param name="master">Master volume level.</param>
    /// <param name="music">Music volume level.</param>
    /// <param name="sfx">SFX volume level.</param>
    public void SaveAudioSettings(float master, float music, float sfx)
    {
        PlayerPrefs.SetFloat("masterVolume", master);
        PlayerPrefs.SetFloat("musicVolume", music);
        PlayerPrefs.SetFloat("sfxVolume", sfx);
        PlayerPrefs.Save();
        MasterVolume = master;
        MusicVolume = music;
        SfxVolume = sfx;
        ApplySettings();
    }

    private void LoadIsFullScreen()
    {
        IsFullScreen = PlayerPrefs.GetInt("isFullScreen", 1) == 1;
    }

    public void SaveIsFullScreen(bool fullscreen)
    {
        PlayerPrefs.SetInt("isFullScreen", fullscreen ? 1 : 0);
        IsFullScreen = fullscreen;
        PlayerPrefs.Save();
        ApplySettings();
    }

    private void LoadResolution()
    {
        int width = PlayerPrefs.GetInt("resolutionWidth", Screen.currentResolution.width);
        int height = PlayerPrefs.GetInt("resolutionHeight", Screen.currentResolution.height);
        GameResolution = new Resolution { width = width, height = height };
    }

    public void SaveResolution(int width, int height, bool fullscreen)
    {
        PlayerPrefs.SetInt("resolutionWidth", width);
        PlayerPrefs.SetInt("resolutionHeight", height);
        SaveIsFullScreen(fullscreen);
        GameResolution = new Resolution { width = width, height = height };
        IsFullScreen = fullscreen;
        PlayerPrefs.Save();
        ApplySettings();
    }

    private void LoadQualityLevel()
    {
        QualityLevel = PlayerPrefs.GetInt("qualityLevel", QualitySettings.GetQualityLevel());
    }

    public void SaveQualityLevel(int qualityLevel)
    {
        PlayerPrefs.SetInt("qualityLevel", qualityLevel);
        PlayerPrefs.Save();
        QualityLevel = qualityLevel;
        ApplySettings();
    }

    #endregion

    #region Settings Application
    /// <summary>
    /// Applies current settings to the game.
    /// </summary>
    public void ApplySettings()
    {
        AudioListener.volume = MasterVolume;
        audioMixer.SetFloat("MasterVolume", LinearToDecibel(MasterVolume));
        audioMixer.SetFloat("MusicVolume", LinearToDecibel(MusicVolume));
        audioMixer.SetFloat("SFXVolume", LinearToDecibel(SfxVolume));
        Screen.SetResolution(GameResolution.width, GameResolution.height, IsFullScreen);
        QualitySettings.SetQualityLevel(QualityLevel);
    }

    /// <summary>
    /// Converts linear volume scale (0 to 1) to decibels for the AudioMixer.
    /// </summary>
    /// <param name="linear">Linear scale volume.</param>
    /// <returns>Volume in decibels.</returns>
    private float LinearToDecibel(float linear)
    {
        return linear > 0 ? 20f * Mathf.Log10(linear) : -80f;
    }
    #endregion


    #region Event Handlers
    private void ApplyAudioSettings(object eventData)
    {
        var newSessionEvent = (AudioSettingsChangedEvent)eventData;
        SaveAudioSettings(newSessionEvent.MasterVolume, newSessionEvent.MusicVolume, newSessionEvent.SfxVolume);
    }

    private void ApplyResolutionSettings(object eventData)
    {
        var newSessionEvent = (ResolutionChangedEvent)eventData;
        SaveResolution(newSessionEvent.Width, newSessionEvent.Height, newSessionEvent.IsFullScreen);
    }
    #endregion

}
