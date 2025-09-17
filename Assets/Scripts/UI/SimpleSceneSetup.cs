using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Simple scene setup helper that works reliably in Unity 6
/// </summary>
public class SimpleSceneSetup : MonoBehaviour
{
    [Header("🚀 ONE-CLICK SCENE SETUP")]
    [Space(10)]
    
    [Tooltip("Setup current scene as Start Screen")]
    public bool setupStartScreen = false;
    
    [Tooltip("Setup current scene as Lobby")]
    public bool setupLobby = false;

    [Header("⚙️ Settings")]
    public string lobbySceneName = "LobbyScene";
    public string gameSceneName = "SampleScene";

    void OnValidate()
    {
        if (setupStartScreen)
        {
            SetupStartScreenScene();
            setupStartScreen = false;
        }
        
        if (setupLobby)
        {
            SetupLobbyScene();
            setupLobby = false;
        }
    }

    [ContextMenu("🎮 Setup Start Screen Scene")]
    public void SetupStartScreenScene()
    {
        Debug.Log("🎮 Setting up Start Screen Scene...");

        // Create GameSceneManager
        CreateGameSceneManager();
        
        // Create EventSystem
        CreateEventSystem();
        
        // Create Start Screen UI
        CreateStartScreenUI();
        
        Debug.Log("✅ Start Screen Scene setup complete!");
        Debug.Log("📝 Next: Create a new scene called 'LobbyScene' and run Setup Lobby on it");
    }

    [ContextMenu("🏢 Setup Lobby Scene")]
    public void SetupLobbyScene()
    {
        Debug.Log("🏢 Setting up Lobby Scene...");

        // Create Alteruna Multiplayer
        CreateAlterunaMultiplayer();
        
        // Create Custom Room Menu UI
        CreateLobbyUI();
        
        Debug.Log("✅ Lobby Scene setup complete!");
        Debug.Log("📝 Next: Update Build Settings to include StartScreen → LobbyScene → GameScene");
    }

    private void CreateGameSceneManager()
    {
        if (FindFirstObjectByType<GameSceneManager>() != null)
        {
            Debug.Log("⚠️ GameSceneManager already exists");
            return;
        }

        GameObject managerObj = new GameObject("GameSceneManager");
        GameSceneManager manager = managerObj.AddComponent<GameSceneManager>();
        
        manager.startScreenScene = "StartScreen";
        manager.lobbyScene = lobbySceneName;
        manager.gameScene = gameSceneName;
        
        Debug.Log("✅ Created GameSceneManager");
    }

    private void CreateEventSystem()
    {
        if (FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() != null)
        {
            Debug.Log("⚠️ EventSystem already exists");
            return;
        }

        GameObject eventSystemObj = new GameObject("EventSystem");
        eventSystemObj.AddComponent<UnityEngine.EventSystems.EventSystem>();
        eventSystemObj.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
        
        Debug.Log("✅ Created EventSystem");
    }

    private void CreateStartScreenUI()
    {
        if (FindFirstObjectByType<StartScreenManager>() != null)
        {
            Debug.Log("⚠️ Start Screen UI already exists");
            return;
        }

        // Create Canvas
        GameObject canvasObj = new GameObject("StartScreenCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        
        canvasObj.AddComponent<GraphicRaycaster>();

        // Create UI Panel
        GameObject panel = CreateUIPanel(canvasObj.transform, "MainPanel");
        
        // Create Title
        GameObject title = CreateUIText(panel.transform, "Title", "OVERCLOCKED");
        TMP_Text titleText = title.GetComponent<TMP_Text>();
        titleText.fontSize = 60;
        titleText.color = Color.white;
        titleText.alignment = TextAlignmentOptions.Center;
        titleText.fontStyle = FontStyles.Bold;
        
        RectTransform titleRect = title.GetComponent<RectTransform>();
        titleRect.anchoredPosition = new Vector2(0, 200);
        titleRect.sizeDelta = new Vector2(500, 100);

        // Create Play Button
        GameObject playButton = CreateUIButton(panel.transform, "PlayButton", "START GAME");
        RectTransform playRect = playButton.GetComponent<RectTransform>();
        playRect.anchoredPosition = new Vector2(0, 0);
        playRect.sizeDelta = new Vector2(300, 60);
        
        // Create Quit Button
        GameObject quitButton = CreateUIButton(panel.transform, "QuitButton", "QUIT");
        RectTransform quitRect = quitButton.GetComponent<RectTransform>();
        quitRect.anchoredPosition = new Vector2(0, -80);
        quitRect.sizeDelta = new Vector2(300, 60);

        // Create StartScreenManager
        GameObject managerObj = new GameObject("StartScreenManager");
        StartScreenManager manager = managerObj.AddComponent<StartScreenManager>();
        
        manager.titleText = titleText;
        manager.playButton = playButton.GetComponent<Button>();
        manager.quitButton = quitButton.GetComponent<Button>();
        manager.lobbySceneName = lobbySceneName;

        // Connect button events
        manager.playButton.onClick.AddListener(manager.OnPlayButtonClicked);
        manager.quitButton.onClick.AddListener(manager.OnQuitButtonClicked);

        Debug.Log("✅ Created Start Screen UI");
    }

    private void CreateAlterunaMultiplayer()
    {
        if (FindFirstObjectByType<Alteruna.Multiplayer>() != null)
        {
            Debug.Log("⚠️ Alteruna Multiplayer already exists");
            return;
        }

        GameObject multiplayerObj = new GameObject("Multiplayer");
        multiplayerObj.AddComponent<Alteruna.Multiplayer>();
        
        Debug.Log("✅ Created Alteruna Multiplayer");
    }

    private void CreateLobbyUI()
    {
        if (FindFirstObjectByType<CustomRoomMenu>() != null)
        {
            Debug.Log("⚠️ Lobby UI already exists");
            return;
        }

        // Create Canvas
        GameObject canvasObj = new GameObject("LobbyCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();

        // Create UI Panel
        GameObject panel = CreateUIPanel(canvasObj.transform, "LobbyPanel");
        
        // Create Title
        GameObject title = CreateUIText(panel.transform, "LobbyTitle", "MULTIPLAYER LOBBY");
        TMP_Text titleText = title.GetComponent<TMP_Text>();
        titleText.fontSize = 40;
        titleText.color = Color.white;
        titleText.alignment = TextAlignmentOptions.Center;
        
        RectTransform titleRect = title.GetComponent<RectTransform>();
        titleRect.anchoredPosition = new Vector2(0, 200);
        titleRect.sizeDelta = new Vector2(500, 80);

        // Create Start Button
        GameObject startButton = CreateUIButton(panel.transform, "StartButton", "CREATE/JOIN ROOM");
        RectTransform startRect = startButton.GetComponent<RectTransform>();
        startRect.anchoredPosition = new Vector2(0, 50);
        startRect.sizeDelta = new Vector2(350, 60);
        
        // Create Leave Button
        GameObject leaveButton = CreateUIButton(panel.transform, "LeaveButton", "LEAVE ROOM");
        RectTransform leaveRect = leaveButton.GetComponent<RectTransform>();
        leaveRect.anchoredPosition = new Vector2(0, -20);
        leaveRect.sizeDelta = new Vector2(350, 60);
        
        // Create Back Button
        GameObject backButton = CreateUIButton(panel.transform, "BackButton", "BACK TO MENU");
        RectTransform backRect = backButton.GetComponent<RectTransform>();
        backRect.anchoredPosition = new Vector2(0, -90);
        backRect.sizeDelta = new Vector2(350, 60);

        // Create Custom Room Menu
        GameObject roomMenuObj = new GameObject("CustomRoomMenu");
        CustomRoomMenu roomMenu = roomMenuObj.AddComponent<CustomRoomMenu>();
        
        roomMenu.titleText = titleText;
        roomMenu.startButton = startButton.GetComponent<Button>();
        roomMenu.leaveButton = leaveButton.GetComponent<Button>();
        roomMenu.backToMenuButton = backButton.GetComponent<Button>();
        roomMenu.gameSceneName = gameSceneName;

        Debug.Log("✅ Created Lobby UI");
    }

    private GameObject CreateUIPanel(Transform parent, string name)
    {
        GameObject panel = new GameObject(name);
        panel.transform.SetParent(parent);
        
        RectTransform rect = panel.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(600, 600);
        rect.anchoredPosition = Vector2.zero;
        rect.localScale = Vector3.one;
        
        Image image = panel.AddComponent<Image>();
        image.color = new Color(0.1f, 0.1f, 0.2f, 0.9f);
        
        return panel;
    }

    private GameObject CreateUIText(Transform parent, string name, string text)
    {
        GameObject textObj = new GameObject(name);
        textObj.transform.SetParent(parent);
        
        RectTransform rect = textObj.AddComponent<RectTransform>();
        rect.localScale = Vector3.one;
        
        TMP_Text tmpText = textObj.AddComponent<TMP_Text>();
        tmpText.text = text;
        tmpText.fontSize = 24;
        tmpText.color = Color.white;
        tmpText.alignment = TextAlignmentOptions.Center;
        
        return textObj;
    }

    private GameObject CreateUIButton(Transform parent, string name, string text)
    {
        GameObject button = new GameObject(name);
        button.transform.SetParent(parent);
        
        RectTransform rect = button.AddComponent<RectTransform>();
        rect.localScale = Vector3.one;
        
        Image image = button.AddComponent<Image>();
        image.color = new Color(0.2f, 0.3f, 0.5f, 1f);
        
        Button btnComponent = button.AddComponent<Button>();
        
        // Create button text
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(button.transform);
        
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        textRect.localScale = Vector3.one;
        
        TMP_Text btnText = textObj.AddComponent<TMP_Text>();
        btnText.text = text;
        btnText.fontSize = 18;
        btnText.color = Color.white;
        btnText.alignment = TextAlignmentOptions.Center;
        btnText.fontStyle = FontStyles.Bold;
        
        return button;
    }
}