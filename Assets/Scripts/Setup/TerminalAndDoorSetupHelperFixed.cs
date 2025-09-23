using UnityEngine;

/// <summary>
/// Helper script to set up the updated AI terminal with new puzzles and configure door interaction
/// </summary>
public class TerminalAndDoorSetupHelperFixed : MonoBehaviour
{
    [Header("🖥️ AI Terminal Setup")]
    [Tooltip("The terminal GameObject to update (usually TerminalCanvas or similar)")]
    public GameObject terminalObject;
    
    [Tooltip("Replace old TerminalControllerNew with updated version")]
    public bool replaceTerminalController = true;
    
    [Header("🚪 Door System Setup")]
    [Tooltip("The door GameObject to configure")]
    public GameObject doorObject;
    
    [Tooltip("Default door code")]
    public string doorCode = "1234";
    
    [Header("🎯 Auto-Find Settings")]
    [Tooltip("Automatically find terminal and door in scene")]
    public bool autoFindComponents = true;
    
    [Tooltip("Show detailed setup logs")]
    public bool showDetailedLogs = true;

    [ContextMenu("🚀 Setup Complete Terminal & Door System")]
    public void SetupCompleteSystem()
    {
        Debug.Log("🚀 Setting up complete AI Terminal and Door system...");
        
        if (autoFindComponents)
        {
            FindComponents();
        }
        
        int setupCount = 0;
        
        // Setup AI Terminal
        if (SetupAITerminal())
            setupCount++;
        
        // Setup Door System
        if (SetupDoorSystem())
            setupCount++;
        
        Debug.Log($"✅ Setup complete! Successfully configured {setupCount}/2 systems");
        Debug.Log("🎮 Your updated escape room features:");
        Debug.Log("  • AI Terminal with Riddle, Cipher, and Color Light puzzles");
        Debug.Log("  • Physical door interaction with E key");
        Debug.Log("  • Central cursor management for all UI systems");
        
        Debug.Log("\n🧪 Test your setup:");
        Debug.Log("  1. Access AI Terminal → Should show new puzzle options");
        Debug.Log("  2. Walk to door → Press E to interact");
        Debug.Log("  3. Test riddle system → AI should give nonsense responses during riddle solving");
    }
    
    void FindComponents()
    {
        if (terminalObject == null)
        {
            // Look for common terminal names
            terminalObject = GameObject.Find("TerminalCanvas");
            if (terminalObject == null)
                terminalObject = GameObject.Find("Terminal");
            if (terminalObject == null)
                terminalObject = GameObject.Find("AITerminal");
            
            if (showDetailedLogs && terminalObject != null)
                Debug.Log($"🔍 Found terminal: {terminalObject.name}");
        }
        
        if (doorObject == null)
        {
            // Look for door
            doorObject = GameObject.Find("Door");
            if (doorObject == null)
            {
                Door doorComponent = FindFirstObjectByType<Door>();
                if (doorComponent != null)
                    doorObject = doorComponent.gameObject;
            }
            
            if (showDetailedLogs && doorObject != null)
                Debug.Log($"🔍 Found door: {doorObject.name}");
        }
    }
    
    bool SetupAITerminal()
    {
        try
        {
            if (terminalObject == null)
            {
                Debug.LogWarning("⚠️ No terminal object found. Please assign or create a TerminalCanvas GameObject.");
                return false;
            }
            
            // Check if we need to replace the old controller
            if (replaceTerminalController)
            {
                TerminalControllerNew oldController = terminalObject.GetComponent<TerminalControllerNew>();
                TerminalControllerUpdatedFixed newController = terminalObject.GetComponent<TerminalControllerUpdatedFixed>();
                
                if (oldController != null && newController == null)
                {
                    // Copy important references from old controller
                    var terminalUI = oldController.terminalUI;
                    var hintText = oldController.hintText;
                    var button1 = oldController.button1;
                    var button2 = oldController.button2;
                    var button3 = oldController.button3;
                    var button4 = oldController.button4;
                    var buttonText1 = oldController.buttonText1;
                    var buttonText2 = oldController.buttonText2;
                    var buttonText3 = oldController.buttonText3;
                    var buttonText4 = oldController.buttonText4;
                    var closeButton = oldController.closeButton;
                    var terminalHeader = oldController.terminalHeader;
                    var instructionText = oldController.instructionText;
                    var mainCamera = oldController.mainCamera;
                    var terminalCamera = oldController.terminalCamera;
                    var typingSpeed = oldController.typingSpeed;
                    var buttonPressSound = oldController.buttonPressSound;
                    var terminalOpenSound = oldController.terminalOpenSound;
                    var terminalCloseSound = oldController.terminalCloseSound;
                    var puzzleCompleteSound = oldController.puzzleCompleteSound;
                    
                    // Remove old controller
                    DestroyImmediate(oldController);
                    
                    // Add new controller
                    newController = terminalObject.AddComponent<TerminalControllerUpdatedFixed>();
                    
                    // Assign references to new controller
                    newController.terminalUI = terminalUI;
                    newController.hintText = hintText;
                    newController.button1 = button1;
                    newController.button2 = button2;
                    newController.button3 = button3;
                    newController.button4 = button4;
                    newController.buttonText1 = buttonText1;
                    newController.buttonText2 = buttonText2;
                    newController.buttonText3 = buttonText3;
                    newController.buttonText4 = buttonText4;
                    newController.closeButton = closeButton;
                    newController.terminalHeader = terminalHeader;
                    newController.instructionText = instructionText;
                    newController.mainCamera = mainCamera;
                    newController.terminalCamera = terminalCamera;
                    newController.typingSpeed = typingSpeed;
                    newController.buttonPressSound = buttonPressSound;
                    newController.terminalOpenSound = terminalOpenSound;
                    newController.terminalCloseSound = terminalCloseSound;
                    newController.puzzleCompleteSound = puzzleCompleteSound;
                    
                    if (showDetailedLogs)
                        Debug.Log("✅ Replaced old TerminalControllerNew with TerminalControllerUpdatedFixed");
                }
                else if (newController == null)
                {
                    // Add new controller if none exists
                    newController = terminalObject.AddComponent<TerminalControllerUpdatedFixed>();
                    if (showDetailedLogs)
                        Debug.Log("✅ Added new TerminalControllerUpdatedFixed");
                }
            }
            
            // Verify puzzle references
            TerminalControllerUpdatedFixed controller = terminalObject.GetComponent<TerminalControllerUpdatedFixed>();
            if (controller != null)
            {
                controller.FindPuzzleReferences();
                if (showDetailedLogs)
                    Debug.Log("✅ AI Terminal configured with new puzzle set");
            }
            
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ Failed to setup AI Terminal: {e.Message}");
            return false;
        }
    }
    
    bool SetupDoorSystem()
    {
        try
        {
            if (doorObject == null)
            {
                Debug.LogWarning("⚠️ No door object found. Create a GameObject named 'Door' to enable door interaction.");
                return false;
            }
            
            // Ensure Door component exists
            Door door = doorObject.GetComponent<Door>();
            if (door == null)
            {
                door = doorObject.AddComponent<Door>();
                if (showDetailedLogs)
                    Debug.Log("✅ Added Door component");
            }
            
            // Set door code
            door.correctCode = doorCode;
            
            // Add DoorInteractionHelper if not present
            DoorInteractionHelper doorHelper = doorObject.GetComponent<DoorInteractionHelper>();
            if (doorHelper == null)
            {
                doorHelper = doorObject.AddComponent<DoorInteractionHelper>();
                if (showDetailedLogs)
                    Debug.Log("✅ Added DoorInteractionHelper");
            }
            
            // Configure door helper
            doorHelper.interactionDistance = 3f;
            doorHelper.promptMessage = "Press E to open door";
            doorHelper.enableDebugLogs = showDetailedLogs;
            doorHelper.showInteractionGizmo = true;
            
            // Setup door UI if not present
            SetupDoorUI(doorHelper);
            
            return true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ Failed to setup door system: {e.Message}");
            return false;
        }
    }
    
    void SetupDoorUI(DoorInteractionHelper doorHelper)
    {
        if (doorHelper.doorUICanvas == null)
        {
            // Look for existing door UI
            Canvas doorCanvas = GameObject.Find("DoorInputPanel")?.GetComponent<Canvas>();
            if (doorCanvas == null)
            {
                // Create door UI
                if (showDetailedLogs)
                    Debug.Log("ℹ️ Door UI not found. You may need to create DoorInputPanel UI manually.");
            }
            else
            {
                doorHelper.doorUICanvas = doorCanvas;
                if (showDetailedLogs)
                    Debug.Log("✅ Connected door UI");
            }
        }
    }
    
    [ContextMenu("📋 Show System Status")]
    public void ShowSystemStatus()
    {
        Debug.Log("📋 Terminal & Door System Status:");
        Debug.Log("==================================");
        
        // Terminal Status
        if (terminalObject != null)
        {
            TerminalControllerNew oldController = terminalObject.GetComponent<TerminalControllerNew>();
            TerminalControllerUpdatedFixed newController = terminalObject.GetComponent<TerminalControllerUpdatedFixed>();
            
            Debug.Log($"🖥️ Terminal Object: ✅ Found ({terminalObject.name})");
            Debug.Log($"📊 Old Controller: {(oldController != null ? "⚠️ Present (needs replacement)" : "✅ Removed")}");
            Debug.Log($"🔄 New Controller: {(newController != null ? "✅ Active" : "❌ Missing")}");
        }
        else
        {
            Debug.Log("🖥️ Terminal Object: ❌ Not Found");
        }
        
        // Door Status
        if (doorObject != null)
        {
            Door door = doorObject.GetComponent<Door>();
            DoorInteractionHelper doorHelper = doorObject.GetComponent<DoorInteractionHelper>();
            
            Debug.Log($"🚪 Door Object: ✅ Found ({doorObject.name})");
            Debug.Log($"🔑 Door Component: {(door != null ? "✅ Present" : "❌ Missing")}");
            Debug.Log($"🎮 Interaction Helper: {(doorHelper != null ? "✅ Present" : "❌ Missing")}");
            
            if (door != null)
            {
                Debug.Log($"🔐 Door Code: {door.correctCode}");
            }
        }
        else
        {
            Debug.Log("🚪 Door Object: ❌ Not Found");
        }
        
        // Puzzle Status
        var riddlePuzzle = FindFirstObjectByType<RiddlePuzzle>();
        var cipherPuzzle = FindFirstObjectByType<ShapeCipherPuzzle>();
        var colorPuzzle = FindFirstObjectByType<ColorLightPuzzle>();
        
        Debug.Log($"🧩 Riddle Puzzle: {(riddlePuzzle != null ? "✅ Present" : "❌ Missing")}");
        Debug.Log($"🔐 Cipher Puzzle: {(cipherPuzzle != null ? "✅ Present" : "❌ Missing")}");
        Debug.Log($"🌈 Color Light Puzzle: {(colorPuzzle != null ? "✅ Present" : "❌ Missing")}");
        
        Debug.Log("==================================");
        
        bool allGood = terminalObject != null && doorObject != null &&
                      terminalObject.GetComponent<TerminalControllerUpdatedFixed>() != null &&
                      doorObject.GetComponent<DoorInteractionHelper>() != null;
        
        if (allGood)
        {
            Debug.Log("🎉 System fully operational! Ready for gameplay!");
        }
        else
        {
            Debug.Log("⚠️ Some components missing - run 'Setup Complete System'");
        }
    }
    
    [ContextMenu("🔧 Find All Components")]
    public void FindAllComponents()
    {
        FindComponents();
        ShowSystemStatus();
    }
}