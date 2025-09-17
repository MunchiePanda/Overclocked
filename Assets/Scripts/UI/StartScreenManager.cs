using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// Manages the start screen and navigation to lobby
/// </summary>
public class StartScreenManager : MonoBehaviour
{
    [Header("🎮 Start Screen UI")]
    [Tooltip("Main title text (TMP_Text or Text)")]
    public TMP_Text titleText;
    
    [Tooltip("Alternative title text (regular Text component)")]
    public Text titleTextLegacy;
    
    [Tooltip("Play button to go to lobby")]
    public Button playButton;
    
    [Tooltip("Quit button to exit game")]
    public Button quitButton;
    
    [Tooltip("Settings button (optional)")]
    public Button settingsButton;

    [Header("🎯 Scene Settings")]
    [Tooltip("Name of the lobby scene to load")]
    public string lobbySceneName = "LobbyScene";

    void Start()
    {
        SetupUI();
        SetupButtons();
    }

    private void SetupUI()
    {
        // Set title if not already set
        if (titleText != null)
        {
            titleText.text = "OVERCLOCKED";
        }

        // Make sure cursor is visible and unlocked
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void SetupButtons()
    {
        // Setup play button
        if (playButton != null)
        {
            playButton.onClick.AddListener(GoToLobby);
        }

        // Setup quit button
        if (quitButton != null)
        {
            quitButton.onClick.AddListener(QuitGame);
        }

        // Setup settings button (placeholder)
        if (settingsButton != null)
        {
            settingsButton.onClick.AddListener(OpenSettings);
        }
    }

    [ContextMenu("🚀 Go to Lobby")]
    public void GoToLobby()
    {
        Debug.Log($"🚀 Loading lobby scene: {lobbySceneName}");
        
        // Load the lobby scene
        SceneManager.LoadScene(lobbySceneName);
    }

    public void QuitGame()
    {
        Debug.Log("👋 Quitting game...");
        
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    public void OpenSettings()
    {
        Debug.Log("⚙️ Settings menu would open here");
        // TODO: Implement settings menu
    }

    // Public method for UI buttons to call
    public void OnPlayButtonClicked()
    {
        GoToLobby();
    }

    public void OnQuitButtonClicked()
    {
        QuitGame();
    }

    public void OnSettingsButtonClicked()
    {
        OpenSettings();
    }
}