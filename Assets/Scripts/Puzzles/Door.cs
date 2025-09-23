using UnityEngine;
using TMPro;
using System.Collections;

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

    [Header("End Screen Integration")]
    [Tooltip("Win screen to show when door is unlocked")]
    public WinScreen winScreen;

    [Tooltip("Show end screen immediately when door unlocks")]
    public bool showEndScreenOnUnlock = true;

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

        // Auto-find WinScreen if not assigned
        if (winScreen == null)
        {
            winScreen = FindFirstObjectByType<WinScreen>();
            if (winScreen != null)
            {
                Debug.Log($"✅ Auto-assigned WinScreen to door '{gameObject.name}'");
            }
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
            Debug.Log("🎉 Door unlocked!");

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

            // Show end screen if enabled
            if (showEndScreenOnUnlock && winScreen != null)
            {
                Debug.Log("🏆 Showing end screen - door unlocked!");
                StartCoroutine(ShowEndScreenWithDelay());
            }
            else if (showEndScreenOnUnlock)
            {
                Debug.LogWarning("⚠️ WinScreen not assigned! Cannot show end screen.");
            }

            // Legacy support - still notify escape code manager if present
            EscapeCodeManager escapeManager = FindFirstObjectByType<EscapeCodeManager>();
            if (escapeManager != null && escapeManager.IsEscapeCodeReady())
            {
                StartCoroutine(TriggerEscapeSequence(escapeManager));
            }
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
    
    /// <summary>
    /// Show the end screen with dramatic timing
    /// </summary>
    private System.Collections.IEnumerator ShowEndScreenWithDelay()
    {
        Debug.Log("🎬 Starting end screen sequence...");
        
        // Optional: Add some dramatic visual effects here
        yield return new WaitForSeconds(1.5f);
        
        // Show the win screen
        if (winScreen != null)
        {
            winScreen.ShowWinScreen();
            Debug.Log("🎆 End screen displayed!");
        }
    }
    
    /// <summary>
    /// Trigger the escape sequence with dramatic timing
    /// </summary>
    /// <param name="escapeManager">The escape code manager</param>
    private System.Collections.IEnumerator TriggerEscapeSequence(EscapeCodeManager escapeManager)
    {
        Debug.Log("🎉 Starting escape sequence...");
        
        // Optional: Add some dramatic visual effects here
        yield return new WaitForSeconds(1.5f);
        
        // Trigger the win screen
        escapeManager.OnEscapeSuccessful();
        
        Debug.Log("🏆 Escape successful!");
    }

    /// <summary>
    /// Public property to check if door is unlocked (for other scripts)
    /// </summary>
    public bool IsUnlocked => isUnlocked;
}
