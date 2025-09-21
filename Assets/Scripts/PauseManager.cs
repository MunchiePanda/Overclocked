using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Comprehensive pause system with configurable GameObject show/hide lists
/// </summary>
public class PauseManager : MonoBehaviour
{
    [Header("🎮 Pause Settings")]
    [Tooltip("Input action for pause (default: Escape key)")]
    public InputAction pauseAction = new InputAction("Pause", binding: "<Keyboard>/escape");
    
    [Tooltip("Time scale when paused (0 = completely paused, 0.1 = slow motion)")]
    [Range(0f, 1f)]
    public float pausedTimeScale = 0f;
    
    [Tooltip("Time scale when unpaused")]
    [Range(0f, 2f)]
    public float normalTimeScale = 1f;

    [Header("📋 GameObjects to Show When Paused")]
    [Tooltip("These objects will be activated/shown when the game is paused")]
    public List<GameObject> objectsToShowWhenPaused = new List<GameObject>();

    [Header("🙈 GameObjects to Hide When Paused")]
    [Tooltip("These objects will be deactivated/hidden when the game is paused")]
    public List<GameObject> objectsToHideWhenPaused = new List<GameObject>();

    [Header("🔊 Audio Settings")]
    [Tooltip("Pause all audio when paused")]
    public bool pauseAudio = true;
    
    [Tooltip("Volume when paused (only if pauseAudio is false)")]
    [Range(0f, 1f)]
    public float pausedAudioVolume = 0.2f;

    [Header("🎯 Cursor Settings")]
    [Tooltip("Show cursor when paused")]
    public bool showCursorWhenPaused = true;
    
    [Tooltip("Lock cursor when unpaused")]
    public bool lockCursorWhenUnpaused = true;

    [Header("🔧 Debug")]
    [Tooltip("Show debug logs for pause/unpause actions")]
    public bool enableDebugLogs = true;

    // Private variables
    private bool isPaused = false;
    private float originalTimeScale;
    private float originalAudioVolume;
    private CursorLockMode originalCursorLockMode;
    private bool originalCursorVisible;

    // Static reference for easy access from other scripts
    public static PauseManager Instance { get; private set; }

    // Events
    public static System.Action OnGamePaused;
    public static System.Action OnGameUnpaused;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // If this is a duplicate, transfer any assigned GameObjects to the existing instance
            TransferReferencesToExistingInstance();
            Destroy(gameObject);
            return;
        }

        // Store original values
        originalTimeScale = Time.timeScale;
        originalAudioVolume = AudioListener.volume;
        originalCursorLockMode = Cursor.lockState;
        originalCursorVisible = Cursor.visible;
    }

    /// <summary>
    /// Transfer GameObject references from this instance to the existing singleton
    /// </summary>
    private void TransferReferencesToExistingInstance()
    {
        if (Instance != null)
        {
            // Transfer objects to show when paused
            foreach (GameObject obj in objectsToShowWhenPaused)
            {
                if (obj != null && !Instance.objectsToShowWhenPaused.Contains(obj))
                {
                    Instance.objectsToShowWhenPaused.Add(obj);
                    if (enableDebugLogs)
                    {
                        Debug.Log($"🔄 Transferred {obj.name} to existing PauseManager show list");
                    }
                }
            }

            // Transfer objects to hide when paused
            foreach (GameObject obj in objectsToHideWhenPaused)
            {
                if (obj != null && !Instance.objectsToHideWhenPaused.Contains(obj))
                {
                    Instance.objectsToHideWhenPaused.Add(obj);
                    if (enableDebugLogs)
                    {
                        Debug.Log($"🔄 Transferred {obj.name} to existing PauseManager hide list");
                    }
                }
            }
        }
    }

    private void OnEnable()
    {
        pauseAction.Enable();
        pauseAction.performed += OnPauseInput;
    }

    private void OnDisable()
    {
        if (pauseAction != null)
        {
            pauseAction.performed -= OnPauseInput;
            pauseAction.Disable();
        }
    }

    private void OnPauseInput(InputAction.CallbackContext context)
    {
        TogglePause();
    }

    /// <summary>
    /// Toggle between paused and unpaused state
    /// </summary>
    public void TogglePause()
    {
        if (isPaused)
        {
            UnpauseGame();
        }
        else
        {
            PauseGame();
        }
    }

    /// <summary>
    /// Pause the game
    /// </summary>
    public void PauseGame()
    {
        if (isPaused) return;

        // Auto-find pause menu if the list is empty or contains destroyed objects
        ValidateAndRefreshGameObjectLists();

        isPaused = true;

        // Time scale
        Time.timeScale = pausedTimeScale;

        // Audio
        if (pauseAudio)
        {
            AudioListener.pause = true;
        }
        else
        {
            AudioListener.volume = pausedAudioVolume;
        }

        // Cursor
        if (showCursorWhenPaused)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        // Disable player camera movement
        DisablePlayerMovement(true);

        // Show/Hide GameObjects
        SetGameObjectsActive(objectsToShowWhenPaused, true);
        SetGameObjectsActive(objectsToHideWhenPaused, false);

        // Debug log
        if (enableDebugLogs)
        {
            Debug.Log("⏸️ Game Paused");
        }

        // Invoke event
        OnGamePaused?.Invoke();
    }

    /// <summary>
    /// Unpause the game
    /// </summary>
    public void UnpauseGame()
    {
        if (!isPaused) return;

        isPaused = false;

        // Time scale
        Time.timeScale = normalTimeScale;

        // Audio
        if (pauseAudio)
        {
            AudioListener.pause = false;
        }
        else
        {
            AudioListener.volume = originalAudioVolume;
        }

        // Cursor
        if (lockCursorWhenUnpaused)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        // Enable player camera movement
        DisablePlayerMovement(false);

        // Show/Hide GameObjects
        SetGameObjectsActive(objectsToShowWhenPaused, false);
        SetGameObjectsActive(objectsToHideWhenPaused, true);

        // Debug log
        if (enableDebugLogs)
        {
            Debug.Log("▶️ Game Unpaused");
        }

        // Invoke event
        OnGameUnpaused?.Invoke();
    }

    /// <summary>
    /// Set active state for a list of GameObjects
    /// </summary>
    private void SetGameObjectsActive(List<GameObject> gameObjects, bool active)
    {
        foreach (GameObject obj in gameObjects)
        {
            if (obj != null)
            {
                obj.SetActive(active);
                
                // Fix broken RectTransform if this is a UI element
                if (active && obj.GetComponent<RectTransform>() != null)
                {
                    FixBrokenRectTransform(obj.GetComponent<RectTransform>());
                }
            }
        }
    }

    /// <summary>
    /// Fix broken RectTransform values (scale 0, etc.)
    /// </summary>
    private void FixBrokenRectTransform(RectTransform rectTransform)
    {
        if (rectTransform.localScale == Vector3.zero)
        {
            rectTransform.localScale = Vector3.one;
            if (enableDebugLogs)
            {
                Debug.Log($"🔧 Fixed broken scale for {rectTransform.name}");
            }
        }

        if (rectTransform.sizeDelta == Vector2.zero)
        {
            rectTransform.sizeDelta = new Vector2(1920, 1080);
            if (enableDebugLogs)
            {
                Debug.Log($"🔧 Fixed broken size for {rectTransform.name}");
            }
        }
    }

    /// <summary>
    /// Disable/Enable player movement and camera controls
    /// </summary>
    private void DisablePlayerMovement(bool disable)
    {
        PlayerController[] playerControllers = FindObjectsByType<PlayerController>(FindObjectsSortMode.None);
        foreach (PlayerController controller in playerControllers)
        {
            if (controller != null)
            {
                controller.canMove = !disable;
                if (enableDebugLogs)
                {
                    Debug.Log($"🎮 Player movement {(disable ? "disabled" : "enabled")}");
                }
            }
        }
    }

    /// <summary>
    /// Validate and refresh GameObject lists, auto-finding missing objects
    /// </summary>
    private void ValidateAndRefreshGameObjectLists()
    {
        // Remove null/destroyed objects
        objectsToShowWhenPaused.RemoveAll(obj => obj == null);
        objectsToHideWhenPaused.RemoveAll(obj => obj == null);

        // Auto-find pause menu if the show list is empty
        if (objectsToShowWhenPaused.Count == 0)
        {
            AutoFindPauseMenu();
        }

        if (enableDebugLogs && objectsToShowWhenPaused.Count == 0)
        {
            Debug.LogWarning("⚠️ No GameObjects set to show when paused! Pause menu may not appear.");
        }
    }

    /// <summary>
    /// Automatically find and register pause menu objects
    /// </summary>
    private void AutoFindPauseMenu()
    {
        // Look for GameObjects with "pause" in the name
        GameObject[] allObjects = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        foreach (GameObject obj in allObjects)
        {
            if (obj.name.ToLower().Contains("pause") && obj.GetComponent<Canvas>() != null)
            {
                AddObjectToShowWhenPaused(obj);
                if (enableDebugLogs)
                {
                    Debug.Log($"🔍 Auto-found pause menu: {obj.name}");
                }
                break;
            }
        }
    }

    /// <summary>
    /// Add a GameObject to show when paused
    /// </summary>
    public void AddObjectToShowWhenPaused(GameObject obj)
    {
        if (obj != null && !objectsToShowWhenPaused.Contains(obj))
        {
            objectsToShowWhenPaused.Add(obj);
        }
    }

    /// <summary>
    /// Add a GameObject to hide when paused
    /// </summary>
    public void AddObjectToHideWhenPaused(GameObject obj)
    {
        if (obj != null && !objectsToHideWhenPaused.Contains(obj))
        {
            objectsToHideWhenPaused.Add(obj);
        }
    }

    /// <summary>
    /// Remove a GameObject from show when paused list
    /// </summary>
    public void RemoveObjectToShowWhenPaused(GameObject obj)
    {
        objectsToShowWhenPaused.Remove(obj);
    }

    /// <summary>
    /// Remove a GameObject from hide when paused list
    /// </summary>
    public void RemoveObjectToHideWhenPaused(GameObject obj)
    {
        objectsToHideWhenPaused.Remove(obj);
    }

    /// <summary>
    /// Check if the game is currently paused
    /// </summary>
    public bool IsPaused()
    {
        return isPaused;
    }

    /// <summary>
    /// Force pause state (for cutscenes, etc.)
    /// </summary>
    public void ForcePause(bool pause)
    {
        if (pause)
        {
            PauseGame();
        }
        else
        {
            UnpauseGame();
        }
    }

    // Context menu for easy testing
    [ContextMenu("🎮 Test Pause")]
    private void TestPause()
    {
        PauseGame();
    }

    [ContextMenu("▶️ Test Unpause")]
    private void TestUnpause()
    {
        UnpauseGame();
    }

    [ContextMenu("🔄 Toggle Pause")]
    private void TestTogglePause()
    {
        TogglePause();
    }

    private void OnDestroy()
    {
        // Restore original values when destroyed
        if (Instance == this)
        {
            Time.timeScale = originalTimeScale;
            AudioListener.volume = originalAudioVolume;
            AudioListener.pause = false;
            Cursor.lockState = originalCursorLockMode;
            Cursor.visible = originalCursorVisible;
        }
    }
}