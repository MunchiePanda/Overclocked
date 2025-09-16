using UnityEngine;

/// <summary>
/// Quick script to clean up the AITerminal GameObject
/// </summary>
public class CleanupAITerminal : MonoBehaviour
{
    [Header("Quick Fix")]
    [Tooltip("Click to clean up AITerminal")]
    public bool cleanupAITerminal = false;

    void OnValidate()
    {
        if (cleanupAITerminal)
        {
            CleanUpAITerminal();
            cleanupAITerminal = false;
        }
    }

    [ContextMenu("Clean Up AI Terminal")]
    public void CleanUpAITerminal()
    {
        GameObject aiTerminal = GameObject.Find("AITerminal");
        if (aiTerminal == null)
        {
            Debug.LogError("AITerminal not found!");
            return;
        }

        // Remove the old InteractableAITerminal component
        InteractableAITerminal oldComponent = aiTerminal.GetComponent<InteractableAITerminal>();
        if (oldComponent != null)
        {
            DestroyImmediate(oldComponent);
            Debug.Log("✅ Removed old InteractableAITerminal component");
        }

        // Set to Interactable layer
        int interactableLayer = LayerMask.NameToLayer("Interactable");
        if (interactableLayer != -1)
        {
            aiTerminal.layer = interactableLayer;
            Debug.Log("✅ Set AITerminal to Interactable layer");
        }

        // Make sure BoxCollider is not a trigger (for raycasting)
        BoxCollider collider = aiTerminal.GetComponent<BoxCollider>();
        if (collider != null)
        {
            collider.isTrigger = false;
            Debug.Log("✅ Set BoxCollider isTrigger to false");
        }

        Debug.Log("🎯 AITerminal cleanup complete! Terminal should now work properly.");
    }
}