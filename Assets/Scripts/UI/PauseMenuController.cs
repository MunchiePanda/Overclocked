using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Controller for pause menu UI interactions
/// </summary>
public class PauseMenuController : MonoBehaviour
{
    [Header("🎮 Button References")]
    [Tooltip("Button to resume the game")]
    public Button resumeButton;
    
    [Tooltip("Button to restart the current level")]
    public Button restartButton;
    
    [Tooltip("Button to go to main menu")]
    public Button mainMenuButton;
    
    [Tooltip("Button to quit the game")]
    public Button quitButton;

    [Header("🎯 Scene Settings")]
    [Tooltip("Name of the main menu scene")]
    public string mainMenuSceneName = "MainMenu";
    
    [Tooltip("Confirm before quitting")]
    public bool confirmQuit = true;

    [Header("🔧 Additional Settings")]
    [Tooltip("Close pause menu when any button is clicked")]
    public bool closeMenuOnAnyButton = false;

    private PauseManager pauseManager;

    private void Start()
    {
        // Find the pause manager
        pauseManager = PauseManager.Instance;
        if (pauseManager == null)
        {
            pauseManager = FindFirstObjectByType<PauseManager>();
        }

        if (pauseManager == null)
        {
            Debug.LogWarning("⚠️ No PauseManager found! Please add a PauseManager to the scene.");
        }

        SetupButtons();
    }

    private void SetupButtons()
    {
        // Resume button
        if (resumeButton != null)
        {
            resumeButton.onClick.RemoveAllListeners();
            resumeButton.onClick.AddListener(ResumeGame);
        }

        // Restart button
        if (restartButton != null)
        {
            restartButton.onClick.RemoveAllListeners();
            restartButton.onClick.AddListener(RestartGame);
        }

        // Main menu button
        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.RemoveAllListeners();
            mainMenuButton.onClick.AddListener(GoToMainMenu);
        }

        // Quit button
        if (quitButton != null)
        {
            quitButton.onClick.RemoveAllListeners();
            quitButton.onClick.AddListener(QuitGame);
        }
    }

    /// <summary>
    /// Resume the game
    /// </summary>
    public void ResumeGame()
    {
        if (pauseManager != null)
        {
            pauseManager.UnpauseGame();
        }

        if (closeMenuOnAnyButton)
        {
            gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Restart the current level
    /// </summary>
    public void RestartGame()
    {
        // Unpause first
        if (pauseManager != null)
        {
            pauseManager.UnpauseGame();
        }

        // Reload current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// Go to main menu
    /// </summary>
    public void GoToMainMenu()
    {
        // Unpause first
        if (pauseManager != null)
        {
            pauseManager.UnpauseGame();
        }

        // Load main menu scene
        if (!string.IsNullOrEmpty(mainMenuSceneName))
        {
            SceneManager.LoadScene(mainMenuSceneName);
        }
        else
        {
            Debug.LogWarning("⚠️ Main menu scene name not set!");
        }
    }

    /// <summary>
    /// Quit the game
    /// </summary>
    public void QuitGame()
    {
        if (confirmQuit)
        {
            if (Application.isEditor)
            {
                Debug.Log("🚪 Quit game requested (Editor mode)");
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#endif
            }
            else
            {
                Application.Quit();
            }
        }
        else
        {
            Application.Quit();
        }
    }

    /// <summary>
    /// Toggle pause (can be called from buttons or other scripts)
    /// </summary>
    public void TogglePause()
    {
        if (pauseManager != null)
        {
            pauseManager.TogglePause();
        }
    }

    // Auto-find buttons if not assigned
    private void OnValidate()
    {
        if (resumeButton == null)
        {
            resumeButton = transform.Find("Panel/ResumeBtn")?.GetComponent<Button>();
            if (resumeButton == null)
            {
                resumeButton = GetComponentInChildren<Button>();
            }
        }
    }
}