using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// One-click setup for the complete pause system
/// </summary>
public class PauseSystemSetup : MonoBehaviour
{
    [Header("🛠️ Auto Setup")]
    [Tooltip("Click this to automatically set up the pause system")]
    public bool setupPauseSystem = false;

    [Header("🎯 Quick Configuration")]
    [Tooltip("Your pause menu GameObject (will be shown when paused)")]
    public GameObject pauseMenuObject;
    
    [Tooltip("Your main game UI (will be hidden when paused)")]
    public GameObject gameUIObject;

    private void OnValidate()
    {
        if (setupPauseSystem)
        {
            SetupCompletePauseSystem();
            setupPauseSystem = false;
        }
    }

    [ContextMenu("🎮 Setup Complete Pause System")]
    public void SetupCompletePauseSystem()
    {
        Debug.Log("🛠️ Setting up complete pause system...");

        // 1. Create PauseManager if it doesn't exist
        PauseManager pauseManager = FindFirstObjectByType<PauseManager>();
        if (pauseManager == null)
        {
            GameObject pauseManagerObj = new GameObject("PauseManager");
            pauseManager = pauseManagerObj.AddComponent<PauseManager>();
            Debug.Log("✅ Created PauseManager");
        }

        // 2. Auto-find pause menu if not assigned
        if (pauseMenuObject == null)
        {
            pauseMenuObject = GameObject.Find("PauseMenu");
            if (pauseMenuObject == null)
            {
                Canvas[] canvases = FindObjectsByType<Canvas>(FindObjectsSortMode.None);
                foreach (Canvas canvas in canvases)
                {
                    if (canvas.name.ToLower().Contains("pause"))
                    {
                        pauseMenuObject = canvas.gameObject;
                        break;
                    }
                }
            }
        }

        // 3. Configure PauseManager lists
        if (pauseMenuObject != null)
        {
            pauseManager.AddObjectToShowWhenPaused(pauseMenuObject);
            Debug.Log($"✅ Added {pauseMenuObject.name} to show when paused");

            // Fix broken RectTransform if needed
            RectTransform pauseRect = pauseMenuObject.GetComponent<RectTransform>();
            if (pauseRect != null)
            {
                if (pauseRect.localScale == Vector3.zero)
                {
                    pauseRect.localScale = Vector3.one;
                    Debug.Log("🔧 Fixed broken pause menu scale");
                }
                if (pauseRect.sizeDelta == Vector2.zero)
                {
                    pauseRect.sizeDelta = new Vector2(1920, 1080);
                    Debug.Log("🔧 Fixed broken pause menu size");
                }
                // Set anchors to fill screen
                pauseRect.anchorMin = Vector2.zero;
                pauseRect.anchorMax = Vector2.one;
                pauseRect.offsetMin = Vector2.zero;
                pauseRect.offsetMax = Vector2.zero;
            }

            // Make sure pause menu starts inactive
            pauseMenuObject.SetActive(false);
        }

        if (gameUIObject != null)
        {
            pauseManager.AddObjectToHideWhenPaused(gameUIObject);
            Debug.Log($"✅ Added {gameUIObject.name} to hide when paused");
        }

        // 4. Add PauseMenuController to pause menu if needed
        if (pauseMenuObject != null)
        {
            PauseMenuController menuController = pauseMenuObject.GetComponent<PauseMenuController>();
            if (menuController == null)
            {
                menuController = pauseMenuObject.AddComponent<PauseMenuController>();
                Debug.Log("✅ Added PauseMenuController");

                // Try to auto-assign resume button
                Button[] buttons = pauseMenuObject.GetComponentsInChildren<Button>(true);
                foreach (Button button in buttons)
                {
                    if (button.name.ToLower().Contains("resume") || 
                        button.name.ToLower().Contains("continue") ||
                        button.name.ToLower().Contains("unpause"))
                    {
                        menuController.resumeButton = button;
                        Debug.Log($"✅ Auto-assigned resume button: {button.name}");
                        break;
                    }
                }
            }
        }

        Debug.Log("🎉 Pause system setup complete!");
        Debug.Log("📋 Instructions:");
        Debug.Log("1. Press ESC to pause/unpause");
        Debug.Log("2. Drag GameObjects to PauseManager lists to control what shows/hides");
        Debug.Log("3. Configure buttons in PauseMenuController component");
    }

    [ContextMenu("🔍 Find All UI Canvases")]
    public void FindAllUICanvases()
    {
        Canvas[] canvases = FindObjectsByType<Canvas>(FindObjectsSortMode.None);
        Debug.Log("🔍 Found UI Canvases:");
        foreach (Canvas canvas in canvases)
        {
            Debug.Log($"• {canvas.name} (Active: {canvas.gameObject.activeSelf})");
        }
    }

    [ContextMenu("🎯 Auto-Detect Game Elements")]
    public void AutoDetectGameElements()
    {
        // Try to find pause menu
        if (pauseMenuObject == null)
        {
            GameObject[] rootObjects = SceneManager.GetActiveScene().GetRootGameObjects();
            foreach (GameObject obj in rootObjects)
            {
                if (obj.name.ToLower().Contains("pause"))
                {
                    pauseMenuObject = obj;
                    Debug.Log($"🎯 Auto-detected pause menu: {obj.name}");
                    break;
                }
            }
        }

        // Try to find main game UI
        if (gameUIObject == null)
        {
            Canvas[] canvases = FindObjectsByType<Canvas>(FindObjectsSortMode.None);
            foreach (Canvas canvas in canvases)
            {
                if (!canvas.name.ToLower().Contains("pause") && 
                    !canvas.name.ToLower().Contains("menu") &&
                    canvas.gameObject.activeSelf)
                {
                    gameUIObject = canvas.gameObject;
                    Debug.Log($"🎯 Auto-detected game UI: {canvas.name}");
                    break;
                }
            }
        }
    }
}