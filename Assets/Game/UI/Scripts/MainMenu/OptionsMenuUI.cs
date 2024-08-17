using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class OptionsMenuUI : MonoBehaviour
{
    #region Fields

    [Header("Panel")]
    [SerializeField] private Button videoButton;
    [SerializeField] private Button audioButton;
    [SerializeField] private Button backButton;

    [Header("Video")]
    [SerializeField] private GameObject videoTab;
    [SerializeField] private TMP_Dropdown resolutionDropwdon;
    [SerializeField] private TMP_Dropdown qualityDropdown;
    [SerializeField] private Toggle fullscreenToggle;
    private Resolution[] resolutions;

    [Header("Audio")]
    [SerializeField] private GameObject audioTab;
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;
    #endregion

    #region Unity Methods

    private void Start()
    {
        InitializeUIComponents();
        LoadResolutions();
        UpdateUIWithCurrentSettings();
    }

    public void Initialize()
    {
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        ToggleVideoTabVisibility(true);
        ToggleAudioTabVisibility(false);
    }

    #endregion

    #region Initialization
    /// <summary>
    /// Initializes UI components and registers callbacks for UI interactions.
    /// </summary>
    private void InitializeUIComponents()
    {
        // Video Button
        videoButton.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlaySound("Click");
            ToggleVideoTabVisibility(true);
            ToggleAudioTabVisibility(false);
            });

        // Audio Button
        audioButton.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlaySound("Click");
            ToggleVideoTabVisibility(false);
            ToggleAudioTabVisibility(true);
        });

        // Back Button
        backButton.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlaySound("Click");
            gameObject.SetActive(false);
        });

        // Fullscreen Toggle
        fullscreenToggle.onValueChanged.AddListener(isOn =>
        {
            AudioManager.Instance.PlaySound("Click");
            SettingsManager.Instance.SaveIsFullScreen(isOn);
        });

        // Master Volume Slider
        masterVolumeSlider.onValueChanged.AddListener(volume =>
        {
            SettingsManager.Instance.SaveAudioSettings(
                volume, 
                musicVolumeSlider.value, 
                sfxVolumeSlider.value
            );
        }); 
        
        // Music Volume Slider 
        musicVolumeSlider.onValueChanged.AddListener(volume =>
        {
            SettingsManager.Instance.SaveAudioSettings(
                masterVolumeSlider.value, 
                volume, 
                sfxVolumeSlider.value
            );
        });

        // SFX Volume Slider
        sfxVolumeSlider.onValueChanged.AddListener(volume =>
        {
                SettingsManager.Instance.SaveAudioSettings(
                masterVolumeSlider.value, 
                musicVolumeSlider.value, 
                volume
            );
        });

        // Assuming resolutionDropdown is correctly set up elsewhere
        resolutionDropwdon.onValueChanged.AddListener(index =>
        {
            AudioManager.Instance.PlaySound("Click");
            SettingsManager.Instance.SaveResolution(
                resolutions[index].width, 
                resolutions[index].height, 
                fullscreenToggle.isOn
            );
        });

        qualityDropdown.onValueChanged.AddListener(index =>
        {
            AudioManager.Instance.PlaySound("Click");
            SettingsManager.Instance.SaveQualityLevel(index);
        });

    }

    private void UpdateUIWithCurrentSettings()
    {
        // Ensure SettingsManager instance is available
        if (SettingsManager.Instance != null)
        {
            // Update sliders
            masterVolumeSlider.value = SettingsManager.Instance.MasterVolume;
            musicVolumeSlider.value  = SettingsManager.Instance.MusicVolume;
            sfxVolumeSlider.value    = SettingsManager.Instance.SfxVolume;

            // Update toggle
            fullscreenToggle.isOn = SettingsManager.Instance.IsFullScreen;

            // Update resolution dropdown - find the current resolution index
            int currentResolutionIndex = Array.FindIndex(resolutions, r => r.width == SettingsManager.Instance.GameResolution.width && r.height == SettingsManager.Instance.GameResolution.height);
            resolutionDropwdon.value = resolutionDropwdon.value = currentResolutionIndex;

            qualityDropdown.value = SettingsManager.Instance.QualityLevel;
        }
        else
        {
            Debug.LogWarning("SettingsManager instance not found. Make sure it's initialized before accessing it.");
        }
    }

    #endregion

    #region Menu Visibility Management

    public void ShowOptionsMenu()
    {
        gameObject.SetActive(true);
    }

    private void ToggleVideoTabVisibility(bool isVisible)
    {
        videoTab.SetActive(isVisible);

        if (isVisible)
        {
            videoButton.GetComponentInChildren<TextMeshProUGUI>().color = new Color(1, 0.89f, 0f);
            videoButton.transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.2f).SetEase(Ease.OutBack);
        }
        else
        {
            videoButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
            videoButton.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);
        }
    }

    private void ToggleAudioTabVisibility(bool isVisible)
    {
        audioTab.SetActive(isVisible);

        if (isVisible)
        {
            audioButton.GetComponentInChildren<TextMeshProUGUI>().color = new Color(1, 0.89f, 0f);
            audioButton.transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.2f).SetEase(Ease.OutBack);
        }
        else
        {
            audioButton.GetComponentInChildren<TextMeshProUGUI>().color = Color.white;
            audioButton.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);
        }
    }

    #endregion

    #region UI Methods
    /// <summary>
    /// Loads and populates the resolution dropdown with available screen resolutions.
    /// </summary>
    private void LoadResolutions()
    {
        resolutions = Screen.resolutions.Select(
            resolution => new Resolution { width = resolution.width, height = resolution.height })
            .DistinctBy(res => new { res.width, res.height })
            .ToArray();
        Array.Reverse(resolutions); // Optional: reverse to have the highest resolution at the top
        List<string> options = new List<string>();
        var currentResolutionIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            var resolution = resolutions[i];
            var option = $"{resolution.width}x{resolution.height}";
            options.Add(option);

            if (resolution.width == Screen.currentResolution.width && resolution.height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }
        resolutionDropwdon.ClearOptions();
        resolutionDropwdon.AddOptions(options);
        resolutionDropwdon.value = currentResolutionIndex;
        resolutionDropwdon.RefreshShownValue();
    }

    #endregion
}