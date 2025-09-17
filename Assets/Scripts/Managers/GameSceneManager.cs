using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// Global scene manager for handling transitions between Start → Lobby → Game
/// </summary>
public class GameSceneManager : MonoBehaviour
{
    public static GameSceneManager Instance { get; private set; }

    [Header("📋 Scene Names")]
    [Tooltip("Name of the start screen scene")]
    public string startScreenScene = "StartScreen";
    
    [Tooltip("Name of the lobby scene")]
    public string lobbyScene = "LobbyScene";
    
    [Tooltip("Name of the main game scene")]
    public string gameScene = "SampleScene";

    [Header("🎮 Loading Settings")]
    [Tooltip("Show loading screen during transitions")]
    public bool showLoadingScreen = true;
    
    [Tooltip("Minimum time to show loading screen")]
    public float minimumLoadTime = 1f;

    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    #region Public Scene Loading Methods

    [ContextMenu("🏠 Go to Start Screen")]
    public void LoadStartScreen()
    {
        LoadSceneWithTransition(startScreenScene);
    }

    [ContextMenu("🏢 Go to Lobby")]
    public void LoadLobby()
    {
        LoadSceneWithTransition(lobbyScene);
    }

    [ContextMenu("🎮 Go to Game")]
    public void LoadGameScene()
    {
        LoadSceneWithTransition(gameScene);
    }

    #endregion

    #region Scene Loading Implementation

    private void LoadSceneWithTransition(string sceneName)
    {
        if (showLoadingScreen)
        {
            StartCoroutine(LoadSceneWithLoadingScreen(sceneName));
        }
        else
        {
            SceneManager.LoadScene(sceneName);
        }
    }

    private IEnumerator LoadSceneWithLoadingScreen(string sceneName)
    {
        Debug.Log($"🔄 Loading scene: {sceneName}");

        // Start loading the scene asynchronously
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        
        // Don't let the scene activate until we're ready
        asyncLoad.allowSceneActivation = false;

        float startTime = Time.time;

        // Wait for loading to complete AND minimum time to pass
        while (!asyncLoad.isDone || (Time.time - startTime) < minimumLoadTime)
        {
            // Progress from 0 to 0.9 is the actual loading
            // 0.9 to 1.0 is activation
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            
            Debug.Log($"Loading progress: {progress * 100:F0}%");

            // Scene is ready to activate
            if (asyncLoad.progress >= 0.9f && (Time.time - startTime) >= minimumLoadTime)
            {
                asyncLoad.allowSceneActivation = true;
            }

            yield return null;
        }

        Debug.Log($"✅ Scene {sceneName} loaded successfully!");
    }

    #endregion

    #region Utility Methods

    public string GetCurrentSceneName()
    {
        return SceneManager.GetActiveScene().name;
    }

    public bool IsInStartScreen()
    {
        return GetCurrentSceneName() == startScreenScene;
    }

    public bool IsInLobby()
    {
        return GetCurrentSceneName() == lobbyScene;
    }

    public bool IsInGame()
    {
        return GetCurrentSceneName() == gameScene;
    }

    #endregion

    #region Static Helper Methods (for UI buttons)

    public static void GoToStartScreen()
    {
        if (Instance != null)
            Instance.LoadStartScreen();
    }

    public static void GoToLobby()
    {
        if (Instance != null)
            Instance.LoadLobby();
    }

    public static void GoToGame()
    {
        if (Instance != null)
            Instance.LoadGameScene();
    }

    #endregion
}