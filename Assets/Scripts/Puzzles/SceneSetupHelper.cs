using UnityEngine;

/// <summary>
/// Helper for setting up the escape room system in new scenes
/// </summary>
public class SceneSetupHelper : MonoBehaviour
{
    [Header("🌟 New Scene Setup")]
    [Space(10)]
    [Header("📋 Instructions:")]
    [Header("1️⃣ Add this component to any GameObject")]
    [Header("2️⃣ Click 'Setup New Scene' below")]
    [Header("3️⃣ Use SetUpHelper that gets created")]
    [Space(10)]
    
    [Tooltip("🚀 Create complete escape room setup in this scene")]
    public bool setupNewScene = false;

    void OnValidate()
    {
        if (setupNewScene)
        {
            SetupNewScene();
            setupNewScene = false;
        }
    }

    [ContextMenu("🌟 Setup New Scene")]
    private void SetupNewScene()
    {
        Debug.Log("🌟 Setting up escape room system in new scene...");

        // Check if SetUpHelper already exists
        if (FindFirstObjectByType<EscapeRoomSetupHelper>() != null)
        {
            Debug.LogWarning("⚠️ EscapeRoomSetupHelper already exists in this scene!");
            return;
        }

        // Create SetUpHelper GameObject
        GameObject setupHelper = new GameObject("SetUpHelper");
        setupHelper.AddComponent<EscapeRoomSetupHelper>();
        
        Debug.Log("✅ Created SetUpHelper GameObject");

        // Create basic Canvas if none exists
        Canvas existingCanvas = FindFirstObjectByType<Canvas>();
        if (existingCanvas == null)
        {
            CreateBasicCanvas();
        }

        // Create EventSystem if none exists
        UnityEngine.EventSystems.EventSystem eventSystem = FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>();
        if (eventSystem == null)
        {
            CreateEventSystem();
        }

        Debug.Log("🎯 Scene setup complete!");
        Debug.Log("📝 Next steps:");
        Debug.Log("   1. Select the SetUpHelper GameObject");
        Debug.Log("   2. Use the Legacy section to create your puzzles");
        Debug.Log("   3. Use the NEW TERMINAL SYSTEM for easy setup");
        Debug.Log("   4. Test everything!");
    }

    private void CreateBasicCanvas()
    {
        GameObject canvasObj = new GameObject("Canvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<UnityEngine.UI.CanvasScaler>();
        canvasObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();
        
        Debug.Log("✅ Created basic Canvas");
    }

    private void CreateEventSystem()
    {
        GameObject eventSystemObj = new GameObject("EventSystem");
        eventSystemObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
        eventSystemObj.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        
        Debug.Log("✅ Created EventSystem");
    }

    [ContextMenu("📋 Show Setup Instructions")]
    private void ShowInstructions()
    {
        Debug.Log("📋 ESCAPE ROOM SETUP INSTRUCTIONS:");
        Debug.Log("");
        Debug.Log("🚀 QUICK START:");
        Debug.Log("   1. Run 'Setup New Scene' (if not done)");
        Debug.Log("   2. Select SetUpHelper GameObject");
        Debug.Log("   3. Create puzzles using Legacy section");
        Debug.Log("   4. Click 'Do Complete Setup' in Terminal System");
        Debug.Log("");
        Debug.Log("🔧 DETAILED SETUP:");
        Debug.Log("   1. Create AI Terminal (Legacy)");
        Debug.Log("   2. Create Cipher Wheel UI (Legacy)");
        Debug.Log("   3. Create Shadow Logic UI (Legacy)");
        Debug.Log("   4. Create Frequency UI (Legacy) - This is the 3rd puzzle!");
        Debug.Log("   5. Clean up AI Terminal (New System)");
        Debug.Log("   6. Setup Terminal System (New System)");
        Debug.Log("   7. Create Puzzle Terminals (New System)");
        Debug.Log("   8. Style All Terminals (New System)");
        Debug.Log("   9. Test Terminal System (New System)");
        Debug.Log("");
        Debug.Log("🐛 FIXING BUTTON ISSUES:");
        Debug.Log("   • SetUpHelper now has UIConnectionHelper component");
        Debug.Log("   • Use 'Fix Cipher Wheel Buttons' if rotate buttons don't work");
        Debug.Log("   • Use 'Fix Shadow Logic Sliders' for slider issues");
        Debug.Log("");
        Debug.Log("🎮 TESTING:");
        Debug.Log("   • Walk up to terminals and press E");
        Debug.Log("   • UI should open in full screen");
        Debug.Log("   • Movement should be locked");
        Debug.Log("   • Press ESC to close and resume movement");
    }
}