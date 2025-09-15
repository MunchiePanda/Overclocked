using UnityEngine;
using TMPro;

/// <summary>
/// Handles door functionality, including locking and unlocking
/// </summary>
public class Door : MonoBehaviour
{
    [Header("Door Settings")]
    [Tooltip("The correct code to unlock the door")]
    public string correctCode = "1234";

    [Tooltip("The animator for the door opening/closing animation")]
    public Animator doorAnimator;

    [Tooltip("The text to display for wrong code entry")]
    public TMP_Text feedbackText;

    [Tooltip("The message to show for wrong code")]
    public string wrongCodeMessage = "Incorrect Code!";

    [Tooltip("The message to show for correct code")]
    public string correctCodeMessage = "Door Unlocked!";

    [Header("Visual Feedback")]
    [Tooltip("The material to use when the door is unlocked")]
    public Material unlockedMaterial;

    [Tooltip("The material to use when the door is locked")]
    public Material lockedMaterial;

    [Tooltip("The color to use when the door is unlocked")]
    public Color unlockedColor = Color.green;

    [Tooltip("The color to use when the door is locked")]
    public Color lockedColor = Color.red;

    private bool isUnlocked = false;
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

    /// <summary>
    /// Attempt to unlock the door with the provided code
    /// </summary>
    /// <param name="code">The code to try</param>
    public void UnlockDoor(string code)
    {
        if (isUnlocked)
        {
            Debug.Log("Door is already unlocked.");
            return;
        }

        if (code == correctCode)
        {
            // Correct code - unlock the door
            isUnlocked = true;
            Debug.Log("Door unlocked!");

            // Show feedback
            if (feedbackText != null)
            {
                feedbackText.text = correctCodeMessage;
            }

            // Play door opening animation
            if (doorAnimator != null)
            {
                doorAnimator.SetBool("isOpen", true);
            }

            // Update visual state
            UpdateVisualState();
        }
        else
        {
            // Wrong code
            Debug.Log("Incorrect code entered.");

            // Show feedback
            if (feedbackText != null)
            {
                feedbackText.text = wrongCodeMessage;
            }
        }
    }

    /// <summary>
    /// Lock the door
    /// </summary>
    public void LockDoor()
    {
        if (!isUnlocked) return;

        isUnlocked = false;
        Debug.Log("Door locked.");

        // Play door closing animation
        if (doorAnimator != null)
        {
            doorAnimator.SetBool("isOpen", false);
        }

        // Update visual state
        UpdateVisualState();
    }

    /// <summary>
    /// Update the visual state of the door
    /// </summary>
    private void UpdateVisualState()
    {
        if (meshRenderer == null) return;

        if (isUnlocked)
        {
            // Use unlocked material if available, otherwise change color
            if (unlockedMaterial != null)
            {
                meshRenderer.material = unlockedMaterial;
            }
            else
            {
                meshRenderer.material.color = unlockedColor;
            }
        }
        else
        {
            // Use locked material if available, otherwise change color
            if (lockedMaterial != null)
            {
                meshRenderer.material = lockedMaterial;
            }
            else
            {
                meshRenderer.material.color = lockedColor;
            }
        }
    }

    /// <summary>
    /// Reset the door to its initial state
    /// </summary>
    public void ResetDoor()
    {
        isUnlocked = false;

        // Play door closing animation
        if (doorAnimator != null)
        {
            doorAnimator.SetBool("isOpen", false);
        }

        // Update visual state
        UpdateVisualState();

        // Clear feedback text
        if (feedbackText != null)
        {
            feedbackText.text = "";
        }

        Debug.Log("Door reset to locked state.");
    }
}
