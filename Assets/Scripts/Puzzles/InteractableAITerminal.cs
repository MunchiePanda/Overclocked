using UnityEngine;
using TMPro;

public class InteractableAITerminal : MonoBehaviour, IInteractable
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

    [Header("Interaction Settings")]
    [Tooltip("Interaction distance")]
    public float interactionDistance = 3f;
    
    [Tooltip("Key to interact with terminal")]
    public KeyCode interactionKey = KeyCode.E;

    private Material originalMaterial;
    private MeshRenderer meshRenderer;
    private bool isPlayerNearby = false;
    private Transform playerTransform;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null)
            originalMaterial = meshRenderer.material;

        if (interactionPrompt != null)
            interactionPrompt.gameObject.SetActive(false);

        // Find terminal controller if not assigned
        if (terminalController == null)
            terminalController = FindFirstObjectByType<TerminalControllerNew>();

        // Find player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            playerTransform = player.transform;

        // Set up audio source
        if (terminalAudio == null)
            terminalAudio = GetComponent<AudioSource>();
        if (terminalAudio == null)
            terminalAudio = gameObject.AddComponent<AudioSource>();

        Debug.Log("AI Terminal initialized and ready for interaction");
    }

    void Update()
    {
        // Check distance to player
        if (playerTransform != null)
        {
            float distance = Vector3.Distance(transform.position, playerTransform.position);
            bool wasNearby = isPlayerNearby;
            isPlayerNearby = distance <= interactionDistance;

            // Handle entering/leaving interaction range
            if (isPlayerNearby && !wasNearby)
            {
                OnHover();
            }
            else if (!isPlayerNearby && wasNearby)
            {
                OnHoverExit();
            }
        }

        // Handle interaction input
        if (isPlayerNearby && Input.GetKeyDown(interactionKey))
        {
            OnInteract();
        }

        // Handle terminal close input
        if (terminalController != null && terminalController.IsTerminalActive() && Input.GetKeyDown(KeyCode.Escape))
        {
            terminalController.ToggleTerminal();
        }
    }

    public void OnHover()
    {
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
            terminalLight.intensity = Mathf.PingPong(Time.time * 2f, 1f) + 0.5f; // Pulsing effect
        }

        // Start particles
        if (terminalParticles != null && !terminalParticles.isPlaying)
        {
            terminalParticles.Play();
        }

        Debug.Log("AI Terminal: Player can interact");
    }

    public void OnHoverExit()
    {
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
        if (terminalLight != null && (terminalController == null || !terminalController.IsTerminalActive()))
        {
            terminalLight.enabled = false;
        }

        // Stop particles
        if (terminalParticles != null && terminalParticles.isPlaying)
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
            Debug.Log("AI Terminal: Terminal toggled");
            
            // Keep light on when terminal is active
            if (terminalLight != null)
            {
                terminalLight.enabled = terminalController.IsTerminalActive();
                if (terminalController.IsTerminalActive())
                {
                    terminalLight.intensity = 1f; // Stop pulsing when active
                }
            }

            // Keep particles running when terminal is active
            if (terminalParticles != null)
            {
                if (terminalController.IsTerminalActive())
                {
                    if (!terminalParticles.isPlaying)
                        terminalParticles.Play();
                }
                else
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

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            OnHover();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            OnHoverExit();
        }
    }

    // Optional: Visualize interaction range in editor
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
    }
}