using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// One-click setup for the complete accessibility system
/// </summary>
public class AccessibilitySystemSetup : MonoBehaviour
{
    [Header("🚀 Quick Setup")]
    [Tooltip("Click to automatically setup the complete accessibility system")]
    public bool setupOnStart = true;
    
    [Header("📋 Setup Status")]
    [SerializeField] private bool gameSettingsSetup = false;
    [SerializeField] private bool cameraEffectsSetup = false;
    [SerializeField] private bool settingsPanelSetup = false;
    [SerializeField] private bool pauseMenuIntegrated = false;
    
    void Start()
    {
        if (setupOnStart)
        {
            SetupCompleteAccessibilitySystem();
        }
    }
    
    [ContextMenu("🚀 Setup Complete Accessibility System")]
    public void SetupCompleteAccessibilitySystem()
    {
        Debug.Log("🚀 Setting up complete accessibility system...");
        
        SetupGameSettings();
        SetupCameraEffects();
        SetupPauseMenuIntegration();
        SetupAccessibilityInitializer();
        
        UpdateSetupStatus();
        
        Debug.Log("✅ Accessibility system setup complete!");
        LogSetupSummary();
    }
    
    void SetupGameSettings()
    {
        if (GameSettings.Instance == null)
        {
            GameObject settingsObj = new GameObject("GameSettings");
            settingsObj.AddComponent<GameSettings>();
            
            // Only call DontDestroyOnLoad in play mode
            if (Application.isPlaying)
            {
                DontDestroyOnLoad(settingsObj);
            }
            
            gameSettingsSetup = true;
            Debug.Log("✅ GameSettings created");
        }
        else
        {
            gameSettingsSetup = true;
            Debug.Log("✅ GameSettings already exists");
        }
    }
    
    void SetupCameraEffects()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
            mainCamera = FindFirstObjectByType<Camera>();
            
        if (mainCamera != null)
        {
            if (mainCamera.GetComponent<ColorBlindnessEffect>() == null)
            {
                mainCamera.gameObject.AddComponent<ColorBlindnessEffect>();
                Debug.Log($"✅ ColorBlindnessEffect added to {mainCamera.name}");
            }
            else
            {
                Debug.Log($"✅ ColorBlindnessEffect already exists on {mainCamera.name}");
            }
            cameraEffectsSetup = true;
        }
        else
        {
            Debug.LogWarning("⚠️ No camera found for ColorBlindnessEffect");
        }
    }
    
    void SetupPauseMenuIntegration()
    {
        PauseMenuController pauseController = FindFirstObjectByType<PauseMenuController>();
        if (pauseController == null)
        {
            Debug.LogWarning("⚠️ PauseMenuController not found - skipping pause menu integration");
            return;
        }
        
        // Add SettingsPanel component if it doesn't exist
        SettingsPanel settingsPanel = pauseController.GetComponent<SettingsPanel>();
        if (settingsPanel == null)
        {
            settingsPanel = pauseController.gameObject.AddComponent<SettingsPanel>();
            Debug.Log("✅ SettingsPanel added to PauseMenuController");
        }
        
        // Auto-find and assign the settings button
        if (pauseController.settingsButton == null)
        {
            Button settingsButton = FindSettingsButton();
            if (settingsButton != null)
            {
                pauseController.settingsButton = settingsButton;
                Debug.Log($"✅ Settings button found and assigned: {settingsButton.name}");
            }
            else
            {
                Debug.LogWarning("⚠️ Settings button not found - assign manually in PauseMenuController");
            }
        }
        
        // Try to find settings panel GameObject
        GameObject settingsPanelGO = FindSettingsPanelGameObject();
        if (settingsPanelGO != null)
        {
            settingsPanel.settingsPanel = settingsPanelGO;
            settingsPanel.settingsButton = pauseController.settingsButton;
            
            // Auto-find close button
            Button closeButton = settingsPanelGO.GetComponentInChildren<Button>();
            if (closeButton != null && closeButton.name.ToLower().Contains("close"))
            {
                settingsPanel.closeButton = closeButton;
            }
            
            Debug.Log("✅ Settings panel UI connected");
            settingsPanelSetup = true;
        }
        else
        {
            Debug.LogWarning("⚠️ Settings panel GameObject not found - create it manually");
        }
        
        pauseMenuIntegrated = true;
    }
    
    void SetupAccessibilityInitializer()
    {
        AccessibilityInitializer initializer = FindFirstObjectByType<AccessibilityInitializer>();
        if (initializer == null)
        {
            GameObject initializerObj = new GameObject("AccessibilityInitializer");
            initializerObj.AddComponent<AccessibilityInitializer>();
            Debug.Log("✅ AccessibilityInitializer created");
        }
        else
        {
            Debug.Log("✅ AccessibilityInitializer already exists");
        }
    }
    
    Button FindSettingsButton()
    {
        Button[] allButtons = FindObjectsByType<Button>(FindObjectsSortMode.None);
        
        foreach (var button in allButtons)
        {
            if (button.name.ToLower().Contains("settings"))
                return button;
                
            // Check button text
            var textComponent = button.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            if (textComponent != null && textComponent.text.ToLower().Contains("settings"))
                return button;
        }
        
        return null;
    }
    
    GameObject FindSettingsPanelGameObject()
    {
        // Look for GameObject named SettingsPanel
        GameObject panel = GameObject.Find("SettingsPanel");
        if (panel != null) return panel;
        
        // Look for any GameObject with "settings" in the name
        GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        foreach (var obj in allObjects)
        {
            if (obj.name.ToLower().Contains("settings") && obj.name.ToLower().Contains("panel"))
                return obj;
        }
        
        return null;
    }
    
    void UpdateSetupStatus()
    {
        gameSettingsSetup = GameSettings.Instance != null;
        cameraEffectsSetup = FindFirstObjectByType<ColorBlindnessEffect>() != null;
        settingsPanelSetup = FindFirstObjectByType<SettingsPanel>() != null;
        
        PauseMenuController pauseController = FindFirstObjectByType<PauseMenuController>();
        pauseMenuIntegrated = pauseController != null && pauseController.settingsButton != null;
    }
    
    void LogSetupSummary()
    {
        Debug.Log("📋 Accessibility System Setup Summary:");
        Debug.Log($"   🎛️ GameSettings: {(gameSettingsSetup ? "✅" : "❌")}");
        Debug.Log($"   📷 Camera Effects: {(cameraEffectsSetup ? "✅" : "❌")}");
        Debug.Log($"   ⚙️ Settings Panel: {(settingsPanelSetup ? "✅" : "❌")}");
        Debug.Log($"   🎮 Pause Menu Integration: {(pauseMenuIntegrated ? "✅" : "❌")}");
        
        if (gameSettingsSetup && cameraEffectsSetup && settingsPanelSetup && pauseMenuIntegrated)
        {
            Debug.Log("🎉 Complete accessibility system is ready!");
            Debug.Log("💡 Next steps:");
            Debug.Log("   1. Create a settings panel UI in your pause menu");
            Debug.Log("   2. Assign UI elements to the SettingsPanel component");
            Debug.Log("   3. Test colorblindness modes in play mode");
        }
        else
        {
            Debug.LogWarning("⚠️ Some components need manual setup - check the warnings above");
        }
    }
    
    [ContextMenu("🔍 Check System Status")]
    public void CheckSystemStatus()
    {
        UpdateSetupStatus();
        LogSetupSummary();
    }
    
    [ContextMenu("🧪 Test Colorblindness")]
    public void TestColorblindnessSystem()
    {
        if (GameSettings.Instance != null)
        {
            GameSettings.Instance.SetColorBlindnessEnabled(true);
            GameSettings.Instance.SetColorBlindnessType(ColorBlindnessType.Deuteranopia);
            Debug.Log("🧪 Testing Deuteranopia mode - check your game visuals!");
        }
        else
        {
            Debug.LogError("❌ GameSettings not found - run setup first");
        }
    }
}