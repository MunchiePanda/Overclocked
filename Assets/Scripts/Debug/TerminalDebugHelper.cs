using UnityEngine;

public class TerminalDebugHelper : MonoBehaviour
{
    [Header("Debug Info")]
    public bool showDebugInfo = true;
    public KeyCode debugKey = KeyCode.F1;
    
    void Update()
    {
        if (Input.GetKeyDown(debugKey))
        {
            DebugSceneInfo();
        }
    }
    
    void DebugSceneInfo()
    {
        Debug.Log("=== TERMINAL DEBUG INFO ===");
        
        // Find all PlayerControllers
        PlayerController[] players = FindObjectsByType<PlayerController>(FindObjectsSortMode.None);
        Debug.Log($"Found {players.Length} PlayerController(s) in scene");
        
        foreach (PlayerController player in players)
        {
            Alteruna.Avatar avatar = player.GetComponent<Alteruna.Avatar>();
            bool isLocal = avatar != null && avatar.IsMe;
            Debug.Log($"Player: {player.name}, Position: {player.transform.position}, IsLocal: {isLocal}");
            Debug.Log($"  - Interaction Distance: {player.maxInteractionDistance}");
            Debug.Log($"  - Interactable Layer: {player.interactableLayer.value} (Layer mask)");
            
            // Get layer names
            string layerNames = "";
            for (int i = 0; i < 32; i++)
            {
                if ((player.interactableLayer.value & (1 << i)) != 0)
                {
                    layerNames += LayerMask.LayerToName(i) + " ";
                }
            }
            Debug.Log($"  - Looking for layers: {layerNames}");
        }
        
        // Find all terminals
        InteractableAITerminal[] oldTerminals = FindObjectsByType<InteractableAITerminal>(FindObjectsSortMode.None);
        InteractableAITerminalNew[] newTerminals = FindObjectsByType<InteractableAITerminalNew>(FindObjectsSortMode.None);
        
        Debug.Log($"Found {oldTerminals.Length} old terminal(s) and {newTerminals.Length} new terminal(s)");
        
        foreach (InteractableAITerminal terminal in oldTerminals)
        {
            Debug.Log($"Old Terminal: {terminal.name}, Layer: {LayerMask.LayerToName(terminal.gameObject.layer)}, Position: {terminal.transform.position}");
        }
        
        foreach (InteractableAITerminalNew terminal in newTerminals)
        {
            Debug.Log($"New Terminal: {terminal.name}, Layer: {LayerMask.LayerToName(terminal.gameObject.layer)}, Position: {terminal.transform.position}");
        }
        
        // Test raycast from camera to terminal
        Camera playerCamera = Camera.main;
        if (playerCamera != null)
        {
            Vector3 cameraPos = playerCamera.transform.position;
            Vector3 cameraForward = playerCamera.transform.forward;
            
            Debug.Log($"Camera Position: {cameraPos}, Forward: {cameraForward}");
            
            // Test raycast to all layers
            Ray ray = new Ray(cameraPos, cameraForward);
            RaycastHit[] hits = Physics.RaycastAll(ray, 10f);
            Debug.Log($"Raycast found {hits.Length} hits:");
            
            foreach (RaycastHit hit in hits)
            {
                Debug.Log($"  - Hit: {hit.collider.name}, Layer: {LayerMask.LayerToName(hit.collider.gameObject.layer)}, Distance: {hit.distance}");
            }
        }
        
        Debug.Log("=== END DEBUG INFO ===");
    }
    
    void OnGUI()
    {
        if (showDebugInfo)
        {
            GUILayout.BeginArea(new Rect(10, 10, 300, 200));
            GUILayout.Label($"Press {debugKey} for debug info");
            
            PlayerController[] players = FindObjectsByType<PlayerController>(FindObjectsSortMode.None);
            GUILayout.Label($"Players in scene: {players.Length}");
            
            if (players.Length > 0)
            {
                PlayerController localPlayer = null;
                foreach (PlayerController player in players)
                {
                    Alteruna.Avatar avatar = player.GetComponent<Alteruna.Avatar>();
                    if (avatar != null && avatar.IsMe)
                    {
                        localPlayer = player;
                        break;
                    }
                }
                
                if (localPlayer != null)
                {
                    GUILayout.Label($"Local player found: {localPlayer.name}");
                    GUILayout.Label($"Position: {localPlayer.transform.position}");
                }
            }
            
            GUILayout.EndArea();
        }
    }
}