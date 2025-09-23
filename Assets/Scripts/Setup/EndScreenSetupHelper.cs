using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Helps ensure the end screen integration is properly configured
/// </summary>
public class EndScreenSetupHelper : MonoBehaviour
{
    [Header("Debug & Testing")]
    [Tooltip("Enable debug logging")]
    public bool enableDebugLogs = true;
    
    [Header("Manual Testing")]
    [Tooltip("Test the end screen display")]
    public bool testEndScreen = false;
    
    void Start()
    {
        if (enableDebugLogs)
            Debug.Log("🔧 EndScreenSetupHelper started - checking integration...");
            
        StartCoroutine(CheckIntegrationAfterFrame());
    }
    
    System.Collections.IEnumerator CheckIntegrationAfterFrame()
    {
        // Wait for one frame to ensure all objects are initialized
        yield return null;
        
        CheckEndScreenIntegration();
    }
    
    void CheckEndScreenIntegration()
    {
        if (enableDebugLogs)
            Debug.Log("🔍 Checking end screen integration...");
        
        // Find essential components
        EscapeCodeManager escapeManager = FindFirstObjectByType<EscapeCodeManager>();
        WinScreen winScreen = FindFirstObjectByType<WinScreen>();
        Door[] doors = FindObjectsByType<Door>(FindObjectsSortMode.None);
        
        // Check EscapeCodeManager
        if (escapeManager == null)
        {
            Debug.LogError("❌ EscapeCodeManager not found! End screen won't work.");
            return;
        }
        else
        {
            if (enableDebugLogs)
                Debug.Log("✅ EscapeCodeManager found");
        }
        
        // Check WinScreen
        if (winScreen == null)
        {
            Debug.LogError("❌ WinScreen component not found! End screen won't work.");
            return;
        }
        else
        {
            if (enableDebugLogs)
                Debug.Log("✅ WinScreen component found");
        }
        
        // Check WinScreen references
        CheckWinScreenReferences(winScreen);
        
        // Check EscapeCodeManager references  
        CheckEscapeManagerReferences(escapeManager, winScreen, doors);
        
        // Check Door references
        CheckDoorReferences(doors, escapeManager);
        
        if (enableDebugLogs)
            Debug.Log("🎯 End screen integration check completed!");
    }
    
    void CheckWinScreenReferences(WinScreen winScreen)
    {
        if (enableDebugLogs)
            Debug.Log("🔍 Checking WinScreen UI references...");
        
        // Use reflection to check private fields since they're serialized
        var winPanelField = winScreen.GetType().GetField("winPanel", 
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        
        if (winPanelField?.GetValue(winScreen) == null)
        {
            Debug.LogWarning("⚠️ WinScreen.winPanel is not assigned! Attempting to find it...");
            
            // Try to find WinPanel
            GameObject winPanel = GameObject.Find("WinPanel");
            if (winPanel == null)
            {
                // Look for it in WinScreen children
                Transform winScreenTransform = winScreen.transform;
                for (int i = 0; i < winScreenTransform.childCount; i++)
                {
                    Transform child = winScreenTransform.GetChild(i);
                    if (child.name.ToLower().Contains("panel"))
                    {
                        winPanel = child.gameObject;
                        break;
                    }
                }
            }
            
            if (winPanel != null)
            {
                winPanelField?.SetValue(winScreen, winPanel);
                if (enableDebugLogs)
                    Debug.Log($"✅ Auto-assigned WinPanel: {winPanel.name}");
            }
            else
            {
                Debug.LogError("❌ WinPanel not found! End screen UI won't display.");
            }
        }
        else
        {
            if (enableDebugLogs)
                Debug.Log("✅ WinScreen.winPanel is assigned");
        }
        
        // Check other UI references
        CheckWinScreenUIElements(winScreen);
    }
    
    void CheckWinScreenUIElements(WinScreen winScreen)
    {
        Transform winScreenTransform = winScreen.transform;
        
        // Find and assign text components if missing
        var congratsTextField = winScreen.GetType().GetField("congratsText", 
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        var escapeTimeTextField = winScreen.GetType().GetField("escapeTimeText", 
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        var restartButtonField = winScreen.GetType().GetField("restartButton", 
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        var quitButtonField = winScreen.GetType().GetField("quitButton", 
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        
        // Auto-assign if missing
        if (congratsTextField?.GetValue(winScreen) == null)
        {
            TMP_Text congratsText = FindTextByName(winScreenTransform, "CongratsText");
            if (congratsText != null)
            {
                congratsTextField?.SetValue(winScreen, congratsText);
                if (enableDebugLogs)
                    Debug.Log("✅ Auto-assigned congratsText");
            }
        }
        
        if (escapeTimeTextField?.GetValue(winScreen) == null)
        {
            TMP_Text escapeTimeText = FindTextByName(winScreenTransform, "EscapeTimeText");
            if (escapeTimeText != null)
            {
                escapeTimeTextField?.SetValue(winScreen, escapeTimeText);
                if (enableDebugLogs)
                    Debug.Log("✅ Auto-assigned escapeTimeText");
            }
        }
        
        if (restartButtonField?.GetValue(winScreen) == null)
        {
            Button restartButton = FindButtonByName(winScreenTransform, "RestartButton");
            if (restartButton != null)
            {
                restartButtonField?.SetValue(winScreen, restartButton);
                if (enableDebugLogs)
                    Debug.Log("✅ Auto-assigned restartButton");
            }
        }
        
        if (quitButtonField?.GetValue(winScreen) == null)
        {
            Button quitButton = FindButtonByName(winScreenTransform, "QuitButton");
            if (quitButton != null)
            {
                quitButtonField?.SetValue(winScreen, quitButton);
                if (enableDebugLogs)
                    Debug.Log("✅ Auto-assigned quitButton");
            }
        }
    }
    
    void CheckEscapeManagerReferences(EscapeCodeManager escapeManager, WinScreen winScreen, Door[] doors)
    {
        if (enableDebugLogs)
            Debug.Log("🔍 Checking EscapeCodeManager references...");
        
        // Check WinScreen reference
        var winScreenField = escapeManager.GetType().GetField("winScreen", 
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        
        if (winScreenField?.GetValue(escapeManager) == null)
        {
            Debug.LogWarning("⚠️ EscapeCodeManager.winScreen is not assigned! Auto-assigning...");
            winScreenField?.SetValue(escapeManager, winScreen);
            if (enableDebugLogs)
                Debug.Log("✅ Auto-assigned WinScreen to EscapeCodeManager");
        }
        else
        {
            if (enableDebugLogs)
                Debug.Log("✅ EscapeCodeManager.winScreen is assigned");
        }
        
        // Check escape door reference
        var escapeDoorField = escapeManager.GetType().GetField("escapeDoor", 
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
        
        if (escapeDoorField?.GetValue(escapeManager) == null && doors.Length > 0)
        {
            Debug.LogWarning("⚠️ EscapeCodeManager.escapeDoor is not assigned! Auto-assigning first door...");
            escapeDoorField?.SetValue(escapeManager, doors[0]);
            if (enableDebugLogs)
                Debug.Log($"✅ Auto-assigned Door '{doors[0].name}' to EscapeCodeManager");
        }
        else if (doors.Length == 0)
        {
            Debug.LogError("❌ No Door components found! Cannot assign escape door.");
        }
        else
        {
            if (enableDebugLogs)
                Debug.Log("✅ EscapeCodeManager.escapeDoor is assigned");
        }
    }
    
    void CheckDoorReferences(Door[] doors, EscapeCodeManager escapeManager)
    {
        if (enableDebugLogs)
            Debug.Log($"🔍 Checking {doors.Length} Door components...");
        
        foreach (Door door in doors)
        {
            // Check if door has required components
            if (door.GetComponent<DoorInteractionHelper>() == null)
            {
                Debug.LogWarning($"⚠️ Door '{door.name}' missing DoorInteractionHelper component!");
            }
        }
    }
    
    TMP_Text FindTextByName(Transform parent, string name)
    {
        TMP_Text[] texts = parent.GetComponentsInChildren<TMP_Text>(true);
        foreach (var text in texts)
        {
            if (text.name.Equals(name, System.StringComparison.OrdinalIgnoreCase))
                return text;
        }
        return null;
    }
    
    Button FindButtonByName(Transform parent, string name)
    {
        Button[] buttons = parent.GetComponentsInChildren<Button>(true);
        foreach (var button in buttons)
        {
            if (button.name.Equals(name, System.StringComparison.OrdinalIgnoreCase))
                return button;
        }
        return null;
    }
    
    // Testing methods
    [ContextMenu("🔍 Check Integration")]
    public void TestCheckIntegration()
    {
        CheckEndScreenIntegration();
    }
    
    [ContextMenu("🎯 Test End Screen")]
    public void TestEndScreen()
    {
        WinScreen winScreen = FindFirstObjectByType<WinScreen>();
        if (winScreen != null)
        {
            Debug.Log("🧪 Testing end screen display...");
            winScreen.ShowWinScreen();
        }
        else
        {
            Debug.LogError("❌ WinScreen not found for testing!");
        }
    }
    
    [ContextMenu("🔑 Test Door Unlock")]
    public void TestDoorUnlock()
    {
        EscapeCodeManager escapeManager = FindFirstObjectByType<EscapeCodeManager>();
        if (escapeManager != null)
        {
            Debug.Log("🧪 Testing escape sequence...");
            
            // Add some test numbers to simulate completed puzzles
            escapeManager.OnPuzzleCompleted(1234);
            escapeManager.OnPuzzleCompleted(5678);
            escapeManager.OnPuzzleCompleted(9012);
            
            // Trigger escape successful
            escapeManager.OnEscapeSuccessful();
        }
        else
        {
            Debug.LogError("❌ EscapeCodeManager not found for testing!");
        }
    }
    
    void Update()
    {
        // Test end screen on key press (for debugging)
        if (testEndScreen && Input.GetKeyDown(KeyCode.F1))
        {
            TestEndScreen();
        }
        
        if (testEndScreen && Input.GetKeyDown(KeyCode.F2))
        {
            TestDoorUnlock();
        }
    }
}