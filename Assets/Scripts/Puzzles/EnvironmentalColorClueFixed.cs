using UnityEngine;

public class EnvironmentalColorClueFixed : MonoBehaviour, IInteractable
{
    [Header("Clue Configuration")]
    public ColorType clueColor = ColorType.Red;
    public int sequencePosition = 0;
    public string clueDescription = "A red painting with passionate brushstrokes";
    public string examinationText = "This vibrant red canvas seems to burn with intensity...";

    [Header("Visual Setup")]
    public Renderer colorRenderer;
    public int materialIndex = 0;
    public Light colorLight;
    public float emissionIntensity = 1f;

    [Header("Discovery Settings")]
    public bool isDiscovered = false;
    public Color discoveredHighlight = Color.white;
    public float highlightIntensity = 2f;

    private Material originalMaterial;
    private Color clueColorValue;
    private bool isExamined = false;

    void Start()
    {
        SetupClue();
    }

    void SetupClue()
    {
        clueColorValue = GetColorFromType(clueColor);
        
        if (colorRenderer != null)
        {
            if (colorRenderer.materials.Length > materialIndex)
            {
                originalMaterial = new Material(colorRenderer.materials[materialIndex]);
                
                Material[] materials = colorRenderer.materials;
                materials[materialIndex].color = clueColorValue;
                materials[materialIndex].EnableKeyword("_EMISSION");
                materials[materialIndex].SetColor("_EmissionColor", clueColorValue * emissionIntensity);
                
                colorRenderer.materials = materials;
            }
        }

        if (colorLight != null)
        {
            colorLight.color = clueColorValue;
        }

        if (GetComponent<Collider>() == null)
        {
            gameObject.AddComponent<BoxCollider>();
        }
    }

    public void OnHover()
    {
        if (colorRenderer != null && colorRenderer.materials.Length > materialIndex)
        {
            Material[] materials = colorRenderer.materials;
            materials[materialIndex].SetColor("_EmissionColor", clueColorValue * (emissionIntensity * 1.5f));
            colorRenderer.materials = materials;
        }
    }

    public void OnHoverExit()
    {
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
            ShowClueDetails();
        }
    }

    void DiscoverClue()
    {
        isDiscovered = true;
        isExamined = true;

        if (colorRenderer != null && colorRenderer.materials.Length > materialIndex)
        {
            Material[] materials = colorRenderer.materials;
            materials[materialIndex].SetColor("_EmissionColor", clueColorValue * emissionIntensity * highlightIntensity);
            colorRenderer.materials = materials;
        }

        ColorLightPuzzle puzzle = FindObjectOfType<ColorLightPuzzle>();
        if (puzzle != null)
        {
            puzzle.ProvideFeedback($"Discovered: {clueDescription}");
        }

        ShowClueDetails();
    }

    void ShowClueDetails()
    {
        string feedback = $"{examinationText}\n";
        feedback += $"Color: {clueColor}\n";
        feedback += $"Position hint: This appears to be element #{sequencePosition + 1} in a sequence.";

        ColorLightPuzzle puzzle = FindObjectOfType<ColorLightPuzzle>();
        if (puzzle != null)
        {
            puzzle.ProvideFeedback(feedback);
        }

        Debug.Log($"Clue examined: {clueColor} at position {sequencePosition}");
    }

    Color GetColorFromType(ColorType colorType)
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
}