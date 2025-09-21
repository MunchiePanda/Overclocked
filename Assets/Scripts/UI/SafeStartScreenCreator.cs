using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Safe Start Screen Creator that uses Unity's built-in Text component (no TextMeshPro dependency)
/// </summary>
public class SafeStartScreenCreator : MonoBehaviour
{
    [Header("🚀 SAFE START SCREEN SETUP")]
    [Space(10)]
    [Tooltip("Create Start Screen UI using Unity's built-in components")]
    public bool createStartScreen = false;

    [Header("⚙️ Settings")]
    public string lobbySceneName = "LobbyScene";
    public string gameSceneName = "SampleScene";

    void OnValidate()
    {
        if (createStartScreen)
        {
            CreateSafeStartScreen();
            createStartScreen = false;
        }
    }

    [ContextMenu("🎮 Create Safe Start Screen")]
    public void CreateSafeStartScreen()
    {
        Debug.Log("🎮 Creating safe Start Screen...");

        try
        {
            // Create EventSystem first
            CreateEventSystem();
            
            // Create Canvas
            GameObject canvasObj = CreateCanvas();
            
            // Create Start Screen UI
            CreateStartScreenUI(canvasObj);
            
            // Create GameSceneManager
            CreateGameSceneManager();
            
            Debug.Log("✅ Safe Start Screen created successfully!");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ Error creating Start Screen: {e.Message}");
            Debug.LogError($"Stack trace: {e.StackTrace}");
        }
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

    private GameObject CreateCanvas()
    {
        // Check if Canvas already exists
        Canvas existingCanvas = FindFirstObjectByType<Canvas>();
        if (existingCanvas != null)
        {
            Debug.Log("⚠️ Canvas already exists, using existing one");
            return existingCanvas.gameObject;
        }

        GameObject canvasObj = new GameObject("Canvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;
        
        canvasObj.AddComponent<GraphicRaycaster>();
        
        Debug.Log("✅ Created Canvas");
        return canvasObj;
    }

    private void CreateStartScreenUI(GameObject canvasObj)
    {
        // Create main panel
        GameObject panel = CreateUIPanel(canvasObj.transform, "StartScreenPanel");
        panel.GetComponent<Image>().color = new Color(0.1f, 0.1f, 0.2f, 0.9f);
        
        RectTransform panelRect = panel.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 0.5f);
        panelRect.anchorMax = new Vector2(0.5f, 0.5f);
        panelRect.sizeDelta = new Vector2(600, 600);
        panelRect.anchoredPosition = Vector2.zero;

        // Create title
        GameObject title = CreateUIText(panel.transform, "Title", "OVERCLOCKED");
        Text titleText = title.GetComponent<Text>();
        titleText.fontSize = 60;
        titleText.color = Color.white;
        titleText.alignment = TextAnchor.MiddleCenter;
        titleText.fontStyle = FontStyle.Bold;
        
        RectTransform titleRect = title.GetComponent<RectTransform>();
        titleRect.anchoredPosition = new Vector2(0, 150);
        titleRect.sizeDelta = new Vector2(500, 80);

        // Create subtitle
        GameObject subtitle = CreateUIText(panel.transform, "Subtitle", "Escape Room Experience");
        Text subtitleText = subtitle.GetComponent<Text>();
        subtitleText.fontSize = 24;
        subtitleText.color = new Color(0.8f, 0.8f, 0.9f, 1f);
        subtitleText.alignment = TextAnchor.MiddleCenter;
        
        RectTransform subtitleRect = subtitle.GetComponent<RectTransform>();
        subtitleRect.anchoredPosition = new Vector2(0, 80);
        subtitleRect.sizeDelta = new Vector2(400, 40);

        // Create play button
        GameObject playButton = CreateUIButton(panel.transform, "PlayButton", "START GAME");
        RectTransform playRect = playButton.GetComponent<RectTransform>();
        playRect.anchoredPosition = new Vector2(0, 0);
        playRect.sizeDelta = new Vector2(300, 60);

        // Create quit button
        GameObject quitButton = CreateUIButton(panel.transform, "QuitButton", "QUIT");
        RectTransform quitRect = quitButton.GetComponent<RectTransform>();
        quitRect.anchoredPosition = new Vector2(0, -80);
        quitRect.sizeDelta = new Vector2(300, 60);

        // Create StartScreenManager
        GameObject managerObj = new GameObject("StartScreenManager");
        StartScreenManager manager = managerObj.AddComponent<StartScreenManager>();
        
        // Set references using Text instead of TMP_Text
        manager.playButton = playButton.GetComponent<Button>();
        manager.quitButton = quitButton.GetComponent<Button>();
        manager.lobbySceneName = lobbySceneName;

        // Connect button events
        manager.playButton.onClick.AddListener(manager.OnPlayButtonClicked);
        manager.quitButton.onClick.AddListener(manager.OnQuitButtonClicked);

        Debug.Log("✅ Created Start Screen UI");
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

    private GameObject CreateUIPanel(Transform parent, string name)
    {
        GameObject panel = new GameObject(name);
        panel.transform.SetParent(parent, false);
        
        RectTransform rect = panel.AddComponent<RectTransform>();
        rect.localScale = Vector3.one;
        
        Image image = panel.AddComponent<Image>();
        image.color = new Color(0.2f, 0.2f, 0.3f, 0.8f);
        
        return panel;
    }

    private GameObject CreateUIText(Transform parent, string name, string text)
    {
        GameObject textObj = new GameObject(name);
        textObj.transform.SetParent(parent, false);
        
        RectTransform rect = textObj.AddComponent<RectTransform>();
        rect.localScale = Vector3.one;
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        
        Text textComponent = textObj.AddComponent<Text>();
        textComponent.text = text;
        textComponent.fontSize = 20;
        textComponent.color = Color.white;
        textComponent.alignment = TextAnchor.MiddleCenter;
        
        // Use built-in font
        textComponent.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        
        return textObj;
    }

    private GameObject CreateUIButton(Transform parent, string name, string text)
    {
        GameObject button = new GameObject(name);
        button.transform.SetParent(parent, false);
        
        RectTransform rect = button.AddComponent<RectTransform>();
        rect.localScale = Vector3.one;
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        
        Image image = button.AddComponent<Image>();
        image.color = new Color(0.2f, 0.3f, 0.5f, 1f);
        
        Button btnComponent = button.AddComponent<Button>();
        
        // Create button text
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(button.transform, false);
        
        RectTransform textRect = textObj.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        textRect.localScale = Vector3.one;
        
        Text btnText = textObj.AddComponent<Text>();
        btnText.text = text;
        btnText.fontSize = 18;
        btnText.color = Color.white;
        btnText.alignment = TextAnchor.MiddleCenter;
        btnText.fontStyle = FontStyle.Bold;
        btnText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        
        return button;
    }
}