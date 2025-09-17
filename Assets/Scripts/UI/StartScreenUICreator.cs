using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Creates the Start Screen UI in the current scene
/// </summary>
public class StartScreenUICreator : MonoBehaviour
{
    [Header("🚀 ONE-CLICK START SCREEN SETUP")]
    [Space(10)]
    [Tooltip("Create complete Start Screen UI with GameSceneManager")]
    public bool createCompleteStartScreen = false;

    void OnValidate()
    {
        if (createCompleteStartScreen)
        {
            CreateCompleteStartScreen();
            createCompleteStartScreen = false;
        }
    }

    [ContextMenu("🚀 Create Complete Start Screen")]
    public void CreateCompleteStartScreen()
    {
        Debug.Log("🚀 Creating complete Start Screen...");

        // Create GameSceneManager first (persistent across scenes)
        CreateGameSceneManager();

        // Create EventSystem if needed
        CreateEventSystem();

        // Create the UI
        CreateStartScreenUI();

        Debug.Log("🎉 Start Screen created successfully!");
        Debug.Log("📝 Next steps:");
        Debug.Log("   1. Test the UI by clicking Play button");
        Debug.Log("   2. Create lobby scene using MultiSceneSetupHelper");
        Debug.Log("   3. Update Build Settings to include all scenes");
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
        
        // Set default scene names
        manager.startScreenScene = "StartScreen";
        manager.lobbyScene = "LobbyScene";
        manager.gameScene = "SampleScene";
        
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
        // Create Canvas
        GameObject canvasObj = new GameObject("StartScreenCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 0;
        
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;
        
        canvasObj.AddComponent<GraphicRaycaster>();

        // Create Background Panel
        GameObject backgroundPanel = CreatePanel(canvasObj.transform, "BackgroundPanel");
        Image bgImage = backgroundPanel.GetComponent<Image>();
        bgImage.color = new Color(0.05f, 0.05f, 0.15f, 1f); // Dark blue background

        // Create Main Panel
        GameObject mainPanel = CreatePanel(backgroundPanel.transform, "MainPanel");
        RectTransform mainRect = mainPanel.GetComponent<RectTransform>();
        mainRect.sizeDelta = new Vector2(600, 800);
        
        Image mainImage = mainPanel.GetComponent<Image>();
        mainImage.color = new Color(0.1f, 0.1f, 0.2f, 0.9f); // Semi-transparent panel

        // Create Title
        GameObject titleObj = CreateText(mainPanel.transform, "GameTitle", "OVERCLOCKED");
        TMP_Text titleText = titleObj.GetComponent<TMP_Text>();
        titleText.fontSize = 60;
        titleText.color = new Color(0.9f, 0.9f, 1f, 1f);
        titleText.alignment = TextAlignmentOptions.Center;
        titleText.fontStyle = FontStyles.Bold;
        
        RectTransform titleRect = titleObj.GetComponent<RectTransform>();
        titleRect.anchoredPosition = new Vector2(0, 200);
        titleRect.sizeDelta = new Vector2(500, 100);

        // Create Subtitle
        GameObject subtitleObj = CreateText(mainPanel.transform, "Subtitle", "Escape Room Experience");
        TMP_Text subtitleText = subtitleObj.GetComponent<TMP_Text>();
        subtitleText.fontSize = 24;
        subtitleText.color = new Color(0.7f, 0.7f, 0.8f, 1f);
        subtitleText.alignment = TextAlignmentOptions.Center;
        subtitleText.fontStyle = FontStyles.Italic;
        
        RectTransform subtitleRect = subtitleObj.GetComponent<RectTransform>();
        subtitleRect.anchoredPosition = new Vector2(0, 150);
        subtitleRect.sizeDelta = new Vector2(400, 50);

        // Create Play Button
        GameObject playButton = CreateButton(mainPanel.transform, "PlayButton", "START GAME");
        RectTransform playRect = playButton.GetComponent<RectTransform>();
        playRect.anchoredPosition = new Vector2(0, 0);
        playRect.sizeDelta = new Vector2(300, 60);
        
        Button playBtnComponent = playButton.GetComponent<Button>();
        Image playBtnImage = playButton.GetComponent<Image>();
        playBtnImage.color = new Color(0.2f, 0.6f, 0.3f, 1f); // Green

        // Create Settings Button
        GameObject settingsButton = CreateButton(mainPanel.transform, "SettingsButton", "SETTINGS");
        RectTransform settingsRect = settingsButton.GetComponent<RectTransform>();
        settingsRect.anchoredPosition = new Vector2(0, -80);
        settingsRect.sizeDelta = new Vector2(300, 60);
        
        Image settingsBtnImage = settingsButton.GetComponent<Image>();
        settingsBtnImage.color = new Color(0.4f, 0.4f, 0.6f, 1f); // Blue-gray

        // Create Quit Button
        GameObject quitButton = CreateButton(mainPanel.transform, "QuitButton", "QUIT");
        RectTransform quitRect = quitButton.GetComponent<RectTransform>();
        quitRect.anchoredPosition = new Vector2(0, -160);
        quitRect.sizeDelta = new Vector2(300, 60);
        
        Image quitBtnImage = quitButton.GetComponent<Image>();
        quitBtnImage.color = new Color(0.6f, 0.2f, 0.2f, 1f); // Red

        // Create StartScreenManager and connect everything
        GameObject managerObj = new GameObject("StartScreenManager");
        StartScreenManager manager = managerObj.AddComponent<StartScreenManager>();
        
        manager.titleText = titleText;
        manager.playButton = playBtnComponent;
        manager.quitButton = quitButton.GetComponent<Button>();
        manager.settingsButton = settingsButton.GetComponent<Button>();
        manager.lobbySceneName = "LobbyScene";

        // Connect button events
        playBtnComponent.onClick.AddListener(manager.OnPlayButtonClicked);
        quitButton.GetComponent<Button>().onClick.AddListener(manager.OnQuitButtonClicked);
        settingsButton.GetComponent<Button>().onClick.AddListener(manager.OnSettingsButtonClicked);

        Debug.Log("✅ Created Start Screen UI with all components connected");
    }

    #region UI Creation Helpers

    private GameObject CreatePanel(Transform parent, string name)
    {
        GameObject panel = new GameObject(name);
        panel.transform.SetParent(parent, false); // false to maintain local positioning
        
        RectTransform rect = panel.AddComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        rect.localScale = Vector3.one;
        
        panel.AddComponent<Image>();
        
        return panel;
    }

    private GameObject CreateText(Transform parent, string name, string text)
    {
        GameObject textObj = new GameObject(name);
        textObj.transform.SetParent(parent, false); // false to maintain local positioning
        
        RectTransform rect = textObj.AddComponent<RectTransform>();
        rect.localScale = Vector3.one;
        
        // Use try-catch to handle TextMeshPro component creation
        TMP_Text tmpText = null;
        try
        {
            tmpText = textObj.AddComponent<TMP_Text>();
            tmpText.text = text;
            tmpText.fontSize = 18;
            tmpText.color = Color.white;
            tmpText.alignment = TextAlignmentOptions.Center;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to create TMP_Text component: {e.Message}");
            // Fallback to regular Text component if TMP fails
            DestroyImmediate(tmpText);
            Text fallbackText = textObj.AddComponent<Text>();
            fallbackText.text = text;
            fallbackText.fontSize = 18;
            fallbackText.color = Color.white;
            fallbackText.alignment = TextAnchor.MiddleCenter;
            // Try to assign a default font
            fallbackText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        }
        
        return textObj;
    }

    private GameObject CreateButton(Transform parent, string name, string text)
    {
        GameObject button = new GameObject(name);
        button.transform.SetParent(parent, false);
        
        RectTransform rect = button.AddComponent<RectTransform>();
        rect.localScale = Vector3.one;
        
        button.AddComponent<Image>();
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
        
        // Use try-catch for TextMeshPro
        try
        {
            TMP_Text btnText = textObj.AddComponent<TMP_Text>();
            btnText.text = text;
            btnText.fontSize = 20;
            btnText.color = Color.white;
            btnText.alignment = TextAlignmentOptions.Center;
            btnText.fontStyle = FontStyles.Bold;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to create TMP_Text for button: {e.Message}");
            // Fallback to regular Text
            Text btnText = textObj.AddComponent<Text>();
            btnText.text = text;
            btnText.fontSize = 20;
            btnText.color = Color.white;
            btnText.alignment = TextAnchor.MiddleCenter;
            btnText.fontStyle = FontStyle.Bold;
            btnText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        }
        
        return button;
    }

    #endregion
}