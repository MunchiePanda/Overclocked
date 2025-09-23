using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Handles player interaction with doors - proximity detection and UI management
/// NOTE: This class is a duplicate - use DoorInteractionHelper in Scripts/Interaction/ folder instead
/// </summary>
public class DoorInteractionHelperDuplicate : MonoBehaviour
{
    [Header("🎮 Interaction Settings")]
    [Tooltip("Maximum distance for player interaction")]
    public float interactionDistance = 3f;
    
    [Tooltip("Key to press for interaction")]
    public KeyCode interactionKey = KeyCode.E;
    
    [Tooltip("Show debug logs")]
    public bool enableDebugLogs = false;
    
    [Tooltip("Show interaction gizmo in scene view")]
    public bool showInteractionGizmo = true;
    
    [Header("🖥️ UI References")]
    [Tooltip("Canvas containing door UI")]
    public Canvas doorUICanvas;
    
    [Tooltip("Input field for code entry")]
    public TMP_InputField codeInputField;
    
    [Tooltip("Submit button")]
    public Button submitButton;
    
    [Tooltip("Close button")]
    public Button closeButton;
    
    [Tooltip("Feedback text")]
    public TMP_Text feedbackText;
    
    [Header("🔔 Interaction Prompt")]
    [Tooltip("GameObject showing interaction prompt")]
    public GameObject interactionPrompt;
    
    [Tooltip("Text component of interaction prompt")]
    public TMP_Text interactionPromptText;
    
    [Tooltip("Show prompt when in range")]
    public bool showPromptInRange = true;
    
    // Internal state
    private bool playerInRange = false;
    private bool uiIsOpen = false;
    private GameObject player;
    private PlayerController playerController;
    private EscapeCodeManager escapeManager;
    
    void Start()
    {
        Initialize();
    }
    
    void Initialize()
    {
        // Find player
        FindPlayer();
        
        // Find escape code manager
        escapeManager = FindFirstObjectByType<EscapeCodeManager>();
        
        // Setup UI button listeners
        SetupUIButtons();
        
        // Initially hide UI and prompt
        if (doorUICanvas != null)
            doorUICanvas.gameObject.SetActive(false);
            
        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);
        
        if (enableDebugLogs)
            Debug.Log($"🎮 Door interaction helper initialized for {gameObject.name}");
    }
    
    void FindPlayer()
    {
        // Try to find player by tag
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        
        if (playerObject == null)
        {
            // Try to find by name
            playerObject = GameObject.Find("Player");
        }
        
        if (playerObject == null)
        {
            // Try to find PlayerController component
            PlayerController controller = FindFirstObjectByType<PlayerController>();
            if (controller != null)
            {
                playerObject = controller.gameObject;
            }
        }
        
        if (playerObject != null)
        {
            player = playerObject;
            playerController = player.GetComponent<PlayerController>();
            
            if (enableDebugLogs)
                Debug.Log($"✅ Found player: {player.name}");
        }
        else
        {
            if (enableDebugLogs)
                Debug.LogWarning("⚠️ No player found - interaction will not work");
        }
    }
    
    void SetupUIButtons()
    {
        if (submitButton != null)
        {
            submitButton.onClick.AddListener(OnSubmitButtonPressed);
        }
        
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(OnCloseButtonPressed);
        }
    }
    
    void Update()
    {
        if (player == null) return;
        
        // Check distance to player
        float distance = Vector3.Distance(transform.position, player.transform.position);
        bool inRange = distance <= interactionDistance;
        
        // Update player in range state
        if (inRange != playerInRange)
        {
            playerInRange = inRange;
            OnPlayerRangeChanged(inRange);
        }
        
        // Handle interaction input
        if (playerInRange && !uiIsOpen && Input.GetKeyDown(interactionKey))
        {
            OpenDoorUI();
        }
        
        // Handle escape key to close UI
        if (uiIsOpen && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseDoorUI();
        }
        
        // Update prompt text based on escape code availability
        UpdatePromptText();
    }
    
    void OnPlayerRangeChanged(bool inRange)
    {
        if (showPromptInRange && interactionPrompt != null)
        {
            interactionPrompt.SetActive(inRange && !uiIsOpen);
        }
        
        if (enableDebugLogs)
            Debug.Log($"🚶 Player {(inRange ? "entered" : "left")} interaction range");
    }
    
    void UpdatePromptText()
    {
        if (interactionPromptText == null) return;
        
        if (escapeManager != null && escapeManager.IsEscapeCodeReady())
        {
            interactionPromptText.text = "Press E to unlock door";
            interactionPromptText.color = Color.green;
        }
        else
        {
            interactionPromptText.text = "Complete puzzles first";
            interactionPromptText.color = Color.yellow;
        }
    }
    
    public void OpenDoorUI()
    {
        if (uiIsOpen) return;
        
        // Check if escape code is ready
        if (escapeManager == null || !escapeManager.IsEscapeCodeReady())
        {
            if (feedbackText != null)
            {
                feedbackText.text = "Complete all puzzles to access the door!";
                feedbackText.color = Color.red;
            }
            
            if (enableDebugLogs)
                Debug.Log("❌ Cannot open door UI - escape code not ready");
            return;
        }
        
        uiIsOpen = true;
        
        // Show UI
        if (doorUICanvas != null)
        {
            doorUICanvas.gameObject.SetActive(true);
        }
        
        // Hide interaction prompt
        if (interactionPrompt != null)
        {
            interactionPrompt.SetActive(false);
        }
        
        // Enable cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        // Pause player movement if possible
        if (playerController != null)
        {
            // Try to disable player movement
            playerController.enabled = false;
        }
        
        // Focus on input field
        if (codeInputField != null)
        {
            codeInputField.Select();
            codeInputField.ActivateInputField();
        }
        
        // Update feedback text
        if (feedbackText != null)
        {
            feedbackText.text = "Enter the escape code to unlock the door";
            feedbackText.color = Color.white;
        }
        
        if (enableDebugLogs)
            Debug.Log("🖥️ Door UI opened");
    }
    
    public void CloseDoorUI()
    {
        if (!uiIsOpen) return;
        
        uiIsOpen = false;
        
        // Hide UI
        if (doorUICanvas != null)
        {
            doorUICanvas.gameObject.SetActive(false);
        }
        
        // Show interaction prompt if player still in range
        if (interactionPrompt != null && playerInRange)
        {
            interactionPrompt.SetActive(true);
        }
        
        // Restore cursor state
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        // Re-enable player movement
        if (playerController != null)
        {
            playerController.enabled = true;
        }
        
        if (enableDebugLogs)
            Debug.Log("❌ Door UI closed");
    }
    
    void OnSubmitButtonPressed()
    {
        if (enableDebugLogs)
            Debug.Log("🔓 Submit button pressed");
        
        // The DoorCodeInput component handles the actual submission
        DoorCodeInput codeInput = doorUICanvas?.GetComponent<DoorCodeInput>();
        if (codeInput != null)
        {
            codeInput.SubmitCode();
        }
    }
    
    void OnCloseButtonPressed()
    {
        if (enableDebugLogs)
            Debug.Log("❌ Close button pressed");
        
        CloseDoorUI();
    }
    
    void OnDrawGizmosSelected()
    {
        if (!showInteractionGizmo) return;
        
        // Draw interaction range
        Gizmos.color = playerInRange ? Color.green : Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
        
        // Draw line to player if in range
        if (player != null && playerInRange)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, player.transform.position);
        }
    }
    
    // Public methods for testing and external control
    [ContextMenu("🧪 Test Show Door UI")]
    public void TestShowDoorUI()
    {
        OpenDoorUI();
    }
    
    [ContextMenu("🧪 Test Close Door UI")]
    public void TestCloseDoorUI()
    {
        CloseDoorUI();
    }
    
    [ContextMenu("🔍 Find UI References")]
    public void FindUIReferences()
    {
        if (doorUICanvas == null)
        {
            doorUICanvas = GetComponentInChildren<Canvas>();
            
            if (doorUICanvas == null)
            {
                doorUICanvas = FindFirstObjectByType<Canvas>();
            }
        }
        
        if (doorUICanvas != null)
        {
            codeInputField = doorUICanvas.GetComponentInChildren<TMP_InputField>();
            feedbackText = doorUICanvas.GetComponentInChildren<TMP_Text>();
            
            Button[] buttons = doorUICanvas.GetComponentsInChildren<Button>();
            foreach (Button btn in buttons)
            {
                if (btn.name.ToLower().Contains("submit"))
                    submitButton = btn;
                else if (btn.name.ToLower().Contains("close"))
                    closeButton = btn;
            }
            
            Debug.Log("✅ UI references found and assigned");
        }
        else
        {
            Debug.LogWarning("❌ No door UI canvas found");
        }
    }
    
    // Public getters
    public bool IsPlayerInRange => playerInRange;
    public bool IsUIOpen => uiIsOpen;
    public GameObject GetPlayer() => player;
}