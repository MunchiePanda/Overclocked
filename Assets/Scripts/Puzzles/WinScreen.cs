using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class WinScreen : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Main win screen panel")]
    public GameObject winPanel;
    
    [Tooltip("Text displaying congratulations message")]
    public TMP_Text congratsText;
    
    [Tooltip("Text showing escape time")]
    public TMP_Text escapeTimeText;
    
    [Tooltip("Text showing collected numbers")]
    public TMP_Text collectedNumbersText;
    
    [Tooltip("Text showing final escape code")]
    public TMP_Text finalCodeText;

    [Header("Buttons")]
    [Tooltip("Button to restart the game")]
    public Button restartButton;
    
    [Tooltip("Button to quit the game")]
    public Button quitButton;
    
    [Tooltip("Button to return to main menu")]
    public Button mainMenuButton;

    [Header("Settings")]
    [Tooltip("Scene name to load for restart")]
    public string restartSceneName = "SampleScene";
    
    [Tooltip("Scene name for main menu")]
    public string mainMenuSceneName = "MainMenu";

    private float escapeTime;
    private bool hasShown = false;

    void Start()
    {
        // Hide win screen initially
        if (winPanel != null)
            winPanel.SetActive(false);

        // Set up button listeners
        if (restartButton != null)
            restartButton.onClick.AddListener(RestartGame);
            
        if (quitButton != null)
            quitButton.onClick.AddListener(QuitGame);
            
        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(GoToMainMenu);

        // Start tracking escape time
        escapeTime = 0f;
    }

    void Update()
    {
        // Track escape time only if game is running and win screen hasn't been shown
        if (!hasShown && Time.timeScale > 0)
        {
            escapeTime += Time.deltaTime;
        }
    }

    public void ShowWinScreen()
    {
        if (hasShown) return;
        
        hasShown = true;
        
        if (winPanel != null)
            winPanel.SetActive(true);

        // Update win screen content
        UpdateWinScreenContent();

        // Pause the game
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log("Win screen displayed!");
    }

    private void UpdateWinScreenContent()
    {
        // Update congratulations text
        if (congratsText != null)
        {
            congratsText.text = "CONGRATULATIONS!\nYou have successfully escaped!";
        }

        // Update escape time
        if (escapeTimeText != null)
        {
            int minutes = Mathf.FloorToInt(escapeTime / 60);
            int seconds = Mathf.FloorToInt(escapeTime % 60);
            escapeTimeText.text = $"Escape Time: {minutes:D2}:{seconds:D2}";
        }

        // Get escape code manager info
        EscapeCodeManager escapeManager = FindObjectOfType<EscapeCodeManager>();
        if (escapeManager != null)
        {
            // Update collected numbers
            if (collectedNumbersText != null)
            {
                string numbersText = "Puzzle Numbers Collected:\n";
                for (int i = 0; i < escapeManager.collectedNumbers.Count; i++)
                {
                    numbersText += $"Puzzle {i + 1}: {escapeManager.collectedNumbers[i]}\n";
                }
                collectedNumbersText.text = numbersText;
            }

            // Update final code
            if (finalCodeText != null)
            {
                finalCodeText.text = $"Final Escape Code: {escapeManager.GetFinalEscapeCode()}";
            }
        }
    }

    public void RestartGame()
    {
        Debug.Log("Restarting game...");
        
        // Reset time scale
        Time.timeScale = 1f;
        
        // Load restart scene
        SceneManager.LoadScene(restartSceneName);
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }

    public void GoToMainMenu()
    {
        Debug.Log("Going to main menu...");
        
        // Reset time scale
        Time.timeScale = 1f;
        
        // Load main menu scene
        SceneManager.LoadScene(mainMenuSceneName);
    }

    public void HideWinScreen()
    {
        if (winPanel != null)
            winPanel.SetActive(false);
            
        Time.timeScale = 1f;
        hasShown = false;
        escapeTime = 0f;
    }

    // Called by EscapeCodeManager when escape is successful
    public void OnEscapeComplete()
    {
        ShowWinScreen();
    }
}