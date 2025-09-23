using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Quick setup component - add this to any GameObject to instantly setup accessibility
/// </summary>
public class AccessibilityQuickSetup : MonoBehaviour
{
    [Header("🎨 Accessibility System Quick Setup")]
    [Space(10)]
    [TextArea(3, 6)]
    public string instructions = "This component will automatically set up the complete colorblindness accessibility system for your game.\n\nClick 'Setup Now' or it will run automatically when the scene starts.";
    
    [Space(10)]
    [Tooltip("Run setup automatically when scene starts")]
    public bool setupOnStart = true;
    
    void Start()
    {
        if (setupOnStart)
        {
            SetupAccessibilitySystem();
        }
    }
    
    [ContextMenu("🚀 Setup Now")]
    public void SetupAccessibilitySystem()
    {
        Debug.Log("🎨 Setting up Accessibility System...");
        
        // Check if we're in play mode for full setup
        if (!Application.isPlaying)
        {
            Debug.LogWarning("⚠️ For complete setup, please run this in PLAY MODE!");
            Debug.Log("💡 Setting up what we can in edit mode...");
        }
        
        // 1. Create GameSettings (global settings manager)
        SetupGameSettings();
        
        // 2. Add ColorBlindness effect to main camera
        SetupCameraEffect();
        
        // 3. Add SettingsPanel to PauseMenu
        SetupPauseMenuSettings();
        
        // 4. Add AccessibilityInitializer for persistence
        SetupInitializer();
        
        Debug.Log("✅ Accessibility System setup complete!");
        
        if (Application.isPlaying)
        {
            Debug.Log("🎮 Your pause menu now has colorblindness settings!");
            Debug.Log("🧪 Test it: Pause → Settings → Enable colorblindness options");
        }
        else
        {
            Debug.Log("🎮 Enter PLAY MODE to test the colorblindness settings!");
        }
        
        // Auto-destroy this component after setup
        if (Application.isPlaying)
        {
            Destroy(this);
        }
    }
    
    void SetupGameSettings()
    {
        if (GameSettings.Instance == null)
        {
            GameObject settingsObj = new GameObject("🎛️ GameSettings");
            settingsObj.AddComponent<GameSettings>();
            
            // Only call DontDestroyOnLoad in play mode
            if (Application.isPlaying)
            {
                DontDestroyOnLoad(settingsObj);
            }
            
            Debug.Log("✅ GameSettings created - manages all accessibility settings");
        }
        else
        {
            Debug.Log("✅ GameSettings already exists");
        }
    }
    
    void SetupCameraEffect()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera == null)
            mainCamera = FindFirstObjectByType<Camera>();
            
        if (mainCamera != null)
        {
            if (mainCamera.GetComponent<ColorBlindnessEffect>() == null)
            {
                mainCamera.gameObject.AddComponent<ColorBlindnessEffect>();
                Debug.Log($"✅ ColorBlindness effect added to {mainCamera.name}");
            }
            else
            {
                Debug.Log($"✅ ColorBlindness effect already on {mainCamera.name}");
            }
        }
        else
        {
            Debug.LogWarning("⚠️ No camera found - add ColorBlindnessEffect manually");
        }
    }
    
    void SetupPauseMenuSettings()
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
            Debug.Log("✅ SettingsPanel added to pause menu");
        }
        
        // Find and assign settings button automatically
        if (pauseController.settingsButton == null)
        {
            // Try to find the settings button
            Transform settingsTransform = pauseController.transform.Find("Panel/Settings");
            if (settingsTransform != null)
            {
                pauseController.settingsButton = settingsTransform.GetComponent<UnityEngine.UI.Button>();
                Debug.Log("✅ Settings button auto-found and connected");
            }
            else
            {
                Debug.LogWarning("⚠️ Settings button not found - assign manually in PauseMenuController");
            }
        }
        
        Debug.Log("✅ Pause menu integrated with settings system");
    }
    
    void SetupInitializer()
    {
        if (FindFirstObjectByType<AccessibilityInitializer>() == null)
        {
            GameObject initObj = new GameObject("🌟 AccessibilityInitializer");
            initObj.AddComponent<AccessibilityInitializer>();
            Debug.Log("✅ AccessibilityInitializer created for scene management");
        }
        else
        {
            Debug.Log("✅ AccessibilityInitializer already exists");
        }
    }
}