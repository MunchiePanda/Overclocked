using UnityEngine;
using System.Collections;

/// <summary>
/// Individual color button with light that can be clicked for sequence puzzles
/// </summary>
public class ColorButton : MonoBehaviour, IInteractable
{
    [Header("Button Configuration")]
    [Tooltip("The color this button represents")]
    public ColorType buttonColor = ColorType.Red;
    
    [Tooltip("Light component for this button")]
    public Light buttonLight;
    
    [Tooltip("Renderer for button material (optional visual feedback)")]
    public Renderer buttonRenderer;
    
    [Tooltip("Material index to change color on (if using renderer)")]
    public int materialIndex = 0;

    [Header("Light Settings")]
    [Tooltip("Default light intensity")]
    public float defaultIntensity = 1f;
    
    [Tooltip("Pulse intensity when pressed")]
    public float pulseIntensity = 3f;
    
    [Tooltip("Duration of pulse effect")]
    public float pulseDuration = 0.3f;
    
    [Tooltip("Range of the light")]
    public float lightRange = 5f;

    [Header("Interaction Settings")]
    [Tooltip("Hover prompt text")]
    public string hoverText = "Press Color Button";
    
    [Tooltip("Sound effect when button is pressed (optional)")]
    public AudioSource audioSource;

    private ColorLightPuzzle parentPuzzle;
    private Color defaultLightColor;
    private bool isPulsing = false;
    private bool isInteractionEnabled = true;

    public ColorType ButtonColor => buttonColor;

    private void Start()
    {
        SetupButton();
    }

    private void SetupButton()
    {
        // Auto-find light component if not assigned
        if (buttonLight == null)
        {
            buttonLight = GetComponentInChildren<Light>();
            if (buttonLight == null)
            {
                // Create light component
                GameObject lightObj = new GameObject("ButtonLight");
                lightObj.transform.SetParent(transform);
                lightObj.transform.localPosition = Vector3.up * 0.5f;
                
                buttonLight = lightObj.AddComponent<Light>();
                buttonLight.type = LightType.Point;
                buttonLight.range = lightRange;
                buttonLight.intensity = defaultIntensity;
            }
        }

        // Set default color based on button color type
        defaultLightColor = GetColorFromType(buttonColor);
        SetLightToDefaultColor();

        // Setup audio if available
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        // Make sure we have a collider for interaction
        if (GetComponent<Collider>() == null)
        {
            gameObject.AddComponent<BoxCollider>();
        }
    }

    public void Initialize(ColorLightPuzzle puzzle)
    {
        parentPuzzle = puzzle;
        SetupButton();
    }

    public void SetLightToDefaultColor()
    {
        if (buttonLight != null)
        {
            buttonLight.color = defaultLightColor;
            buttonLight.intensity = defaultIntensity;
        }

        // Update material color if using renderer
        if (buttonRenderer != null && buttonRenderer.materials.Length > materialIndex)
        {
            Material[] materials = buttonRenderer.materials;
            materials[materialIndex].color = defaultLightColor;
            materials[materialIndex].SetColor("_EmissionColor", defaultLightColor * 0.5f);
            buttonRenderer.materials = materials;
        }
    }

    public void FlashColor(Color flashColor, float duration)
    {
        StartCoroutine(FlashRoutine(flashColor, duration));
    }

    public void PulseLight()
    {
        if (!isPulsing)
        {
            StartCoroutine(PulseRoutine());
        }
    }

    private IEnumerator FlashRoutine(Color flashColor, float duration)
    {
        Color originalColor = buttonLight.color;
        float originalIntensity = buttonLight.intensity;

        // Flash to new color
        buttonLight.color = flashColor;
        buttonLight.intensity = pulseIntensity;

        yield return new WaitForSeconds(duration);

        // Return to default
        SetLightToDefaultColor();
    }

    private IEnumerator PulseRoutine()
    {
        isPulsing = true;
        
        float originalIntensity = buttonLight.intensity;
        float elapsed = 0f;

        // Pulse up
        while (elapsed < pulseDuration / 2)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / (pulseDuration / 2);
            buttonLight.intensity = Mathf.Lerp(originalIntensity, pulseIntensity, progress);
            yield return null;
        }

        // Pulse down
        elapsed = 0f;
        while (elapsed < pulseDuration / 2)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / (pulseDuration / 2);
            buttonLight.intensity = Mathf.Lerp(pulseIntensity, originalIntensity, progress);
            yield return null;
        }

        buttonLight.intensity = originalIntensity;
        isPulsing = false;
    }

    private Color GetColorFromType(ColorType colorType)
    {
        return colorType switch
        {
            ColorType.Red => Color.red,
            ColorType.Blue => Color.blue,
            ColorType.Green => Color.green,
            ColorType.Yellow => Color.yellow,
            ColorType.Purple => new Color(0.8f, 0f, 0.8f), // Purple/Magenta
            ColorType.Orange => new Color(1f, 0.5f, 0f),   // Orange
            ColorType.Cyan => Color.cyan,
            ColorType.White => Color.white,
            _ => Color.white
        };
    }

    // IInteractable implementation
    // IInteractable interface implementation  
    public void OnHover()
    {
        if (isInteractionEnabled)
        {
            // Optional: Add subtle glow or highlight
            if (buttonLight != null && !isPulsing)
            {
                buttonLight.intensity = defaultIntensity * 1.2f;
            }
        }
    }

    public void OnHoverExit()
    {
        if (isInteractionEnabled && !isPulsing)
        {
            // Remove highlight
            if (buttonLight != null)
            {
                buttonLight.intensity = defaultIntensity;
            }
        }
    }

    public void OnInteract()
    {
        if (isInteractionEnabled && parentPuzzle != null)
        {
            // Play sound effect
            if (audioSource != null && audioSource.clip != null)
            {
                audioSource.Play();
            }

            // Notify parent puzzle
            parentPuzzle.OnColorButtonPressed(buttonColor);
        }
    }

    public void SetInteractionEnabled(bool enabled)
    {
        isInteractionEnabled = enabled;
    }

    // Debug methods
    [ContextMenu("🔍 Test Button Press")]
    private void TestButtonPress()
    {
        OnInteract();
    }

    [ContextMenu("💡 Test Light Pulse")]
    private void TestLightPulse()
    {
        PulseLight();
    }

    [ContextMenu("⚡ Test Flash Red")]
    private void TestFlashRed()
    {
        FlashColor(Color.red, 1f);
    }

    [ContextMenu("⚡ Test Flash Green")]
    private void TestFlashGreen()
    {
        FlashColor(Color.green, 1f);
    }
}

/// <summary>
/// Enum for different color types available
/// </summary>
public enum ColorType
{
    Red,
    Blue, 
    Green,
    Yellow,
    Purple,
    Orange,
    Cyan,
    White
}