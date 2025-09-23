using UnityEngine;

/// <summary>
/// Simple script to set up the enhanced door system
/// Add this to any GameObject in the scene and run it
/// </summary>
public class SetupEnhancedDoorSystem : MonoBehaviour
{
    [ContextMenu("🚀 Setup Enhanced Door System")]
    public void SetupSystem()
    {
        // Create the enhanced door system builder
        GameObject builderObject = new GameObject("EnhancedDoorSystemBuilder");
        EnhancedDoorSystemBuilder builder = builderObject.AddComponent<EnhancedDoorSystemBuilder>();
        
        // Run the enhancement
        builder.EnhanceExistingDoorSystem();
        
        Debug.Log("✅ Enhanced door system setup complete!");
        Debug.Log("🎮 Your existing doors have been enhanced with:");
        Debug.Log("   • Player interaction detection");
        Debug.Log("   • UI management");
        Debug.Log("   • Escape code integration");
        Debug.Log("   • Win screen connection");
        
        // Clean up this setup script
        Destroy(gameObject);
    }
}