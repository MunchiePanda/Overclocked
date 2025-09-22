using UnityEngine;
using TMPro;

/// <summary>
/// Terminal for riddle puzzles - allows players to interact and trigger riddle UI
/// </summary>
public class RiddleTerminal : MonoBehaviour, IInteractable
{
    [Header("Terminal Setup")]
    [Tooltip("Name of this riddle terminal")]
    public string terminalName = "Riddle Terminal";
    
    [Tooltip("UI text to show interaction prompt")]
    public TMP_Text interactionPrompt;
    
    [Tooltip("Text to display when hovering")]
    public string hoverText = "Press E to access riddle challenge";

    [Header("Riddle Puzzle Reference")]
    [Tooltip("The riddle puzzle this terminal controls")]
    public RiddlePuzzle linkedRiddlePuzzle;

    [Header("Visual Feedback")]
    [Tooltip("Material to use when hovering")]
    public Material hoverMaterial;
    
    [Tooltip("Light to activate when hovering")]
    public Light terminalLight;
    
    [Tooltip("Color when riddle is solved")]
    public Color solvedColor = Color.green;
    
    [Tooltip("Color when riddle is active")]
    public Color activeColor = Color.cyan;
    
    [Tooltip("Color when riddle is unsolved")]
    public Color unsolvedColor = Color.blue;

    [Header("Terminal Display")]
    [Tooltip("Text shown on terminal screen")]
    public TMP_Text terminalScreen;
    
    [Tooltip("Default message shown on terminal")]
    [TextArea(3, 5)]
    public string defaultMessage = "RIDDLE CHALLENGE TERMINAL\n\nPress E to begin mental exercise\n\nStatus: READY";

    [Header("Audio")]
    [Tooltip("Sound when terminal is accessed")]
    public AudioClip accessSound;
    
    [Tooltip("Sound when hovering")]
    public AudioClip hoverSound;

    private Material originalMaterial;
    private MeshRenderer meshRenderer;
    private bool isPlayerNearby = false;
    private bool isTerminalActive = false;
    private PlayerController currentPlayer;
    private AudioSource audioSource;

    void Start()
    {
        InitializeComponents();
        SetupLayer();
        SetupUI();
        
        Debug.Log($"Riddle Terminal '{terminalName}' initialized");
    }

    void InitializeComponents()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null)
            originalMaterial = meshRenderer.material;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        // Find linked riddle puzzle if not assigned
        if (linkedRiddlePuzzle == null)
        {
            linkedRiddlePuzzle = FindFirstObjectByType<RiddlePuzzle>();
            if (linkedRiddlePuzzle != null)
                Debug.Log($"Auto-assigned RiddlePuzzle to {terminalName}");
        }
    }

    void SetupLayer()
    {
        // Make sure this object is on the Interactable layer
        int interactableLayer = LayerMask.NameToLayer("Interactable");
        if (interactableLayer != -1)
        {
            gameObject.layer = interactableLayer;
            Debug.Log($"Set {terminalName} to Interactable layer");
        }
        else
        {
            Debug.LogWarning("Interactable layer not found! Player may not be able to interact with this terminal.");
        }
    }

    void SetupUI()
    {
        // Hide interaction prompt initially
        if (interactionPrompt != null)
            interactionPrompt.gameObject.SetActive(false);

        // Set terminal screen message
        if (terminalScreen != null)
            terminalScreen.text = defaultMessage;

        UpdateVisualState();
    }

    void Update()
    {
        UpdateLightEffects();
        UpdateTerminalScreen();
        CheckRiddleState();
    }

    void UpdateLightEffects()
    {
        if (terminalLight != null)
        {
            bool riddleIsSolved = linkedRiddlePuzzle != null && linkedRiddlePuzzle.IsCompleted();
            bool riddleIsActive = linkedRiddlePuzzle != null && linkedRiddlePuzzle.puzzleUI != null && linkedRiddlePuzzle.puzzleUI.activeInHierarchy;
            
            if (riddleIsSolved)
            {
                terminalLight.color = solvedColor;
                terminalLight.intensity = 1f;
            }
            else if (riddleIsActive)
            {
                terminalLight.color = activeColor;
                terminalLight.intensity = Mathf.PingPong(Time.time * 3f, 1f) + 0.5f;
            }
            else if (isPlayerNearby)
            {
                terminalLight.color = unsolvedColor;
                terminalLight.intensity = Mathf.PingPong(Time.time * 2f, 1f) + 0.5f;
            }
            else
            {
                terminalLight.intensity = 0.2f;
            }
        }
    }

    void UpdateTerminalScreen()
    {
        if (terminalScreen == null || linkedRiddlePuzzle == null) return;

        if (linkedRiddlePuzzle.IsCompleted())
        {
            terminalScreen.text = "RIDDLE CHALLENGE TERMINAL\n\n✓ CHALLENGE COMPLETED\n\nWell done! Riddle solved.";
        }
        else if (linkedRiddlePuzzle.puzzleUI != null && linkedRiddlePuzzle.puzzleUI.activeInHierarchy)
        {
            terminalScreen.text = "RIDDLE CHALLENGE TERMINAL\n\n◉ CHALLENGE ACTIVE\n\nSolve the riddle to proceed";
        }
        else
        {
            terminalScreen.text = defaultMessage;
        }
    }

    void CheckRiddleState()
    {
        if (linkedRiddlePuzzle != null && linkedRiddlePuzzle.IsCompleted())
        {
            UpdateVisualState();
        }
    }

    void UpdateVisualState()
    {
        if (linkedRiddlePuzzle == null) return;

        bool riddleIsSolved = linkedRiddlePuzzle.IsCompleted();
        
        if (riddleIsSolved)
        {
            // Update hover text for solved riddle
            hoverText = "Riddle Challenge Completed!";
            
            if (terminalLight != null)
            {
                terminalLight.color = solvedColor;
                terminalLight.enabled = true;
            }
        }
        else
        {
            hoverText = "Press E to access riddle challenge";
        }
    }

    public void OnHover()
    {
        isPlayerNearby = true;

        // Show interaction prompt
        if (interactionPrompt != null)
        {
            interactionPrompt.text = hoverText;
            interactionPrompt.gameObject.SetActive(true);
        }

        // Change material if available
        if (meshRenderer != null && hoverMaterial != null)
        {
            meshRenderer.material = hoverMaterial;
        }

        // Activate light
        if (terminalLight != null)
        {
            terminalLight.enabled = true;
        }

        // Play hover sound
        if (hoverSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(hoverSound);
        }

        Debug.Log($"Riddle Terminal '{terminalName}': Player can interact");
    }

    public void OnHoverExit()
    {
        isPlayerNearby = false;

        // Hide interaction prompt
        if (interactionPrompt != null)
        {
            interactionPrompt.gameObject.SetActive(false);
        }

        // Restore original material
        if (meshRenderer != null && originalMaterial != null)
        {
            meshRenderer.material = originalMaterial;
        }

        // Keep light on if riddle is solved or active
        if (terminalLight != null)
        {
            bool riddleIsSolved = linkedRiddlePuzzle != null && linkedRiddlePuzzle.IsCompleted();
            bool riddleIsActive = linkedRiddlePuzzle != null && linkedRiddlePuzzle.puzzleUI != null && linkedRiddlePuzzle.puzzleUI.activeInHierarchy;
            bool keepLightOn = riddleIsSolved || riddleIsActive;
            terminalLight.enabled = keepLightOn;
        }

        Debug.Log($"Riddle Terminal '{terminalName}': Player left interaction range");
    }

    public void OnInteract()
    {
        if (linkedRiddlePuzzle == null)
        {
            Debug.LogError($"Riddle Terminal '{terminalName}': No linked riddle puzzle found!");
            return;
        }

        // Play access sound
        if (accessSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(accessSound);
        }

        // Check if riddle is already solved
        if (linkedRiddlePuzzle.IsCompleted())
        {
            Debug.Log($"Riddle Terminal '{terminalName}': Riddle already solved");
            return;
        }

        // Activate the riddle puzzle
        if (linkedRiddlePuzzle.puzzleUI == null || !linkedRiddlePuzzle.puzzleUI.activeInHierarchy)
        {
            linkedRiddlePuzzle.ShowPuzzle();
            isTerminalActive = true;
            Debug.Log($"Riddle Terminal '{terminalName}': Riddle puzzle activated");
        }
        else
        {
            // Riddle is already active, just show the UI again
            linkedRiddlePuzzle.ShowPuzzle();
            Debug.Log($"Riddle Terminal '{terminalName}': Riddle puzzle UI shown");
        }

        // Notify terminal system manager if available
        TerminalSystemManager systemManager = FindFirstObjectByType<TerminalSystemManager>();
        if (systemManager != null)
        {
            systemManager.OnTerminalAccessed(terminalName, "Riddle");
            systemManager.PlayTerminalSound();
        }
    }

    // Public methods for external access
    public bool IsPlayerNearby() => isPlayerNearby;
    public bool IsTerminalActive() => isTerminalActive;
    public RiddlePuzzle GetLinkedRiddlePuzzle() => linkedRiddlePuzzle;
    public bool IsRiddleSolved() => linkedRiddlePuzzle != null && linkedRiddlePuzzle.IsCompleted();
    public bool IsRiddleActive() => linkedRiddlePuzzle != null && linkedRiddlePuzzle.puzzleUI != null && linkedRiddlePuzzle.puzzleUI.activeInHierarchy;

    // Method to manually link a riddle puzzle
    public void SetLinkedRiddlePuzzle(RiddlePuzzle riddlePuzzle)
    {
        linkedRiddlePuzzle = riddlePuzzle;
        Debug.Log($"Riddle Terminal '{terminalName}': Linked to riddle puzzle '{riddlePuzzle.puzzleName}'");
    }

    void OnDrawGizmos()
    {
        // Draw terminal bounds
        Collider terminalCollider = GetComponent<Collider>();
        if (terminalCollider != null)
        {
            if (IsRiddleSolved())
                Gizmos.color = Color.green;
            else if (IsRiddleActive())
                Gizmos.color = Color.cyan;
            else
                Gizmos.color = Color.blue;
                
            Gizmos.DrawWireCube(transform.position, terminalCollider.bounds.size);
            
            if (isPlayerNearby)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireCube(transform.position, terminalCollider.bounds.size * 1.1f);
            }
        }

        // Draw connection to linked puzzle
        if (linkedRiddlePuzzle != null)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawLine(transform.position, linkedRiddlePuzzle.transform.position);
        }
    }
}