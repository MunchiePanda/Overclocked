using UnityEngine;

/// <summary>
/// Cleanup duplicate PauseManagers and fix the pause system
/// </summary>
public class CleanupDuplicatePauseManagers : MonoBehaviour
{
    [Header("🧹 Cleanup Tool")]
    [Tooltip("Click this to clean up duplicate PauseManagers")]
    public bool cleanupNow = false;

    private void OnValidate()
    {
        if (cleanupNow)
        {
            CleanupDuplicates();
            cleanupNow = false;
        }
    }

    [ContextMenu("🧹 Cleanup Duplicate PauseManagers")]
    public void CleanupDuplicates()
    {
        Debug.Log("🧹 Starting cleanup of duplicate PauseManagers...");

        // Find all PauseManager components
        PauseManager[] pauseManagers = FindObjectsByType<PauseManager>(FindObjectsSortMode.None);
        
        if (pauseManagers.Length <= 1)
        {
            Debug.Log("✅ No duplicates found!");
            return;
        }

        Debug.Log($"🔍 Found {pauseManagers.Length} PauseManagers");

        // Find the pause menu GameObject
        GameObject pauseMenu = GameObject.Find("PauseMenu");
        if (pauseMenu == null)
        {
            Debug.LogWarning("⚠️ Could not find PauseMenu GameObject!");
            return;
        }

        // Keep only one PauseManager - prefer the one NOT on the PauseMenu
        PauseManager keepManager = null;
        
        foreach (PauseManager manager in pauseManagers)
        {
            if (manager.gameObject != pauseMenu)
            {
                keepManager = manager;
                break;
            }
        }

        // If all managers are on PauseMenu, keep the first one
        if (keepManager == null)
        {
            keepManager = pauseManagers[0];
        }

        // Remove PauseManager from PauseMenu (it should only have PauseMenuController)
        PauseManager pauseMenuManager = pauseMenu.GetComponent<PauseManager>();
        if (pauseMenuManager != null && pauseMenuManager != keepManager)
        {
            Debug.Log("🗑️ Removing PauseManager from PauseMenu");
            DestroyImmediate(pauseMenuManager);
        }

        // Make sure the kept manager has the pause menu in its list
        if (keepManager != null)
        {
            keepManager.AddObjectToShowWhenPaused(pauseMenu);
            Debug.Log($"✅ Added PauseMenu to {keepManager.gameObject.name}");
        }

        // Fix the pause menu RectTransform
        FixPauseMenuRectTransform(pauseMenu);

        Debug.Log("🎉 Cleanup complete!");
    }

    private void FixPauseMenuRectTransform(GameObject pauseMenu)
    {
        RectTransform rect = pauseMenu.GetComponent<RectTransform>();
        if (rect != null)
        {
            if (rect.localScale == Vector3.zero)
            {
                rect.localScale = Vector3.one;
                Debug.Log("🔧 Fixed broken scale");
            }

            if (rect.sizeDelta == Vector2.zero)
            {
                rect.sizeDelta = new Vector2(1920, 1080);
                Debug.Log("🔧 Fixed broken size");
            }

            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            rect.anchoredPosition = Vector2.zero;
            rect.localPosition = Vector3.zero;

            Debug.Log("✅ Fixed PauseMenu RectTransform");
        }

        // Make sure Canvas has correct settings
        Canvas canvas = pauseMenu.GetComponent<Canvas>();
        if (canvas != null)
        {
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;
            Debug.Log("✅ Fixed Canvas settings");
        }

        // Make sure it starts inactive
        pauseMenu.SetActive(false);
    }
}