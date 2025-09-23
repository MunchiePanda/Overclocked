using UnityEngine;

/// <summary>
/// Quick setup to add the updated terminal controller to your TerminalCanvas
/// </summary>
public class QuickTerminalSetup : MonoBehaviour
{
    [ContextMenu("✨ Add Updated Terminal Controller")]
    public void AddUpdatedTerminalController()
    {
        // Check if we already have a terminal controller
        TerminalControllerNew oldController = GetComponent<TerminalControllerNew>();
        TerminalControllerUpdatedFixed newController = GetComponent<TerminalControllerUpdatedFixed>();
        
        if (newController != null)
        {
            Debug.Log("✅ TerminalControllerUpdatedFixed already present!");
            return;
        }
        
        if (oldController != null)
        {
            Debug.Log("🔄 Replacing old TerminalControllerNew with updated version...");
            
            // Copy references from old controller before removing it
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
            newController = gameObject.AddComponent<TerminalControllerUpdatedFixed>();
            
            // Transfer all references
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
        }
        else
        {
            Debug.Log("➕ Adding new TerminalControllerUpdatedFixed...");
            newController = gameObject.AddComponent<TerminalControllerUpdatedFixed>();
        }
        
        // Auto-find puzzle references
        newController.FindPuzzleReferences();
        
        Debug.Log("🎉 Terminal setup complete!");
        Debug.Log("📋 New Features:");
        Debug.Log("  • Button 1: Riddle Challenge (with AI nonsense responses)");
        Debug.Log("  • Button 2: Cipher Wheel Puzzle"); 
        Debug.Log("  • Button 3: Color Light Puzzle");
        Debug.Log("  • Button 4: Advanced Terminal Access (unlocks when all puzzles complete)");
        
        // Remove this setup component as it's no longer needed
        Debug.Log("🧹 Removing setup component...");
        DestroyImmediate(this);
    }
    
    [ContextMenu("🔍 Find Missing UI References")]
    public void FindMissingReferences()
    {
        TerminalControllerUpdatedFixed controller = GetComponent<TerminalControllerUpdatedFixed>();
        if (controller == null)
        {
            Debug.LogWarning("⚠️ No TerminalControllerUpdatedFixed found. Run 'Add Updated Terminal Controller' first.");
            return;
        }
        
        Debug.Log("🔍 Searching for UI components...");
        
        // Auto-find UI elements in children
        if (controller.terminalUI == null)
        {
            Transform terminalUI = transform.Find("TerminalUI");
            if (terminalUI == null)
                terminalUI = transform.Find("Terminal");
            if (terminalUI == null)
                terminalUI = transform.Find("Panel");
            
            if (terminalUI != null)
            {
                controller.terminalUI = terminalUI.gameObject;
                Debug.Log($"✅ Found terminalUI: {terminalUI.name}");
            }
        }
        
        // Find hint text
        if (controller.hintText == null)
        {
            TMPro.TMP_Text[] texts = GetComponentsInChildren<TMPro.TMP_Text>(true);
            foreach (var text in texts)
            {
                if (text.name.ToLower().Contains("hint") || text.name.ToLower().Contains("description"))
                {
                    controller.hintText = text;
                    Debug.Log($"✅ Found hintText: {text.name}");
                    break;
                }
            }
        }
        
        // Find buttons
        UnityEngine.UI.Button[] buttons = GetComponentsInChildren<UnityEngine.UI.Button>(true);
        int buttonIndex = 1;
        foreach (var button in buttons)
        {
            if (button.name.ToLower().Contains("button") && buttonIndex <= 4)
            {
                switch (buttonIndex)
                {
                    case 1:
                        if (controller.button1 == null) 
                        {
                            controller.button1 = button;
                            Debug.Log($"✅ Found button1: {button.name}");
                        }
                        break;
                    case 2:
                        if (controller.button2 == null) 
                        {
                            controller.button2 = button;
                            Debug.Log($"✅ Found button2: {button.name}");
                        }
                        break;
                    case 3:
                        if (controller.button3 == null) 
                        {
                            controller.button3 = button;
                            Debug.Log($"✅ Found button3: {button.name}");
                        }
                        break;
                    case 4:
                        if (controller.button4 == null) 
                        {
                            controller.button4 = button;
                            Debug.Log($"✅ Found button4: {button.name}");
                        }
                        break;
                }
                buttonIndex++;
            }
            else if (button.name.ToLower().Contains("close"))
            {
                if (controller.closeButton == null)
                {
                    controller.closeButton = button;
                    Debug.Log($"✅ Found closeButton: {button.name}");
                }
            }
        }
        
        Debug.Log("🔍 UI reference search complete!");
    }
}