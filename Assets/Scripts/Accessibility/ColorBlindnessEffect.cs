using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Post-processing effect for colorblindness assistance using URP
/// </summary>
public class ColorBlindnessEffect : MonoBehaviour
{
    [Header("Effect Settings")]
    public Material colorBlindnessMaterial;
    
    [Header("Auto-Setup")]
    [Tooltip("Automatically find and use the main camera")]
    public bool autoSetupMainCamera = true;
    
    [Header("Debug")]
    public bool enableDebugLogs = false;
    
    private Camera targetCamera;
    private AccessibilitySettings currentSettings;
    
    // Shader property IDs for performance
    private static readonly int ColorBlindnessTypeID = Shader.PropertyToID("_ColorBlindnessType");
    private static readonly int ContrastBoostID = Shader.PropertyToID("_ContrastBoost");
    private static readonly int SaturationAdjustmentID = Shader.PropertyToID("_SaturationAdjustment");
    private static readonly int BrightnessAdjustmentID = Shader.PropertyToID("_BrightnessAdjustment");
    private static readonly int EffectStrengthID = Shader.PropertyToID("_EffectStrength");
    
    void Start()
    {
        SetupCamera();
        SetupMaterial();
        RegisterForSettingsChanges();
        
        // Apply current settings if GameSettings exists
        if (GameSettings.Instance != null)
        {
            UpdateSettings(GameSettings.Instance.accessibility);
        }
    }
    
    void SetupCamera()
    {
        targetCamera = GetComponent<Camera>();
        
        if (targetCamera == null && autoSetupMainCamera)
        {
            targetCamera = Camera.main;
            if (targetCamera == null)
            {
                targetCamera = FindFirstObjectByType<Camera>();
            }
        }
        
        if (targetCamera == null)
        {
            Debug.LogWarning("⚠️ ColorBlindnessEffect: No camera found!");
            return;
        }
        
        if (enableDebugLogs)
            Debug.Log($"✅ ColorBlindnessEffect setup on camera: {targetCamera.name}");
    }
    
    void SetupMaterial()
    {
        if (colorBlindnessMaterial == null)
        {
            // Try to find the material automatically
            Shader shader = Shader.Find("Hidden/ColorBlindnessCorrection");
            if (shader != null)
            {
                colorBlindnessMaterial = new Material(shader);
                if (enableDebugLogs)
                    Debug.Log("✅ Auto-created ColorBlindness material");
            }
            else
            {
                Debug.LogError("❌ ColorBlindness shader not found! Make sure ColorBlindnessCorrection shader is in project.");
            }
        }
    }
    
    void RegisterForSettingsChanges()
    {
        GameSettings.OnAccessibilitySettingsChanged += UpdateSettings;
    }
    
    void OnDestroy()
    {
        GameSettings.OnAccessibilitySettingsChanged -= UpdateSettings;
    }
    
    public void UpdateSettings(AccessibilitySettings settings)
    {
        currentSettings = settings;
        UpdateMaterialProperties();
    }
    
    void UpdateMaterialProperties()
    {
        if (colorBlindnessMaterial == null || currentSettings == null) return;
        
        // Update shader properties
        colorBlindnessMaterial.SetInt(ColorBlindnessTypeID, (int)currentSettings.colorBlindnessType);
        colorBlindnessMaterial.SetFloat(ContrastBoostID, currentSettings.contrastBoost);
        colorBlindnessMaterial.SetFloat(SaturationAdjustmentID, currentSettings.saturationAdjustment);
        colorBlindnessMaterial.SetFloat(BrightnessAdjustmentID, currentSettings.brightnessAdjustment);
        colorBlindnessMaterial.SetFloat(EffectStrengthID, currentSettings.colorBlindnessEnabled ? 1.0f : 0.0f);
        
        if (enableDebugLogs)
            Debug.Log($"🎨 Updated colorblindness effect: {currentSettings.colorBlindnessType}, Enabled: {currentSettings.colorBlindnessEnabled}");
    }
    
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (colorBlindnessMaterial != null && currentSettings != null && currentSettings.colorBlindnessEnabled)
        {
            Graphics.Blit(source, destination, colorBlindnessMaterial);
        }
        else
        {
            Graphics.Blit(source, destination);
        }
    }
    
    // Manual update for testing
    [ContextMenu("🧪 Test Deuteranopia")]
    void TestDeuteranopia()
    {
        if (GameSettings.Instance != null)
        {
            GameSettings.Instance.SetColorBlindnessType(ColorBlindnessType.Deuteranopia);
            GameSettings.Instance.SetColorBlindnessEnabled(true);
        }
    }
    
    [ContextMenu("🧪 Test Protanopia")]
    void TestProtanopia()
    {
        if (GameSettings.Instance != null)
        {
            GameSettings.Instance.SetColorBlindnessType(ColorBlindnessType.Protanopia);
            GameSettings.Instance.SetColorBlindnessEnabled(true);
        }
    }
    
    [ContextMenu("🧪 Test Tritanopia")]
    void TestTritanopia()
    {
        if (GameSettings.Instance != null)
        {
            GameSettings.Instance.SetColorBlindnessType(ColorBlindnessType.Tritanopia);
            GameSettings.Instance.SetColorBlindnessEnabled(true);
        }
    }
    
    [ContextMenu("🧪 Disable Effect")]
    void DisableEffect()
    {
        if (GameSettings.Instance != null)
        {
            GameSettings.Instance.SetColorBlindnessEnabled(false);
        }
    }
}