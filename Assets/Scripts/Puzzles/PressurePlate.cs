using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Handles individual pressure plate functionality
/// </summary>
public class PressurePlate : MonoBehaviour
{
    [Header("Pressure Plate Settings")]
    [Tooltip("Event triggered when the pressure plate is activated")]
    public UnityEvent onActivate;

    [Tooltip("Event triggered when the pressure plate is deactivated")]
    public UnityEvent onDeactivate;

    [Tooltip("Is the plate currently active?")]
    public bool isActive = false;

    [Header("Visual Feedback")]
    [Tooltip("The material to use when activated")]
    public Material activeMaterial;

    [Tooltip("The material to use when deactivated")]
    public Material inactiveMaterial;

    [Tooltip("The color to use when activated")]
    public Color activeColor = Color.green;

    [Tooltip("The color to use when deactivated")]
    public Color inactiveColor = Color.red;

    private MeshRenderer meshRenderer;
    private Material originalMaterial;
    private Color originalColor;

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();

        // Store original material and color
        if (meshRenderer != null)
        {
            originalMaterial = meshRenderer.material;
            originalColor = meshRenderer.material.color;
        }

        // Set initial state
        UpdateVisualState();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Or check for a specific tag
        {
            Activate();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) // Or check for a specific tag
        {
            Deactivate();
        }
    }

    /// <summary>
    /// Activate the pressure plate
    /// </summary>
    public void Activate()
    {
        if (!isActive)
        {
            isActive = true;
            onActivate.Invoke();
            UpdateVisualState();
            Debug.Log("Pressure plate activated: " + gameObject.name);
        }
    }

    /// <summary>
    /// Deactivate the pressure plate
    /// </summary>
    public void Deactivate()
    {
        if (isActive)
        {
            isActive = false;
            onDeactivate.Invoke();
            UpdateVisualState();
            Debug.Log("Pressure plate deactivated: " + gameObject.name);
        }
    }

    /// <summary>
    /// Update the visual state of the pressure plate
    /// </summary>
    private void UpdateVisualState()
    {
        if (meshRenderer == null) return;

        if (isActive)
        {
            // Use active material if available, otherwise change color
            if (activeMaterial != null)
            {
                meshRenderer.material = activeMaterial;
            }
            else
            {
                meshRenderer.material.color = activeColor;
            }
        }
        else
        {
            // Use inactive material if available, otherwise change color
            if (inactiveMaterial != null)
            {
                meshRenderer.material = inactiveMaterial;
            }
            else
            {
                meshRenderer.material.color = inactiveColor;
            }
        }
    }

    /// <summary>
    /// Reset the pressure plate to its initial state
    /// </summary>
    public void ResetPlate()
    {
        isActive = false;
        UpdateVisualState();
        Debug.Log("Pressure plate reset: " + gameObject.name);
    }
}
