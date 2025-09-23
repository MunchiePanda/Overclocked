using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Helper to create a professional alpha end screen for Overclocked
/// </summary>
public class AlphaEndScreenHelper : MonoBehaviour
{
    [Header("Game Information")]
    [Tooltip("Game version for the alpha")]
    public string gameVersion = "Alpha v0.1.0";
    
    [Tooltip("Game title")]
    public string gameTitle = "OVERCLOCKED";
    
    [Tooltip("Developer name")]
    public string developerName = "Your Studio Name";
    
    [Header("UI Style Settings")]
    [Tooltip("Background color for the screen")]
    public Color backgroundColor = new Color(0.1f, 0.1f, 0.1f, 0.95f);
    
    [Tooltip("Primary text color")]
    public Color primaryTextColor = Color.white;
    
    [Tooltip("Accent color for highlights")]
    public Color accentColor = new Color(0.2f, 0.8f, 1f, 1f);
    
    [Tooltip("Success color for celebration text")]
    public Color successColor = new Color(0.2f, 1f, 0.3f, 1f);
    
    [Header("Animation Settings")]
    [Tooltip("Fade in duration")]
    public float fadeInDuration = 2f;
    
    [Tooltip("Text animation delay between lines")]
    public float textAnimationDelay = 0.5f;
    
    [Tooltip("Enable typewriter effect")]
    public bool useTypewriterEffect = true;
    
    [Header("Audio Settings")]
    [Tooltip("Success sound effect")]
    public AudioClip successSound;
    
    [Tooltip("Background music for end screen")]
    public AudioClip endScreenMusic;
    
    [Tooltip("Audio source for sounds")]
    public AudioSource audioSource;
    
    [Header("References")]
    [Tooltip("Target canvas for the end screen")]
    public Canvas targetCanvas;
    
    [Tooltip("Escape code manager reference")]
    public EscapeCodeManager escapeCodeManager;
    
    // Internal references
    private GameObject endScreenPanel;
    private CanvasGroup canvasGroup;
    private AudioSource musicSource;
    
    void Start()
    {
        FindReferences();
    }
    
    private void FindReferences()
    {
        if (targetCanvas == null)
        {
            targetCanvas = FindFirstObjectByType<Canvas>();
        }
        
        if (escapeCodeManager == null)
        {
            escapeCodeManager = FindFirstObjectByType<EscapeCodeManager>();
        }
        
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }
    }
    
    [ContextMenu("🎉 Create Alpha End Screen")]
    public void CreateAlphaEndScreen()
    {
        FindReferences();
        
        if (targetCanvas == null)
        {
            Debug.LogError("❌ No target canvas found!");
            return;
        }
        
        // Remove existing end screen if present
        CleanupExistingEndScreen();
        
        // Create the main end screen
        CreateMainEndScreenPanel();
        CreateHeaderSection();
        CreateCelebrationSection();
        CreateThankYouSection();
        CreateFeedbackSection();
        CreateActionButtons();
        CreateFooterSection();
        
        // Initially hide the screen
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            endScreenPanel.SetActive(false);
        }
        
        Debug.Log("✅ Alpha end screen created successfully!");
    }
    
    private void CreateMainEndScreenPanel()
    {
        endScreenPanel = new GameObject("AlphaEndScreen");
        endScreenPanel.transform.SetParent(targetCanvas.transform, false);
        
        RectTransform panelRect = endScreenPanel.AddComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;
        
        // Background
        Image background = endScreenPanel.AddComponent<Image>();
        background.color = backgroundColor;
        background.raycastTarget = true;
        
        // Canvas group for fading
        canvasGroup = endScreenPanel.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
        
        // Set high sorting order to appear on top
        Canvas endScreenCanvas = endScreenPanel.AddComponent<Canvas>();
        endScreenCanvas.overrideSorting = true;
        endScreenCanvas.sortingOrder = 1000;
    }
    
    private void CreateHeaderSection()
    {
        GameObject headerContainer = new GameObject("HeaderContainer");
        headerContainer.transform.SetParent(endScreenPanel.transform, false);
        
        RectTransform headerRect = headerContainer.AddComponent<RectTransform>();
        headerRect.anchorMin = new Vector2(0f, 0.8f);
        headerRect.anchorMax = new Vector2(1f, 1f);
        headerRect.offsetMin = Vector2.zero;
        headerRect.offsetMax = Vector2.zero;
        
        // Game title
        CreateText(headerContainer, "GameTitle", gameTitle, 48, primaryTextColor, FontStyles.Bold, TextAlignmentOptions.Center);
        
        // Subtitle
        GameObject subtitle = CreateText(headerContainer, "Subtitle", "ESCAPE ROOM THRILLER", 18, accentColor, FontStyles.Italic, TextAlignmentOptions.Center);
        RectTransform subtitleRect = subtitle.GetComponent<RectTransform>();
        subtitleRect.anchorMin = new Vector2(0f, 0.2f);
        subtitleRect.anchorMax = new Vector2(1f, 0.5f);
    }
    
    private void CreateCelebrationSection()
    {
        GameObject celebrationContainer = new GameObject("CelebrationContainer");
        celebrationContainer.transform.SetParent(endScreenPanel.transform, false);
        
        RectTransform celebrationRect = celebrationContainer.AddComponent<RectTransform>();
        celebrationRect.anchorMin = new Vector2(0f, 0.6f);
        celebrationRect.anchorMax = new Vector2(1f, 0.8f);
        celebrationRect.offsetMin = Vector2.zero;
        celebrationRect.offsetMax = Vector2.zero;
        
        // Success message
        CreateText(celebrationContainer, "SuccessMessage", "🎉 CONGRATULATIONS! 🎉", 36, successColor, FontStyles.Bold, TextAlignmentOptions.Center);
        
        // Escape message
        GameObject escapeMsg = CreateText(celebrationContainer, "EscapeMessage", "YOU SUCCESSFULLY ESCAPED!", 28, primaryTextColor, FontStyles.Bold, TextAlignmentOptions.Center);
        RectTransform escapeMsgRect = escapeMsg.GetComponent<RectTransform>();
        escapeMsgRect.anchorMin = new Vector2(0f, 0.3f);
        escapeMsgRect.anchorMax = new Vector2(1f, 0.7f);
    }
    
    private void CreateThankYouSection()
    {
        GameObject thankYouContainer = new GameObject("ThankYouContainer");
        thankYouContainer.transform.SetParent(endScreenPanel.transform, false);
        
        RectTransform thankYouRect = thankYouContainer.AddComponent<RectTransform>();
        thankYouRect.anchorMin = new Vector2(0.1f, 0.35f);
        thankYouRect.anchorMax = new Vector2(0.9f, 0.6f);
        thankYouRect.offsetMin = Vector2.zero;
        thankYouRect.offsetMax = Vector2.zero;
        
        // Thank you message
        string thankYouText = $"Thank you for playing {gameTitle} {gameVersion}!\n\n" +
                            "This is an early alpha build showcasing the core escape room mechanics. " +
                            "Your feedback is incredibly valuable in shaping the future of this game.\n\n" +
                            "I have many exciting features planned and would love your input " +
                            "to make this experience even better!";
        
        CreateText(thankYouContainer, "ThankYouText", thankYouText, 18, primaryTextColor, FontStyles.Normal, TextAlignmentOptions.Center);
    }
    
    private void CreateFeedbackSection()
    {
        GameObject feedbackContainer = new GameObject("FeedbackContainer");
        feedbackContainer.transform.SetParent(endScreenPanel.transform, false);
        
        RectTransform feedbackRect = feedbackContainer.AddComponent<RectTransform>();
        feedbackRect.anchorMin = new Vector2(0.1f, 0.15f);
        feedbackRect.anchorMax = new Vector2(0.9f, 0.35f);
        feedbackRect.offsetMin = Vector2.zero;
        feedbackRect.offsetMax = Vector2.zero;
        
        // Feedback request
        string feedbackText = "💭 Share Your Thoughts:\n" +
                            "• What did you enjoy most?\n" +
                            "• Which puzzles were your favorite?\n" +
                            "• What would you like to see in future updates?\n" +
                            "• Any bugs or issues you encountered?\n\n" +
                            "📧 Send feedback to: [Your Email Here]\n" +
                            "🐦 Follow development: @YourTwitter";
        
        CreateText(feedbackContainer, "FeedbackText", feedbackText, 14, accentColor, FontStyles.Normal, TextAlignmentOptions.TopLeft);
    }
    
    private void CreateActionButtons()
    {
        GameObject buttonContainer = new GameObject("ButtonContainer");
        buttonContainer.transform.SetParent(endScreenPanel.transform, false);
        
        RectTransform buttonRect = buttonContainer.AddComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(0.2f, 0.05f);
        buttonRect.anchorMax = new Vector2(0.8f, 0.15f);
        buttonRect.offsetMin = Vector2.zero;
        buttonRect.offsetMax = Vector2.zero;
        
        // Play Again button
        CreateActionButton(buttonContainer, "PlayAgainButton", "🔄 PLAY AGAIN", new Vector2(0f, 0f), new Vector2(0.45f, 1f), 
            () => RestartGame(), successColor);
        
        // Quit button
        CreateActionButton(buttonContainer, "QuitButton", "❌ QUIT", new Vector2(0.55f, 0f), new Vector2(1f, 1f), 
            () => QuitGame(), new Color(0.8f, 0.3f, 0.3f, 1f));
    }
    
    private void CreateFooterSection()
    {
        GameObject footerContainer = new GameObject("FooterContainer");
        footerContainer.transform.SetParent(endScreenPanel.transform, false);
        
        RectTransform footerRect = footerContainer.AddComponent<RectTransform>();
        footerRect.anchorMin = new Vector2(0f, 0f);
        footerRect.anchorMax = new Vector2(1f, 0.05f);
        footerRect.offsetMin = Vector2.zero;
        footerRect.offsetMax = Vector2.zero;
        
        // Version and developer info
        string footerText = $"{gameVersion} • Developed by {developerName} • Made with Unity {Application.unityVersion}";
        CreateText(footerContainer, "FooterText", footerText, 12, new Color(0.7f, 0.7f, 0.7f, 1f), FontStyles.Italic, TextAlignmentOptions.Center);
    }
    
    private GameObject CreateText(GameObject parent, string name, string text, float fontSize, Color color, FontStyles style, TextAlignmentOptions alignment)
    {
        GameObject textObject = new GameObject(name);
        textObject.transform.SetParent(parent.transform, false);
        
        RectTransform textRect = textObject.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        
        TMP_Text textComponent = textObject.AddComponent<TextMeshProUGUI>();
        textComponent.text = text;
        textComponent.fontSize = fontSize;
        textComponent.color = color;
        textComponent.fontStyle = style;
        textComponent.alignment = alignment;
        textComponent.textWrappingMode = TMPro.TextWrappingModes.Normal;
        
        return textObject;
    }
    
    private void CreateActionButton(GameObject parent, string name, string text, Vector2 anchorMin, Vector2 anchorMax, System.Action onClick, Color buttonColor)
    {
        GameObject buttonObject = new GameObject(name);
        buttonObject.transform.SetParent(parent.transform, false);
        
        RectTransform buttonRect = buttonObject.AddComponent<RectTransform>();
        buttonRect.anchorMin = anchorMin;
        buttonRect.anchorMax = anchorMax;
        buttonRect.offsetMin = new Vector2(5, 5);
        buttonRect.offsetMax = new Vector2(-5, -5);
        
        Button button = buttonObject.AddComponent<Button>();
        Image buttonImage = buttonObject.AddComponent<Image>();
        
        // Button styling
        ColorBlock colors = button.colors;
        colors.normalColor = buttonColor;
        colors.highlightedColor = buttonColor * 1.2f;
        colors.pressedColor = buttonColor * 0.8f;
        colors.selectedColor = buttonColor;
        button.colors = colors;
        
        buttonImage.color = buttonColor;
        button.targetGraphic = buttonImage;
        
        // Button text
        GameObject textObject = new GameObject("Text");
        textObject.transform.SetParent(buttonObject.transform, false);
        
        RectTransform textRect = textObject.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        
        TMP_Text buttonText = textObject.AddComponent<TextMeshProUGUI>();
        buttonText.text = text;
        buttonText.fontSize = 16;
        buttonText.color = Color.white;
        buttonText.alignment = TextAlignmentOptions.Center;
        buttonText.fontStyle = FontStyles.Bold;
        
        // Wire up the button
        button.onClick.AddListener(() => onClick?.Invoke());
    }
    
    public void ShowEndScreen()
    {
        if (endScreenPanel == null)
        {
            CreateAlphaEndScreen();
        }
        
        StartCoroutine(ShowEndScreenSequence());
    }
    
    private IEnumerator ShowEndScreenSequence()
    {
        Debug.Log("🎉 Showing alpha end screen...");
        
        // Enable cursor for UI interaction
        PauseManager.RequestCursor("AlphaEndScreen", CursorLockMode.None, true, 90);
        
        // Pause the game
        Time.timeScale = 0f;
        
        // Show the panel
        endScreenPanel.SetActive(true);
        
        // Play success sound
        if (audioSource != null && successSound != null)
        {
            audioSource.PlayOneShot(successSound);
        }
        
        // Start background music
        if (endScreenMusic != null)
        {
            if (musicSource == null)
            {
                musicSource = gameObject.AddComponent<AudioSource>();
                musicSource.loop = true;
                musicSource.volume = 0.3f;
            }
            musicSource.clip = endScreenMusic;
            musicSource.Play();
        }
        
        // Fade in the screen
        float elapsedTime = 0f;
        while (elapsedTime < fadeInDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeInDuration);
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }
        canvasGroup.alpha = 1f;
        
        // Optional: Animate text elements
        if (useTypewriterEffect)
        {
            yield return StartCoroutine(AnimateTextElements());
        }
        
        Debug.Log("✅ End screen fully displayed");
    }
    
    private IEnumerator AnimateTextElements()
    {
        // This could be expanded to create typewriter effects for each text element
        yield return new WaitForSecondsRealtime(textAnimationDelay);
        // Add typewriter animation logic here if desired
    }
    
    private void RestartGame()
    {
        Debug.Log("🔄 Restarting game...");
        
        // Stop music
        if (musicSource != null)
        {
            musicSource.Stop();
        }
        
        // Reset time scale
        Time.timeScale = 1f;
        
        // Release cursor control
        PauseManager.ReleaseCursor("AlphaEndScreen");
        
        // Restart the scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    private void QuitGame()
    {
        Debug.Log("❌ Quitting game...");
        
        #if UNITY_EDITOR
        EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
    
    public void HideEndScreen()
    {
        if (endScreenPanel != null)
        {
            endScreenPanel.SetActive(false);
            
            // Stop music
            if (musicSource != null)
            {
                musicSource.Stop();
            }
            
            // Restore game state
            Time.timeScale = 1f;
            PauseManager.ReleaseCursor("AlphaEndScreen");
        }
    }
    
    private void CleanupExistingEndScreen()
    {
        Transform existingScreen = targetCanvas.transform.Find("AlphaEndScreen");
        if (existingScreen != null)
        {
            DestroyImmediate(existingScreen.gameObject);
            Debug.Log("🧹 Removed existing end screen");
        }
    }
    
    // Integration with EscapeCodeManager
    public void ConnectToEscapeManager()
    {
        FindReferences();
        
        if (escapeCodeManager != null)
        {
            // Replace the existing win screen reference
            WinScreen winScreenComponent = endScreenPanel.GetComponent<WinScreen>();
            if (winScreenComponent == null)
            {
                winScreenComponent = endScreenPanel.AddComponent<WinScreen>();
            }
            escapeCodeManager.winScreen = winScreenComponent;
            Debug.Log("✅ Connected to EscapeCodeManager");
        }
        else
        {
            Debug.LogWarning("⚠️ EscapeCodeManager not found");
        }
    }
    
    [ContextMenu("🧹 Remove End Screen")]
    public void RemoveEndScreen()
    {
        CleanupExistingEndScreen();
        Debug.Log("🧹 End screen removed");
    }
    
    [ContextMenu("🎭 Test End Screen")]
    public void TestEndScreen()
    {
        if (endScreenPanel == null)
        {
            CreateAlphaEndScreen();
        }
        ShowEndScreen();
    }
    
    [ContextMenu("🔗 Connect to Escape Manager")]
    public void TestConnectToEscapeManager()
    {
        ConnectToEscapeManager();
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(AlphaEndScreenHelper))]
public class AlphaEndScreenHelperEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Alpha End Screen Setup", EditorStyles.boldLabel);
        
        AlphaEndScreenHelper helper = (AlphaEndScreenHelper)target;
        
        if (GUILayout.Button("🎉 Create Alpha End Screen", GUILayout.Height(40)))
        {
            helper.CreateAlphaEndScreen();
        }
        
        EditorGUILayout.Space();
        
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("🔗 Connect to Escape Manager", GUILayout.Height(30)))
        {
            helper.ConnectToEscapeManager();
        }
        
        if (GUILayout.Button("🎭 Test End Screen", GUILayout.Height(30)))
        {
            helper.TestEndScreen();
        }
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.Space();
        
        if (GUILayout.Button("🧹 Remove End Screen", GUILayout.Height(30)))
        {
            helper.RemoveEndScreen();
        }
        
        EditorGUILayout.Space();
        EditorGUILayout.HelpBox("Creates a professional alpha end screen with congratulations, thank you message, feedback request, and action buttons.", MessageType.Info);
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Quick Setup Tips:", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox("1. Set your game version and developer name\n2. Click 'Create Alpha End Screen'\n3. Click 'Connect to Escape Manager'\n4. Test with 'Test End Screen'", MessageType.Info);
    }
}
#endif