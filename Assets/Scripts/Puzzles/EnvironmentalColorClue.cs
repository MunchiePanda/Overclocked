using UnityEngine;

/// <summary>
/// Environmental color clue that players can discover around the room
/// </summary>
public class EnvironmentalColorClue : MonoBehaviour, IInteractable
{
    [Header("Clue Configuration")]
    [Tooltip("The color this clue represents")]
    public ColorType clueColor = ColorType.Red;
    
    [Tooltip("Position in the sequence (0-based)")]
    public int sequencePosition = 0;
    
    [Tooltip("Description of this clue")]
    public string clueDescription = "A red painting with passionate brushstrokes";
    
    [Tooltip("Text shown when player examines this clue")]
    public string examinationText = "This vibrant red canvas seems to burn with intensity...";

    [Header("Visual Setup")]
    [Tooltip("Renderer to apply color material to")]
    public Renderer colorRenderer;
    
    [Tooltip("Material index to change (if multiple materials)")]
    public int materialIndex = 0;
    
    [Tooltip("Light component to match color (optional)")]
    public Light colorLight;
    
    [Tooltip("Emission intensity for glowing effect")]
    public float emissionIntensity = 1f;

    [Header("Discovery Settings")]
    [Tooltip("Has this clue been discovered by the player?")]
    public bool isDiscovered = false;
    
    [Tooltip("Highlight color when discovered")]
    public Color discoveredHighlight = Color.white;
    
    [Tooltip("Intensity of discovery highlight")]
    public float highlightIntensity = 2f;

    private Material originalMaterial;
    private Color clueColorValue;
    private bool isExamined = false;

    private void Start()
    {
        SetupClue();
    }

    private void SetupClue()
    {
        // Get color value from enum
        clueColorValue = GetColorFromType(clueColor);
        
        // Setup visual appearance
        if (colorRenderer != null)
        {
            // Store original material for backup
            if (colorRenderer.materials.Length > materialIndex)
            {
                originalMaterial = new Material(colorRenderer.materials[materialIndex]);
                
                // Apply clue color
                Material[] materials = colorRenderer.materials;
                materials[materialIndex].color = clueColorValue;
                
                // Add emission for glow effect
                materials[materialIndex].EnableKeyword("_EMISSION");
                materials[materialIndex].SetColor("_EmissionColor", clueColorValue * emissionIntensity);
                
                colorRenderer.materials = materials;
            }
        }

        // Setup light color
        if (colorLight != null)
        {
            colorLight.color = clueColorValue;
        }

        // Make sure we have a collider for interaction
        if (GetComponent<Collider>() == null)
        {
            gameObject.AddComponent<BoxCollider>();
        }
    }

    // IInteractable interface implementation
    public void OnHover()
    {
        // Add subtle highlight when hovering
        if (colorRenderer != null && colorRenderer.materials.Length > materialIndex)
        {
            Material[] materials = colorRenderer.materials;
            materials[materialIndex].SetColor("_EmissionColor", clueColorValue * (emissionIntensity * 1.5f));
            colorRenderer.materials = materials;
        }
    }

    public void OnHoverExit()
    {
        // Remove hover highlight
        if (colorRenderer != null && colorRenderer.materials.Length > materialIndex)
        {
            Material[] materials = colorRenderer.materials;
            float intensity = isDiscovered ? emissionIntensity * highlightIntensity : emissionIntensity;
            materials[materialIndex].SetColor("_EmissionColor", clueColorValue * intensity);
            colorRenderer.materials = materials;
        }
    }

    public void OnInteract()
    {
        if (!isExamined)
        {
            DiscoverClue();
        }
        else
        {
            // Re-examine the clue
            ShowClueDetails();
        }
    }

    private void DiscoverClue()
    {
        isDiscovered = true;
        isExamined = true;

        // Visual feedback for discovery
        if (colorRenderer != null && colorRenderer.materials.Length > materialIndex)
        {
            Material[] materials = colorRenderer.materials;
            materials[materialIndex].SetColor("_EmissionColor", clueColorValue * emissionIntensity * highlightIntensity);
            colorRenderer.materials = materials;
        }

        // Notify the color light puzzle about discovery
        ColorLightPuzzle puzzle = FindObjectOfType<ColorLightPuzzle>();
        if (puzzle != null)
        {
            puzzle.ProvideFeedback($"Discovered: {clueDescription}");
        }

        ShowClueDetails();
    }

    private void ShowClueDetails()
    {
        // Create detailed feedback
        string feedback = $"{examinationText}\n";
        feedback += $"Color: {clueColor}\n";
        feedback += $"Position hint: This appears to be element #{sequencePosition + 1} in a sequence.";

        // Find puzzle and give feedback
        ColorLightPuzzle puzzle = FindObjectOfType<ColorLightPuzzle>();
        if (puzzle != null)
        {
            puzzle.ProvideFeedback(feedback);
        }

        Debug.Log($"Clue examined: {clueColor} at position {sequencePosition}");
    }

    private Color GetColorFromType(ColorType colorType)
    {
        return colorType switch
        {
            ColorType.Red => Color.red,
            ColorType.Blue => Color.blue,
            ColorType.Green => Color.green,
            ColorType.Yellow => Color.yellow,
            ColorType.Purple => new Color(0.8f, 0f, 0.8f),
            ColorType.Orange => new Color(1f, 0.5f, 0f),
            ColorType.Cyan => Color.cyan,
            ColorType.White => Color.white,
            _ => Color.white
        };
    }

    // Method for puzzle to check if this clue has been discovered
    public bool IsDiscovered()
    {
        return isDiscovered;
    }

    public ColorType GetClueColor()
    {
        return clueColor;
    }

    public int GetSequencePosition()
    {
        return sequencePosition;
    }

    public string GetClueDescription()
    {
        return clueDescription;
    }

    // Debug methods
    [ContextMenu("🔍 Discover This Clue")]
    private void DebugDiscoverClue()
    {
        DiscoverClue();
    }

    [ContextMenu("🎨 Preview Color")]
    private void DebugPreviewColor()
    {
        SetupClue();
        Debug.Log($"Clue color: {clueColor} = {GetColorFromType(clueColor)}");
    }

    [ContextMenu("💡 Test Highlight")]
    private void DebugTestHighlight()
    {
        OnHover();
    }
}