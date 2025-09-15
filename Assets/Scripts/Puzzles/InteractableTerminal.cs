using UnityEngine;

public class InteractableTerminal : MonoBehaviour, IInteractable
{
    [Tooltip("Reference to the TerminalController script")]
    public TerminalController terminalController;

    [Tooltip("Name of the terminal for UI display")]
    public string terminalName = "Computer Terminal";

    [Tooltip("Range at which the player can interact with the terminal")]
    public float interactionRange = 3f;

    [Tooltip("UI element to show when player can interact")]
    public GameObject interactionPrompt;

    [Tooltip("Enable debug logging")]
    public bool debugMode = true;

    [Tooltip("Should the terminal be interactable?")]
    public bool isInteractable = true;

    private Transform playerTransform;
    private bool playerInRange = false;
    private bool playerFound = false;
    private GameObject playerObject;

    void Start()
    {
        Log("Starting InteractableTerminal on " + gameObject.name);

        // Find the player (assuming the player has the Player tag)
        playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            playerTransform = playerObject.transform;
            playerFound = true;
            Log("Found player at position: " + playerTransform.position);
        }
        else
        {
            LogWarning("No player found with tag 'Player'");
            // Try to find any player-like object
            playerObject = GameObject.Find("Player");
            if (playerObject != null)
            {
                playerTransform = playerObject.transform;
                playerFound = true;
                Log("Found player GameObject (without Player tag) at position: " + playerTransform.position);
            }
        }

        // Hide interaction prompt by default
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
            Log("Interaction prompt found and hidden");
        }
        else
        {
            LogWarning("No interaction prompt assigned");
        }

        // Try to find TerminalController if not assigned
        if (terminalController == null)
        {
            terminalController = GetComponent<TerminalController>();
            if (terminalController == null)
            {
                Log("TerminalController reference not set on " + gameObject.name + ". Trying to find it in children...");
                terminalController = GetComponentInChildren<TerminalController>();
                if (terminalController == null)
                {
                    LogWarning("Still couldn't find TerminalController. It needs to be assigned manually.");
                }
                else
                {
                    Log("Found TerminalController in children");
                }
            }
            else
            {
                Log("Found TerminalController on same GameObject");
            }
        }
        else
        {
            Log("TerminalController reference already assigned");
        }
    }

    void Update()
    {
        // Check if player is in range
        if (playerTransform != null && playerFound && isInteractable)
        {
            float distance = Vector3.Distance(transform.position, playerTransform.position);
            bool inRangeNow = distance <= interactionRange;

            // Debug distance
            if (debugMode && Time.frameCount % 30 == 0) // Log every 30 frames to reduce spam
            {
                Log("Distance to player: " + distance + " (Range: " + interactionRange + ")");
            }

            // If range status changed
            if (inRangeNow != playerInRange)
            {
                playerInRange = inRangeNow;
                Log("Player in range: " + playerInRange);

                // Show/hide interaction prompt
                if (interactionPrompt != null)
                {
                    interactionPrompt.SetActive(playerInRange);
                    Log("Interaction prompt " + (playerInRange ? "shown" : "hidden"));
                }
            }

            // If in range and pressing E, interact with terminal
            if (playerInRange && Input.GetKeyDown(KeyCode.E))
            {
                Log("E key pressed - interacting with terminal");
                OnInteract();
            }
        }
        else if (!playerFound && isInteractable)
        {
            // Try to find player again in case it was spawned after start
            playerObject = GameObject.FindGameObjectWithTag("Player");

            if (playerObject == null)
            {
                playerObject = GameObject.Find("Player");
            }

            if (playerObject != null)
            {
                playerTransform = playerObject.transform;
                playerFound = true;
                Log("Found player after delayed search: " + playerTransform.position);
            }
        }

        // If terminal is active, check for Enter key to mark puzzle as solved
        if (terminalController != null && terminalController.IsTerminalActive() && Input.GetKeyDown(KeyCode.Return))
        {
            terminalController.OnPuzzleSolved();
        }
    }

    public void OnInteract()
    {
        if (terminalController != null && isInteractable)
        {
            Log("Toggling terminal");
            terminalController.ToggleTerminal();
        }
        else if (!isInteractable)
        {
            Log("Terminal is not interactable");
        }
        else
        {
            LogError("TerminalController reference is missing on " + gameObject.name);
        }
    }

    public void OnHover()
    {
        Log("OnHover called for " + terminalName);
    }

    public void OnHoverExit()
    {
        Log("OnHoverExit called for " + terminalName);
    }

    // Visualize interaction range in editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }

    // Helper methods for logging
    private void Log(string message)
    {
        if (debugMode)
        {
            Debug.Log("[InteractableTerminal] " + message);
        }
    }

    private void LogWarning(string message)
    {
        Debug.LogWarning("[InteractableTerminal] " + message);
    }

    private void LogError(string message)
    {
        Debug.LogError("[InteractableTerminal] " + message);
    }

    // Set interactable state
    public void SetInteractable(bool interactable)
    {
        isInteractable = interactable;
        Log("Terminal interactable state set to: " + interactable);

        // If not interactable, hide prompt and reset range state
        if (!interactable)
        {
            playerInRange = false;
            if (interactionPrompt != null)
            {
                interactionPrompt.SetActive(false);
            }
        }
    }

    // Get interactable state
    public bool IsInteractable()
    {
        return isInteractable;
    }
}
