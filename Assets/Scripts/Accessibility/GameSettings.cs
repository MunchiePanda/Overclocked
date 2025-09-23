using UnityEngine;
using System;

[Serializable]
public enum ColorBlindnessType
{
    None = 0,
    Deuteranopia = 1,    // Green-blind (most common)
    Protanopia = 2,      // Red-blind
    Tritanopia = 3       // Blue-blind (very rare)
}

[System.Serializable]
public class AccessibilitySettings
{
    [Header("Colorblindness Support")]
    public bool colorBlindnessEnabled = false;
    public ColorBlindnessType colorBlindnessType = ColorBlindnessType.Deuteranopia;
    
    [Header("Visual Assistance")]
    [Range(0.5f, 2.0f)]
    public float contrastBoost = 1.0f;
    
    [Range(0.8f, 1.5f)]
    public float saturationAdjustment = 1.0f;
    
    [Range(0.5f, 1.5f)]
    public float brightnessAdjustment = 1.0f;
}

[System.Serializable]
public class GameplaySettings
{
    [Header("Audio")]
    [Range(0f, 1f)]
    public float masterVolume = 1.0f;
    
    [Range(0f, 1f)]
    public float musicVolume = 0.7f;
    
    [Range(0f, 1f)]
    public float soundEffectsVolume = 1.0f;
    
    [Header("Graphics")]
    [Range(0.5f, 2.0f)]
    public float renderScale = 1.0f;
    
    public bool enableVSync = true;
    
    [Header("Controls")]
    [Range(0.1f, 3.0f)]
    public float mouseSensitivity = 1.0f;
    
    public bool invertMouseY = false;
}

/// <summary>
/// Global settings manager that persists across scenes
/// </summary>
public class GameSettings : MonoBehaviour
{
    public static GameSettings Instance { get; private set; }
    
    [Header("Settings")]
    public AccessibilitySettings accessibility = new AccessibilitySettings();
    public GameplaySettings gameplay = new GameplaySettings();
    
    public static event Action<AccessibilitySettings> OnAccessibilitySettingsChanged;
    public static event Action<GameplaySettings> OnGameplaySettingsChanged;
    
    private const string ACCESSIBILITY_KEY = "OverclockedAccessibilitySettings";
    private const string GAMEPLAY_KEY = "OverclockedGameplaySettings";
    
    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadSettings();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        // Apply settings immediately
        ApplyAllSettings();
    }
    
    #region Accessibility Settings
    public void SetColorBlindnessEnabled(bool enabled)
    {
        accessibility.colorBlindnessEnabled = enabled;
        ApplyAccessibilitySettings();
        SaveSettings();
        
        Debug.Log($"🎨 Colorblindness mode: {(enabled ? "ENABLED" : "DISABLED")}");
    }
    
    public void SetColorBlindnessType(ColorBlindnessType type)
    {
        accessibility.colorBlindnessType = type;
        ApplyAccessibilitySettings();
        SaveSettings();
        
        Debug.Log($"🎨 Colorblindness type changed to: {type}");
    }
    
    public void SetContrastBoost(float contrast)
    {
        accessibility.contrastBoost = Mathf.Clamp(contrast, 0.5f, 2.0f);
        ApplyAccessibilitySettings();
        SaveSettings();
    }
    
    public void SetSaturationAdjustment(float saturation)
    {
        accessibility.saturationAdjustment = Mathf.Clamp(saturation, 0.8f, 1.5f);
        ApplyAccessibilitySettings();
        SaveSettings();
    }
    
    public void SetBrightnessAdjustment(float brightness)
    {
        accessibility.brightnessAdjustment = Mathf.Clamp(brightness, 0.5f, 1.5f);
        ApplyAccessibilitySettings();
        SaveSettings();
    }
    #endregion
    
    #region Gameplay Settings
    public void SetMasterVolume(float volume)
    {
        gameplay.masterVolume = Mathf.Clamp01(volume);
        ApplyGameplaySettings();
        SaveSettings();
    }
    
    public void SetMusicVolume(float volume)
    {
        gameplay.musicVolume = Mathf.Clamp01(volume);
        ApplyGameplaySettings();
        SaveSettings();
    }
    
    public void SetSoundEffectsVolume(float volume)
    {
        gameplay.soundEffectsVolume = Mathf.Clamp01(volume);
        ApplyGameplaySettings();
        SaveSettings();
    }
    
    public void SetMouseSensitivity(float sensitivity)
    {
        gameplay.mouseSensitivity = Mathf.Clamp(sensitivity, 0.1f, 3.0f);
        ApplyGameplaySettings();
        SaveSettings();
    }
    
    public void SetInvertMouseY(bool invert)
    {
        gameplay.invertMouseY = invert;
        ApplyGameplaySettings();
        SaveSettings();
    }
    
    public void SetVSync(bool enabled)
    {
        gameplay.enableVSync = enabled;
        QualitySettings.vSyncCount = enabled ? 1 : 0;
        ApplyGameplaySettings();
        SaveSettings();
    }
    #endregion
    
    public void ApplyAllSettings()
    {
        ApplyAccessibilitySettings();
        ApplyGameplaySettings();
    }
    
    public void ApplyAccessibilitySettings()
    {
        OnAccessibilitySettingsChanged?.Invoke(accessibility);
        
        // Apply to all active cameras with the colorblind effect
        ColorBlindnessEffect[] effects = FindObjectsByType<ColorBlindnessEffect>(FindObjectsSortMode.None);
        foreach (var effect in effects)
        {
            effect.UpdateSettings(accessibility);
        }
    }
    
    public void ApplyGameplaySettings()
    {
        OnGameplaySettingsChanged?.Invoke(gameplay);
        
        // Apply audio settings
        AudioListener.volume = gameplay.masterVolume;
        
        // Apply graphics settings
        QualitySettings.vSyncCount = gameplay.enableVSync ? 1 : 0;
    }
    
    void SaveSettings()
    {
        string accessibilityJson = JsonUtility.ToJson(accessibility, true);
        PlayerPrefs.SetString(ACCESSIBILITY_KEY, accessibilityJson);
        
        string gameplayJson = JsonUtility.ToJson(gameplay, true);
        PlayerPrefs.SetString(GAMEPLAY_KEY, gameplayJson);
        
        PlayerPrefs.Save();
    }
    
    void LoadSettings()
    {
        if (PlayerPrefs.HasKey(ACCESSIBILITY_KEY))
        {
            string accessibilityJson = PlayerPrefs.GetString(ACCESSIBILITY_KEY);
            JsonUtility.FromJsonOverwrite(accessibilityJson, accessibility);
        }
        
        if (PlayerPrefs.HasKey(GAMEPLAY_KEY))
        {
            string gameplayJson = PlayerPrefs.GetString(GAMEPLAY_KEY);
            JsonUtility.FromJsonOverwrite(gameplayJson, gameplay);
        }
    }
    
    public void ResetToDefaults()
    {
        accessibility = new AccessibilitySettings();
        gameplay = new GameplaySettings();
        ApplyAllSettings();
        SaveSettings();
        Debug.Log("🔄 All settings reset to defaults");
    }
    
    // Helper properties for UI
    public bool IsColorBlindnessEnabled => accessibility.colorBlindnessEnabled;
    public ColorBlindnessType CurrentColorBlindnessType => accessibility.colorBlindnessType;
    
    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus) SaveSettings();
    }
    
    void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus) SaveSettings();
    }
}