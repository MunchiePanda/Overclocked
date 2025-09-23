using UnityEngine;

/// <summary>
/// Helper script to add to your existing PauseSystemSetup
/// </summary>
public class AddToSetupHelper : MonoBehaviour
{
    [ContextMenu("🚀 Complete Accessibility Setup")]
    public void CompleteSetup()
    {
        // Add AccessibilityQuickSetup if not present
        if (GetComponent<AccessibilityQuickSetup>() == null)
        {
            gameObject.AddComponent<AccessibilityQuickSetup>();
            Debug.Log("✅ AccessibilityQuickSetup added!");
        }
        
        // Trigger the setup
        var quickSetup = GetComponent<AccessibilityQuickSetup>();
        if (quickSetup != null)
        {
            quickSetup.SetupAccessibilitySystem();
        }
        
        // Remove this helper component
        DestroyImmediate(this);
    }
}