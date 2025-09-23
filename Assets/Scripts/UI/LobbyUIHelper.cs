using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Helps manage UI interactions in lobby/menu scenes to prevent cursor issues
/// </summary>
public class LobbyUIHelper : MonoBehaviour
{
    [Header("🌐 Lobby UI Helper")]
    [Tooltip("Automatically setup cursor for lobby interactions")]
    public bool autoSetupOnStart = true;
    
    [Tooltip("Monitor start button clicks")]
    public bool monitorStartButton = true;
    
    [Tooltip("Enable debug logging")]
    public bool enableDebugLogs = true;
    
    private Button startButton;
    
    void Start()
    {
        if (autoSetupOnStart)
        {
            SetupLobbyUI();
        }
    }
    
    /// <summary>
    /// Setup lobby UI cursor management
    /// </summary>
    public void SetupLobbyUI()
    {
        // Ensure cursor is visible for lobby interactions
        PauseManager.RequestCursor("LobbyUI", CursorLockMode.None, true, 65);
        
        if (monitorStartButton)
        {
            FindAndSetupStartButton();
        }
        
        if (enableDebugLogs)
        {
            string sceneName = SceneManager.GetActiveScene().name;
            Debug.Log($"🌐 Lobby UI Helper initialized in '{sceneName}' - cursor visible & unlocked");
        }
    }
    
    /// <summary>
    /// Find and setup the start button to handle clicks properly
    /// </summary>
    private void FindAndSetupStartButton()
    {
        // Try to find the start button in the scene
        Button[] allButtons = FindObjectsByType<Button>(FindObjectsSortMode.None);
        
        foreach (Button button in allButtons)
        {
            if (button.name.ToLower().Contains("start"))
            {
                startButton = button;
                break;
            }
        }
        
        if (startButton != null)
        {
            // Add our click handler
            startButton.onClick.AddListener(OnStartButtonClicked);
            
            if (enableDebugLogs)
            {
                Debug.Log($"🎯 Found and setup start button: {startButton.name}");
            }
        }
        else if (enableDebugLogs)
        {
            Debug.LogWarning("⚠️ Could not find start button in scene");
        }
    }
    
    /// <summary>
    /// Handle start button click
    /// </summary>
    public void OnStartButtonClicked()
    {
        if (enableDebugLogs)
        {
            Debug.Log("🚀 Start button clicked - maintaining cursor state");
        }
        
        // Ensure cursor remains available during any UI transitions
        // The priority 65 should keep it visible until the game scene loads
        // and PlayerController takes over with priority 0
    }
    
    /// <summary>
    /// Call this when transitioning to gameplay scene
    /// </summary>
    public void OnTransitionToGameplay()
    {
        if (enableDebugLogs)
        {
            Debug.Log("🎮 Transitioning to gameplay - releasing lobby cursor");
        }
        
        // Release lobby cursor control
        PauseManager.ReleaseCursor("LobbyUI");
    }
    
    /// <summary>
    /// Manual setup for start button (call this if you know the button reference)
    /// </summary>
    public void SetupStartButton(Button button)
    {
        if (button != null)
        {
            startButton = button;
            startButton.onClick.AddListener(OnStartButtonClicked);
            
            if (enableDebugLogs)
            {
                Debug.Log($"🎯 Manually setup start button: {button.name}");
            }
        }
    }
    
    [ContextMenu("🔍 Find Start Button")]
    public void FindStartButtonManually()
    {
        FindAndSetupStartButton();
    }
    
    [ContextMenu("🧪 Test Cursor Request")]
    public void TestCursorRequest()
    {
        PauseManager.RequestCursor("TEST_LOBBY", CursorLockMode.None, true, 65);
        Debug.Log("🧪 Requested test lobby cursor");
    }
    
    [ContextMenu("🔄 Test Cursor Release")]
    public void TestCursorRelease()
    {
        PauseManager.ReleaseCursor("TEST_LOBBY");
        Debug.Log("🔄 Released test lobby cursor");
    }
    
    void OnDestroy()
    {
        // Clean up when destroyed
        PauseManager.ReleaseCursor("LobbyUI");
        
        // Remove click listener
        if (startButton != null)
        {
            startButton.onClick.RemoveListener(OnStartButtonClicked);
        }
    }
}