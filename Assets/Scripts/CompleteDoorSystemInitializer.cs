using UnityEngine;

/// <summary>
/// Simple script to initialize the complete door system
/// Add this to any GameObject in the scene to automatically setup the escape door
/// </summary>
public class CompleteDoorSystemInitializer : MonoBehaviour
{
    [Header("🚀 Complete Door System Initializer")]
    [Tooltip("Automatically create the door system on start")]
    public bool initializeOnStart = true;
    
    [Header("🎯 Door Settings")]
    [Tooltip("Position where door should be created")]
    public Vector3 doorPosition = new Vector3(15f, 0f, -46f);
    
    [Tooltip("Number of puzzles required for escape")]
    public int requiredPuzzles = 3;
    
    void Start()
    {
        if (initializeOnStart)
        {
            InitializeDoorSystem();
        }
    }
    
    [ContextMenu("🚀 Initialize Complete Door System")]
    public void InitializeDoorSystem()
    {
        // Create the door setup helper
        GameObject helperObject = new GameObject("CompleteDoorSetupHelper");
        CompleteDoorSetupHelper doorHelper = helperObject.AddComponent<CompleteDoorSetupHelper>();
        
        // Configure the helper
        doorHelper.doorPosition = doorPosition;
        doorHelper.requiredPuzzleCount = requiredPuzzles;
        doorHelper.autoSetupOnStart = true;
        doorHelper.showDetailedLogs = true;
        
        Debug.Log("🚀 Complete door system initializer created!");
        Debug.Log("🎮 The door system will be automatically set up");
        
        // Destroy this initializer as it's no longer needed
        Destroy(gameObject);
    }
}