using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Initializes the accessibility system for the entire game
/// </summary>
public class AccessibilityInitializer : MonoBehaviour
{
    [Header("🌟 Auto-Setup Options")]
    public bool autoSetupGameSettings = true;
    public bool autoSetupCameraEffects = true;
    public bool autoSetupSettingsPanel = true;
    public bool enableDebugLogs = true;
    
    [Header("📷 Camera Settings")]
    public bool setupMainCameraOnly = true;
    public Camera[] additionalCameras;
    
    void Awake()
    {
        // Initialize early to ensure settings are available
        if (autoSetupGameSettings)
            SetupGameSettings();
    }
    
    void Start()
    {
        InitializeAccessibilitySystem();
    }
    
    void InitializeAccessibilitySystem()
    {
        if (enableDebugLogs)
            Debug.Log("🌟 Initializing Accessibility System...");
        
        SetupCameraEffects();
        SetupSettingsPanel();
        
        if (enableDebugLogs)
            Debug.Log("✅ Accessibility System initialized successfully!");
    }
    
    void SetupGameSettings()
    {
        if (GameSettings.Instance == null)
        {
            GameObject settingsObj = new GameObject("GameSettings");
            settingsObj.AddComponent<GameSettings>();
            
            if (enableDebugLogs)
                Debug.Log("✅ GameSettings created");
        }
        else
        {
            if (enableDebugLogs)
                Debug.Log("✅ GameSettings already exists");
        }
    }
    
    void SetupCameraEffects()
    {
        if (!autoSetupCameraEffects) return;
        
        // Setup main camera
        if (setupMainCameraOnly)
        {
            Camera mainCamera = Camera.main;
            if (mainCamera == null)
                mainCamera = FindFirstObjectByType<Camera>();
                
            if (mainCamera != null)
            {
                SetupCameraEffect(mainCamera);
            }
            else
            {
                Debug.LogWarning("⚠️ No main camera found for accessibility effects");
            }
        }
        
        // Setup additional cameras
        if (additionalCameras != null)
        {
            foreach (var camera in additionalCameras)
            {
                if (camera != null)
                    SetupCameraEffect(camera);
            }
        }
    }
    
    void SetupCameraEffect(Camera camera)
    {
        ColorBlindnessEffect existingEffect = camera.GetComponent<ColorBlindnessEffect>();
        if (existingEffect == null)
        {
            camera.gameObject.AddComponent<ColorBlindnessEffect>();
            
            if (enableDebugLogs)
                Debug.Log($"✅ ColorBlindnessEffect added to camera: {camera.name}");
        }
        else
        {
            if (enableDebugLogs)
                Debug.Log($"✅ ColorBlindnessEffect already exists on camera: {camera.name}");
        }
    }
    
    void SetupSettingsPanel()
    {
        if (!autoSetupSettingsPanel) return;
        
        // Find existing settings panel
        SettingsPanel existingPanel = FindFirstObjectByType<SettingsPanel>();
        if (existingPanel != null)
        {
            if (enableDebugLogs)
                Debug.Log("✅ SettingsPanel already exists");
            return;
        }
        
        // Find pause menu controller and connect settings button
        PauseMenuController pauseController = FindFirstObjectByType<PauseMenuController>();
        if (pauseController != null)
        {
            SetupPauseMenuSettings(pauseController);
        }
        else
        {
            if (enableDebugLogs)
                Debug.LogWarning("⚠️ PauseMenuController not found - settings panel setup skipped");
        }
    }
    
    void SetupPauseMenuSettings(PauseMenuController pauseController)
    {
        // Try to find the settings button in the pause menu
        Button settingsButton = FindSettingsButtonInPauseMenu();
        
        if (settingsButton != null)
        {
            // Add SettingsPanel component to the pause menu
            SettingsPanel settingsPanel = pauseController.gameObject.AddComponent<SettingsPanel>();
            
            // Find the settings panel GameObject (should be created manually in the UI)
            GameObject settingsPanelGO = GameObject.Find("SettingsPanel");
            if (settingsPanelGO == null)
            {
                // Look for it as a child of the pause menu
                settingsPanelGO = pauseController.transform.Find("SettingsPanel")?.gameObject;
            }
            
            if (settingsPanelGO != null)
            {
                // Configure the settings panel
                settingsPanel.settingsPanel = settingsPanelGO;
                settingsPanel.settingsButton = settingsButton;
                
                if (enableDebugLogs)
                    Debug.Log("✅ SettingsPanel connected to pause menu");
            }
            else
            {
                if (enableDebugLogs)
                    Debug.LogWarning("⚠️ SettingsPanel GameObject not found - create it manually in the pause menu");
            }
        }
        else
        {
            if (enableDebugLogs)
                Debug.LogWarning("⚠️ Settings button not found in pause menu");
        }
    }
    
    Button FindSettingsButtonInPauseMenu()
    {
        // Look for button with "Settings" in the name
        Button[] allButtons = FindObjectsByType<Button>(FindObjectsSortMode.None);
        
        foreach (var button in allButtons)
        {
            if (button.name.ToLower().Contains("settings"))
                return button;
                
            // Also check if the button has text that says "Settings"
            var textComponent = button.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            if (textComponent != null && textComponent.text.ToLower().Contains("settings"))
                return button;
        }
        
        return null;
    }
    
    [ContextMenu("🔄 Reinitialize System")]
    public void ReinitializeSystem()
    {
        InitializeAccessibilitySystem();
    }
    
    [ContextMenu("🧪 Test Colorblindness")]
    public void TestColorblindness()
    {
        if (GameSettings.Instance != null)
        {
            GameSettings.Instance.SetColorBlindnessEnabled(true);
            GameSettings.Instance.SetColorBlindnessType(ColorBlindnessType.Deuteranopia);
            Debug.Log("🧪 Testing Deuteranopia mode");
        }
    }
    
    [ContextMenu("📋 System Status")]
    public void LogSystemStatus()
    {
        Debug.Log("🔍 Accessibility System Status:");
        Debug.Log($"   GameSettings: {(GameSettings.Instance != null ? "✅" : "❌")}");
        
        ColorBlindnessEffect[] effects = FindObjectsByType<ColorBlindnessEffect>(FindObjectsSortMode.None);
        Debug.Log($"   ColorBlindness Effects: {effects.Length}");
        
        SettingsPanel[] panels = FindObjectsByType<SettingsPanel>(FindObjectsSortMode.None);
        Debug.Log($"   Settings Panels: {panels.Length}");
        
        if (GameSettings.Instance != null)
        {
            var settings = GameSettings.Instance.accessibility;
            Debug.Log($"   Colorblindness Enabled: {settings.colorBlindnessEnabled}");
            Debug.Log($"   Colorblindness Type: {settings.colorBlindnessType}");
        }
    }
}