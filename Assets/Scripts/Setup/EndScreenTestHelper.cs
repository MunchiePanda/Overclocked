using UnityEngine;
using TMPro;

/// <summary>
/// Simple helper to test the end screen integration
/// </summary>
public class EndScreenTestHelper : MonoBehaviour
{
    [Header("Testing Controls")]
    [Tooltip("Press this key to simulate completing all puzzles and unlocking door")]
    public KeyCode testUnlockKey = KeyCode.F9;
    
    [Tooltip("Press this key to directly show the end screen")]
    public KeyCode testEndScreenKey = KeyCode.F10;
    
    [Tooltip("Press this key to reset the escape system")]
    public KeyCode resetKey = KeyCode.F11;
    
    [Header("Debug Info")]
    [Tooltip("Display debug info on screen")]
    public bool showDebugInfo = true;
    
    [Tooltip("Text component to show debug info")]
    public TMP_Text debugInfoText;
    
    private EscapeCodeManager escapeManager;
    private WinScreen winScreen;
    private Door[] doors;
    
    void Start()
    {
        // Find components
        escapeManager = FindFirstObjectByType<EscapeCodeManager>();
        winScreen = FindFirstObjectByType<WinScreen>();
        doors = FindObjectsByType<Door>(FindObjectsSortMode.None);
        
        // Create debug info text if none assigned
        if (showDebugInfo && debugInfoText == null)
        {
            CreateDebugInfoText();
        }
        
        Debug.Log("🧪 EndScreenTestHelper ready! Press F9 to test unlock, F10 for end screen, F11 to reset");
    }
    
    void Update()
    {
        // Handle test inputs
        if (Input.GetKeyDown(testUnlockKey))
        {
            TestCompleteEscape();
        }
        
        if (Input.GetKeyDown(testEndScreenKey))
        {
            TestShowEndScreen();
        }
        
        if (Input.GetKeyDown(resetKey))
        {
            TestReset();
        }
        
        // Update debug info
        if (showDebugInfo && debugInfoText != null)
        {
            UpdateDebugInfo();
        }
    }
    
    void TestCompleteEscape()
    {
        Debug.Log("🧪 Testing complete escape sequence...");
        
        if (escapeManager == null)
        {
            Debug.LogError("❌ EscapeCodeManager not found!");
            return;
        }
        
        // Simulate completing all required puzzles
        Debug.Log("📝 Simulating puzzle completions...");
        escapeManager.OnPuzzleCompleted(1234);
        escapeManager.OnPuzzleCompleted(5678);
        escapeManager.OnPuzzleCompleted(9012);
        
        // Wait a moment then unlock door
        Invoke(nameof(UnlockDoorWithGeneratedCode), 1f);
    }
    
    void UnlockDoorWithGeneratedCode()
    {
        if (escapeManager != null && escapeManager.IsEscapeCodeReady())
        {
            string finalCode = escapeManager.GetFinalEscapeCode();
            Debug.Log($"🔑 Generated escape code: {finalCode}");
            
            // Find the escape door and unlock it
            if (doors.Length > 0)
            {
                doors[0].UnlockDoor(finalCode);
                Debug.Log($"🚪 Unlocked door with code: {finalCode}");
            }
            else
            {
                Debug.LogWarning("⚠️ No doors found to unlock!");
                // Directly trigger escape successful
                escapeManager.OnEscapeSuccessful();
            }
        }
        else
        {
            Debug.LogError("❌ Escape code not ready or EscapeCodeManager missing!");
        }
    }
    
    void TestShowEndScreen()
    {
        Debug.Log("🧪 Testing end screen display directly...");
        
        if (winScreen != null)
        {
            winScreen.ShowWinScreen();
            Debug.Log("✅ End screen show command sent");
        }
        else
        {
            Debug.LogError("❌ WinScreen component not found!");
        }
    }
    
    void TestReset()
    {
        Debug.Log("🧪 Testing system reset...");
        
        if (escapeManager != null)
        {
            escapeManager.ResetEscapeProgress();
            Debug.Log("✅ Escape system reset");
        }
        
        if (winScreen != null)
        {
            winScreen.HideWinScreen();
            Debug.Log("✅ Win screen hidden");
        }
    }
    
    void CreateDebugInfoText()
    {
        // Create a canvas for debug info
        GameObject debugCanvas = new GameObject("DebugInfoCanvas");
        Canvas canvas = debugCanvas.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 1000;
        
        // Add CanvasScaler
        debugCanvas.AddComponent<UnityEngine.UI.CanvasScaler>();
        
        // Create text object
        GameObject textObj = new GameObject("DebugInfoText");
        textObj.transform.SetParent(debugCanvas.transform, false);
        
        debugInfoText = textObj.AddComponent<TMP_Text>();
        debugInfoText.text = "Debug Info";
        debugInfoText.fontSize = 14;
        debugInfoText.color = Color.white;
        
        // Position in top-left corner
        RectTransform rectTransform = textObj.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0, 1);
        rectTransform.anchorMax = new Vector2(0, 1);
        rectTransform.pivot = new Vector2(0, 1);
        rectTransform.anchoredPosition = new Vector2(10, -10);
        rectTransform.sizeDelta = new Vector2(400, 200);
        
        Debug.Log("📊 Debug info display created");
    }
    
    void UpdateDebugInfo()
    {
        if (debugInfoText == null) return;
        
        string info = "🧪 END SCREEN TEST HELPER\n";
        info += $"F9: Test Complete Escape | F10: Show End Screen | F11: Reset\n\n";
        
        // Escape Manager Info
        if (escapeManager != null)
        {
            info += $"🎯 Escape Manager: ✅\n";
            info += $"   Collected: {escapeManager.GetCollectedCount()}/{escapeManager.GetRequiredCount()}\n";
            info += $"   Code Ready: {(escapeManager.IsEscapeCodeReady() ? "✅" : "❌")}\n";
            if (escapeManager.IsEscapeCodeReady())
                info += $"   Final Code: {escapeManager.GetFinalEscapeCode()}\n";
        }
        else
        {
            info += "🎯 Escape Manager: ❌\n";
        }
        
        // Win Screen Info
        if (winScreen != null)
        {
            info += $"🏆 Win Screen: ✅\n";
        }
        else
        {
            info += "🏆 Win Screen: ❌\n";
        }
        
        // Door Info
        info += $"🚪 Doors Found: {doors.Length}\n";
        
        debugInfoText.text = info;
    }
    
    void OnGUI()
    {
        if (!showDebugInfo) return;
        
        // Show instructions
        GUI.color = Color.yellow;
        GUI.Label(new Rect(10, Screen.height - 80, 300, 60), 
            "F9: Test Complete Escape\nF10: Show End Screen\nF11: Reset System");
        GUI.color = Color.white;
    }
}