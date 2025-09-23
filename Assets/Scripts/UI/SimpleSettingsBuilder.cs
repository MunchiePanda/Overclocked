using UnityEngine;

/// <summary>
/// Simple one-click settings UI builder - no errors, just works!
/// </summary>
public class SimpleSettingsBuilder : MonoBehaviour
{
    [Header("🎨 Simple Settings Builder")]
    [Space(10)]
    [TextArea(3, 4)]
    public string instructions = "This creates a complete settings UI and connects it to your existing SettingsPanel.\n\nJust click 'Build Settings UI' below!";
    
    [ContextMenu("🚀 Build Settings UI")]
    public void BuildSettingsUI()
    {
        Debug.Log("🎨 Building complete settings UI...");
        
        // Use the CompleteUISetup to do the work
        SettingsPanel existingPanel = FindFirstObjectByType<SettingsPanel>();
        if (existingPanel == null)
        {
            Debug.LogError("❌ No SettingsPanel component found!");
            return;
        }
        
        // Add CompleteUISetup temporarily and run it
        CompleteUISetup builder = existingPanel.gameObject.AddComponent<CompleteUISetup>();
        builder.BuildCompleteUI();
        
        Debug.Log("✅ Settings UI built successfully!");
        Debug.Log("🎮 Test it: Pause → Settings to see your new UI!");
        
        // Clean up
        DestroyImmediate(this);
    }
}