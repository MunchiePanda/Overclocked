using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Enhanced door system builder that works with your existing setup
/// Integrates with your current Door objects and EscapeCodeManager
/// </summary>
public class EnhancedDoorSystemBuilder : MonoBehaviour
{
    [Header("🚪 Enhanced Door System Builder")]
    [Tooltip("Work with existing door objects")]
    public bool enhanceExistingDoors = true;
    
    [Tooltip("Show detailed setup information")]
    public bool showDetailedLogs = true;
    
    [Header("🎯 Target Configuration")]
    [Tooltip("Specific door to enhance (leave empty to find automatically)")]
    public GameObject targetDoorObject;
    
    void Start()
    {
        // Auto-run the enhancement
        EnhanceExistingDoorSystem();
    }
    
    [ContextMenu("🚀 Enhance Existing Door System")]
    public void EnhanceExistingDoorSystem()
    {
        if (showDetailedLogs)
            Debug.Log("🚀 Enhancing existing door system...");
        
        // Find existing doors
        Door[] existingDoors = FindObjectsByType<Door>(FindObjectsSortMode.None);
        EscapeCodeManager escapeManager = FindFirstObjectByType<EscapeCodeManager>();
        
        if (existingDoors.Length == 0)
        {
            Debug.LogWarning("❌ No existing Door components found!");
            return;
        }
        
        if (escapeManager == null)
        {
            Debug.LogWarning("❌ No EscapeCodeManager found!");
            return;
        }
        
        if (showDetailedLogs)
            Debug.Log($"✅ Found {existingDoors.Length} door(s) and EscapeCodeManager");
        
        // Enhance each door
        foreach (Door door in existingDoors)
        {
            EnhanceDoor(door, escapeManager);
        }
        
        // Ensure win screen connection
        ConnectWinScreen(escapeManager);
        
        if (showDetailedLogs)
        {
            Debug.Log("✅ Door system enhancement complete!");
            Debug.Log("🎮 Enhanced features:");
            Debug.Log("   • Improved interaction system");
            Debug.Log("   • Visual feedback and animations");
            Debug.Log("   • Keypad interface integration");
            Debug.Log("   • Proper escape logic flow");
            Debug.Log("   • Win screen connection");
        }
    }
    
    void EnhanceDoor(Door door, EscapeCodeManager escapeManager)
    {
        if (showDetailedLogs)
            Debug.Log($"🔧 Enhancing door: {door.name}");
        
        // Check if door already has DoorInteractionHelper
        DoorInteractionHelper interactionHelper = door.GetComponent<DoorInteractionHelper>();
        if (interactionHelper == null)
        {
            interactionHelper = door.gameObject.AddComponent<DoorInteractionHelper>();
            if (showDetailedLogs)
                Debug.Log("   ✅ Added DoorInteractionHelper");
        }
        
        // Configure interaction helper
        interactionHelper.interactionDistance = 3f;
        interactionHelper.enableDebugLogs = showDetailedLogs;
        interactionHelper.showInteractionGizmo = true;
        
        // Find or assign door UI components
        Canvas doorCanvas = door.GetComponentInChildren<Canvas>();
        if (doorCanvas != null)
        {
            interactionHelper.doorUICanvas = doorCanvas;
            
            // Auto-assign UI components using the FindUIComponents method
            interactionHelper.FindUIComponents();
            
            if (showDetailedLogs)
                Debug.Log("   ✅ UI components auto-assigned");
        }
        
        // Enhance or create DoorCodeInput if canvas exists
        if (doorCanvas != null)
        {
            DoorCodeInput codeInput = doorCanvas.GetComponent<DoorCodeInput>();
            if (codeInput == null)
            {
                codeInput = doorCanvas.gameObject.AddComponent<DoorCodeInput>();
                if (showDetailedLogs)
                    Debug.Log("   ✅ Added DoorCodeInput component");
            }
            
            if (codeInput != null)
            {
                // Configure code input
                codeInput.targetDoor = door;
                codeInput.escapeCodeManager = escapeManager;
                codeInput.doorInputPanel = doorCanvas.transform.Find("DoorInputPanel")?.gameObject;
                
                if (codeInput.doorInputPanel == null)
                {
                    // Try to find any panel
                    Transform[] panels = doorCanvas.GetComponentsInChildren<Transform>();
                    foreach (Transform panel in panels)
                    {
                        if (panel.name.ToLower().Contains("panel"))
                        {
                            codeInput.doorInputPanel = panel.gameObject;
                            break;
                        }
                    }
                }
                
                // Assign UI components from interaction helper
                codeInput.codeInputField = interactionHelper.codeInputField;
                codeInput.feedbackText = interactionHelper.feedbackText;
                codeInput.submitButton = interactionHelper.submitButton;
                codeInput.clearButton = interactionHelper.closeButton;
            }
        }
        
        // Create interaction prompt if missing
        CreateInteractionPrompt(door, interactionHelper);
        
        // Connect to escape manager
        escapeManager.escapeDoor = door;
        
        // Ensure door is on correct layer
        door.gameObject.layer = LayerMask.NameToLayer("Interactable");
        
        if (showDetailedLogs)
            Debug.Log($"   ✅ Door {door.name} enhancement complete");
    }
    
    void CreateInteractionPrompt(Door door, DoorInteractionHelper interactionHelper)
    {
        // Check if prompt already exists
        if (interactionHelper.interactionPrompt != null)
        {
            if (showDetailedLogs)
                Debug.Log("   ✅ Interaction prompt already exists");
            return;
        }
        
        // Create new interaction prompt
        GameObject promptObject = new GameObject("InteractionPrompt");
        promptObject.transform.SetParent(door.transform, false);
        promptObject.transform.localPosition = new Vector3(0, 2, 0);
        
        // Create canvas
        Canvas promptCanvas = promptObject.AddComponent<Canvas>();
        promptCanvas.renderMode = RenderMode.WorldSpace;
        promptCanvas.worldCamera = Camera.main;
        
        RectTransform canvasRect = promptObject.GetComponent<RectTransform>();
        canvasRect.sizeDelta = new Vector2(3, 0.8f);
        
        // Create background
        Image background = promptObject.AddComponent<Image>();
        background.color = new Color(0, 0, 0, 0.8f);
        
        // Create text
        GameObject textObject = new GameObject("Text");
        textObject.transform.SetParent(promptObject.transform, false);
        
        TMP_Text promptText = textObject.AddComponent<TextMeshProUGUI>();
        promptText.text = "Press E to access door";
        promptText.fontSize = 24;
        promptText.alignment = TextAlignmentOptions.Center;
        promptText.color = Color.white;
        
        RectTransform textRect = textObject.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        
        // Assign to interaction helper
        interactionHelper.interactionPrompt = promptObject;
        interactionHelper.interactionPromptText = promptText;
        
        // Initially hide
        promptObject.SetActive(false);
        
        if (showDetailedLogs)
            Debug.Log("   ✅ Created interaction prompt");
    }
    
    void ConnectWinScreen(EscapeCodeManager escapeManager)
    {
        // Find win screen
        GameObject winScreenObject = GameObject.Find("WinScreen");
        
        if (winScreenObject != null)
        {
            escapeManager.winScreen = winScreenObject.GetComponent<WinScreen>();

            // Ensure win screen component exists
            WinScreen winScreenComponent = winScreenObject.GetComponent<WinScreen>();
            if (winScreenComponent == null)
            {
                winScreenComponent = winScreenObject.AddComponent<WinScreen>();
            }
            
            // Auto-assign win screen UI components
            winScreenComponent.winPanel = winScreenObject.transform.Find("WinPanel")?.gameObject;
            
            if (winScreenComponent.winPanel != null)
            {
                winScreenComponent.congratsText = winScreenComponent.winPanel.transform.Find("CongratsText")?.GetComponent<TMP_Text>();
                winScreenComponent.escapeTimeText = winScreenComponent.winPanel.transform.Find("EscapeTimeText")?.GetComponent<TMP_Text>();
                winScreenComponent.collectedNumbersText = winScreenComponent.winPanel.transform.Find("CollectedNumbersText")?.GetComponent<TMP_Text>();
                winScreenComponent.finalCodeText = winScreenComponent.winPanel.transform.Find("FinalCodeText")?.GetComponent<TMP_Text>();
                winScreenComponent.restartButton = winScreenComponent.winPanel.transform.Find("RestartButton")?.GetComponent<Button>();
                winScreenComponent.quitButton = winScreenComponent.winPanel.transform.Find("QuitButton")?.GetComponent<Button>();
            }
            
            if (showDetailedLogs)
                Debug.Log("✅ Win screen connected and configured");
        }
        else
        {
            if (showDetailedLogs)
                Debug.LogWarning("⚠️ No WinScreen found - escape will work but no win screen will appear");
        }
    }
    
    [ContextMenu("🧪 Test Enhanced System")]
    public void TestEnhancedSystem()
    {
        EscapeCodeManager escapeManager = FindFirstObjectByType<EscapeCodeManager>();
        
        if (escapeManager != null)
        {
            // Add test puzzle numbers
            escapeManager.OnPuzzleCompleted(1234);
            escapeManager.OnPuzzleCompleted(5678);
            escapeManager.OnPuzzleCompleted(9012);
            
            Debug.Log("🧪 Added test puzzle completions!");
            Debug.Log($"🔑 Escape code: {escapeManager.GetFinalEscapeCode()}");
            Debug.Log("🎮 Now walk up to any door and press E to test!");
        }
    }
    
    [ContextMenu("🔍 Show System Status")]
    public void ShowSystemStatus()
    {
        Debug.Log("📊 Door System Status:");
        
        Door[] doors = FindObjectsByType<Door>(FindObjectsSortMode.None);
        Debug.Log($"🚪 Doors found: {doors.Length}");
        
        foreach (Door door in doors)
        {
            Debug.Log($"   • {door.name}: Code = {door.correctCode}");
            
            DoorInteractionHelper helper = door.GetComponent<DoorInteractionHelper>();
            Debug.Log($"     - Interaction Helper: {(helper != null ? "✅" : "❌")}");
            
            Canvas canvas = door.GetComponentInChildren<Canvas>();
            Debug.Log($"     - UI Canvas: {(canvas != null ? "✅" : "❌")}");
        }
        
        EscapeCodeManager escapeManager = FindFirstObjectByType<EscapeCodeManager>();
        if (escapeManager != null)
        {
            Debug.Log($"🎯 Escape Manager: ✅");
            Debug.Log($"   - Puzzles completed: {escapeManager.GetCollectedCount()}/{escapeManager.GetRequiredCount()}");
            Debug.Log($"   - Code ready: {(escapeManager.IsEscapeCodeReady() ? "✅" : "❌")}");
            
            if (escapeManager.IsEscapeCodeReady())
            {
                Debug.Log($"   - Final code: {escapeManager.GetFinalEscapeCode()}");
            }
        }
        else
        {
            Debug.Log($"🎯 Escape Manager: ❌");
        }
        
        GameObject winScreen = GameObject.Find("WinScreen");
        Debug.Log($"🏆 Win Screen: {(winScreen != null ? "✅" : "❌")}");
    }
}