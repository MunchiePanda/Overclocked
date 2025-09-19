using UnityEngine;

/// <summary>
/// Quick fix for broken pause menu - run this once to fix everything
/// </summary>
public class FixPauseMenuNow : MonoBehaviour
{
    [Header("🔧 Emergency Fix")]
    [Tooltip("Click this to immediately fix the pause menu")]
    public bool fixPauseMenuNow = false;

    private void OnValidate()
    {
        if (fixPauseMenuNow)
        {
            FixPauseMenuImmediately();
            fixPauseMenuNow = false;
        }
    }

    [ContextMenu("🔧 Fix Pause Menu Immediately")]
    public void FixPauseMenuImmediately()
    {
        Debug.Log("🔧 Fixing pause menu immediately...");

        // Find the pause menu
        GameObject pauseMenu = GameObject.Find("PauseMenu");
        if (pauseMenu == null)
        {
            Debug.LogError("❌ Could not find PauseMenu GameObject!");
            return;
        }

        // Fix the broken RectTransform
        RectTransform rect = pauseMenu.GetComponent<RectTransform>();
        if (rect != null)
        {
            // Fix scale
            if (rect.localScale == Vector3.zero)
            {
                rect.localScale = Vector3.one;
                Debug.Log("✅ Fixed broken scale");
            }

            // Fix size
            if (rect.sizeDelta == Vector2.zero)
            {
                rect.sizeDelta = new Vector2(1920, 1080);
                Debug.Log("✅ Fixed broken size");
            }

            // Set anchors to fill screen
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            Debug.Log("✅ Fixed anchors");

            // Make sure it's positioned correctly
            rect.anchoredPosition = Vector2.zero;
            rect.localPosition = Vector3.zero;
            Debug.Log("✅ Fixed position");
        }

        // Make sure Canvas has correct settings
        Canvas canvas = pauseMenu.GetComponent<Canvas>();
        if (canvas != null)
        {
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100; // High sorting order to appear on top
            Debug.Log("✅ Fixed Canvas settings");
        }

        // Make sure it starts inactive
        pauseMenu.SetActive(false);
        Debug.Log("✅ Set to inactive");

        // Find PauseManager and add this to the show list
        PauseManager pauseManager = FindFirstObjectByType<PauseManager>();
        if (pauseManager != null)
        {
            pauseManager.AddObjectToShowWhenPaused(pauseMenu);
            Debug.Log("✅ Added to PauseManager show list");
        }
        else
        {
            Debug.LogWarning("⚠️ No PauseManager found - please add one to the scene");
        }

        Debug.Log("🎉 Pause menu fix complete! Try pressing ESC now.");
    }
}