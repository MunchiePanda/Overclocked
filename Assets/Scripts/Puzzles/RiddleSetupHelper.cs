using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;

[System.Serializable]
public class RiddleSetupHelper : MonoBehaviour
{
    [Header("🧩 RIDDLE PUZZLE SETUP HELPER")]
    [Space(10)]
    
    [Header("📋 SETUP STEPS:")]
    [Header("1️⃣ Click 'Create Riddle Puzzle' to generate complete UI")]
    [Header("2️⃣ Customize riddles in the generated RiddlePuzzle component")]
    [Header("3️⃣ Test with 'Test Puzzle' button")]
    [Header("4️⃣ Connect to AI Terminal if needed")]
    [Space(10)]
    
    [Header("🚀 QUICK ACTIONS")]
    [Space(5)]
    
    [Tooltip("🎯 Creates complete riddle puzzle system with UI")]
    public bool createRiddlePuzzle = false;
    
    [Tooltip("🧪 Test the created riddle puzzle")]
    public bool testRiddlePuzzle = false;
    
    [Tooltip("🔗 Auto-connect to existing AI Terminal")]
    public bool connectAITerminal = false;
    
    [Tooltip("🗑️ Remove existing riddle puzzle (cleanup)")]
    public bool removeRiddlePuzzle = false;
    
    [Space(10)]
    [Header("🎨 CUSTOMIZATION OPTIONS")]
    [Space(5)]
    
    [Tooltip("Canvas sorting order (higher = on top)")]
    [Range(0, 100)]
    public int canvasSortingOrder = 10;
    
    [Tooltip("Main panel size")]
    public Vector2 panelSize = new Vector2(800, 600);
    
    [Tooltip("UI color theme")]
    public Color primaryColor = Color.cyan;
    
    [Tooltip("Button text color")]
    public Color buttonTextColor = Color.white;
    
    [Space(10)]
    [Header("📊 STATUS")]
    [Space(5)]
    
    [SerializeField, Tooltip("Current riddle puzzle in scene")]
    private RiddlePuzzle currentRiddlePuzzle;
    
    [SerializeField, Tooltip("Current riddle canvas in scene")]
    private GameObject currentRiddleCanvas;
    
    void OnValidate()
    {
        if (createRiddlePuzzle)
        {
            CreateCompletePuzzleSystem();
            createRiddlePuzzle = false;
        }
        
        if (testRiddlePuzzle)
        {
            TestPuzzleSystem();
            testRiddlePuzzle = false;
        }
        
        if (connectAITerminal)
        {
            ConnectToAITerminal();
            connectAITerminal = false;
        }
        
        if (removeRiddlePuzzle)
        {
            RemovePuzzleSystem();
            removeRiddlePuzzle = false;
        }
        
        // Update status
        RefreshStatus();
    }
    
    void Start()
    {
        RefreshStatus();
    }
    
    [ContextMenu("Create Complete Riddle Puzzle System")]
    public void CreateCompletePuzzleSystem()
    {
        Debug.Log("🧩 Creating complete riddle puzzle system...");
        
        // Remove existing if any
        RemovePuzzleSystem();
        
        // Create the UI
        CreateRiddleUI();
        
        // Create the controller
        CreateRiddleController();
        
        // Connect everything
        ConnectUIToController();
        
        // Apply styling
        ApplyStyling();
        
        // Auto-connect AI terminal if available
        ConnectToAITerminal();
        
        RefreshStatus();
        
        Debug.Log("✅ Riddle puzzle system created successfully!");
        Debug.Log("💡 Check the RiddlePuzzleController in your hierarchy to customize riddles.");
    }
    
    private void CreateRiddleUI()
    {
        // Create main canvas
        GameObject canvas = new GameObject("RiddleCanvas");
        Canvas canvasComponent = canvas.AddComponent<Canvas>();
        canvasComponent.renderMode = RenderMode.ScreenSpaceOverlay;
        canvasComponent.sortingOrder = canvasSortingOrder;
        
        CanvasScaler scaler = canvas.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;
        
        canvas.AddComponent<GraphicRaycaster>();
        canvas.SetActive(false);
        currentRiddleCanvas = canvas;
        
        // Create main panel
        GameObject mainPanel = CreateUIPanel(canvas.transform, "RiddlePanel", panelSize);
        
        // Create title
        GameObject title = CreateText(mainPanel.transform, "TitleText", "🧩 RIDDLE CHALLENGE", 
            new Vector2(600, 50), new Vector2(0, 250));
        TMP_Text titleText = title.GetComponent<TMP_Text>();
        titleText.fontSize = 28;
        titleText.fontStyle = FontStyles.Bold;
        titleText.alignment = TextAlignmentOptions.Center;
        
        // Create riddle display area
        GameObject riddleDisplay = CreateText(mainPanel.transform, "RiddleDisplay", 
            "Welcome to the Riddle Challenge! Use the navigation buttons to browse riddles.",
            new Vector2(700, 120), new Vector2(0, 150));
        TMP_Text riddleText = riddleDisplay.GetComponent<TMP_Text>();
        riddleText.fontSize = 18;
        riddleText.alignment = TextAlignmentOptions.Center;
        
        // Create progress display
        GameObject progressDisplay = CreateText(mainPanel.transform, "RiddleProgressDisplay", "Riddle 1 of 4",
            new Vector2(200, 30), new Vector2(0, 80));
        TMP_Text progressText = progressDisplay.GetComponent<TMP_Text>();
        progressText.alignment = TextAlignmentOptions.Center;
        progressText.fontStyle = FontStyles.Bold;
        
        // Create navigation buttons
        GameObject prevBtn = CreateButton(mainPanel.transform, "PreviousRiddleButton", "← Previous",
            new Vector2(120, 40), new Vector2(-150, 40));
        GameObject nextBtn = CreateButton(mainPanel.transform, "NextRiddleButton", "Next →",
            new Vector2(120, 40), new Vector2(150, 40));
        
        // Create input section
        GameObject inputLabel = CreateText(mainPanel.transform, "InputLabel", "Your Answer:",
            new Vector2(200, 25), new Vector2(-200, -10));
        inputLabel.GetComponent<TMP_Text>().alignment = TextAlignmentOptions.Left;
        
        GameObject inputField = CreateInputField(mainPanel.transform, "RiddleAnswerInput", "Enter your answer here...",
            new Vector2(400, 40), new Vector2(0, -10));
        
        // Create action buttons
        GameObject submitBtn = CreateButton(mainPanel.transform, "SubmitAnswerButton", "Submit Answer",
            new Vector2(140, 40), new Vector2(-100, -60));
        GameObject resetBtn = CreateButton(mainPanel.transform, "ResetButton", "Clear",
            new Vector2(80, 40), new Vector2(50, -60));
        
        // Create solved riddles display
        GameObject solvedDisplay = CreateText(mainPanel.transform, "SolvedRiddlesDisplay", 
            "Solved Riddles:\nNone yet",
            new Vector2(300, 120), new Vector2(-200, -140));
        TMP_Text solvedText = solvedDisplay.GetComponent<TMP_Text>();
        solvedText.fontSize = 14;
        solvedText.alignment = TextAlignmentOptions.TopLeft;
        
        // Create feedback area
        GameObject feedback = CreateText(mainPanel.transform, "FeedbackText", 
            "💡 Tip: If you're stuck, try asking the AI terminal for 'help'!",
            new Vector2(600, 60), new Vector2(0, -200));
        TMP_Text feedbackText = feedback.GetComponent<TMP_Text>();
        feedbackText.fontSize = 16;
        feedbackText.alignment = TextAlignmentOptions.Center;
        
        // Create control buttons
        GameObject closeBtn = CreateButton(mainPanel.transform, "CloseButton", "✕ Close",
            new Vector2(100, 40), new Vector2(300, 250));
        GameObject hintBtn = CreateButton(mainPanel.transform, "HintButton", "💡 Hint",
            new Vector2(100, 40), new Vector2(200, -60));
        
        Debug.Log("🎨 Riddle UI created successfully!");
    }
    
    private void CreateRiddleController()
    {
        GameObject controllerObj = new GameObject("RiddlePuzzleController");
        currentRiddlePuzzle = controllerObj.AddComponent<RiddlePuzzle>();
        
        // Set basic properties
        currentRiddlePuzzle.puzzleName = "Riddle Challenge";
        currentRiddlePuzzle.puzzleDescription = "Solve all riddles to unlock your escape code!";
        
        Debug.Log("🎮 Riddle controller created successfully!");
    }
    
    private void ConnectUIToController()
    {
        if (currentRiddlePuzzle == null || currentRiddleCanvas == null) return;
        
        // Connect main UI
        currentRiddlePuzzle.puzzleUI = currentRiddleCanvas;
        
        // Find and connect all UI elements
        currentRiddlePuzzle.riddleDisplay = FindUIComponent<TMP_Text>("RiddleDisplay");
        currentRiddlePuzzle.riddleProgressDisplay = FindUIComponent<TMP_Text>("RiddleProgressDisplay");
        currentRiddlePuzzle.riddleAnswerInput = FindUIComponent<TMP_InputField>("RiddleAnswerInput");
        currentRiddlePuzzle.submitAnswerButton = FindUIComponent<Button>("SubmitAnswerButton");
        currentRiddlePuzzle.nextRiddleButton = FindUIComponent<Button>("NextRiddleButton");
        currentRiddlePuzzle.previousRiddleButton = FindUIComponent<Button>("PreviousRiddleButton");
        currentRiddlePuzzle.solvedRiddlesDisplay = FindUIComponent<TMP_Text>("SolvedRiddlesDisplay");
        currentRiddlePuzzle.resetButton = FindUIComponent<Button>("ResetButton");
        currentRiddlePuzzle.feedbackText = FindUIComponent<TMP_Text>("FeedbackText");
        currentRiddlePuzzle.closeButton = FindUIComponent<Button>("CloseButton");
        
        Debug.Log("🔗 UI connected to controller successfully!");
    }
    
    private T FindUIComponent<T>(string name) where T : Component
    {
        GameObject obj = GameObject.Find(name);
        return obj != null ? obj.GetComponent<T>() : null;
    }
    
    private void ApplyStyling()
    {
        if (currentRiddleCanvas == null) return;
        
        // Style feedback text
        TMP_Text feedback = FindUIComponent<TMP_Text>("FeedbackText");
        if (feedback != null) feedback.color = primaryColor;
        
        // Style title
        TMP_Text title = FindUIComponent<TMP_Text>("TitleText");
        if (title != null) title.color = primaryColor;
        
        // Style progress
        TMP_Text progress = FindUIComponent<TMP_Text>("RiddleProgressDisplay");
        if (progress != null) progress.color = primaryColor;
        
        Debug.Log("🎨 Styling applied successfully!");
    }
    
    [ContextMenu("Test Puzzle System")]
    public void TestPuzzleSystem()
    {
        RefreshStatus();
        
        if (currentRiddlePuzzle == null)
        {
            Debug.LogWarning("⚠️ No riddle puzzle found! Create one first.");
            return;
        }
        
        Debug.Log("🧪 Testing riddle puzzle...");
        currentRiddlePuzzle.TestShowPuzzle();
    }
    
    [ContextMenu("Connect to AI Terminal")]
    public void ConnectToAITerminal()
    {
        RefreshStatus();
        
        if (currentRiddlePuzzle == null)
        {
            Debug.LogWarning("⚠️ No riddle puzzle found! Create one first.");
            return;
        }
        
        TerminalControllerNew terminal = FindFirstObjectByType<TerminalControllerNew>();
        if (terminal != null)
        {
            currentRiddlePuzzle.aiTerminal = terminal;
            Debug.Log("🔗 Connected to AI Terminal successfully!");
        }
        else
        {
            Debug.LogWarning("⚠️ No AI Terminal found in scene.");
        }
    }
    
    [ContextMenu("Remove Puzzle System")]
    public void RemovePuzzleSystem()
    {
        // Remove existing riddle components
        RiddlePuzzle[] existing = FindObjectsByType<RiddlePuzzle>(FindObjectsSortMode.None);
        foreach (RiddlePuzzle puzzle in existing)
        {
            DestroyImmediate(puzzle.gameObject);
        }
        
        // Remove existing canvas
        GameObject existingCanvas = GameObject.Find("RiddleCanvas");
        if (existingCanvas != null)
        {
            DestroyImmediate(existingCanvas);
        }
        
        currentRiddlePuzzle = null;
        currentRiddleCanvas = null;
        
        Debug.Log("🗑️ Existing riddle puzzle removed.");
    }
    
    private void RefreshStatus()
    {
        currentRiddlePuzzle = FindFirstObjectByType<RiddlePuzzle>();
        currentRiddleCanvas = GameObject.Find("RiddleCanvas");
    }
    
    // UI Creation Helper Methods
    private GameObject CreateUIPanel(Transform parent, string name, Vector2 size)
    {
        GameObject panel = new GameObject(name);
        panel.transform.SetParent(parent, false);
        
        Image image = panel.AddComponent<Image>();
        image.color = new Color(0.1f, 0.1f, 0.1f, 0.8f);
        
        RectTransform rect = panel.GetComponent<RectTransform>();
        rect.sizeDelta = size;
        rect.anchorMin = Vector2.one * 0.5f;
        rect.anchorMax = Vector2.one * 0.5f;
        rect.anchoredPosition = Vector2.zero;
        
        return panel;
    }
    
    private GameObject CreateText(Transform parent, string name, string text, Vector2 size, Vector2 position)
    {
        GameObject textObj = new GameObject(name);
        textObj.transform.SetParent(parent, false);
        
        // Add RectTransform first
        RectTransform rect = textObj.AddComponent<RectTransform>();
        rect.sizeDelta = size;
        rect.anchoredPosition = position;
        
        // Try to add TMP_Text component safely
        TMP_Text tmpText = null;
        try
        {
            tmpText = textObj.AddComponent<TMP_Text>();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to add TMP_Text component: {e.Message}");
            // Fallback to regular Text component
            Text fallbackText = textObj.AddComponent<Text>();
            fallbackText.text = text;
            fallbackText.fontSize = 16;
            fallbackText.color = Color.white;
            fallbackText.alignment = TextAnchor.MiddleCenter;
            
            // Add font
            fallbackText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            return textObj;
        }
        
        if (tmpText != null)
        {
            tmpText.text = text;
            tmpText.fontSize = 16;
            tmpText.color = Color.white;
            tmpText.alignment = TextAlignmentOptions.Center;
            tmpText.textWrappingMode = TextWrappingModes.Normal;
            
            // Set default font if available
            try
            {
                if (tmpText.font == null)
                {
                    tmpText.font = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
                    if (tmpText.font == null)
                    {
                        // Try to find any TMP font asset
                        TMP_FontAsset defaultFont = Resources.FindObjectsOfTypeAll<TMP_FontAsset>().FirstOrDefault();
                        if (defaultFont != null)
                        {
                            tmpText.font = defaultFont;
                        }
                    }
                }
            }
            catch
            {
                Debug.LogWarning("Could not set default TMP font, text may not display correctly");
            }
        }
        
        return textObj;
    }
    
    private GameObject CreateButton(Transform parent, string name, string text, Vector2 size, Vector2 position)
    {
        GameObject buttonObj = new GameObject(name);
        buttonObj.transform.SetParent(parent, false);
        
        Image image = buttonObj.AddComponent<Image>();
        image.color = new Color(0.2f, 0.3f, 0.8f, 1f);
        
        Button button = buttonObj.AddComponent<Button>();
        
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform, false);
        
        // Try to use TMP_Text, fallback to regular Text
        try
        {
            TMP_Text tmpText = textObj.AddComponent<TMP_Text>();
            tmpText.text = text;
            tmpText.fontSize = 14;
            tmpText.color = buttonTextColor;
            tmpText.alignment = TextAlignmentOptions.Center;
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"Failed to add TMP_Text to button, using fallback: {e.Message}");
            Text fallbackText = textObj.AddComponent<Text>();
            fallbackText.text = text;
            fallbackText.fontSize = 14;
            fallbackText.color = buttonTextColor;
            fallbackText.alignment = TextAnchor.MiddleCenter;
            fallbackText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        }
        
        RectTransform buttonRect = buttonObj.GetComponent<RectTransform>();
        buttonRect.sizeDelta = size;
        buttonRect.anchoredPosition = position;
        
        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.sizeDelta = Vector2.zero;
        textRect.anchoredPosition = Vector2.zero;
        
        return buttonObj;
    }
    
    private GameObject CreateInputField(Transform parent, string name, string placeholder, Vector2 size, Vector2 position)
    {
        GameObject inputObj = new GameObject(name);
        inputObj.transform.SetParent(parent, false);
        
        Image image = inputObj.AddComponent<Image>();
        image.color = new Color(0.15f, 0.15f, 0.15f, 1f);
        
        TMP_InputField inputField = null;
        
        try
        {
            inputField = inputObj.AddComponent<TMP_InputField>();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to add TMP_InputField: {e.Message}");
            // Fallback to regular InputField
            InputField fallbackInput = inputObj.AddComponent<InputField>();
            
            // Create simple text for fallback
            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(inputObj.transform, false);
            Text text = textObj.AddComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.color = Color.white;
            
            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = Vector2.zero;
            textRect.anchoredPosition = Vector2.zero;
            
            fallbackInput.textComponent = text;
            
            RectTransform inputRect = inputObj.GetComponent<RectTransform>();
            inputRect.sizeDelta = size;
            inputRect.anchoredPosition = position;
            
            return inputObj;
        }
        
        if (inputField != null)
        {
            // Create text area
            GameObject textArea = new GameObject("Text Area");
            textArea.transform.SetParent(inputObj.transform, false);
            
            RectTransform textAreaRect = textArea.GetComponent<RectTransform>();
            textAreaRect.anchorMin = Vector2.zero;
            textAreaRect.anchorMax = Vector2.one;
            textAreaRect.sizeDelta = Vector2.zero;
            textAreaRect.anchoredPosition = Vector2.zero;
            textAreaRect.offsetMin = new Vector2(10, 2);
            textAreaRect.offsetMax = new Vector2(-10, -2);
            
            // Create placeholder
            GameObject placeholderObj = new GameObject("Placeholder");
            placeholderObj.transform.SetParent(textArea.transform, false);
            
            TMP_Text placeholderText = placeholderObj.AddComponent<TMP_Text>();
            placeholderText.text = placeholder;
            placeholderText.fontSize = 14;
            placeholderText.color = new Color(0.7f, 0.7f, 0.7f, 0.5f);
            placeholderText.fontStyle = FontStyles.Italic;
            
            RectTransform placeholderRect = placeholderObj.GetComponent<RectTransform>();
            placeholderRect.anchorMin = Vector2.zero;
            placeholderRect.anchorMax = Vector2.one;
            placeholderRect.sizeDelta = Vector2.zero;
            placeholderRect.anchoredPosition = Vector2.zero;
            
            // Create text
            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(textArea.transform, false);
            
            TMP_Text text = textObj.AddComponent<TMP_Text>();
            text.text = "";
            text.fontSize = 14;
            text.color = Color.white;
            
            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = Vector2.zero;
            textRect.anchorMax = Vector2.one;
            textRect.sizeDelta = Vector2.zero;
            textRect.anchoredPosition = Vector2.zero;
            
            // Configure input field
            inputField.textViewport = textAreaRect;
            inputField.textComponent = text;
            inputField.placeholder = placeholderText;
        }
        
        RectTransform finalInputRect = inputObj.GetComponent<RectTransform>();
        finalInputRect.sizeDelta = size;
        finalInputRect.anchoredPosition = position;
        
        return inputObj;
    }
}

[CustomEditor(typeof(RiddleSetupHelper))]
public class RiddleSetupHelperEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        RiddleSetupHelper helper = (RiddleSetupHelper)target;
        
        EditorGUILayout.Space(20);
        EditorGUILayout.LabelField("🎯 Quick Actions", EditorStyles.boldLabel);
        
        if (GUILayout.Button("🧩 Create Complete Riddle System", GUILayout.Height(30)))
        {
            helper.CreateCompletePuzzleSystem();
        }
        
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("🧪 Test Puzzle"))
        {
            helper.TestPuzzleSystem();
        }
        
        if (GUILayout.Button("🔗 Connect AI Terminal"))
        {
            helper.ConnectToAITerminal();
        }
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.Space(10);
        if (GUILayout.Button("🗑️ Remove Puzzle System", GUILayout.Height(25)))
        {
            if (EditorUtility.DisplayDialog("Remove Puzzle System", 
                "This will delete the riddle puzzle and its UI. Continue?", "Yes", "Cancel"))
            {
                helper.RemovePuzzleSystem();
            }
        }
    }
}
#endif