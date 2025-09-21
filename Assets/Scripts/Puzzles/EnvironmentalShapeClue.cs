using UnityEngine;
using TMPro;

/// <summary>
/// Environmental shape clue that players can find and examine in the level
/// Each shape corresponds to a letter in the cipher puzzle
/// </summary>
public class EnvironmentalShapeClue : MonoBehaviour, IInteractable
{
    [Header("Shape Clue Settings")]
    [Tooltip("The shape symbol this clue represents")]
    public string shapeSymbol = "△";
    
    [Tooltip("The letter this shape decodes to")]
    public string decodedLetter = "A";
    
    [Tooltip("Position in the final word (0-based index)")]
    public int wordPosition = 0;
    
    [Tooltip("Description when examining this clue")]
    [TextArea(2, 4)]
    public string examinationText = "You found a mysterious shape carved into the surface.";
    
    [Tooltip("Additional hint about this clue")]
    public string clueHint = "This symbol appears to be part of a larger code.";

    [Header("Visual Settings")]
    [Tooltip("Material to use when highlighting this clue")]
    public Material highlightMaterial;
    
    [Tooltip("Color to use when this clue is discovered")]
    public Color discoveredColor = Color.green;
    
    [Tooltip("Light component to activate when discovered")]
    public Light clueLight;

    [Header("UI References")]
    [Tooltip("Text to show interaction prompt")]
    public TMP_Text interactionPrompt;
    
    [Tooltip("Text to display when hovering")]
    public string hoverText = "Press E to examine";

    private Material originalMaterial;
    private MeshRenderer meshRenderer;
    private bool isDiscovered = false;
    private bool isPlayerNearby = false;

    void Start()
    {
        InitializeClue();
    }

    void InitializeClue()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null)
            originalMaterial = meshRenderer.material;

        // Hide interaction prompt initially
        if (interactionPrompt != null)
            interactionPrompt.gameObject.SetActive(false);

        // Set up layer for interaction
        gameObject.layer = LayerMask.NameToLayer("Interactable");
        
        // Ensure we have a collider for interaction
        if (GetComponent<Collider>() == null)
        {
            gameObject.AddComponent<BoxCollider>();
        }

        Debug.Log($"Environmental shape clue initialized: {shapeSymbol} → {decodedLetter} (Position: {wordPosition})");
    }

    public void OnHover()
    {
        isPlayerNearby = true;

        // Show interaction prompt
        if (interactionPrompt != null)
        {
            interactionPrompt.text = isDiscovered ? "Shape already discovered" : hoverText;
            interactionPrompt.gameObject.SetActive(true);
        }

        // Highlight the clue if not discovered
        if (!isDiscovered && meshRenderer != null && highlightMaterial != null)
        {
            meshRenderer.material = highlightMaterial;
        }

        Debug.Log($"Player can examine shape clue: {shapeSymbol}");
    }

    public void OnHoverExit()
    {
        isPlayerNearby = false;

        // Hide interaction prompt
        if (interactionPrompt != null)
        {
            interactionPrompt.gameObject.SetActive(false);
        }

        // Restore original material if not discovered
        if (!isDiscovered && meshRenderer != null && originalMaterial != null)
        {
            meshRenderer.material = originalMaterial;
        }

        Debug.Log($"Player left shape clue: {shapeSymbol}");
    }

    public void OnInteract()
    {
        if (isDiscovered)
        {
            // Show already discovered message
            ProvideFeedback("You've already examined this shape clue.");
            return;
        }

        // Mark as discovered
        isDiscovered = true;
        
        // Update visual state
        UpdateVisualState();
        
        // Notify the cipher puzzle about the discovery
        ShapeCipherPuzzle cipherPuzzle = FindFirstObjectByType<ShapeCipherPuzzle>();
        if (cipherPuzzle != null)
        {
            cipherPuzzle.OnShapeDiscovered(shapeSymbol, decodedLetter, wordPosition);
            cipherPuzzle.ProvideFeedback($"Discovered shape: {shapeSymbol} → {decodedLetter}");
        }

        ShowClueDetails();
        
        Debug.Log($"Shape clue discovered: {shapeSymbol} → {decodedLetter} at position {wordPosition}");
    }

    void UpdateVisualState()
    {
        // Change material color to show it's discovered
        if (meshRenderer != null)
        {
            if (highlightMaterial != null)
            {
                Material discoveredMaterial = new Material(highlightMaterial);
                discoveredMaterial.color = discoveredColor;
                meshRenderer.material = discoveredMaterial;
            }
        }

        // Activate light if available
        if (clueLight != null)
        {
            clueLight.color = discoveredColor;
            clueLight.enabled = true;
        }
    }

    void ShowClueDetails()
    {
        string feedback = $"{examinationText}\n\n";
        feedback += $"Shape: {shapeSymbol}\n";
        feedback += $"Decodes to: {decodedLetter}\n";
        feedback += $"Position hint: This appears to be letter #{wordPosition + 1} in the word.\n";
        feedback += $"Hint: {clueHint}";

        ProvideFeedback(feedback);
    }

    void ProvideFeedback(string message)
    {
        ShapeCipherPuzzle cipherPuzzle = FindFirstObjectByType<ShapeCipherPuzzle>();
        if (cipherPuzzle != null)
        {
            cipherPuzzle.ProvideFeedback(message);
        }
        else
        {
            Debug.Log($"Shape Clue Feedback: {message}");
        }
    }

    // Public getters for the cipher puzzle
    public string GetShapeSymbol() => shapeSymbol;
    public string GetDecodedLetter() => decodedLetter;
    public int GetWordPosition() => wordPosition;
    public bool IsDiscovered() => isDiscovered;
    public bool IsPlayerNearby() => isPlayerNearby;

    // Method to manually discover this clue (for testing)
    [ContextMenu("Discover This Clue")]
    public void ForceDiscover()
    {
        OnInteract();
    }

    void OnDrawGizmos()
    {
        // Draw gizmo to show this is a shape clue
        Gizmos.color = isDiscovered ? discoveredColor : Color.yellow;
        Gizmos.DrawWireCube(transform.position, GetComponent<Collider>()?.bounds.size ?? Vector3.one);
        
        if (isPlayerNearby)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireCube(transform.position, (GetComponent<Collider>()?.bounds.size ?? Vector3.one) * 1.2f);
        }

        // Draw text label in scene view
        #if UNITY_EDITOR
        UnityEditor.Handles.Label(transform.position + Vector3.up * 0.5f, $"{shapeSymbol}→{decodedLetter}");
        #endif
    }
}