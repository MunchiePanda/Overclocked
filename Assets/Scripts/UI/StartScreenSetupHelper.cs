using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Automatically sets up StartScreenManager component references
/// </summary>
public class StartScreenSetupHelper : MonoBehaviour
{
    [Header("Auto-Setup Options")]
    [Tooltip("Auto-assign UI references on Start")]
    public bool autoSetupOnStart = true;
    
    [Tooltip("Enable debug logging")]
    public bool enableDebugLogs = true;
    
    void Start()
    {
        if (autoSetupOnStart)
        {
            SetupStartScreenManager();
        }
    }
    
    void SetupStartScreenManager()
    {
        StartScreenManager manager = FindFirstObjectByType<StartScreenManager>();
        
        if (manager == null)
        {
            Debug.LogError("❌ StartScreenManager not found!");
            return;
        }
        
        if (enableDebugLogs)
            Debug.Log("🔧 Setting up StartScreenManager references...");
        
        // Use reflection to set private fields since they're serialized
        var playButtonField = manager.GetType().GetField("playButton", 
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        var quitButtonField = manager.GetType().GetField("quitButton", 
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        var titleTextField = manager.GetType().GetField("titleText", 
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        
        // Find and assign Play Button
        if (playButtonField?.GetValue(manager) == null)
        {
            Button playButton = FindButtonByName("PlayButton");
            if (playButton != null)
            {
                playButtonField?.SetValue(manager, playButton);
                if (enableDebugLogs)
                    Debug.Log($"✅ Auto-assigned PlayButton to StartScreenManager");
            }
            else
            {
                Debug.LogWarning("⚠️ PlayButton not found!");
            }
        }
        
        // Find and assign Quit Button
        if (quitButtonField?.GetValue(manager) == null)
        {
            Button quitButton = FindButtonByName("QuitButton");
            if (quitButton != null)
            {
                quitButtonField?.SetValue(manager, quitButton);
                if (enableDebugLogs)
                    Debug.Log($"✅ Auto-assigned QuitButton to StartScreenManager");
            }
            else
            {
                Debug.LogWarning("⚠️ QuitButton not found!");
            }
        }
        
        // Find and assign Title Text
        if (titleTextField?.GetValue(manager) == null)
        {
            TMP_Text titleText = FindTitleText();
            if (titleText != null)
            {
                titleTextField?.SetValue(manager, titleText);
                if (enableDebugLogs)
                    Debug.Log($"✅ Auto-assigned Title Text to StartScreenManager");
            }
            else
            {
                Debug.LogWarning("⚠️ Title Text not found!");
            }
        }
        
        // Manually call the setup methods to ensure buttons are properly connected
        var setupButtonsMethod = manager.GetType().GetMethod("SetupButtons", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        setupButtonsMethod?.Invoke(manager, null);
        
        if (enableDebugLogs)
            Debug.Log("✅ StartScreenManager setup completed!");
    }
    
    Button FindButtonByName(string name)
    {
        Button[] buttons = FindObjectsByType<Button>(FindObjectsSortMode.None);
        foreach (var button in buttons)
        {
            if (button.name.Equals(name, System.StringComparison.OrdinalIgnoreCase))
                return button;
        }
        return null;
    }
    
    TMP_Text FindTitleText()
    {
        TMP_Text[] texts = FindObjectsByType<TMP_Text>(FindObjectsSortMode.None);
        foreach (var text in texts)
        {
            // Look for text that might be the title
            if (text.name.ToLower().Contains("title") || 
                text.text.ToUpper().Contains("OVERCLOCKED") ||
                text.text.ToUpper().Contains("TITLE"))
            {
                return text;
            }
        }
        return null;
    }
    
    [ContextMenu("🔧 Setup StartScreenManager")]
    public void ManualSetup()
    {
        SetupStartScreenManager();
    }
    
    [ContextMenu("🧪 Test Button Clicks")]
    public void TestButtonClicks()
    {
        StartScreenManager manager = FindFirstObjectByType<StartScreenManager>();
        if (manager != null)
        {
            Debug.Log("🧪 Testing Play button click...");
            manager.OnPlayButtonClicked();
        }
        else
        {
            Debug.LogError("❌ StartScreenManager not found for testing!");
        }
    }
}