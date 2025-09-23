using UnityEngine;

/// <summary>
/// Instant door creator - creates the complete escape door system immediately
/// </summary>
public class InstantDoorCreator : MonoBehaviour
{
    [ContextMenu("🚀 Create Complete Escape Door System NOW")]
    public void CreateEscapeDoorSystemNow()
    {
        Debug.Log("🚀 Creating complete escape door system...");
        
        // Find existing helper or create new one
        CompleteDoorSetupHelper helper = FindFirstObjectByType<CompleteDoorSetupHelper>();
        
        if (helper == null)
        {
            GameObject helperObject = new GameObject("CompleteDoorSetupHelper");
            helper = helperObject.AddComponent<CompleteDoorSetupHelper>();
        }
        
        // Configure and run
        helper.autoSetupOnStart = false; // Don't auto-run
        helper.showDetailedLogs = true;
        helper.doorPosition = new Vector3(15f, 0f, -46f);
        helper.requiredPuzzleCount = 3;
        
        // Create the system immediately
        helper.CreateCompleteEscapeDoorSystem();
        
        Debug.Log("✅ Complete escape door system created!");
        Debug.Log("🎮 System includes:");
        Debug.Log("   • Physical door with mesh and materials");
        Debug.Log("   • Interactive keypad on the door");
        Debug.Log("   • UI interface with keypad and input field");
        Debug.Log("   • Integration with escape code manager");
        Debug.Log("   • Connection to win screen");
        Debug.Log("   • Player interaction system");
        Debug.Log("");
        Debug.Log("🎯 To test:");
        Debug.Log("   1. Walk up to the door (you'll see a prompt)");
        Debug.Log("   2. Press E to open the interface");
        Debug.Log("   3. Complete puzzles to generate escape code");
        Debug.Log("   4. Enter code to unlock and win!");
    }
}