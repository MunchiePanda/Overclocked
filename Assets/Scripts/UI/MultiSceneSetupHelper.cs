using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// Helper to set up the complete multi-scene flow: Start → Lobby → Game
/// </summary>
public class MultiSceneSetupHelper : MonoBehaviour
{
    [Header("🌟 MULTI-SCENE SETUP")]
    [Space(10)]
    [Header("📋 SETUP GUIDE:")]
    [Header("1️⃣ Setup Start Screen → 2️⃣ Create Lobby Scene → 3️⃣ Setup Build Settings")]
    [Space(10)]
    
    [Tooltip("🎮 Setup the current scene as Start Screen")]
    public bool setupStartScreen = false;
    
    [Tooltip("🏢 Create and setup Lobby Scene")]
    public bool createLobbyScene = false;
    
    [Tooltip("⚙️ Update Build Settings with all scenes")]
    public bool updateBuildSettings = false;

    [Header("🎯 Scene Names")]
    [Tooltip("Name for the lobby scene")]
    public string lobbySceneName = "LobbyScene";
    
    [Tooltip("Name of your game scene")]
    public string gameSceneName = "SampleScene";

    void OnValidate()
    {
        if (setupStartScreen)
        {
            SetupStartScreen();
            setupStartScreen = false;
        }
        
        if (createLobbyScene)
        {
            CreateLobbyScene();
            createLobbyScene = false;
        }
        
        if (updateBuildSettings)
        {
            UpdateBuildSettings();
            updateBuildSettings = false;
        }
    }

    [ContextMenu("🎮 Setup Start Screen")]
    private void SetupStartScreen()
    {
        Debug.Log("🎮 Setting up Start Screen...");

        // Create Canvas if none exists
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            canvas = CreateStartScreenCanvas();
        }

        // Create GameSceneManager
        if (FindFirstObjectByType<GameSceneManager>() == null)
        {
            CreateGameSceneManager();
        }

        // Create StartScreenManager
        if (FindFirstObjectByType<StartScreenManager>() == null)
        {
            CreateStartScreenManager(canvas);
        }

        Debug.Log("✅ Start Screen setup complete!");
    }

    [ContextMenu("🏢 Create Lobby Scene")]
    private void CreateLobbyScene()
    {
        Debug.Log("🏢 Creating Lobby Scene...");
        
        #if UNITY_EDITOR
        // Create new scene using Unity 6 API
        UnityEngine.SceneManagement.Scene newScene = UnityEditor.SceneManagement.EditorSceneManager.NewScene(
            UnityEditor.SceneManagement.NewSceneSetup.DefaultGameObjects, 
            UnityEditor.SceneManagement.NewSceneMode.Additive
        );

        // Set scene name
        newScene.name = lobbySceneName;

        // Save the scene
        string scenePath = $"Assets/Scenes/{lobbySceneName}.unity";
        UnityEditor.SceneManagement.EditorSceneManager.SaveScene(newScene, scenePath);

        Debug.Log($"✅ Created lobby scene: {scenePath}");
        Debug.Log("📝 Next: Switch to lobby scene and run 'Setup Lobby Scene' to complete setup");
        #else
        Debug.LogWarning("⚠️ Scene creation only works in Editor");
        #endif
    }

    [ContextMenu("🏢 Setup Lobby Scene (Run this in Lobby Scene)")]
    public void SetupLobbyScene()
    {
        Debug.Log("🏢 Setting up Lobby Scene...");

        // Create Canvas
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            canvas = CreateLobbyCanvas();
        }

        // Create Alteruna Multiplayer
        if (FindFirstObjectByType<Alteruna.Multiplayer>() == null)
        {
            CreateAlterunaMultiplayer();
        }

        // Create Custom Room Menu
        if (FindFirstObjectByType<CustomRoomMenu>() == null)
        {
            CreateCustomRoomMenu(canvas);
        }

        Debug.Log("✅ Lobby Scene setup complete!");
    }

    [ContextMenu("⚙️ Update Build Settings")]
    private void UpdateBuildSettings()
    {
        Debug.Log("⚙️ Updating Build Settings...");
        
        #if UNITY_EDITOR
        var scenes = new System.Collections.Generic.List<UnityEditor.EditorBuildSettingsScene>();

        // Add Start Screen
        string startScreenPath = SceneManager.GetActiveScene().path;
        if (!string.IsNullOrEmpty(startScreenPath))
        {
            scenes.Add(new UnityEditor.EditorBuildSettingsScene(startScreenPath, true));
            Debug.Log($"✅ Added Start Screen: {startScreenPath}");
        }

        // Add Lobby Scene
        string lobbyPath = $"Assets/Scenes/{lobbySceneName}.unity";
        if (System.IO.File.Exists(lobbyPath))
        {
            scenes.Add(new UnityEditor.EditorBuildSettingsScene(lobbyPath, true));
            Debug.Log($"✅ Added Lobby Scene: {lobbyPath}");
        }

        // Add Game Scene
        string gamePath = $"Assets/Scenes/{gameSceneName}.unity";
        if (System.IO.File.Exists(gamePath))
        {
            scenes.Add(new UnityEditor.EditorBuildSettingsScene(gamePath, true));
            Debug.Log($"✅ Added Game Scene: {gamePath}");
        }

        // Update build settings
        UnityEditor.EditorBuildSettings.scenes = scenes.ToArray();
        
        Debug.Log("✅ Build Settings updated!");
        #endif
    }

    #region Creation Methods

    private Canvas CreateStartScreenCanvas()
    {
        GameObject canvasObj = new GameObject("StartScreenCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 0;
        
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();
        
        // Create UI Panel
        GameObject panel = new GameObject("StartPanel");
        panel.transform.SetParent(canvasObj.transform);
        
        RectTransform panelRect = panel.AddComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;
        
        panel.AddComponent<Image>().color = new Color(0.1f, 0.1f, 0.2f, 0.8f);

        // Create Title
        CreateTitleText(panel.transform);
        
        // Create Buttons
        CreateStartScreenButtons(panel.transform);

        Debug.Log("✅ Created Start Screen Canvas");
        return canvas;
    }

    private void CreateTitleText(Transform parent)
    {
        GameObject titleObj = new GameObject("Title");
        titleObj.transform.SetParent(parent);
        
        RectTransform titleRect = titleObj.AddComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.5f, 0.7f);
        titleRect.anchorMax = new Vector2(0.5f, 0.7f);
        titleRect.sizeDelta = new Vector2(400, 100);
        titleRect.anchoredPosition = Vector2.zero;
        
        TMP_Text titleText = titleObj.AddComponent<TMP_Text>();
        titleText.text = "OVERCLOCKED";
        titleText.fontSize = 48;
        titleText.color = Color.white;
        titleText.alignment = TextAlignmentOptions.Center;
    }

    private void CreateStartScreenButtons(Transform parent)
    {
        // Play Button
        CreateButton(parent, "PlayButton", "PLAY", new Vector2(0.5f, 0.5f), new Vector2(200, 50));
        
        // Quit Button
        CreateButton(parent, "QuitButton", "QUIT", new Vector2(0.5f, 0.4f), new Vector2(200, 50));
    }

    private GameObject CreateButton(Transform parent, string name, string text, Vector2 anchor, Vector2 size)
    {
        GameObject buttonObj = new GameObject(name);
        buttonObj.transform.SetParent(parent);
        
        RectTransform buttonRect = buttonObj.AddComponent<RectTransform>();
        buttonRect.anchorMin = anchor;
        buttonRect.anchorMax = anchor;
        buttonRect.sizeDelta = size;
        buttonRect.anchoredPosition = Vector2.zero;
        
        Button button = buttonObj.AddComponent<Button>();
        buttonObj.AddComponent<Image>().color = new Color(0.2f, 0.3f, 0.5f, 1f);
        
        // Button Text
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform);
        
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        
        TMP_Text buttonText = textObj.AddComponent<TMP_Text>();
        buttonText.text = text;
        buttonText.fontSize = 18;
        buttonText.color = Color.white;
        buttonText.alignment = TextAlignmentOptions.Center;
        
        return buttonObj;
    }

    private Canvas CreateLobbyCanvas()
    {
        GameObject canvasObj = new GameObject("LobbyCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();
        
        Debug.Log("✅ Created Lobby Canvas");
        return canvas;
    }

    private void CreateGameSceneManager()
    {
        GameObject managerObj = new GameObject("GameSceneManager");
        GameSceneManager manager = managerObj.AddComponent<GameSceneManager>();
        
        // Set scene names
        manager.lobbyScene = lobbySceneName;
        manager.gameScene = gameSceneName;
        
        Debug.Log("✅ Created GameSceneManager");
    }

    private void CreateStartScreenManager(Canvas canvas)
    {
        GameObject managerObj = new GameObject("StartScreenManager");
        StartScreenManager manager = managerObj.AddComponent<StartScreenManager>();
        
        manager.lobbySceneName = lobbySceneName;
        
        // Try to connect UI elements
        manager.titleText = canvas.GetComponentInChildren<TMP_Text>();
        Button[] buttons = canvas.GetComponentsInChildren<Button>();
        
        if (buttons.Length >= 1) manager.playButton = buttons[0];
        if (buttons.Length >= 2) manager.quitButton = buttons[1];
        
        Debug.Log("✅ Created StartScreenManager");
    }

    private void CreateAlterunaMultiplayer()
    {
        GameObject multiplayerObj = new GameObject("Multiplayer");
        multiplayerObj.AddComponent<Alteruna.Multiplayer>();
        
        Debug.Log("✅ Created Alteruna Multiplayer");
    }

    private void CreateCustomRoomMenu(Canvas canvas)
    {
        GameObject roomMenuObj = new GameObject("CustomRoomMenu");
        CustomRoomMenu roomMenu = roomMenuObj.AddComponent<CustomRoomMenu>();
        
        roomMenu.gameSceneName = gameSceneName;
        
        Debug.Log("✅ Created Custom Room Menu");
    }

    #endregion
}