using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Handles physical door interaction and UI management
/// </summary>
[RequireComponent(typeof(Door))]
public class DoorInteractionHelper : MonoBehaviour
{
    [Header("🚪 Door Interaction Settings")]
    [Tooltip("Maximum distance for interaction")]
    public float interactionDistance = 3f;
    
    [Tooltip("Layer mask for player detection")]
    public LayerMask playerLayerMask = -1;
    
    [Tooltip("Input action name for interaction (default: 'Interact')")]
    public string interactionInput = "Interact";
    
    [Header("🖥️ UI References")]
    [Tooltip("Canvas with the door input panel")]
    public Canvas doorUICanvas;
    
    [Tooltip("Input field for door code")]
    public TMP_InputField codeInputField;
    
    [Tooltip("Submit button for door code")]
    public Button submitButton;
    
    [Tooltip("Close/Cancel button")]
    public Button closeButton;
    
    [Tooltip("Feedback text display")]
    public TMP_Text feedbackText;
    
    [Header("🎯 Interaction Prompt")]
    [Tooltip("Interaction prompt UI")]
    public GameObject interactionPrompt;
    
    [Tooltip("Text for interaction prompt")]
    public TMP_Text interactionPromptText = null;
    
    [Tooltip("Default interaction prompt message")]
    public string promptMessage = "Press E to open door";
    
    [Header("🔧 Debug")]
    [Tooltip("Enable debug logging")]
    public bool enableDebugLogs = true;
    
    [Tooltip("Show interaction range gizmo")]
    public bool showInteractionGizmo = true;
    
    // Private variables
    private Door doorScript;
    private bool playerInRange = false;
    private bool uiVisible = false;
    private Transform playerTransform;
    
    // Input detection
    private UnityEngine.InputSystem.InputAction interactAction;
    
    void Start()
    {
        Initialize();
    }
    
    void Initialize()
    {
        // Get door component
        doorScript = GetComponent<Door>();
        if (doorScript == null)
        {
            Debug.LogError("❌ DoorInteractionHelper requires a Door component!");
            enabled = false;
            return;
        }
        
        // Setup UI
        SetupUI();
        
        // Setup input
        SetupInput();
        
        // Find player
        FindPlayer();
        
        if (enableDebugLogs)
            Debug.Log($"✅ Door interaction helper initialized for {gameObject.name}");
    }
    
    void SetupUI()
    {
        // Find door UI canvas if not assigned
        if (doorUICanvas == null)
        {
            doorUICanvas = GameObject.Find("DoorInputPanel")?.GetComponent<Canvas>();
            if (doorUICanvas == null)
            {
                // Look in children
                doorUICanvas = GetComponentInChildren<Canvas>();
            }
        }
        
        // Find UI components automatically if not assigned
        if (doorUICanvas != null)
        {
            if (codeInputField == null)
                codeInputField = doorUICanvas.GetComponentInChildren<TMP_InputField>();
            
            if (submitButton == null)
            {
                Button[] buttons = doorUICanvas.GetComponentsInChildren<Button>();
                foreach (var btn in buttons)
                {
                    if (btn.name.ToLower().Contains("submit") || btn.name.ToLower().Contains("enter"))
                    {
                        submitButton = btn;
                        break;
                    }
                }
            }
            
            if (closeButton == null)
            {
                Button[] buttons = doorUICanvas.GetComponentsInChildren<Button>();
                foreach (var btn in buttons)
                {
                    if (btn.name.ToLower().Contains("close") || btn.name.ToLower().Contains("cancel"))
                    {
                        closeButton = btn;
                        break;
                    }
                }
            }
            
            if (feedbackText == null)
                feedbackText = doorUICanvas.GetComponentInChildren<TMP_Text>();
        }
        
        // Setup button events
        if (submitButton != null)
            submitButton.onClick.AddListener(SubmitCode);
        
        if (closeButton != null)
            closeButton.onClick.AddListener(CloseDoorUI);
        
        // Setup interaction prompt
        if (interactionPrompt != null && interactionPromptText != null)
            interactionPromptText.text = promptMessage;
        
        // Initially hide UI
        CloseDoorUI();
    }
    
    void SetupInput()
    {
        // Create input action for interaction
        interactAction = new UnityEngine.InputSystem.InputAction("Interact", binding: "<Keyboard>/e");
        interactAction.Enable();
        interactAction.performed += OnInteractInput;
        
        if (enableDebugLogs)
            Debug.Log("🎮 Door interaction input setup (E key)");
    }
    
    void FindPlayer()
    {
        // Try to find player by tag first
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerTransform = player.transform;
            return;
        }
        
        // Try to find PlayerController
        PlayerController playerController = FindFirstObjectByType<PlayerController>();
        if (playerController != null)
        {
            playerTransform = playerController.transform;
            return;
        }
        
        // Try to find main camera as fallback
        Camera mainCam = Camera.main;
        if (mainCam != null)
        {
            playerTransform = mainCam.transform;
        }
        
        if (enableDebugLogs)
        {
            if (playerTransform != null)
                Debug.Log($"🎯 Found player reference: {playerTransform.name}");
            else
                Debug.LogWarning("⚠️ Could not find player reference!");
        }
    }
    
    void Update()
    {
        if (playerTransform == null) return;
        
        // Check if player is in interaction range
        float distance = Vector3.Distance(transform.position, playerTransform.position);
        bool wasInRange = playerInRange;
        playerInRange = distance <= interactionDistance;
        
        // Show/hide interaction prompt
        if (playerInRange != wasInRange)
        {
            if (playerInRange && !uiVisible)
            {
                ShowInteractionPrompt();
            }
            else if (!playerInRange)
            {
                HideInteractionPrompt();
                if (uiVisible)
                    CloseDoorUI();
            }
        }
    }
    
    void OnInteractInput(UnityEngine.InputSystem.InputAction.CallbackContext context)
    {
        if (!playerInRange) return;
        
        if (!uiVisible)
        {
            ShowDoorUI();
        }
        else
        {
            SubmitCode();
        }
    }
    
    public void ShowDoorUI()
    {
        if (doorUICanvas == null) 
        {
            Debug.LogWarning("⚠️ Door UI Canvas not found!");
            return;
        }
        
        uiVisible = true;
        doorUICanvas.gameObject.SetActive(true);
        
        // Request cursor for door UI
        PauseManager.RequestCursor("DoorUI", CursorLockMode.None, true, 70);
        
        // Focus input field
        if (codeInputField != null)
        {
            codeInputField.text = "";
            codeInputField.ActivateInputField();
            codeInputField.Select();
        }
        
        // Clear feedback
        if (feedbackText != null)
            feedbackText.text = "Enter door code:";
        
        // Hide interaction prompt
        HideInteractionPrompt();
        
        if (enableDebugLogs)
            Debug.Log("🚪 Door UI shown - cursor requested");
    }
    
    public void CloseDoorUI()
    {
        if (doorUICanvas == null) return;
        
        uiVisible = false;
        doorUICanvas.gameObject.SetActive(false);
        
        // Release cursor
        PauseManager.ReleaseCursor("DoorUI");
        
        // Show interaction prompt if player still in range
        if (playerInRange)
            ShowInteractionPrompt();
        
        if (enableDebugLogs)
            Debug.Log("🚪 Door UI hidden - cursor released");
    }
    
    public void SubmitCode()
    {
        if (codeInputField == null || doorScript == null) return;
        
        string code = codeInputField.text.Trim();
        
        if (string.IsNullOrEmpty(code))
        {
            ShowFeedback("Please enter a code");
            return;
        }
        
        // Try to unlock door
        doorScript.UnlockDoor(code);
        
        // The Door script will provide feedback through its own feedbackText
        // We can close UI if door was unlocked successfully
        if (doorScript.GetType().GetField("isUnlocked", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(doorScript) is bool isUnlocked && isUnlocked)
        {
            // Door unlocked, close UI after a short delay
            Invoke(nameof(CloseDoorUI), 2f);
        }
        else
        {
            // Wrong code, clear input field
            codeInputField.text = "";
            codeInputField.ActivateInputField();
        }
        
        if (enableDebugLogs)
            Debug.Log($"🔑 Door code submitted: '{code}'");
    }
    
    void ShowFeedback(string message)
    {
        if (feedbackText != null)
            feedbackText.text = message;
        
        if (enableDebugLogs)
            Debug.Log($"💬 Door feedback: {message}");
    }
    
    void ShowInteractionPrompt()
    {
        if (interactionPrompt != null)
            interactionPrompt.SetActive(true);
        
        if (enableDebugLogs)
            Debug.Log("👁️ Interaction prompt shown");
    }
    
    void HideInteractionPrompt()
    {
        if (interactionPrompt != null)
            interactionPrompt.SetActive(false);
        
        if (enableDebugLogs)
            Debug.Log("👁️ Interaction prompt hidden");
    }
    
    void OnDestroy()
    {
        // Clean up input
        if (interactAction != null)
        {
            interactAction.performed -= OnInteractInput;
            interactAction.Dispose();
        }
        
        // Release cursor if UI is open
        if (uiVisible)
            PauseManager.ReleaseCursor("DoorUI");
        
        // Clean up button events
        if (submitButton != null)
            submitButton.onClick.RemoveListener(SubmitCode);
        
        if (closeButton != null)
            closeButton.onClick.RemoveListener(CloseDoorUI);
    }
    
    void OnDrawGizmosSelected()
    {
        if (!showInteractionGizmo) return;
        
        // Draw interaction range
        Gizmos.color = playerInRange ? Color.green : Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionDistance);
        
        // Draw line to player if in range
        if (playerTransform != null && playerInRange)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, playerTransform.position);
        }
    }
    
    // Context menu methods for testing
    [ContextMenu("🚪 Test Show Door UI")]
    public void TestShowDoorUI()
    {
        ShowDoorUI();
    }
    
    [ContextMenu("🔒 Test Close Door UI")]
    public void TestCloseDoorUI()
    {
        CloseDoorUI();
    }
    
    [ContextMenu("🔍 Find UI Components")]
    public void FindUIComponents()
    {
        SetupUI();
        Debug.Log("🔍 UI components search completed");
    }
    
    [ContextMenu("🎯 Test Find Player")]
    public void TestFindPlayer()
    {
        FindPlayer();
    }
}