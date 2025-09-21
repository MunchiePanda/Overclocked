using UnityEngine;
using System.Collections;

public class ColorButton : MonoBehaviour, IInteractable
{
    [Header("Button Configuration")]
    [Tooltip("The color this button represents")]
    public ColorType buttonColor = ColorType.Red;
    
    [Tooltip("The parent puzzle that manages this button")]
    public ColorLightPuzzle parentPuzzle;

    // Property for backward compatibility
    public ColorType ButtonColor => buttonColor;

    [Header("Visual Components")]
    [Tooltip("Light component that shows the button color")]
    public Light buttonLight;
    
    [Tooltip("Renderer for the button material")]
    public Renderer buttonRenderer;
    
    [Tooltip("Material index to change")]
    public int materialIndex = 0;

    [Header("Interaction Settings")]
    [Tooltip("Can the button be pressed?")]
    public bool isInteractionEnabled = true;
    
    [Tooltip("Text shown when hovering")]
    public string hoverText = "Press Color Button";

    [Header("Audio")]
    [Tooltip("Audio source for button press sound")]
    public AudioSource audioSource;

    [Header("Visual Feedback")]
    [Tooltip("How long to pulse when pressed")]
    public float pulseLength = 0.5f;
    
    [Tooltip("Pulse intensity multiplier")]
    public float pulseIntensity = 3f;

    private Color buttonColorValue;
    private float defaultIntensity = 1f;
    private bool isPulsing = false;
    private Material originalMaterial;

    void Start()
    {
        InitializeButton();
    }

    void InitializeButton()
    {
        buttonColorValue = GetColorFromType(buttonColor);

        if (buttonLight != null)
        {
            buttonLight.color = buttonColorValue;
            defaultIntensity = buttonLight.intensity;
        }

        if (buttonRenderer != null && buttonRenderer.materials.Length > materialIndex)
        {
            originalMaterial = new Material(buttonRenderer.materials[materialIndex]);
            
            Material[] materials = buttonRenderer.materials;
            materials[materialIndex].color = buttonColorValue;
            materials[materialIndex].EnableKeyword("_EMISSION");
            materials[materialIndex].SetColor("_EmissionColor", buttonColorValue * 0.5f);
            
            buttonRenderer.materials = materials;
        }

        if (GetComponent<Collider>() == null)
        {
            gameObject.AddComponent<BoxCollider>();
        }

        if (parentPuzzle == null)
        {
            parentPuzzle = FindFirstObjectByType<ColorLightPuzzle>();
        }
    }

    public void Initialize(ColorType color, ColorLightPuzzle puzzle)
    {
        buttonColor = color;
        parentPuzzle = puzzle;
        InitializeButton();
    }

    public void SetLightToDefaultColor()
    {
        if (buttonLight != null)
        {
            buttonLight.color = buttonColorValue;
            buttonLight.intensity = defaultIntensity;
        }
    }

    public void PulseLight()
    {
        PulseButtonColor();
    }

    public void FlashColor(Color color)
    {
        StartCoroutine(FlashCoroutine(color, 0.2f, 3));
    }

    public void OnHover()
    {
        if (isInteractionEnabled)
        {
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
            if (audioSource != null && audioSource.clip != null)
            {
                audioSource.Play();
            }

            parentPuzzle.OnColorButtonPressed(buttonColor);
        }
    }

    public void PulseCorrect()
    {
        StartCoroutine(PulseCoroutine(Color.green));
    }

    public void PulseIncorrect()
    {
        StartCoroutine(PulseCoroutine(Color.red));
    }

    public void PulseButtonColor()
    {
        StartCoroutine(PulseCoroutine(buttonColorValue));
    }

    private IEnumerator PulseCoroutine(Color pulseColor)
    {
        isPulsing = true;
        float originalLightIntensity = buttonLight != null ? buttonLight.intensity : defaultIntensity;
        Color originalLightColor = buttonLight != null ? buttonLight.color : buttonColorValue;

        float timer = 0f;
        while (timer < pulseLength)
        {
            timer += Time.deltaTime;
            float progress = timer / pulseLength;
            float intensity = Mathf.Lerp(originalLightIntensity, originalLightIntensity * pulseIntensity, Mathf.Sin(progress * Mathf.PI * 4));

            if (buttonLight != null)
            {
                buttonLight.intensity = intensity;
                buttonLight.color = Color.Lerp(originalLightColor, pulseColor, Mathf.Sin(progress * Mathf.PI * 2) * 0.5f);
            }

            yield return null;
        }

        if (buttonLight != null)
        {
            buttonLight.intensity = originalLightIntensity;
            buttonLight.color = originalLightColor;
        }

        isPulsing = false;
    }

    public void FlashSuccess()
    {
        StartCoroutine(FlashCoroutine(Color.green, 0.3f, 3));
    }

    public void FlashFailure()
    {
        StartCoroutine(FlashCoroutine(Color.red, 0.2f, 5));
    }

    private IEnumerator FlashCoroutine(Color flashColor, float flashInterval, int flashCount)
    {
        isPulsing = true;
        Color originalColor = buttonLight != null ? buttonLight.color : buttonColorValue;

        for (int i = 0; i < flashCount; i++)
        {
            if (buttonLight != null)
                buttonLight.color = flashColor;
            
            yield return new WaitForSeconds(flashInterval);
            
            if (buttonLight != null)
                buttonLight.color = originalColor;
            
            yield return new WaitForSeconds(flashInterval);
        }

        isPulsing = false;
    }

    public void SetInteractionEnabled(bool enabled)
    {
        isInteractionEnabled = enabled;
        
        if (buttonLight != null)
        {
            buttonLight.intensity = enabled ? defaultIntensity : defaultIntensity * 0.3f;
        }
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

    [ContextMenu("🎨 Test Button Press")]
    private void DebugTestPress()
    {
        OnInteract();
    }

    [ContextMenu("✅ Test Success Pulse")]
    private void DebugTestSuccess()
    {
        PulseCorrect();
    }

    [ContextMenu("❌ Test Failure Pulse")]
    private void DebugTestFailure()
    {
        PulseIncorrect();
    }
}