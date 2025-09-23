using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Safe setup that works in both edit mode and play mode
/// </summary>
public class SafeAccessibilitySetup : MonoBehaviour
{
    [Header("🎨 Safe Accessibility Setup")]
    [Space(10)]
    [TextArea(3, 5)]
    public string info = "This setup works safely in both edit mode and play mode.\n\nFor FULL functionality, run setup in PLAY MODE.\nEdit mode setup prepares components only.";
    
    [Space(10)]
    public bool autoSetupOnStart = true;
    
    [Header("📋 Setup Progress")]
    public bool cameraEffectAdded = false;
    public bool settingsPanelAdded = false;
    public bool pauseMenuConnected = false;
    
    void Start()
    {
        if (autoSetupOnStart && Application.isPlaying)
        {
            SetupAccessibilitySystem();
        }
    }
    
    [ContextMenu("🚀 Setup Accessibility System")]
    public void SetupAccessibilitySystem()
    {
        Debug.Log("🎨 Starting Safe Accessibility Setup...");
        
        SetupCameraEffect();
        SetupPauseMenuIntegration();
        
        if (Application.isPlaying)
        {
            SetupGameSettingsInPlayMode();
            TestAccessibilitySystem();
        }
        else
        {
            Debug.Log("🎮 Run in PLAY MODE for complete functionality!");
        }
        
        LogSetupStatus();
    }
    
    void SetupCameraEffect()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
            mainCamera = FindFirstObjectByType<Camera>();
            
        if (mainCamera != null)
        {
            ColorBlindnessEffect effect = mainCamera.GetComponent<ColorBlindnessEffect>();
            if (effect == null)
            {
                mainCamera.gameObject.AddComponent<ColorBlindnessEffect>();
                Debug.Log($"✅ ColorBlindnessEffect added to {mainCamera.name}");
                cameraEffectAdded = true;
            }
            else
            {
                Debug.Log($"✅ ColorBlindnessEffect already exists on {mainCamera.name}");
                cameraEffectAdded = true;
            }
        }
        else
        {
            Debug.LogWarning("⚠️ No camera found for ColorBlindnessEffect");
            cameraEffectAdded = false;
        }
    }
    
    void SetupPauseMenuIntegration()
    {
        PauseMenuController pauseController = FindFirstObjectByType<PauseMenuController>();
        if (pauseController == null)
        {
            Debug.LogWarning("⚠️ PauseMenuController not found");
            return;
        }
        
        // Add SettingsPanel component
        SettingsPanel settingsPanel = pauseController.GetComponent<SettingsPanel>();
        if (settingsPanel == null)
        {
            settingsPanel = pauseController.gameObject.AddComponent<SettingsPanel>();
            Debug.Log("✅ SettingsPanel added to PauseMenuController");
            settingsPanelAdded = true;
        }
        else
        {
            Debug.Log("✅ SettingsPanel already exists");
            settingsPanelAdded = true;
        }
        
        // Connect the settings button
        if (pauseController.settingsButton == null)
        {
            Button settingsButton = FindSettingsButtonInPauseMenu(pauseController);
            if (settingsButton != null)
            {
                pauseController.settingsButton = settingsButton;
                Debug.Log($"✅ Settings button connected: {settingsButton.name}");
                pauseMenuConnected = true;
            }
            else
            {
                Debug.LogWarning("⚠️ Settings button not found - please assign manually");
                pauseMenuConnected = false;
            }
        }
        else
        {
            Debug.Log("✅ Settings button already assigned");
            pauseMenuConnected = true;
        }
    }
    
    void SetupGameSettingsInPlayMode()
    {
        if (GameSettings.Instance == null)
        {
            GameObject settingsObj = new GameObject("🎛️ GameSettings");
            GameSettings gameSettings = settingsObj.AddComponent<GameSettings>();
            DontDestroyOnLoad(settingsObj);
            Debug.Log("✅ GameSettings created and persisted");
        }
        else
        {
            Debug.Log("✅ GameSettings already exists");
        }
    }
    
    Button FindSettingsButtonInPauseMenu(PauseMenuController pauseController)
    {
        // Look for settings button by path
        Transform settingsTransform = pauseController.transform.Find("Panel/Settings");
        if (settingsTransform != null)
        {
            return settingsTransform.GetComponent<Button>();
        }
        
        // Look for any button with "Settings" in name
        Button[] buttons = pauseController.GetComponentsInChildren<Button>();
        foreach (var button in buttons)
        {
            if (button.name.ToLower().Contains("settings"))
                return button;
        }
        
        return null;
    }
    
    void TestAccessibilitySystem()
    {
        if (GameSettings.Instance != null)
        {
            // Test the system briefly
            GameSettings.Instance.SetColorBlindnessEnabled(true);
            GameSettings.Instance.SetColorBlindnessType(ColorBlindnessType.Deuteranopia);
            
            Debug.Log("🧪 Testing colorblindness effect...");
            
            // Reset after a moment
            Invoke(nameof(ResetTestEffect), 2f);
        }
    }
    
    void ResetTestEffect()
    {
        if (GameSettings.Instance != null)
        {
            GameSettings.Instance.SetColorBlindnessEnabled(false);
            Debug.Log("🧪 Test complete - effect disabled");
        }
    }
    
    void LogSetupStatus()
    {
        Debug.Log("📋 Accessibility Setup Status:");
        Debug.Log($"   📷 Camera Effect: {(cameraEffectAdded ? "✅" : "❌")}");
        Debug.Log($"   ⚙️ Settings Panel: {(settingsPanelAdded ? "✅" : "❌")}");
        Debug.Log($"   🎮 Pause Menu Connected: {(pauseMenuConnected ? "✅" : "❌")}");
        Debug.Log($"   🎛️ Game Settings: {(GameSettings.Instance != null ? "✅" : "❌")}");
        
        if (cameraEffectAdded && settingsPanelAdded && pauseMenuConnected)
        {
            Debug.Log("🎉 Accessibility System is ready!");
            
            if (Application.isPlaying)
            {
                Debug.Log("💡 Try pausing the game and clicking Settings!");
            }
            else
            {
                Debug.Log("💡 Enter PLAY MODE and pause to test settings!");
            }
        }
        else
        {
            Debug.LogWarning("⚠️ Some setup steps failed - check the logs above");
        }
    }
    
    [ContextMenu("🔍 Check System Status")]
    public void CheckSystemStatus()
    {
        cameraEffectAdded = FindFirstObjectByType<ColorBlindnessEffect>() != null;
        settingsPanelAdded = FindFirstObjectByType<SettingsPanel>() != null;
        
        PauseMenuController pauseController = FindFirstObjectByType<PauseMenuController>();
        pauseMenuConnected = pauseController != null && pauseController.settingsButton != null;
        
        LogSetupStatus();
    }
}