using UnityEngine;
using TMPro;

public class InteractableAITerminalNew : MonoBehaviour, IInteractable
{
    [Header("Terminal References")]
    [Tooltip("The new terminal controller to use")]
    public TerminalControllerNew terminalController;
    
    [Tooltip("UI text to show interaction prompt")]
    public TMP_Text interactionPrompt;
    
    [Tooltip("Text to display when hovering")]
    public string hoverText = "Press E to access AI Terminal";

    [Header("Visual Feedback")]
    [Tooltip("Material to use when hovering")]
    public Material hoverMaterial;
    
    [Tooltip("Light to activate when hovering")]
    public Light terminalLight;
    
    [Tooltip("Particle system for terminal activity")]
    public ParticleSystem terminalParticles;
    
    [Tooltip("Audio source for terminal sounds")]
    public AudioSource terminalAudio;
    
    [Tooltip("Sound to play when accessing terminal")]
    public AudioClip accessSound;

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
        
        Debug.Log("AI Terminal initialized for Alteruna networking system");
    }

    void InitializeComponents()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null)
            originalMaterial = meshRenderer.material;

        // Find terminal controller if not assigned
        if (terminalController == null)
            terminalController = FindFirstObjectByType<TerminalControllerNew>();

        // Set up audio source
        if (terminalAudio == null)
            terminalAudio = GetComponent<AudioSource>();
        if (terminalAudio == null)
            terminalAudio = gameObject.AddComponent<AudioSource>();
    }

    void SetupLayer()
    {
        // Make sure this object is on the Interactable layer so PlayerController can detect it
        int interactableLayer = LayerMask.NameToLayer("Interactable");
        if (interactableLayer != -1)
        {
            gameObject.layer = interactableLayer;
            Debug.Log("AI Terminal set to Interactable layer for raycasting detection");
        }
        else
        {
            Debug.LogWarning("Interactable layer not found! Make sure to add 'Interactable' layer in Project Settings > Tags and Layers");
        }
    }

    void SetupUI()
    {
        // Try to find interaction prompt if not assigned
        if (interactionPrompt == null)
        {
            // Look for InteractionPrompt in children
            TMP_Text[] texts = GetComponentsInChildren<TMP_Text>();
            foreach (TMP_Text text in texts)
            {
                if (text.name.Contains("Interaction") || text.name.Contains("Prompt"))
                {
                    interactionPrompt = text;
                    break;
                }
            }
        }

        // Hide interaction prompt initially
        if (interactionPrompt != null)
        {
            interactionPrompt.gameObject.SetActive(false);
            Debug.Log("Interaction prompt UI found and hidden");
        }
        else
        {
            Debug.LogWarning("No interaction prompt UI found! The PlayerController will handle interaction feedback via raycasting.");
        }
    }

    void Update()
    {
        UpdateLightEffects();
        HandleManualInput();
    }

    void HandleManualInput()
    {
        // Handle terminal close input (backup in case player doesn't use E again)
        if (isTerminalOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            OnInteract(); // Toggle off
        }
    }

    void UpdateLightEffects()
    {
        if (terminalLight != null && isPlayerNearby && !isTerminalOpen)
        {
            // Pulsing effect when player is nearby but terminal is closed
            terminalLight.intensity = Mathf.PingPong(Time.time * 2f, 1f) + 0.5f;
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

        // Start particles
        if (terminalParticles != null && !terminalParticles.isPlaying)
        {
            terminalParticles.Play();
        }

        Debug.Log("AI Terminal: Player can interact - Press E to access terminal");
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

        // Deactivate light if terminal is not active
        if (terminalLight != null && !isTerminalOpen)
        {
            terminalLight.enabled = false;
        }

        // Stop particles if terminal is not open
        if (terminalParticles != null && !isTerminalOpen && terminalParticles.isPlaying)
        {
            terminalParticles.Stop();
        }

        Debug.Log("AI Terminal: Player left interaction range");
    }

    public void OnInteract()
    {
        if (terminalController != null)
        {
            // Play access sound
            if (terminalAudio != null && accessSound != null)
            {
                terminalAudio.PlayOneShot(accessSound);
            }

            terminalController.ToggleTerminal();
            isTerminalOpen = terminalController.IsTerminalActive();
            
            Debug.Log($"AI Terminal: Terminal {(isTerminalOpen ? "opened" : "closed")}");
            
            // Find the current player to control camera/cursor
            FindCurrentPlayer();
            
            // Update cursor state and player movement based on terminal state
            if (isTerminalOpen)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                
                // Disable player movement while terminal is open
                if (currentPlayer != null)
                {
                    currentPlayer.canMove = false;
                }
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                
                // Re-enable player movement
                if (currentPlayer != null)
                {
                    currentPlayer.canMove = true;
                }
            }
            
            // Keep light on when terminal is active
            if (terminalLight != null)
            {
                terminalLight.enabled = isTerminalOpen || isPlayerNearby;
                if (isTerminalOpen)
                {
                    terminalLight.intensity = 1f; // Stop pulsing when active
                }
            }

            // Keep particles running when terminal is active
            if (terminalParticles != null)
            {
                if (isTerminalOpen)
                {
                    if (!terminalParticles.isPlaying)
                        terminalParticles.Play();
                }
                else if (!isPlayerNearby)
                {
                    terminalParticles.Stop();
                }
            }
        }
        else
        {
            Debug.LogWarning("AI Terminal: No terminal controller assigned!");
        }
    }

    void FindCurrentPlayer()
    {
        // Find the local player (the one controlled by this client)
        PlayerController[] players = FindObjectsByType<PlayerController>(FindObjectsSortMode.None);
        foreach (PlayerController player in players)
        {
            // Check if this is the local player by looking for the Alteruna Avatar component
            Alteruna.Avatar avatar = player.GetComponent<Alteruna.Avatar>();
            if (avatar != null && avatar.IsMe)
            {
                currentPlayer = player;
                break;
            }
        }
        
        if (currentPlayer == null)
        {
            Debug.LogWarning("Could not find local player! Terminal interaction may not work properly.");
        }
    }

    // Optional: Visualize interaction range in editor
    void OnDrawGizmos()
    {
        // Show the interaction range that matches PlayerController's maxInteractionDistance
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, GetComponent<Collider>().bounds.size);
        
        if (isPlayerNearby)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(transform.position, GetComponent<Collider>().bounds.size * 1.1f);
        }
    }

    // Public methods for external scripts
    public bool IsPlayerNearby() => isPlayerNearby;
    public bool IsTerminalOpen() => isTerminalOpen;
    public void SetInteractionText(string text) 
    { 
        hoverText = text;
        if (interactionPrompt != null && isPlayerNearby)
        {
            interactionPrompt.text = text;
        }
    }
}