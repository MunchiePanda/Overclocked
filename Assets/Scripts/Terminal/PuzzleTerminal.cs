using UnityEngine;
using TMPro;

/// <summary>
/// Smaller terminals for individual puzzles - simplified version of the main AI terminal
/// </summary>
public class PuzzleTerminal : MonoBehaviour, IInteractable
{
    [Header("Puzzle Terminal Setup")]
    [Tooltip("Name of this terminal")]
    public string terminalName = "Puzzle Terminal";
    
    [Tooltip("Type of puzzle this terminal controls")]
    public string puzzleType = "Generic";
    
    [Tooltip("UI text to show interaction prompt")]
    public TMP_Text interactionPrompt;
    
    [Tooltip("Text to display when hovering")]
    public string hoverText = "Press E to access puzzle";

    [Header("Puzzle Reference")]
    [Tooltip("The puzzle this terminal controls")]
    public MonoBehaviour linkedPuzzle;

    [Header("Terminal UI")]
    [Tooltip("Canvas with terminal interface")]
    public Canvas terminalCanvas;
    
    [Tooltip("Panel containing terminal UI")]
    public GameObject terminalPanel;
    
    [Tooltip("Text showing terminal title")]
    public TMP_Text terminalTitle;
    
    [Tooltip("Text showing puzzle instructions")]
    public TMP_Text instructionsText;

    [Header("Visual Feedback")]
    [Tooltip("Material to use when hovering")]
    public Material hoverMaterial;
    
    [Tooltip("Light to activate when hovering")]
    public Light terminalLight;
    
    [Tooltip("Color when puzzle is solved")]
    public Color solvedColor = Color.green;
    
    [Tooltip("Color when puzzle is unsolved")]
    public Color unsolvedColor = Color.blue;

    private Material originalMaterial;
    private MeshRenderer meshRenderer;
    private bool isPlayerNearby = false;
    private bool isTerminalOpen = false;
    private PlayerController currentPlayer;

    void Start()
    {
        InitializeComponents();
        SetupLayer();
        SetupUI();
        
        Debug.Log($"Puzzle Terminal '{terminalName}' initialized");
    }

    void InitializeComponents()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null)
            originalMaterial = meshRenderer.material;

        // Find linked puzzle if not assigned
        if (linkedPuzzle == null)
        {
            linkedPuzzle = GetComponentInChildren<MonoBehaviour>();
            if (linkedPuzzle == null)
                linkedPuzzle = FindFirstObjectByType<MonoBehaviour>();
        }
    }

    void SetupLayer()
    {
        // Make sure this object is on the Interactable layer
        int interactableLayer = LayerMask.NameToLayer("Interactable");
        if (interactableLayer != -1)
        {
            gameObject.layer = interactableLayer;
        }
    }

    void SetupUI()
    {
        // Hide interaction prompt initially
        if (interactionPrompt != null)
            interactionPrompt.gameObject.SetActive(false);

        // Hide terminal UI initially
        if (terminalCanvas != null)
            terminalCanvas.gameObject.SetActive(false);
        else if (terminalPanel != null)
            terminalPanel.SetActive(false);

        // Set terminal title
        if (terminalTitle != null)
            terminalTitle.text = terminalName;

        // Set instructions
        if (instructionsText != null)
        {
            if (linkedPuzzle is IPuzzle puzzle)
                instructionsText.text = puzzle.GetPuzzleInstructions();
            else
                instructionsText.text = "Use this terminal to control the puzzle.";
        }

        UpdateVisualState();
    }

    void Update()
    {
        UpdateLightEffects();
        HandleManualInput();
        CheckPuzzleState();
    }

    void HandleManualInput()
    {
        if (isTerminalOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            OnInteract(); // Close terminal
        }
    }

    void UpdateLightEffects()
    {
        if (terminalLight != null)
        {
            bool puzzleIsSolved = linkedPuzzle is IPuzzle puzzle && puzzle.IsSolved();
            
            if (puzzleIsSolved)
            {
                terminalLight.color = solvedColor;
                terminalLight.intensity = 1f;
            }
            else if (isPlayerNearby && !isTerminalOpen)
            {
                terminalLight.color = unsolvedColor;
                terminalLight.intensity = Mathf.PingPong(Time.time * 2f, 1f) + 0.5f;
            }
            else if (isTerminalOpen)
            {
                terminalLight.color = unsolvedColor;
                terminalLight.intensity = 1f;
            }
        }
    }

    void CheckPuzzleState()
    {
        if (linkedPuzzle is IPuzzle puzzle && puzzle.IsSolved())
        {
            UpdateVisualState();
        }
    }

    void UpdateVisualState()
    {
        bool puzzleIsSolved = linkedPuzzle is IPuzzle puzzle && puzzle.IsSolved();
        
        if (puzzleIsSolved)
        {
            // Update hover text for solved puzzle
            hoverText = "Puzzle Completed!";
            
            if (terminalLight != null)
            {
                terminalLight.color = solvedColor;
                terminalLight.enabled = true;
            }
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

        Debug.Log($"Puzzle Terminal '{terminalName}': Player can interact");
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

        // Keep light on if puzzle is solved or terminal is open
        if (terminalLight != null)
        {
            bool puzzleIsSolved = linkedPuzzle is IPuzzle puzzle && puzzle.IsSolved();
            bool keepLightOn = puzzleIsSolved || isTerminalOpen;
            terminalLight.enabled = keepLightOn;
        }

        Debug.Log($"Puzzle Terminal '{terminalName}': Player left interaction range");
    }

    public void OnInteract()
    {
        // Notify terminal system manager
        if (TerminalSystemManager.Instance != null)
        {
            if (!isTerminalOpen)
                TerminalSystemManager.Instance.OnTerminalAccessed(terminalName, puzzleType);
            else
                TerminalSystemManager.Instance.OnTerminalClosed(terminalName);
            
            TerminalSystemManager.Instance.PlayTerminalSound();
        }

        isTerminalOpen = !isTerminalOpen;
        
        Debug.Log($"Puzzle Terminal '{terminalName}': Terminal {(isTerminalOpen ? "opened" : "closed")}");
        
        // Toggle terminal UI
        if (terminalCanvas != null)
            terminalCanvas.gameObject.SetActive(isTerminalOpen);
        else if (terminalPanel != null)
            terminalPanel.SetActive(isTerminalOpen);

        // Activate linked puzzle UI
        if (linkedPuzzle is BasePuzzle puzzle)
        {
            if (isTerminalOpen)
            {
                puzzle.ShowPuzzle();
                Debug.Log($"Activating puzzle UI for {puzzle.puzzleName}");
            }
            else
            {
                puzzle.HidePuzzle();
                Debug.Log($"Hiding puzzle UI for {puzzle.puzzleName}");
            }
        }
        else if (linkedPuzzle is ShapeCipherPuzzle shapePuzzle)
        {
            if (isTerminalOpen)
            {
                shapePuzzle.ShowPuzzle();
                Debug.Log($"Activating Shape Cipher Puzzle UI");
            }
            else
            {
                shapePuzzle.HidePuzzle();
                Debug.Log($"Hiding Shape Cipher Puzzle UI");
            }
        }

        // Find and control player
        FindCurrentPlayer();
        
        // Update cursor state and player movement
        if (isTerminalOpen)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            
            if (currentPlayer != null)
                currentPlayer.canMove = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            
            if (currentPlayer != null)
                currentPlayer.canMove = true;
        }

        // Update light
        if (terminalLight != null)
        {
            bool puzzleIsSolved = linkedPuzzle is IPuzzle iPuzzle && iPuzzle.IsSolved();
            terminalLight.enabled = isTerminalOpen || isPlayerNearby || puzzleIsSolved;
        }
    }

    void FindCurrentPlayer()
    {
        PlayerController[] players = FindObjectsByType<PlayerController>(FindObjectsSortMode.None);
        foreach (PlayerController player in players)
        {
            Alteruna.Avatar avatar = player.GetComponent<Alteruna.Avatar>();
            if (avatar != null && avatar.IsMe)
            {
                currentPlayer = player;
                break;
            }
        }
    }

    // Public methods
    public bool IsPlayerNearby() => isPlayerNearby;
    public bool IsTerminalOpen() => isTerminalOpen;
    public MonoBehaviour GetLinkedPuzzle() => linkedPuzzle;
    public bool IsPuzzleSolved() => linkedPuzzle is IPuzzle puzzle && puzzle.IsSolved();

    void OnDrawGizmos()
    {
        Gizmos.color = IsPuzzleSolved() ? Color.green : Color.cyan;
        Gizmos.DrawWireCube(transform.position, GetComponent<Collider>().bounds.size);
        
        if (isPlayerNearby)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(transform.position, GetComponent<Collider>().bounds.size * 1.1f);
        }
    }
}