using UnityEngine;

public class FixTerminalSetup : MonoBehaviour
{
    [Header("Quick Fix")]
    [Tooltip("Click to automatically fix the AITerminal setup")]
    public bool fixTerminalSetup = false;
    
    void OnValidate()
    {
        if (fixTerminalSetup)
        {
            FixAITerminal();
            fixTerminalSetup = false;
        }
    }
    
    [ContextMenu("Fix AI Terminal")]
    public void FixAITerminal()
    {
        // Find the AITerminal GameObject
        GameObject aiTerminal = GameObject.Find("AITerminal");
        if (aiTerminal == null)
        {
            Debug.LogError("AITerminal GameObject not found!");
            return;
        }
        
        Debug.Log("Found AITerminal, fixing setup...");
        
        // 1. Set the correct layer
        int interactableLayer = LayerMask.NameToLayer("Interactable");
        if (interactableLayer != -1)
        {
            aiTerminal.layer = interactableLayer;
            Debug.Log($"Set AITerminal layer to: {LayerMask.LayerToName(interactableLayer)}");
        }
        else
        {
            Debug.LogError("Interactable layer not found! Please add it in Project Settings > Tags and Layers");
            return;
        }
        
        // 2. Remove old component and add new one
        InteractableAITerminal oldComponent = aiTerminal.GetComponent<InteractableAITerminal>();
        if (oldComponent != null)
        {
            // Save references before destroying
            TerminalControllerNew terminalController = oldComponent.terminalController;
            string hoverText = oldComponent.hoverText;
            
            DestroyImmediate(oldComponent);
            Debug.Log("Removed old InteractableAITerminal component");
            
            // Add new component
            InteractableAITerminalNew newComponent = aiTerminal.AddComponent<InteractableAITerminalNew>();
            newComponent.terminalController = terminalController;
            newComponent.hoverText = hoverText;
            
            Debug.Log("Added new InteractableAITerminalNew component");
        }
        
        // 3. Fix collider setup
        BoxCollider collider = aiTerminal.GetComponent<BoxCollider>();
        if (collider != null)
        {
            collider.isTrigger = false; // For raycasting detection
            Debug.Log("Set BoxCollider isTrigger to false for raycast detection");
        }
        
        // 4. Check for terminal controller
        TerminalControllerNew controller = aiTerminal.GetComponent<TerminalControllerNew>();
        if (controller == null)
        {
            Debug.LogWarning("No TerminalControllerNew found on AITerminal!");
        }
        
        Debug.Log("AITerminal setup fixed! Now test the interaction:");
        Debug.Log("1. Make sure you have a PlayerController in the scene (spawned via Alteruna)");
        Debug.Log("2. Look at the terminal and press E");
    }
    
    [ContextMenu("Debug Player Info")]
    public void DebugPlayerInfo()
    {
        PlayerController[] players = FindObjectsByType<PlayerController>(FindObjectsSortMode.None);
        Debug.Log($"Found {players.Length} PlayerController(s):");
        
        foreach (PlayerController player in players)
        {
            Alteruna.Avatar avatar = player.GetComponent<Alteruna.Avatar>();
            bool isLocal = avatar != null && avatar.IsMe;
            Debug.Log($"- {player.name}: Local={isLocal}, Position={player.transform.position}");
            Debug.Log($"  Interaction Distance: {player.maxInteractionDistance}");
            Debug.Log($"  Interactable Layer Mask: {player.interactableLayer.value}");
        }
    }
}