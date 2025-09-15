using UnityEngine;

public class InteractableTerminal : MonoBehaviour, IInteractable
{
    public TerminalController terminalController;
    public string terminalName = "Computer Terminal";
    public float interactionRange = 3f;
    public GameObject interactionPrompt; // UI element to show when player can interact

    private Transform playerTransform;
    private bool playerInRange = false;

    void Start()
    {
        // Find the player (assuming the player has the Player tag)
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
        }

        // Hide interaction prompt by default
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }
    }

    void Update()
    {
        // Check if player is in range
        if (playerTransform != null)
        {
            float distance = Vector3.Distance(transform.position, playerTransform.position);
            bool inRangeNow = distance <= interactionRange;

            // If range status changed
            if (inRangeNow != playerInRange)
            {
                playerInRange = inRangeNow;

                // Show/hide interaction prompt
                if (interactionPrompt != null)
                {
                    interactionPrompt.SetActive(playerInRange);
                }
            }

            // If in range and pressing E, interact with terminal
            if (playerInRange && Input.GetKeyDown(KeyCode.E))
            {
                OnInteract();
            }
        }
    }

    public void OnInteract()
    {
        if (terminalController != null)
        {
            terminalController.ToggleTerminal();
        }
    }

    public void OnHover()
    {
        // Visual feedback can be handled in Update() with the interaction prompt
    }

    public void OnHoverExit()
    {
        // Remove visual feedback
    }

    // Visualize interaction range in editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}
