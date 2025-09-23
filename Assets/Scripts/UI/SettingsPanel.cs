using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

/// <summary>
/// Comprehensive settings panel for accessibility and gameplay options
/// </summary>
public class SettingsPanel : MonoBehaviour
{
    [Header("🎛️ Panel Control")]
    public GameObject settingsPanel;
    public Button settingsButton;
    public Button closeButton;
    public Button resetButton;
    
    [Header("🎨 Colorblindness Controls")]
    public Toggle colorBlindnessToggle;
    public TMP_Dropdown colorBlindnessTypeDropdown;
    public Slider contrastSlider;
    public Slider saturationSlider;
    public Slider brightnessSlider;
    public TextMeshProUGUI contrastValueText;
    public TextMeshProUGUI saturationValueText;
    public TextMeshProUGUI brightnessValueText;
    
    [Header("🎵 Audio Controls")]
    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;
    public TextMeshProUGUI masterVolumeText;
    public TextMeshProUGUI musicVolumeText;
    public TextMeshProUGUI sfxVolumeText;
    
    [Header("🎮 Controls")]
    public Slider mouseSensitivitySlider;
    public Toggle invertMouseYToggle;
    public TextMeshProUGUI mouseSensitivityText;
    
    [Header("🖼️ Graphics")]
    public Toggle vSyncToggle;
    
    [Header("🧪 Test Colors")]
    public Image[] testColorImages;
    public Color[] testColors = {
        Color.red, Color.green, Color.blue,
        Color.yellow, Color.magenta, Color.cyan,
        new Color(1f, 0.5f, 0f), // Orange
        new Color(0.5f, 0f, 1f)  // Purple
    };
    
    [Header("⚙️ Settings")]
    public bool enableDebugLogs = false;
    
    private bool isInitialized = false;
    
    void Start()
    {
        SetupUI();
        LoadCurrentSettings();
        isInitialized = true;
    }
    
    void SetupUI()
    {
        // Setup main panel buttons
        if (settingsButton != null)
            settingsButton.onClick.AddListener(OpenSettings);
            
        if (closeButton != null)
            closeButton.onClick.AddListener(CloseSettings);
            
        if (resetButton != null)
            resetButton.onClick.AddListener(ResetToDefaults);
        
        // Setup colorblindness controls
        SetupColorBlindnessControls();
        
        // Setup audio controls
        SetupAudioControls();
        
        // Setup control settings
        SetupControlSettings();
        
        // Setup graphics settings
        SetupGraphicsSettings();
        
        // Setup test colors
        SetupTestColors();
        
        // Initially hide settings panel
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
            
        if (enableDebugLogs)
            Debug.Log("✅ Settings panel UI setup complete");
    }
    
    #region UI Setup Methods
    void SetupColorBlindnessControls()
    {
        if (colorBlindnessToggle != null)
            colorBlindnessToggle.onValueChanged.AddListener(OnColorBlindnessToggled);
            
        if (colorBlindnessTypeDropdown != null)
        {
            SetupColorBlindnessDropdown();
            colorBlindnessTypeDropdown.onValueChanged.AddListener(OnColorBlindnessTypeChanged);
        }
        
        if (contrastSlider != null)
            contrastSlider.onValueChanged.AddListener(OnContrastChanged);
            
        if (saturationSlider != null)
            saturationSlider.onValueChanged.AddListener(OnSaturationChanged);
            
        if (brightnessSlider != null)
            brightnessSlider.onValueChanged.AddListener(OnBrightnessChanged);
    }
    
    void SetupAudioControls()
    {
        if (masterVolumeSlider != null)
            masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
            
        if (musicVolumeSlider != null)
            musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
            
        if (sfxVolumeSlider != null)
            sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
    }
    
    void SetupControlSettings()
    {
        if (mouseSensitivitySlider != null)
            mouseSensitivitySlider.onValueChanged.AddListener(OnMouseSensitivityChanged);
            
        if (invertMouseYToggle != null)
            invertMouseYToggle.onValueChanged.AddListener(OnInvertMouseYChanged);
    }
    
    void SetupGraphicsSettings()
    {
        if (vSyncToggle != null)
            vSyncToggle.onValueChanged.AddListener(OnVSyncChanged);
    }
    #endregion
    
    void SetupColorBlindnessDropdown()
    {
        colorBlindnessTypeDropdown.ClearOptions();
        colorBlindnessTypeDropdown.AddOptions(new System.Collections.Generic.List<string>
        {
            "Normal Vision",
            "Deuteranopia (Green-Blind)",
            "Protanopia (Red-Blind)",
            "Tritanopia (Blue-Blind)"
        });
    }
    
    void SetupTestColors()
    {
        if (testColorImages != null && testColors != null)
        {
            for (int i = 0; i < testColorImages.Length && i < testColors.Length; i++)
            {
                if (testColorImages[i] != null)
                    testColorImages[i].color = testColors[i];
            }
        }
    }
    
    void LoadCurrentSettings()
    {
        if (GameSettings.Instance == null) return;
        
        var accessibility = GameSettings.Instance.accessibility;
        var gameplay = GameSettings.Instance.gameplay;
        
        // Load accessibility settings without triggering events
        if (colorBlindnessToggle != null)
            colorBlindnessToggle.SetIsOnWithoutNotify(accessibility.colorBlindnessEnabled);
            
        if (colorBlindnessTypeDropdown != null)
            colorBlindnessTypeDropdown.SetValueWithoutNotify((int)accessibility.colorBlindnessType);
            
        // Visual adjustments
        LoadSliderValue(contrastSlider, contrastValueText, accessibility.contrastBoost);
        LoadSliderValue(saturationSlider, saturationValueText, accessibility.saturationAdjustment);
        LoadSliderValue(brightnessSlider, brightnessValueText, accessibility.brightnessAdjustment);
        
        // Audio settings
        LoadSliderValue(masterVolumeSlider, masterVolumeText, gameplay.masterVolume, true);
        LoadSliderValue(musicVolumeSlider, musicVolumeText, gameplay.musicVolume, true);
        LoadSliderValue(sfxVolumeSlider, sfxVolumeText, gameplay.soundEffectsVolume, true);
        
        // Control settings
        LoadSliderValue(mouseSensitivitySlider, mouseSensitivityText, gameplay.mouseSensitivity);
        
        if (invertMouseYToggle != null)
            invertMouseYToggle.SetIsOnWithoutNotify(gameplay.invertMouseY);
            
        // Graphics settings
        if (vSyncToggle != null)
            vSyncToggle.SetIsOnWithoutNotify(gameplay.enableVSync);
    }
    
    void LoadSliderValue(Slider slider, TextMeshProUGUI text, float value, bool isPercentage = false)
    {
        if (slider != null)
        {
            slider.SetValueWithoutNotify(value);
            UpdateSliderText(text, value, isPercentage);
        }
    }
    
    void UpdateSliderText(TextMeshProUGUI text, float value, bool isPercentage = false)
    {
        if (text != null)
        {
            if (isPercentage)
                text.text = $"{(value * 100):F0}%";
            else
                text.text = $"{value:F1}";
        }
    }
    
    #region Panel Control
    public void OpenSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
            LoadCurrentSettings(); // Refresh current values
            
            // Pause the game when opening settings
            if (PauseManager.Instance != null)
                PauseManager.Instance.PauseGame();
        }
        
        if (enableDebugLogs)
            Debug.Log("🔧 Settings panel opened");
    }
    
    public void CloseSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
            
        if (enableDebugLogs)
            Debug.Log("🔧 Settings panel closed");
    }
    
    public void ResetToDefaults()
    {
        if (GameSettings.Instance != null)
        {
            GameSettings.Instance.ResetToDefaults();
            LoadCurrentSettings();
            
            if (enableDebugLogs)
                Debug.Log("🔄 Settings reset to defaults");
        }
    }
    #endregion
    
    #region Accessibility Event Handlers
    void OnColorBlindnessToggled(bool enabled)
    {
        if (!isInitialized) return;
        
        if (GameSettings.Instance != null)
        {
            GameSettings.Instance.SetColorBlindnessEnabled(enabled);
        }
    }
    
    void OnColorBlindnessTypeChanged(int typeIndex)
    {
        if (!isInitialized) return;
        
        if (GameSettings.Instance != null)
        {
            GameSettings.Instance.SetColorBlindnessType((ColorBlindnessType)typeIndex);
        }
    }
    
    void OnContrastChanged(float value)
    {
        if (!isInitialized) return;
        
        if (GameSettings.Instance != null)
        {
            GameSettings.Instance.SetContrastBoost(value);
        }
        UpdateSliderText(contrastValueText, value);
    }
    
    void OnSaturationChanged(float value)
    {
        if (!isInitialized) return;
        
        if (GameSettings.Instance != null)
        {
            GameSettings.Instance.SetSaturationAdjustment(value);
        }
        UpdateSliderText(saturationValueText, value);
    }
    
    void OnBrightnessChanged(float value)
    {
        if (!isInitialized) return;
        
        if (GameSettings.Instance != null)
        {
            GameSettings.Instance.SetBrightnessAdjustment(value);
        }
        UpdateSliderText(brightnessValueText, value);
    }
    #endregion
    
    #region Audio Event Handlers
    void OnMasterVolumeChanged(float value)
    {
        if (!isInitialized) return;
        
        if (GameSettings.Instance != null)
        {
            GameSettings.Instance.SetMasterVolume(value);
        }
        UpdateSliderText(masterVolumeText, value, true);
    }
    
    void OnMusicVolumeChanged(float value)
    {
        if (!isInitialized) return;
        
        if (GameSettings.Instance != null)
        {
            GameSettings.Instance.SetMusicVolume(value);
        }
        UpdateSliderText(musicVolumeText, value, true);
    }
    
    void OnSFXVolumeChanged(float value)
    {
        if (!isInitialized) return;
        
        if (GameSettings.Instance != null)
        {
            GameSettings.Instance.SetSoundEffectsVolume(value);
        }
        UpdateSliderText(sfxVolumeText, value, true);
    }
    #endregion
    
    #region Control Event Handlers
    void OnMouseSensitivityChanged(float value)
    {
        if (!isInitialized) return;
        
        if (GameSettings.Instance != null)
        {
            GameSettings.Instance.SetMouseSensitivity(value);
        }
        UpdateSliderText(mouseSensitivityText, value);
    }
    
    void OnInvertMouseYChanged(bool invert)
    {
        if (!isInitialized) return;
        
        if (GameSettings.Instance != null)
        {
            GameSettings.Instance.SetInvertMouseY(invert);
        }
    }
    #endregion
    
    #region Graphics Event Handlers
    void OnVSyncChanged(bool enabled)
    {
        if (!isInitialized) return;
        
        if (GameSettings.Instance != null)
        {
            GameSettings.Instance.SetVSync(enabled);
        }
    }
    #endregion
    
    #region Testing Methods
    [ContextMenu("🧪 Test All Colorblindness Types")]
    public void TestAllColorBlindnessTypes()
    {
        StartCoroutine(CycleColorBlindnessTypes());
    }
    
    System.Collections.IEnumerator CycleColorBlindnessTypes()
    {
        var types = System.Enum.GetValues(typeof(ColorBlindnessType));
        
        foreach (ColorBlindnessType type in types)
        {
            Debug.Log($"🧪 Testing: {type}");
            
            if (GameSettings.Instance != null)
            {
                GameSettings.Instance.SetColorBlindnessType(type);
                GameSettings.Instance.SetColorBlindnessEnabled(type != ColorBlindnessType.None);
            }
            
            LoadCurrentSettings();
            yield return new WaitForSeconds(3f);
        }
        
        Debug.Log("🧪 Colorblindness test cycle completed");
    }
    
    [ContextMenu("🎨 Show Test Colors")]
    public void ShowTestColors()
    {
        if (testColorImages != null)
        {
            foreach (var image in testColorImages)
            {
                if (image != null)
                    image.gameObject.SetActive(true);
            }
        }
    }
    #endregion
}